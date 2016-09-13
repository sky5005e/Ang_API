using BSFinancial.Common;
using BSFinancial.Data;
using BSFinancial.Models;
using BSFinancial.Providers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace BSFinancial.Controllers
{
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private BSFinancialRepository _repo = null;

        private ApplicationUserManager userManager;
        public ApplicationUserManager UserManager
        {
            get { return userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { userManager = value; }
        }
        private IAuthenticationManager Authentication
        {
            get { return HttpContext.Current.GetOwinContext().Authentication; }
        }

        public AccountController(BSFinancialRepository repo)
        {
            _repo = repo;
        }

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterViewModel userModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!String.IsNullOrEmpty(userModel.InvitedToken))
            {
                InvitedUser invitedUser = _repo.GetInvitedUserByEmailToken(userModel.InvitedToken);
                if (invitedUser != null)
                    userModel.CompId = invitedUser.InvitedCompId;
            }
            userModel.CreatedOn = DateTime.UtcNow;

            IdentityResult result = await _repo.RegisterUser(userModel);
            IHttpActionResult errorResult = GetErrorResult(result);

            if (errorResult != null)
            {
                return errorResult;
            }

            var userDetail = _repo.GetUserByEmail(userModel.Email);
            if (userDetail != null)
            {
                var subscription = new Subscription { UserId = userDetail.AccountId, CompanyId = userDetail.CompanyId, PlanType = string.IsNullOrEmpty(userModel.PlanType) ? PlanType.Free.ToString() : Enum.Parse(typeof(PlanType), userModel.PlanType, true).ToString() };
                _repo.InsertOrUpdateSubscription(subscription);
            }

            return Ok();
        }


        // POST api/Account/InviteUser
        [AllowAnonymous]
        [Route("InviteUser")]
        public async Task<IHttpActionResult> InviteUser(InviteUserModel userModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string tokenCode = EncryptionProviders.RandomString(32);
            InvitedUser invitedUser = _repo.GetInvitedUserByEmail(userModel.Email);
            if (invitedUser != null)
            {
                return BadRequest("User was invited earliear");
            }
            var currUser = AccountModel.GetCurrentUser(_repo);
            if (currUser != null)
            {
                userModel.EmailToken = tokenCode;
                userModel.InvitedUserId = currUser.Id;
                userModel.InvitedCompanyId = currUser.CompanyId;
                bool isSuccess = _repo.CreatedInvitedUser(userModel);

                if (isSuccess)
                {
                    string Domain = HttpContext.Current.Request.Url.Scheme + System.Uri.SchemeDelimiter + HttpContext.Current.Request.Url.Host + (HttpContext.Current.Request.Url.IsDefaultPort ? "" : ":" + HttpContext.Current.Request.Url.Port);
                    var callbackUrl = String.Format("{0}/#/register?id={1}&acc={2}", Domain, tokenCode, currUser.AccountId);
                    IdentityMessage message = new IdentityMessage();
                    message.Destination = userModel.Email;
                    message.Body = "You have been invited by (" + currUser.Email + ") to register please accept this inviation :  <a href=\"" + callbackUrl + "\">Accept Invitation </a>";
                    message.Subject = "Your are invited!!!";
                    await new EmailService().SendEmailAsync(message);
                }
                else
                    return BadRequest("Failed to invite user");
            }

            return Ok();
        }

        // POST api/Account/AcceptingUser
        [AllowAnonymous]
        [Route("AcceptingUser")]
        public async Task<IHttpActionResult> AcceptingUser(InviteUserModel userModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_repo.ConfirmedUser(userModel))
                return BadRequest("Failed to confirmed user");

            return Ok();
        }

        [Authorize]
        [HttpGet]
        [ActionName("GetPDF")]
        public HttpResponseMessage GetPDF(int id)
        {
            //BSFinancial.Services.PDFService pdfService = new BSFinancial.Services.PDFService();
            Loan loan = _repo.GetLoan(id);
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var contentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
           // byte[] pdfByteArray = pdfService.GeneratePDF(loan);
            //result.Content = new ByteArrayContent(pdfByteArray);
            //result.Content.Headers.ContentType = contentType;
            return result;
        }

      
        [AllowAnonymous]
        [HttpGet]
        [Route("ObtainLocalAccessToken")]
        public async Task<IHttpActionResult> ObtainLocalAccessToken(string provider, string externalAccessToken)
        {

            if (string.IsNullOrWhiteSpace(provider) || string.IsNullOrWhiteSpace(externalAccessToken))
            {
                return BadRequest("Provider or external access token is not sent");
            }

            var verifiedAccessToken = await VerifyExternalAccessToken(provider, externalAccessToken);
            if (verifiedAccessToken == null)
            {
                return BadRequest("Invalid Provider or External Access Token");
            }

            ApplicationUser user = await _repo.FindAsync(new UserLoginInfo(provider, verifiedAccessToken.user_id));
            if (!_repo.IsEmailVerified(user.Email))
            {
                return BadRequest("Your email id is not confirmed");
            }
            bool hasRegistered = user != null;

            if (!hasRegistered)
            {
                return BadRequest("External user is not registered");
            }

            //generate access token response
            var accessTokenResponse = GenerateLocalAccessTokenResponse(user.UserName);

            return Ok(accessTokenResponse);

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _repo.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Helpers

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private string ValidateClientAndRedirectUri(HttpRequestMessage request, ref string redirectUriOutput)
        {

            Uri redirectUri;

            var redirectUriString = GetQueryString(Request, "redirect_uri");

            if (string.IsNullOrWhiteSpace(redirectUriString))
            {
                return "redirect_uri is required";
            }

            bool validUri = Uri.TryCreate(redirectUriString, UriKind.Absolute, out redirectUri);

            if (!validUri)
            {
                return "redirect_uri is invalid";
            }

            var clientId = GetQueryString(Request, "client_id");

            if (string.IsNullOrWhiteSpace(clientId))
            {
                return "client_Id is required";
            }

            var user = _repo.FindUser(clientId);

            if (user == null)
            {
                return string.Format("Client_id '{0}' is not registered in the system.", clientId);
            }

            redirectUriOutput = redirectUri.AbsoluteUri;

            return string.Empty;

        }

        private string GetQueryString(HttpRequestMessage request, string key)
        {
            var queryStrings = request.GetQueryNameValuePairs();

            if (queryStrings == null) return null;

            var match = queryStrings.FirstOrDefault(keyValue => string.Compare(keyValue.Key, key, true) == 0);

            if (string.IsNullOrEmpty(match.Value)) return null;

            return match.Value;
        }

        private async Task<ParsedExternalAccessToken> VerifyExternalAccessToken(string provider, string accessToken)
        {
            ParsedExternalAccessToken parsedToken = null;

            var verifyTokenEndPoint = "";

            if (provider == "Facebook")
            {
                //You can get it from here: https://developers.facebook.com/tools/accesstoken/
                //More about debug_tokn here: http://stackoverflow.com/questions/16641083/how-does-one-get-the-app-access-token-for-debug-token-inspection-on-facebook
                var appToken = "xxxxxx";
                verifyTokenEndPoint = string.Format("https://graph.facebook.com/debug_token?input_token={0}&access_token={1}", accessToken, appToken);
            }
            else if (provider == "Google")
            {
                verifyTokenEndPoint = string.Format("https://www.googleapis.com/oauth2/v1/tokeninfo?access_token={0}", accessToken);
            }
            else
            {
                return null;
            }

            var client = new HttpClient();
            var uri = new Uri(verifyTokenEndPoint);
            var response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                dynamic jObj = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(content);

                parsedToken = new ParsedExternalAccessToken();

                //                 if (provider == "Facebook")
                //                 {
                //                     parsedToken.user_id = jObj["data"]["user_id"];
                //                     parsedToken.app_id = jObj["data"]["app_id"];
                // 
                //                     if (!string.Equals(Startup.facebookAuthOptions.AppId, parsedToken.app_id, StringComparison.OrdinalIgnoreCase))
                //                     {
                //                         return null;
                //                     }
                //                 }
                //                 else if (provider == "Google")
                //                 {
                //                     parsedToken.user_id = jObj["user_id"];
                //                     parsedToken.app_id = jObj["audience"];
                // 
                //                     if (!string.Equals(Startup.googleAuthOptions.ClientId, parsedToken.app_id, StringComparison.OrdinalIgnoreCase))
                //                     {
                //                         return null;
                //                     }
                // 
                //                 }

            }

            return parsedToken;
        }

        private JObject GenerateLocalAccessTokenResponse(string userName)
        {

            var tokenExpiration = TimeSpan.FromDays(1);

            ClaimsIdentity identity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);

            identity.AddClaim(new Claim(ClaimTypes.Name, userName));
            identity.AddClaim(new Claim("role", "user"));

            var props = new AuthenticationProperties()
            {
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.Add(tokenExpiration),
            };

            var ticket = new AuthenticationTicket(identity, props);

            var accessToken = Startup.OAuthBearerOptions.AccessTokenFormat.Protect(ticket);

            JObject tokenResponse = new JObject(
                                        new JProperty("userName", userName),
                                        new JProperty("access_token", accessToken),
                                        new JProperty("token_type", "bearer"),
                                        new JProperty("expires_in", tokenExpiration.TotalSeconds.ToString()),
                                        new JProperty(".issued", ticket.Properties.IssuedUtc.ToString()),
                                        new JProperty(".expires", ticket.Properties.ExpiresUtc.ToString())
        );

            return tokenResponse;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }
            public string ExternalAccessToken { get; set; }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer) || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name),
                    ExternalAccessToken = identity.FindFirstValue("ExternalAccessToken"),
                };
            }
        }

        #endregion
    }
}
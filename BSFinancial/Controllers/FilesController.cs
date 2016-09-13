using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using System.Drawing;
using System.Drawing.Imaging;

namespace BSFinancial.Controllers
{
    [RoutePrefix("api/files")]
    public class FilesController : ApiController
    {
        // GET: api/Upload
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Upload/5
        public string Get(int id)
        {
            return "value";
        }
       
        [HttpPost] // This is from System.Web.Http, and not from System.Web.Mvc
        public async Task<HttpResponseMessage> Upload()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);
            }

            var provider = GetMultipartProvider();
            var result = await Request.Content.ReadAsMultipartAsync(provider);

            // On upload, files are given a generic name like "BodyPart_26d6abe1-3ae1-416a-9429-b35f15e6e5d5"
            // so this is how you can get the original file name
            string originalFileName = GetDeserializedFileName(result.FileData.First()).Replace(' ', '-');

            // uploadedFileInfo object will give you some additional stuff like file length,
            // creation time, directory name, a few filesystem methods etc..
            var uploadedFileInfo = new FileInfo(result.FileData.First().LocalFileName);
            string guid = Guid.NewGuid().ToString();

            string uploadFolder = "~/upload"; // you could put this to web.config
            string root = HttpContext.Current.Server.MapPath(uploadFolder);
            Directory.CreateDirectory(root);

            string newFileName = root + "/" + guid + "_" + originalFileName;
            string uploadedFile = "upload/" + guid + "_" + originalFileName;
            uploadedFileInfo.CopyTo(newFileName);

            //Image image = Image.FromFile(newFileName);
            //Image thumb = image.GetThumbnailImage(40, 40, () => false, IntPtr.Zero);
            //thumb.Save(Path.ChangeExtension(newFileName, "thumb"));
            //CreateThumbnail(root + "\\" + guid + "_Thumb_" + originalFileName, root + "\\" + guid + "_" + originalFileName, 40, 40);

            // Remove this line as well as GetFormData method if you're not
            // sending any form data with your upload request
            //var fileUploadObj = GetFormData<UploadDataModel>(result);

            // Through the request response you can return an object to the Angular controller
            // You will be able to access this in the .success callback through its data attribute
            // If you want to send something to the .error callback, use the HttpStatusCode.BadRequest instead
            string returnData = "ReturnTest";
            return Request.CreateResponse(HttpStatusCode.OK, new { uploadedFile });
        }

        private void CreateThumbnail(string newFile, string orgFile, int lnWidth, int lnHeight)
        {
            System.Drawing.Bitmap bmpOut = null;
            try
            {
                Bitmap loBMP = new Bitmap(orgFile);
                ImageFormat loFormat = loBMP.RawFormat;

                decimal lnRatio;
                int lnNewWidth = 0;
                int lnNewHeight = 0;

                //*** If the image is smaller than a thumbnail just return   it
                if (loBMP.Width < lnWidth && loBMP.Height < lnHeight)
                {
                    loBMP.Save(newFile);
                    return;
                }

                if (loBMP.Width > loBMP.Height)
                {
                    lnRatio = (decimal)lnWidth / loBMP.Width;
                    lnNewWidth = lnWidth;
                    decimal lnTemp = loBMP.Height * lnRatio;
                    lnNewHeight = (int)lnTemp;
                }
                else
                {
                    lnRatio = (decimal)lnHeight / loBMP.Height;
                    lnNewHeight = lnHeight;
                    decimal lnTemp = loBMP.Width * lnRatio;
                    lnNewWidth = (int)lnTemp;
                }
                bmpOut = new Bitmap(lnNewWidth, lnNewHeight);
                Graphics g = Graphics.FromImage(bmpOut);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.FillRectangle(Brushes.White, 0, 0, lnNewWidth, lnNewHeight);
                g.DrawImage(loBMP, 0, 0, lnNewWidth, lnNewHeight);

                loBMP.Dispose();
            }
            catch (Exception ex)
            {
                return;
            }

            bmpOut.Save(newFile);
            return;
        }
        // You could extract these two private methods to a separate utility class since
        // they do not really belong to a controller class but that is up to you
        private MultipartFormDataStreamProvider GetMultipartProvider()
        {
            string uploadFolder = "~/upload"; // you could put this to web.config
            string root = HttpContext.Current.Server.MapPath(uploadFolder);
            Directory.CreateDirectory(root);
            return new MultipartFormDataStreamProvider(root);
        }

        // Extracts Request FormatData as a strongly typed model
        private object GetFormData<T>(MultipartFormDataStreamProvider result)
        {
            if (result.FormData.HasKeys())
            {
                string unescapedFormData = Uri.UnescapeDataString(result.FormData
                    .GetValues(0).FirstOrDefault() ?? String.Empty);
                if (!String.IsNullOrEmpty(unescapedFormData))
                {
                    return JsonConvert.DeserializeObject<T>(unescapedFormData);
                }
            }

            return null;
        }

        private string GetDeserializedFileName(MultipartFileData fileData)
        {
            string fileName = GetFileName(fileData);
            return JsonConvert.DeserializeObject(fileName).ToString();
        }

        public string GetFileName(MultipartFileData fileData)
        {
            return fileData.Headers.ContentDisposition.FileName;
        }

        public void Put(int id, [FromBody] string value) { }

        // DELETE: api/Upload/5
        public void Delete(int id) { }
    }
}
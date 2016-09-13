using BSFinancial.Models;
using BSFinancial.Providers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;
using System.Web.Mvc;
using BSFinancial.Services;

namespace BSFinancial.Data
{
    public class BSFinancialRepository : IBSFinancialRepository
    {
        private BSFinancialContext _ctx;
        private ApplicationUserManager _userManager;
        public BSFinancialRepository(BSFinancialContext ctx)
        {
            _ctx = ctx;

            InitRepository();
        }

        public BSFinancialRepository()
        {
            _ctx = new BSFinancialContext();

            InitRepository();
        }

        public void InitRepository()
        {
            _userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(new ApplicationDbContext()));

            _userManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

        }
        public bool Save()
        {
            try
            {
                return _ctx.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                // TODO log error
                var msg = ex.Message;
                return false;
            }
        }
        #region Prospects
        public bool InsertOrUpdateProposal(Prospect prospect)
        {
            try
            {
                if (prospect.Id > 0)
                {
                    _ctx.Prospects.Attach(prospect);
                    _ctx.Entry(prospect).State = EntityState.Modified;

                }
                else
                {
                    _ctx.Prospects.Add(prospect);
                }

                _ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return false;
            }
        }

        public IQueryable<Prospect> GetProspects()
        {
            return _ctx.Prospects;
        }
        public Prospect GetProspect(int prospectID)
        {
            var rst = _ctx.Prospects.FirstOrDefault(r => r.Id == prospectID);
            return rst;
        }
        public IQueryable<Prospect> DeleteProspect(int prospectID)
        {
            var delItem = _ctx.Prospects.FirstOrDefault(r => r.Id == prospectID);
            _ctx.Prospects.Remove(delItem);
            _ctx.SaveChanges();
            var rst = GetProspects();
            return rst;
        }
        #endregion
        #region Proposal prospects

        public bool AddProposalProspects(ProposalProspects Prospect)
        {
            try
            {

                _ctx.ProposalProspects.Add(Prospect);
                _ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return false;
            }
        }

        public bool AddProposalProperties(ProposalProperty property)
        {
            try
            {
                _ctx.ProposalProperties.Add(property);
                _ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return false;
            }
        }

        public bool RemoveProposalProperty(int proposalId, int propertyId)
        {
            try
            {
                var delItem = _ctx.ProposalProperties.FirstOrDefault(r => r.ProposalId == proposalId && r.PropertyID == propertyId);
                _ctx.ProposalProperties.Remove(delItem);
                _ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return false;
            }

        }
        public IQueryable<ProposalProspectModel> GetProposalProspects()
        {
            var propertyProspects = _ctx.ProposalProspects
                .Join(_ctx.Prospects, proppros => proppros.ProspectId, pros => pros.Id, (proppros, pros) => new { proppros, pros })
                .Join(_ctx.Users, pp => pp.proppros.UserId, ur => ur.Id, (pp, ur) => new { pp.proppros, pp.pros, ur = ur })
                .Select(p => new ProposalProspectModel
                {
                    ProspectId = p.pros.Id,
                    Id = p.proppros.Id,
                    FullName = p.pros.FirstName + " " + p.pros.LastName,
                    FirstName = p.pros.FirstName,
                    LastName = p.pros.LastName,
                    Phone = p.pros.Phone,
                    Email = p.pros.Email,
                    Agents = p.ur.FirstName + " " + p.ur.LastName,
                    ProposalDate = p.proppros.ProposalDate,
                    ProposalFor = p.proppros.ProposalFor,
                    SentOn = p.proppros.SentOn,
                    btnDisplayText = p.proppros.SentOn == null ? "Send":"Re-send",
                    TokenKey = p.proppros.TokenKey
                });
            return propertyProspects;
        }


        public bool UpdateProposalProspects(int id, DateTime? viewedDate, DateTime? sentDate)
        {
            try
            {
                if (id > 0)
                {
                    var proposalProspect = _ctx.ProposalProspects.FirstOrDefault(r => r.Id == id);
                    if (viewedDate != null)
                        proposalProspect.LastViewedDate = viewedDate;
                    if (sentDate != null)
                        proposalProspect.SentOn = sentDate;
                    _ctx.ProposalProspects.Attach(proposalProspect);
                    _ctx.Entry(proposalProspect).State = EntityState.Modified;

                }

                _ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return false;
            }
        }
        public IQueryable<ProposalProspectModel> DeleteProposalProspect(int proposalId)
        {
            var delItem = _ctx.ProposalProspects.FirstOrDefault(r => r.Id == proposalId);
            var delProposalPropertyies = _ctx.ProposalProperties.Where(p => p.ProposalId == delItem.ProspectId).ToList();
            foreach (var item in delProposalPropertyies)
            {
                _ctx.ProposalProperties.Remove(item);
            }
            _ctx.ProposalProspects.Remove(delItem);
            _ctx.SaveChanges();
            var rst = GetProposalProspects();
            return rst;
        }

        public ProposalPropertyModel GetProposalProperty(int proposalID)
        {
            var rst = _ctx.ProposalProspects.Where(r => r.Id == proposalID)
                .Join(_ctx.Prospects, proppros => proppros.ProspectId, pros => pros.Id, (proppros, pros) => new { proppros, pros })
                .Join(_ctx.Users, pp => pp.proppros.UserId, ur => ur.Id, (pp, ur) => new { pp.proppros, pp.pros, ur = ur })
                .Select(p => new ProposalPropertyModel
                {
                    ProposalId = p.proppros.Id,
                    Prospects = p.pros.FirstName + " " + p.pros.LastName,
                    ProposalFor = p.proppros.ProposalFor,
                    Agents = p.ur.FirstName + " " + p.ur.LastName,
                    ProposalDate = p.proppros.ProposalDate,
                    LastViewedDate = p.proppros.LastViewedDate
                }).FirstOrDefault();

            rst.Properties = GetProposalPropertiesInfo(proposalID).ToList();

            return rst;
        }

        public IQueryable<PropertyModel> GetProposalPropertiesInfo(int proposalID)
        {
            var properties = _ctx.ProposalProperties.Where(q => q.ProposalId == proposalID)
                .Join(_ctx.Properties, propprop => propprop.PropertyID, prop => prop.Id, (propprop, prop) => new { propprop, prop })
                .Select(p => new PropertyModel
                {
                    Id = p.prop.Id,
                    Address = p.prop.Address,
                    BuildSquareFt = p.prop.BuildSquareFt,
                    AskingPrice = p.prop.AskingPrice,
                    City = p.prop.City,
                    LegalDescription = p.prop.LegalDescription,
                    LotSquareFt = p.prop.LotSquareFt,
                    ProjectStatus = p.prop.ProjectStatus,
                    PurchasePrice = p.prop.PurchasePrice,
                    State = p.prop.State,
                    Zip = p.prop.Zip,
                    Latitude = p.prop.Latitude,
                    Longitude = p.prop.Longitude
                });
            return properties;
        }

        #endregion

        #region Property View Info
        public bool InsertOrUpdatePropertyViewInfo(PropertyViewInfo pvi)
        {
            try
            {
                if (pvi.Id > 0)
                {
                    _ctx.PropertyViewInfos.Attach(pvi);
                    _ctx.Entry(pvi).State = EntityState.Modified;
                }
                else
                {
                    _ctx.PropertyViewInfos.Add(pvi);
                }
                _ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return false;
            }
        }


        public IQueryable<ViewedPropertyModel> GetViewedProperty(int proposalID)
        {
            var properties = _ctx.PropertyViewInfos.Where(q => q.ProposalId == proposalID)
                .Join(_ctx.Properties, pvi => pvi.PropertyId, prop => prop.Id, (pvi, prop) => new { pvi, prop })
                .GroupBy(model => model.prop.Id)
                .Select(p => new ViewedPropertyModel
                {
                    Id = p.Select(x => x.prop.Id).FirstOrDefault(),
                    Address = p.Select(x => x.prop.Address).FirstOrDefault(),
                    City = p.Select(x => x.prop.City).FirstOrDefault(),
                    LegalDescription = p.Select(x => x.prop.LegalDescription).FirstOrDefault(),
                    ProjectStatus = p.Select(x => x.prop.ProjectStatus).FirstOrDefault(),
                    ViewedDuration = ( p.Sum(x => x.pvi.ViewDuration) / p.Count()),
                    PurchasePrice = p.Select(x => x.prop.PurchasePrice).FirstOrDefault(),
                    LastViewDate = p.OrderByDescending(o=>o.pvi.LastViewDate).Select(o=>o.pvi.LastViewDate).FirstOrDefault(),
                    TotalViewed = p.Count()
                   
                });
            return properties;
        }
        #endregion

        #region Property
        public IQueryable<Property> GetPropertiesByCompany(int companyId)
        {
            return _ctx.Properties.Where(r => r.CompanyId == companyId);
        }

        public IQueryable<PropertyValue> GetPropertyValuesByProperty(int propertyId)
        {
            return _ctx.PropertyValues.Where(r => r.PropertyId == propertyId);
        }

        public Property GetProperty(int propertyId)
        {
            var rst = _ctx.Properties.FirstOrDefault(r => r.Id == propertyId);
            rst.Documents = GetDocumentsOfProperty(propertyId).ToList();
            rst.Photos = GetPhotosOfProperty(propertyId).ToList();
            
            return rst;
        }
        public IQueryable<Property> DeleteProperty(int propertyId, int companyId)
        {
            var delItem = _ctx.Properties.FirstOrDefault(r => r.Id == propertyId);
            _ctx.Properties.Remove(delItem);
            _ctx.SaveChanges();
            return _ctx.Properties.Where(r => r.CompanyId == companyId);
        }

        public IQueryable<Property> RemoveProperty(int propertyId, int companyId)
        {
            var proItem = _ctx.Properties.FirstOrDefault(r => r.Id == propertyId);
            proItem.LoanId = null;
            _ctx.Properties.Attach(proItem);
            _ctx.Entry(proItem).State = EntityState.Modified;
            _ctx.SaveChanges();
            return _ctx.Properties.Where(r => r.CompanyId == companyId);
        }

        public bool InsertOrUpdateProperty(Property property)
        {
            try
            {
                if (property.Id > 0)
                {
                    _ctx.Properties.Attach(property);
                    _ctx.Entry(property).State = EntityState.Modified;

                }
                else
                {
                    _ctx.Properties.Add(property);
                }

                _ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return false;
            }
        }

        public bool InsertOrUpdatePropertyRatingInfo(PropertyRating pr)
        {
            try
            {
                if (pr.Id > 0)
                {
                    _ctx.PropertyRatings.Attach(pr);
                    _ctx.Entry(pr).State = EntityState.Modified;
                }
                else
                {
                    _ctx.PropertyRatings.Add(pr);
                }
                _ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return false;
            }
        }
        public PropertyRating GetPropertyRating(int propertyId)
        {
            var rst = _ctx.PropertyRatings.FirstOrDefault(r => r.propertyId == propertyId);
            return rst;
        }
        #endregion Property

        #region Documents
        public bool InsertDocument(Document document)
        {
            try
            {
                _ctx.Documents.Add(document);
                _ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return false;
            }
        }

        public IQueryable<Document> DeleteDocument(int documentId, int companyId)
        {
            var delItem = _ctx.Documents.FirstOrDefault(r => r.Id == documentId);
            _ctx.Documents.Remove(delItem);
            _ctx.SaveChanges();
            return _ctx.Documents.Where(r => r.CompanyId == companyId);
        }

        public IQueryable<Document> GetDocumentsOfLoan(int loanId)
        {
            return _ctx.Documents.Where(r => r.LoanId == loanId);
        }
        public bool InsertPropertyDocument(PropertyDocument document)
        {
            try
            {
                _ctx.PropertyDocuments.Add(document);
                _ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return false;
            }
        }

        public IQueryable<PropertyDocument> DeletePropertyDocument(int documentId, int propertyID)
        {
            var delItem = _ctx.PropertyDocuments.FirstOrDefault(r => r.Id == documentId);
            _ctx.PropertyDocuments.Remove(delItem);
            _ctx.SaveChanges();
            return _ctx.PropertyDocuments.Where(r => r.propertyId == propertyID);
        }

        public IQueryable<PropertyDocument> GetDocumentsOfProperty(int propertyId)
        {
            return _ctx.PropertyDocuments.Where(r => r.propertyId == propertyId);
        }

        public bool InsertPropertyPhoto(PropertyPhoto photo)
        {
            try
            {
                _ctx.PropertyPhotos.Add(photo);
                _ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return false;
            }
        }

        public IQueryable<PropertyPhoto> DeletePropertyPhoto(int photoId, int propertyID)
        {
            var delItem = _ctx.PropertyPhotos.FirstOrDefault(r => r.Id == photoId);
            _ctx.PropertyPhotos.Remove(delItem);
            _ctx.SaveChanges();
            return _ctx.PropertyPhotos.Where(r => r.propertyId == propertyID);
        }

        public IQueryable<PropertyPhoto> GetPhotosOfProperty(int propertyId)
        {
            return _ctx.PropertyPhotos.Where(r => r.propertyId == propertyId);
        }
        #endregion

        #region Company
        public IQueryable<Company> GetCompanies(int companyId)
        {
            return _ctx.Companies.Where(r => r.Id == companyId);
        }
        public bool AddCompany(Company newCompany)
        {
            try
            {
                _ctx.Companies.Add(newCompany);
                return true;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return false;
            }
        }
        #endregion

        #region Inquiry
        public IQueryable<ContactModel> GetInquiriesByCompany(int companyId)
        {
            return _ctx.Inquiries.Where(r => r.CompanyId == companyId);
        }

        public bool AddInquiry(ContactModel newInquiry)
        {
            try
            {
                newInquiry.CreatedOn = DateTime.UtcNow;
                _ctx.Inquiries.Add(newInquiry);
                //_ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return false;
            }
        }

        public ContactModel GetInquiry(int inquiryId)
        {
            return _ctx.Inquiries.FirstOrDefault(r => r.Id == inquiryId);
        }
        #endregion

        #region Applications
        public List<Application> GetApplicationsByCompany(int companyId)
        {
            //            return _ctx.Applications.Where(r => r.CompanyId == companyId);

            var rst = _ctx.Applications.Where(r => r.CompanyId == companyId).ToList();

            //List<Applicant> applicantList = GetApplicantsByCompany(companyId).ToList();

            foreach (var item in rst)
            {
                item.MainApplicant = GetApplicant(item.ApplicantId);
                //item.co_applicant = applicantList.FirstOrDefault(r => r.Id == item.CoApplicantId);
            }

            return rst;
        }
        public Application GetApplication(int id)
        {
            var rst = _ctx.Applications.AsNoTracking().FirstOrDefault(r => r.Id == id);

            rst.MainApplicant = GetApplicant(rst.ApplicantId);
            rst.Applicants = GetApplicantByIds(rst.ApplicantIdList.ToList());
            rst.Employments = GetEmploymentsOfApp(id).ToList();
            rst.Addrs = GetApplicantAddrs(id).ToList();
            //rst.applicant = GetApplicant(rst.ApplicantId);
            //rst.co_applicant = GetApplicant(rst.CoApplicantId);

            return rst;
        }

        public bool InsertOrUpdateApplication(Application app)
        {
            try
            {
                if (app.Id > 0) //Edit
                {
                    List<int> appList = app.Applicants.Where(m => m.Id != 0).Select(m => m.Id).ToList();

                    foreach (var item in app.Applicants)
                    {
                        if (item.Id == 0)
                        {
                            item.CompanyId = app.CompanyId;
                            item.CreatedOn = DateTime.Now;
                            _ctx.Applicants.Add(item);
                            _ctx.SaveChanges();

                            appList.Add(item.Id);

                            if (app.ApplicantId == 0 || !app.ApplicantIdList.Contains(app.ApplicantId))
                            {
                                app.ApplicantId = item.Id;
                            }
                        }
                    }
                    //                     app.Applicants = null;
                    //                     app.MainApplicant = null;

                    app.ApplicantIdList = appList;
                    _ctx.Applications.Attach(app);
                    _ctx.Entry(app).State = EntityState.Modified;
                }
                else
                {
                    app.CreatedOn = DateTime.UtcNow;

                    _ctx.Applications.Add(app);
                    //                     _ctx.Entry(app.applicant).State = EntityState.Unchanged;
                    //                     _ctx.Entry(app.co_applicant).State = EntityState.Unchanged;
                }

                _ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return false;
            }
        }

        public bool SaveApplicants(Application app)
        {
            try
            {
                if (app.Id > 0) //Edit
                {
                    var application = GetApplication(app.Id);

                    application.ApplicantIdList = app.Applicants.Select(m => m.Id).ToList();
                    _ctx.Entry(app).State = EntityState.Modified;
                    _ctx.SaveChanges();
                    return true;
                }

            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return false;
            }

            return false;
        }

        public IQueryable<Application> DeleteApplication(int appId, int companyId)
        {
            var delItem = _ctx.Applications.FirstOrDefault(r => r.Id == appId);
            _ctx.Applications.Remove(delItem);
            _ctx.SaveChanges();

            return _ctx.Applications.Where(r => r.CompanyId == companyId);
        }
        public bool SetMainApplicant(int id, int applicantId)
        {
            try
            {
                var application = _ctx.Applications.FirstOrDefault(r => r.Id == id);
                application.ApplicantId = applicantId;
                _ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool RemoveApplicant(int id, int applicantId)
        {
            try
            {
                List<int> exceptid = new List<int>();
                exceptid.Add(applicantId);
                var application = _ctx.Applications.FirstOrDefault(r => r.Id == id);
                var exceptedlist = application.ApplicantIdList.Except(exceptid);
                application.ApplicantIdList = exceptedlist.ToList();

                var delemp = _ctx.Employments.Where(r => r.ApplicationId == id && r.ApplicantId == applicantId);
                _ctx.Employments.RemoveRange(delemp);
                var deladdr = _ctx.ApplicantAddrs.Where(r => r.ApplicationId == id && r.ApplicantId == applicantId);
                _ctx.ApplicantAddrs.RemoveRange(deladdr);

                _ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion

        #region Applicant
        public IQueryable<Applicant> GetApplicantsByCompany(int companyId)
        {
            return _ctx.Applicants.AsNoTracking().Where(r => r.CompanyId == companyId);
        }

        public Applicant GetApplicant(int applicantId)
        {
            return _ctx.Applicants.AsNoTracking().FirstOrDefault(r => r.Id == applicantId);
        }
        public List<Applicant> GetApplicantByIds(List<int> idlist)
        {
            return _ctx.Applicants.AsNoTracking().Where(r => idlist.Contains(r.Id)).ToList();
        }

        public bool InsertOrUpdateApplicant(Applicant applicant)
        {
            try
            {
                if (applicant.Id > 0)
                {
                    _ctx.Applicants.Attach(applicant);
                    _ctx.Entry(applicant).State = EntityState.Modified;

                }
                else
                {
                    applicant.CreatedOn = DateTime.UtcNow;

                    _ctx.Applicants.Add(applicant);
                }

                _ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return false;
            }
        }
        public IQueryable<Applicant> DeleteApplicant(int applicantId, int companyId)
        {
            var delItem = _ctx.Applicants.FirstOrDefault(r => r.Id == applicantId);
            _ctx.Applicants.Remove(delItem);
            _ctx.SaveChanges();

            return _ctx.Applicants.Where(r => r.CompanyId == companyId);
        }

        #endregion

        #region Fund
        public IQueryable<Fund> GetFundsByCompany(int companyId)
        {
            return _ctx.Funds.Where(r => r.CompanyId == companyId);
        }
        public Fund GetFund(int fundId)
        {
            var rst = _ctx.Funds.FirstOrDefault(r => r.Id == fundId);
            rst.FundInvestors = GetInvestorsOfFund(fundId);
            rst.FundDistributions = GetDistributionsOfFund(fundId);

            return rst;
        }
        public IQueryable<InvestorModel> GetInvestorsOfFund(int fundId)
        {
            var fundInvestors = _ctx.FundInvestors.Where(fi => fi.FundId == fundId)
                .Join(_ctx.Investors, fund => fund.InvestorId, investor => investor.Id, (fund, investor) => new { fund, investor })
                .Select(f => new InvestorModel
                {
                    FundInvestorId = f.fund.Id,
                    Id = f.investor.Id,
                    FullName = f.investor.FirstName + " " + f.investor.LastName,
                    FirstName = f.investor.FirstName,
                    LastName = f.investor.LastName,
                    Ssn = f.investor.Ssn,
                    Phone = f.investor.Phone,
                    Email = f.investor.Email,
                    AmountPaid = f.fund.AmountPaid,
                    Shares = f.fund.Shares
                });
            return fundInvestors;
        }
        public bool InsertOrUpdateFund(Fund fund)
        {
            try
            {
                if (fund.Id > 0)
                {
                    _ctx.Funds.Attach(fund);
                    _ctx.Entry(fund).State = EntityState.Modified;
                }
                else
                {
                    fund.CreatedOn = DateTime.UtcNow;
                    _ctx.Funds.Add(fund);
                }
                _ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return false;
            }
        }

        public IQueryable<Fund> DeleteFund(int fundId, int companyId)
        {
            var delItem = _ctx.Funds.FirstOrDefault(r => r.Id == fundId);
            _ctx.Funds.Remove(delItem);
            _ctx.SaveChanges();
            return _ctx.Funds.Where(r => r.CompanyId == companyId);
        }

        #endregion

        #region Investor
        public IQueryable<Investor> GetinvestorsByCompany(int companyId)
        {
            return _ctx.Investors.Where(r => r.CompanyId == companyId);
        }
        public Investor GetInvestor(int investorId)
        {
            var rst = _ctx.Investors.FirstOrDefault(r => r.Id == investorId);
            return rst;
        }
        public bool InsertOrUpdateInvestor(Investor investor)
        {
            try
            {
                if (investor.Id > 0)
                {
                    _ctx.Investors.Attach(investor);
                    _ctx.Entry(investor).State = EntityState.Modified;
                }
                else
                {
                    investor.CreatedOn = DateTime.UtcNow;
                    _ctx.Investors.Add(investor);
                }
                _ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return false;
            }
        }

        public IQueryable<Investor> DeleteInvestor(int investorId, int companyId)
        {
            var delItem = _ctx.Investors.FirstOrDefault(r => r.Id == investorId);
            _ctx.Investors.Remove(delItem);
            _ctx.SaveChanges();
            return _ctx.Investors.Where(r => r.CompanyId == companyId);
        }

        public FundInvestor GetFundInvestorById(int Id)
        {
            return _ctx.FundInvestors.Where(r => r.Id == Id).FirstOrDefault();
        }

        public bool AddInvestorsToFund(int FundID, List<InvestorModel> Investors)
        {
            bool status = false;
            using (var trans = _ctx.Database.BeginTransaction())
            {
                try
                {
                    foreach (var investor in Investors)
                    {
                        var invesItem = new FundInvestor();
                        invesItem.FundId = FundID;
                        invesItem.InvestorId = investor.Id;
                        invesItem.Shares = investor.Shares;
                        invesItem.AmountPaid = investor.AmountPaid;
                        invesItem.CreatedOn = DateTime.UtcNow;
                        _ctx.FundInvestors.Add(invesItem);
                    }
                    _ctx.SaveChanges();
                    trans.Commit();
                    status = true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    status = false;
                }
            }

            return status;
        }

        public bool UpdateInvestorsToFund(int FundID, InvestorModel Investor)
        {
            bool status = false;
            using (var trans = _ctx.Database.BeginTransaction())
            {
                try
                {

                    var fundInvesItem = GetFundInvestorById(Investor.FundInvestorId);
                    fundInvesItem.FundId = FundID;
                    fundInvesItem.InvestorId = Investor.Id;
                    fundInvesItem.Shares = Investor.Shares;
                    fundInvesItem.AmountPaid = Investor.AmountPaid;
                    _ctx.FundInvestors.Attach(fundInvesItem);
                    _ctx.Entry(fundInvesItem).State = EntityState.Modified;

                    var invItem = GetInvestor(Investor.Id);
                    invItem.FirstName = Investor.FirstName;
                    invItem.LastName = Investor.LastName;
                    invItem.Email = Investor.Email;
                    invItem.Ssn = Investor.Ssn;
                    invItem.Phone = Investor.Phone;
                    invItem.Shares = Investor.Shares;
                    _ctx.Investors.Attach(invItem);
                    _ctx.Entry(invItem).State = EntityState.Modified;

                    _ctx.SaveChanges();
                    trans.Commit();
                    status = true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    status = false;
                }
            }

            return status;
        }

        public IQueryable<FundInvestor> RemoveFundInvestor(int fundInvestorId, int fundId)
        {
            var fiItem = _ctx.FundInvestors.FirstOrDefault(r => r.Id == fundInvestorId);
            _ctx.FundInvestors.Remove(fiItem);
            _ctx.SaveChanges();
            return _ctx.FundInvestors.Where(r => r.FundId == fundId);
        }

        #endregion

        #region Distributions
        public bool AddDistributionToFund(DistributionModel distribution)
        {
            bool status = false;
            using (var trans = _ctx.Database.BeginTransaction())
            {
                try
                {
                    var fundInvs = GetInvestorsOfFund(distribution.FundId);
                    var fund = GetFund(distribution.FundId);

                    decimal distributionAmount = distribution.Amount;
                    foreach (var investor in fundInvs)
                    {
                        var disItem = new Distribution();
                        disItem.FundId = distribution.FundId;
                        disItem.InvestorId = investor.Id;
                        disItem.Amount = distributionAmount;
                        decimal amt = Math.Round((Convert.ToDecimal(investor.Shares) / Convert.ToDecimal(fund.SharesAllowed)) * Convert.ToDecimal(distributionAmount), 2);
                        disItem.InvestorAmount = amt;
                        disItem.CreatedOn = DateTime.UtcNow;
                        _ctx.Distributions.Add(disItem);
                    }
                    _ctx.SaveChanges();
                    trans.Commit();
                    status = true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    status = false;
                }
            }

            return status;
        }
        public IQueryable<DistributionModel> GetDistributionsOfFund(int fundId)
        {
            var fundDistributions = _ctx.Distributions.Where(di => di.FundId == fundId)
                .Join(_ctx.Investors, dis => dis.InvestorId, investor => investor.Id, (dis, investor) => new { dis, investor })
                .Select(d => new DistributionModel
                {
                    FundId = d.dis.FundId,
                    Id = d.dis.Id,
                    FullName = d.investor.FirstName + " " + d.investor.LastName,
                    Amount = d.dis.Amount,
                    InvestorAmount = d.dis.InvestorAmount,
                    CreatedOn = d.dis.CreatedOn
                });
            return fundDistributions;
        }
        public Distribution GetDistributionById(int Id)
        {
            return _ctx.Distributions.Where(r => r.Id == Id).FirstOrDefault();
        }
        public IQueryable<DistributionModel> DeleteInvestorDistributionById(int investorDistributionID, int fundId)
        {
            var delItem = _ctx.Distributions.FirstOrDefault(r => r.Id == investorDistributionID);
            //.Investors.FirstOrDefault(r => r.Id == investorId);
            _ctx.Distributions.Remove(delItem);
            _ctx.SaveChanges();
            return GetDistributionsOfFund(fundId);
        }


        #endregion

        #region Loan
        public IQueryable<Loan> GetLoansByCompany(int companyId)
        {
            return _ctx.Loans.Where(r => r.CompanyId == companyId);
        }
        public IQueryable<PaymentModel> GetAllLoansPaymentByCompany(int companyId)
        {
            var payments = _ctx.Payments.Where(p => p.CompanyId == companyId)
                .Join(_ctx.Loans, p => p.LoanId, l => l.Id, (pmts, loans) => new { pmts, loans })
                .Join(_ctx.Applications, pl => pl.loans.ApplicationId, ap => ap.Id, (pl, ap) => new { pl.pmts, pl.loans, ap = ap })
                .Join(_ctx.Properties, pl2 => pl2.loans.Id, po => po.LoanId, (pl2, po) => new { pl2.pmts, pl2.loans, pl2.ap, po = po })
                .Select(p => new PaymentModel
                        {
                            Id = p.pmts.Id,
                            User = p.ap.ApplicationName,
                            LoanId = p.pmts.LoanId,
                            Address = p.po.Address,
                            PayDate = p.pmts.PayDate,
                            TotalAmt = p.pmts.TotalAmt,
                            PrincipalAmt = p.pmts.PrincipalAmt,
                            InterestAmt = p.pmts.InterestAmt,
                            EscrowAmt = p.pmts.EscrowAmt

                        });
            return payments;
        }
        public Loan GetLoan(int loanId)
        {
            var rst = _ctx.Loans.FirstOrDefault(r => r.Id == loanId);
            rst.application = GetApplication(rst.ApplicationId);
            rst.Payments = GetPaymentsOfLoan(loanId).ToList();
            rst.Properties = GetPropertiesOfLoan(loanId).ToList();
            rst.Documents = GetDocumentsOfLoan(loanId).ToList();
            rst.Disbursements = GetDisbursementsOfLoan(loanId).ToList();
            return rst;
        }
        public bool InsertOrUpdateLoan(Loan loan)
        {
            try
            {
                if (loan.Id > 0)
                {
                    if (loan.application != null)
                    {
                        loan.ApplicationId = loan.application.Id;
                    }
                    _ctx.Loans.Attach(loan);
                    _ctx.Entry(loan).State = EntityState.Modified;

                }
                else
                {
                    if (loan.application != null)
                    {
                        loan.ApplicationId = loan.application.Id;
                    }
                    else
                    {
                        //loan.application = _ctx.Applications.FirstOrDefault(m => m.Id == 1);
                    }
                    loan.CreatedOn = DateTime.UtcNow;
                    loan.BeginDate = DateTime.UtcNow;
                    loan.MaturityDate = DateTime.UtcNow;
                    _ctx.Loans.Add(loan);
                    _ctx.Entry(loan.application).State = EntityState.Unchanged;
                }

                _ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return false;
            }
        }

        public IQueryable<Loan> DeleteLoan(int loanId, int companyId)
        {
            var delItem = _ctx.Loans.FirstOrDefault(r => r.Id == loanId);
            _ctx.Loans.Remove(delItem);
            _ctx.SaveChanges();

            return _ctx.Loans.Where(r => r.CompanyId == companyId);
        }

        public IQueryable<Payment> GetPaymentsOfLoan(int loanId)
        {
            return _ctx.Payments.Where(r => r.LoanId == loanId);
        }
        public IQueryable<Property> GetPropertiesOfLoan(int loanId)
        {
            return _ctx.Properties.Where(r => r.LoanId == loanId);
        }

        public bool AddPropertiesToLoan(int LoadID, List<PropertyModel> Properties)
        {
            bool status = false;
            using (var trans = _ctx.Database.BeginTransaction())
            {
                try
                {
                    foreach (var property in Properties)
                    {
                        var proItem = _ctx.Properties.FirstOrDefault(r => r.Id == property.Id);
                        proItem.LoanId = LoadID;
                        _ctx.Properties.Attach(proItem);
                        _ctx.Entry(proItem).State = EntityState.Modified;
                    }
                    _ctx.SaveChanges();
                    trans.Commit();
                    status = true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    status = false;
                }
            }

            return status;
        }
        public bool InsertOrUpdatePayment(Payment payment)
        {
            try
            {
                if (payment.Id > 0)
                {
                    _ctx.Payments.Attach(payment);
                    _ctx.Entry(payment).State = EntityState.Modified;

                }
                else
                {
                    payment.CreatedOn = DateTime.UtcNow;
                    _ctx.Payments.Add(payment);
                }

                _ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return false;
            }
        }
        public bool DeletePayment(int id, int companyId)
        {
            try
            {
                var delItem = _ctx.Payments.FirstOrDefault(r => r.Id == id);
                _ctx.Payments.Remove(delItem);
                _ctx.SaveChanges();

                return true;

            }
            catch (Exception ex)
            {
            }

            return false;
        }
        #endregion

        #region Disbursements
        public IQueryable<Disbursement> GetDisbursementsOfLoan(int loanId)
        {
            return _ctx.Disbursements.Where(r => r.LoanId == loanId);
        }
        public bool InsertOrUpdateDisbursement(Disbursement Disbursement)
        {
            try
            {
                if (Disbursement.Id > 0)
                {
                    _ctx.Disbursements.Attach(Disbursement);
                    _ctx.Entry(Disbursement).State = EntityState.Modified;

                }
                else
                {
                    Disbursement.CreatedOn = DateTime.UtcNow;
                    _ctx.Disbursements.Add(Disbursement);
                }

                _ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return false;
            }
        }
        public bool DeleteDisbursement(int id, int companyId)
        {
            try
            {
                var delItem = _ctx.Disbursements.FirstOrDefault(r => r.Id == id);
                _ctx.Disbursements.Remove(delItem);
                _ctx.SaveChanges();

                return true;

            }
            catch (Exception ex)
            {
            }

            return false;
        }
        #endregion

        #region ApplicantAddr
        public List<ApplicantAddr> GetApplicantAddrs(int appId)
        {
            var rst = _ctx.ApplicantAddrs.AsNoTracking().Where(r => r.ApplicationId == appId).ToList();

            foreach (var item in rst)
            {
                item.applicant = GetApplicant(item.ApplicantId);
            }

            return rst;
        }

        public bool InsertOrUpdateApplicantAddr(ApplicantAddr addr)
        {
            try
            {
                if (addr.applicant != null)
                {
                    _ctx.Entry(addr.applicant).State = EntityState.Unchanged;
                }

                if (addr.Id > 0)
                {
                    _ctx.ApplicantAddrs.Attach(addr);
                    _ctx.Entry(addr).State = EntityState.Modified;
                }
                else
                {
                    addr.CreatedOn = DateTime.UtcNow;
                    _ctx.ApplicantAddrs.Add(addr);
                }

                _ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return false;
            }
        }
        public bool DeleteApplicantAddr(int id, int companyId)
        {
            try
            {
                var delItem = _ctx.ApplicantAddrs.FirstOrDefault(r => r.Id == id);
                _ctx.ApplicantAddrs.Remove(delItem);
                _ctx.SaveChanges();

                return true;

            }
            catch (Exception ex)
            {
            }

            return false;
        }
        #endregion

        #region Employment
        public List<Employment> GetEmploymentsOfApp(int appId)
        {
            var rst = _ctx.Employments.AsNoTracking().Where(r => r.ApplicationId == appId).ToList();

            foreach (var item in rst)
            {
                item.applicant = GetApplicant(item.ApplicantId);
            }

            return rst;
        }

        public bool InsertOrUpdateEmployment(Employment employment)
        {
            try
            {
                if (employment.applicant != null)
                {
                    _ctx.Entry(employment.applicant).State = EntityState.Unchanged;
                }

                if (employment.Id > 0)
                {
                    _ctx.Employments.Attach(employment);
                    _ctx.Entry(employment).State = EntityState.Modified;
                }
                else
                {
                    employment.CreatedOn = DateTime.UtcNow;
                    _ctx.Employments.Add(employment);
                }

                _ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return false;
            }
        }
        public bool DeleteEmployment(int id, int companyId)
        {
            try
            {
                var delItem = _ctx.Employments.FirstOrDefault(r => r.Id == id);
                _ctx.Employments.Remove(delItem);
                _ctx.SaveChanges();

                return true;

            }
            catch (Exception ex)
            {
            }

            return false;
        }
        #endregion

        #region Invited Users
        public InvitedUser GetInvitedUserByEmail(string email)
        {
            return _ctx.InvitedUsers.FirstOrDefault(u => u.Email == email);
        }
        public InvitedUser GetInvitedUserByEmailToken(string token)
        {
            return _ctx.InvitedUsers.FirstOrDefault(u => u.EmailToken == token);
        }
        public bool ConfirmedUser(InviteUserModel userModel)
        {
            bool status = false;
            using (var trans = _ctx.Database.BeginTransaction())
            {
                try
                {
                    if (!String.IsNullOrEmpty(userModel.UserId))
                    {
                        ApplicationUser appUser = GetApplicationUserById(userModel.UserId);
                        appUser.EmailConfirmed = true;
                        _userManager.Update(appUser);
                        if (appUser != null)
                        {
                            InvitedUser user = GetInvitedUserByEmail(appUser.Email);
                            if (user != null)
                            {
                                user.Status = true; // confirmed user
                            }
                        }
                        _ctx.SaveChanges();
                    }

                    trans.Commit();
                    status = true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    status = false;
                }
            }

            return status;
        }
        public bool CreatedInvitedUser(InviteUserModel userModel)
        {
            bool status = false;
            using (var trans = _ctx.Database.BeginTransaction())
            {
                try
                {
                    InvitedUser newInvitedUser = new InvitedUser();
                    newInvitedUser.Email = userModel.Email;
                    newInvitedUser.CreatedOn = DateTime.Now;
                    newInvitedUser.CreatedBy = userModel.InvitedUserId;
                    newInvitedUser.EmailToken = userModel.EmailToken;
                    newInvitedUser.InvitedCompId = userModel.InvitedCompanyId;
                    newInvitedUser.IsDeleted = false;
                    newInvitedUser.Status = false; // true for activated and false for pending users
                    _ctx.InvitedUsers.Add(newInvitedUser);
                    _ctx.SaveChanges();
                    trans.Commit();
                    status = true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    status = false;
                }
            }

            return status;
        }
        #endregion

        #region User Authentication
        public async Task<IdentityResult> RegisterUser(RegisterViewModel userModel)
        {
            using (var trans = _ctx.Database.BeginTransaction())
            {
                try
                {
                    // Bring the current value into local scope.
                    var context = System.Web.HttpContext.Current;

                    ApplicationUser user = new ApplicationUser
                    {
                        UserName = userModel.Email,
                        Email = userModel.Email
                    };

                    var result = await _userManager.CreateAsync(user, userModel.Password);

                    if (!result.Succeeded)
                    {
                        trans.Rollback();
                        return result;
                    }
                    Company newComp = new Company();
                    if (String.IsNullOrEmpty(userModel.AccId))
                    {
                        newComp.Name = userModel.Company;
                        newComp.CreatedOn = DateTime.UtcNow;
                        newComp.IsDeleted = false;
                        _ctx.Companies.Add(newComp);
                        _ctx.SaveChanges();
                    }

                    User newUser = new User();
                    newUser.CompanyId = String.IsNullOrEmpty(userModel.InvitedToken) == true ? newComp.Id : userModel.CompId;
                    newUser.AccountId = user.Id.ToString();
                    newUser.FirstName = userModel.FirstName;
                    newUser.LastName = userModel.LastName;
                    newUser.Email = userModel.Email;
                    newUser.Role = String.IsNullOrEmpty(userModel.InvitedToken) == true ? "Admin" : "User";
                    _ctx.Users.Add(newUser);
                    _ctx.SaveChanges();

                    trans.Commit();

                    string code = String.Empty;
                    //var provider = new DpapiDataProtectionProvider("BSFinancial");
                    var provider = new MachineKeyProtectionProvider();
                    _userManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(provider.Create("EmailConfirmation"));
                    //_userManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(provider.Create("EmailConfirmation"));
                    // send email confirmation email
                    // if (userModel.CompId > 0 && !String.IsNullOrEmpty(userModel.AccId))
                    //   code = userModel.InvitedToken;
                    //else
                    code = _userManager.GenerateEmailConfirmationToken(user.Id);

                    string Domain = context.Request.Url.Scheme + System.Uri.SchemeDelimiter + context.Request.Url.Host + (context.Request.Url.IsDefaultPort ? "" : ":" + context.Request.Url.Port);
                    var callbackUrl = String.Format("{0}/#/confirmation?id={1}&acc={2}", Domain, code, user.Id);

                    IdentityMessage message = new IdentityMessage();
                    message.Destination = userModel.Email;
                    message.Body = "Please confirm your account by clicking this link :  <a href=\"" + callbackUrl + "\">Please click here </a>";
                    message.Subject = "Account activation request!!!";
                    await new EmailService().SendEmailAsync(message);

                    return result;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                }
            }

            return null;
        }
        public bool IsEmailVerified(string userName)
        {
            var user = _userManager.FindByName(userName);
            return _userManager.IsEmailConfirmed(user.Id);
        }
        public ApplicationUser GetApplicationUserById(string userId)
        {
            return _userManager.FindById(userId);
        }
        public async Task<ApplicationUser> FindAsync(UserLoginInfo loginInfo)
        {
            ApplicationUser user = await _userManager.FindAsync(loginInfo);

            return user;
        }

        public async Task<IdentityUser> FindUser(string userName, string password)
        {
            IdentityUser user = await _userManager.FindAsync(userName, password);

            return user;
        }

        public User FindUser(string userId)
        {
            var user = _ctx.Users.Find(userId);

            return user;
        }

        public User GetUserByEmail(string email)
        {
            return _ctx.Users.FirstOrDefault(u => u.Email == email);
        }

        public IQueryable<User> GetUsersByCompany(int companyId)
        {
            return _ctx.Users.Where(r => r.CompanyId == companyId);
        }
        #endregion

        #region Subscription
        public bool InsertOrUpdateSubscription(Subscription subscription)
        {
            try
            {
                if (subscription.Id > 0)
                {
                    _ctx.Subscriptions.Attach(subscription);
                    _ctx.Entry(subscription).State = EntityState.Modified;

                }
                else
                {
                    subscription.CreatedOn = DateTime.UtcNow;

                    _ctx.Subscriptions.Add(subscription);
                    //_ctx.Entry(subscription).State = EntityState.Unchanged;
                }

                _ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return false;
            }
        }

        public Subscription GetSubscriptionByUserId(string userId)
        {
            return _ctx.Subscriptions.Where(r => r.UserId == userId).FirstOrDefault();
        }

        public Subscription GetSubscriptionByCompanyId(int id)
        {
            return _ctx.Subscriptions.Where(r => r.CompanyId == id).FirstOrDefault();
        }

        #endregion

        public void Dispose()
        {
            _ctx.Dispose();
        }


    }
}

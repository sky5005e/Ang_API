using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFinancial.Data
{
    public interface IBSFinancialRepository
    {
        IQueryable<Company> GetCompanies(int companyId);
        User GetUserByEmail(string email);
        IQueryable<User> GetUsersByCompany(int companyId);
        IQueryable<Property> GetPropertiesByCompany(int companyId);
        IQueryable<PropertyValue> GetPropertyValuesByProperty(int propertyId);
        IQueryable<ContactModel> GetInquiriesByCompany(int companyId);
        ContactModel GetInquiry(int inquiryId);
        IQueryable<Applicant> GetApplicantsByCompany(int companyId);
        Applicant GetApplicant(int applicantId);
        bool Save();
        bool AddCompany(Company newCompany);
        bool AddInquiry(ContactModel newInquiry);
        bool InsertOrUpdateApplicant(Applicant applicant);

        IQueryable<Loan> GetLoansByCompany(int companyId);
        Loan GetLoan(int loanId);
        bool InsertOrUpdateLoan(Loan newLoan);

        List<Application> GetApplicationsByCompany(int companyId);
        Application GetApplication(int appId);
        bool InsertOrUpdateApplication(Application newApp);

        IQueryable<Payment> GetPaymentsOfLoan(int loanId);
        bool InsertOrUpdatePayment(Payment payment);

        List<Employment> GetEmploymentsOfApp(int applicationId);
        bool InsertOrUpdateEmployment(Employment employment);

        List<ApplicantAddr> GetApplicantAddrs(int applicationId);
        bool InsertOrUpdateApplicantAddr(ApplicantAddr addr);

    }
}

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFinancial.Data
{
    public class BSFinancialContext : DbContext
    {
        public BSFinancialContext()
            : base("DefaultConnection")
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;

            Database.SetInitializer(
                new MigrateDatabaseToLatestVersion<BSFinancialContext, BSFinancialMigrationsConfiguration>()
                );
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //             modelBuilder.Entity<Application>()
            //                         .HasRequired(m => m.applicant)
            //                         .WithMany(t => t.Applicants)
            //                         .HasForeignKey(m => m.ApplicantId)
            //                         .WillCascadeOnDelete(false);
            // 
            //             modelBuilder.Entity<Application>()
            //                         .HasRequired(m => m.co_applicant)
            //                         .WithMany(t => t.CoApplicants)
            //                         .HasForeignKey(m => m.CoApplicantId)
            //                         .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }


        public DbSet<Company> Companies { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<PropertyValue> PropertyValues { get; set; }
        public DbSet<ContactModel> Inquiries { get; set; }
        public DbSet<Applicant> Applicants { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Employment> Employments { get; set; }
        public DbSet<ApplicantAddr> ApplicantAddrs { get; set; }
        public DbSet<InvitedUser> InvitedUsers { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Fund> Funds { get; set; }
        public DbSet<Investor> Investors { get; set; }
        public DbSet<FundInvestor> FundInvestors { get; set; }
        public DbSet<Disbursement> Disbursements { get; set; }
        public DbSet<Distribution> Distributions { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<PropertyDocument> PropertyDocuments { get; set; }
        public DbSet<PropertyPhoto> PropertyPhotos { get; set; }
        public DbSet<Prospect> Prospects { get; set; }
        public DbSet<ProposalProspects> ProposalProspects { get; set; }
        public DbSet<ProposalProperty> ProposalProperties { get; set; }
        public DbSet<PropertyViewInfo> PropertyViewInfos { get; set; }
        public DbSet<PropertyRating> PropertyRatings { get; set; }
    }
}

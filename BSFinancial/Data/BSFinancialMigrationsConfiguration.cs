using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace BSFinancial.Data
{
    class BSFinancialMigrationsConfiguration 
        : DbMigrationsConfiguration<BSFinancialContext>
    {
        public BSFinancialMigrationsConfiguration()
        {
            this.AutomaticMigrationDataLossAllowed = true;
            this.AutomaticMigrationsEnabled = true;
            this.ContextKey = "BSFinancial.Data.BSFinancialContext";
        }

        protected override void Seed(BSFinancialContext context)
        {
            base.Seed(context);

#if DEBUG
            if (context.Companies.Count() == 0)
            {
                //var company1 = new Company()
                //{
                //    Name = "Dheeraj and Kristi Singal",
                //    CreatedOn = DateTime.UtcNow,
                //    IsDeleted = false,
                //    Users = new List<User>()
                //    {
                //        new User()
                //        {
                //            Email = "dsingal@fsscommerce.com",
                //            FirstName = "Dheeraj",
                //            LastName = "Singal"
                //        }
                //    },
                //    Properties = new List<Property>()
                //    {
                //        new Property() {
                //            Address = "4436 Valerie St",
                //            City = "Bellaire",
                //            State = "Texas",
                //            Zip = "77401"
                //        }
                //    }
                //};
                //context.Companies.Add(company1);

                var company2 = new Company()
                {
                    Name = "Singal Properties, LLC",
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false,
                    Users = new List<User>()
                    {
                        new User()
                        {
                            Email = "dheeraj@singal.net",
                            FirstName = "Dheeraj",
                            LastName = "Singal"
                        }
                    },
                    Properties = new List<Property>() 
                    {
                        new Property()
                        {
                            Address = "5722 Cheltenham Drive",
                            City = "Houston",
                            State = "Texas",
                            Zip = "77096",
                            HCADValues = new List<PropertyValue>()
                            {
                                new PropertyValue()
                                {
                                    MarketValue = 247542,
                                    Year = 2014
                                },
                                new PropertyValue()
                                {
                                    MarketValue = 221685,
                                    Year = 2013
                                },
                                new PropertyValue()
                                {
                                    MarketValue = 221685,
                                    Year = 2012
                                },
                                new PropertyValue()
                                {
                                    MarketValue = 221685,
                                    Year = 2011
                                }
                            }
                        },
                        new Property()
                        {
                            Address = "6019 Claridge",
                            City = "Houston",
                            State = "Texas",
                            Zip = "77096",
                            HCADValues = new List<PropertyValue>()
                            {
                                new PropertyValue()
                                {
                                    MarketValue = 197466,
                                    Year = 2014
                                },
                                new PropertyValue()
                                {
                                    MarketValue = 163677,
                                    Year = 2013
                                },
                                new PropertyValue()
                                {
                                    MarketValue = 156620,
                                    Year = 2012
                                },
                                new PropertyValue()
                                {
                                    MarketValue = 156620,
                                    Year = 2011
                                }
                            }
                        }
                    }
                };
                context.Companies.Add(company2);

                try
                {
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    var msg = ex.Message;
                    Debug.WriteLine(ex.Message);
                }
            }
#endif
        }
    }
}

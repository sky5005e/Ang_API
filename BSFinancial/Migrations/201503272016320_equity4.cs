namespace BSFinancial.Data
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class equity4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Loans", "DebtRate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Loans", "Equity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Loans", "Debt", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Loans", "Variable", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.Loans", "DebtRate");
            DropColumn("dbo.Loans", "Debt");
            DropColumn("dbo.Loans", "Equity");
            DropColumn("dbo.Loans", "Variable");
        }
    }
}

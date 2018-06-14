namespace MultichoiceCollection.Models.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReportModelDeleted : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Transactions", "accountNumber", c => c.String());
            AddColumn("dbo.Transactions", "transactionDate", c => c.String());
            AddColumn("dbo.Transactions", "apiClientId", c => c.String());
            AddColumn("dbo.Transactions", "customerNumber", c => c.String());
            AddColumn("dbo.Transactions", "deviceNumber", c => c.String());
            AddColumn("dbo.Transactions", "packageCode", c => c.String());
            AddColumn("dbo.Transactions", "businessType", c => c.String());
            AddColumn("dbo.Transactions", "emailAddress", c => c.String());
            AddColumn("dbo.Transactions", "mobileNumber", c => c.String());
            AddColumn("dbo.Transactions", "amount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Transactions", "transFees", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Transactions", "posted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Transactions", "auditReferenceNo", c => c.String());
            AddColumn("dbo.Transactions", "success", c => c.Boolean(nullable: false));
            AddColumn("dbo.Transactions", "url", c => c.String());
            AddColumn("dbo.Transactions", "CreatedDate", c => c.DateTime(nullable: false));
            DropTable("dbo.Reports");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Reports",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id);
            
            DropColumn("dbo.Transactions", "CreatedDate");
            DropColumn("dbo.Transactions", "url");
            DropColumn("dbo.Transactions", "success");
            DropColumn("dbo.Transactions", "auditReferenceNo");
            DropColumn("dbo.Transactions", "posted");
            DropColumn("dbo.Transactions", "transFees");
            DropColumn("dbo.Transactions", "amount");
            DropColumn("dbo.Transactions", "mobileNumber");
            DropColumn("dbo.Transactions", "emailAddress");
            DropColumn("dbo.Transactions", "businessType");
            DropColumn("dbo.Transactions", "packageCode");
            DropColumn("dbo.Transactions", "deviceNumber");
            DropColumn("dbo.Transactions", "customerNumber");
            DropColumn("dbo.Transactions", "apiClientId");
            DropColumn("dbo.Transactions", "transactionDate");
            DropColumn("dbo.Transactions", "accountNumber");
        }
    }
}

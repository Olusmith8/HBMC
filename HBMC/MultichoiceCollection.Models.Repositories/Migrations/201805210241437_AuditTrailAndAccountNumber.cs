namespace MultichoiceCollection.Models.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AuditTrailAndAccountNumber : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AuditTrails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Title = c.String(),
                        Detail = c.String(),
                        RefUrl = c.String(),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            AddColumn("dbo.AspNetUsers", "AccountNumber", c => c.String());
            DropColumn("dbo.AspNetUsers", "AgentTill");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "AgentTill", c => c.String());
            DropForeignKey("dbo.AuditTrails", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.AuditTrails", new[] { "UserId" });
            DropColumn("dbo.AspNetUsers", "AccountNumber");
            DropTable("dbo.AuditTrails");
        }
    }
}

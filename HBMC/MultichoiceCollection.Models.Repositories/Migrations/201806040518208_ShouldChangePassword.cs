namespace MultichoiceCollection.Models.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ShouldChangePassword : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "ShouldChangePassword", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "ShouldChangePassword");
        }
    }
}

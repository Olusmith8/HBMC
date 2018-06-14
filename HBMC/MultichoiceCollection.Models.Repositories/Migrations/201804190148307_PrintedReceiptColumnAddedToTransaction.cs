namespace MultichoiceCollection.Models.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PrintedReceiptColumnAddedToTransaction : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Transactions", "PrintedReceipt", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Transactions", "PrintedReceipt");
        }
    }
}

namespace FileUpload.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Code : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Transaction", "Code", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Transaction", "Code");
        }
    }
}

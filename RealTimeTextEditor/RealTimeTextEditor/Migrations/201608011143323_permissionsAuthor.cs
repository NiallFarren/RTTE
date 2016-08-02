namespace RealTimeTextEditor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class permissionsAuthor : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DocPermissions", "Author", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DocPermissions", "Author");
        }
    }
}

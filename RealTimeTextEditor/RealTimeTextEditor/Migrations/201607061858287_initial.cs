namespace RealTimeTextEditor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DocPermissions",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        DocumentID = c.Int(nullable: false),
                        Email = c.String(),
                        Read = c.Boolean(nullable: false),
                        Edit = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Documents", t => t.DocumentID, cascadeDelete: true)
                .Index(t => t.DocumentID);
            
            CreateTable(
                "dbo.Documents",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserID = c.String(),
                        AuthorName = c.String(),
                        Title = c.String(),
                        Public = c.Boolean(nullable: false),
                        _Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DocPermissions", "DocumentID", "dbo.Documents");
            DropIndex("dbo.DocPermissions", new[] { "DocumentID" });
            DropTable("dbo.Documents");
            DropTable("dbo.DocPermissions");
        }
    }
}

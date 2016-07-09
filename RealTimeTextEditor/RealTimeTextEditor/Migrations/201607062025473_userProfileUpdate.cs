namespace RealTimeTextEditor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userProfileUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserProfiles", "UserID", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserProfiles", "UserID");
        }
    }
}

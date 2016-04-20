namespace Download_Vouchery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class auto_increment_id : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Vouchers");
            AlterColumn("dbo.Vouchers", "VoucherId", c => c.Guid(nullable: false, identity: true));
            AddPrimaryKey("dbo.Vouchers", "VoucherId");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.Vouchers");
            AlterColumn("dbo.Vouchers", "VoucherId", c => c.Guid(nullable: false));
            AddPrimaryKey("dbo.Vouchers", "VoucherId");
        }
    }
}

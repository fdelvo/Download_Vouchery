namespace Download_Vouchery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Vouchers", "VoucherRedemptionDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Vouchers", "VoucherRedemptionDate", c => c.DateTime(nullable: false));
        }
    }
}

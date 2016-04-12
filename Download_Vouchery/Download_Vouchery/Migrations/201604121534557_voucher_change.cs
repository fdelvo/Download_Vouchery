namespace Download_Vouchery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class voucher_change : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Vouchers", "VoucherFileId", c => c.Guid(nullable: false));
            DropColumn("dbo.Vouchers", "VoucherFilePath");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Vouchers", "VoucherFilePath", c => c.String(nullable: false));
            DropColumn("dbo.Vouchers", "VoucherFileId");
        }
    }
}

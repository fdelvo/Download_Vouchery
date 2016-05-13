namespace Download_Vouchery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class voucher_counter : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Vouchers", "VoucherRedemptionCounter", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Vouchers", "VoucherRedemptionCounter");
        }
    }
}

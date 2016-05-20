namespace Download_Vouchery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class online_vouchers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Vouchers", "VoucherOnlineShared", c => c.Boolean());
            AddColumn("dbo.Vouchers", "Discriminator", c => c.String(nullable: false, maxLength: 128));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Vouchers", "Discriminator");
            DropColumn("dbo.Vouchers", "VoucherOnlineShared");
        }
    }
}

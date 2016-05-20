namespace Download_Vouchery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class online_vouchers_changes : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Vouchers", newName: "OnlineVouchers");
            DropIndex("dbo.OnlineVouchers", new[] { "VoucherFileId_FileId" });
            DropPrimaryKey("dbo.OnlineVouchers");
            CreateTable(
                "dbo.Vouchers",
                c => new
                    {
                        VoucherId = c.Guid(nullable: false, identity: true),
                        VoucherCode = c.String(nullable: false),
                        VoucherRedeemed = c.Boolean(nullable: false),
                        VoucherCreationDate = c.DateTime(nullable: false),
                        VoucherRedemptionDate = c.DateTime(),
                        VoucherRedemptionCounter = c.Int(nullable: false),
                        VoucherFileId_FileId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.VoucherId)
                .Index(t => t.VoucherFileId_FileId);
            
            AddColumn("dbo.OnlineVouchers", "OnlineVoucherId", c => c.Guid(nullable: false, identity: true));
            AddColumn("dbo.OnlineVouchers", "OnlineVoucherCode", c => c.String(nullable: false));
            AddColumn("dbo.OnlineVouchers", "OnlineVoucherRedeemed", c => c.Boolean(nullable: false));
            AddColumn("dbo.OnlineVouchers", "OnlineVoucherCreationDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.OnlineVouchers", "OnlineVoucherRedemptionDate", c => c.DateTime());
            AddColumn("dbo.OnlineVouchers", "OnlineVoucherRedemptionCounter", c => c.Int(nullable: false));
            AddColumn("dbo.OnlineVouchers", "OnlineVoucherShared", c => c.Boolean(nullable: false));
            AddColumn("dbo.OnlineVouchers", "OnlineVoucherFileId_FileId", c => c.Guid());
            AddPrimaryKey("dbo.OnlineVouchers", "OnlineVoucherId");
            CreateIndex("dbo.OnlineVouchers", "OnlineVoucherFileId_FileId");
            AddForeignKey("dbo.OnlineVouchers", "OnlineVoucherFileId_FileId", "dbo.BlobUploadModels", "FileId");
            DropColumn("dbo.OnlineVouchers", "VoucherId");
            DropColumn("dbo.OnlineVouchers", "VoucherCode");
            DropColumn("dbo.OnlineVouchers", "VoucherRedeemed");
            DropColumn("dbo.OnlineVouchers", "VoucherCreationDate");
            DropColumn("dbo.OnlineVouchers", "VoucherRedemptionDate");
            DropColumn("dbo.OnlineVouchers", "VoucherRedemptionCounter");
            DropColumn("dbo.OnlineVouchers", "VoucherOnlineShared");
            DropColumn("dbo.OnlineVouchers", "Discriminator");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OnlineVouchers", "VoucherFileId_FileId", c => c.Guid(nullable: false));
            AddColumn("dbo.OnlineVouchers", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.OnlineVouchers", "VoucherOnlineShared", c => c.Boolean());
            AddColumn("dbo.OnlineVouchers", "VoucherRedemptionCounter", c => c.Int(nullable: false));
            AddColumn("dbo.OnlineVouchers", "VoucherRedemptionDate", c => c.DateTime());
            AddColumn("dbo.OnlineVouchers", "VoucherCreationDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.OnlineVouchers", "VoucherRedeemed", c => c.Boolean(nullable: false));
            AddColumn("dbo.OnlineVouchers", "VoucherCode", c => c.String(nullable: false));
            AddColumn("dbo.OnlineVouchers", "VoucherId", c => c.Guid(nullable: false, identity: true));
            DropForeignKey("dbo.OnlineVouchers", "OnlineVoucherFileId_FileId", "dbo.BlobUploadModels");
            DropIndex("dbo.Vouchers", new[] { "VoucherFileId_FileId" });
            DropIndex("dbo.OnlineVouchers", new[] { "OnlineVoucherFileId_FileId" });
            DropPrimaryKey("dbo.OnlineVouchers");
            DropColumn("dbo.OnlineVouchers", "OnlineVoucherFileId_FileId");
            DropColumn("dbo.OnlineVouchers", "OnlineVoucherShared");
            DropColumn("dbo.OnlineVouchers", "OnlineVoucherRedemptionCounter");
            DropColumn("dbo.OnlineVouchers", "OnlineVoucherRedemptionDate");
            DropColumn("dbo.OnlineVouchers", "OnlineVoucherCreationDate");
            DropColumn("dbo.OnlineVouchers", "OnlineVoucherRedeemed");
            DropColumn("dbo.OnlineVouchers", "OnlineVoucherCode");
            DropColumn("dbo.OnlineVouchers", "OnlineVoucherId");
            DropTable("dbo.Vouchers");
            AddPrimaryKey("dbo.OnlineVouchers", "VoucherId");
            CreateIndex("dbo.OnlineVouchers", "VoucherFileId_FileId");
            RenameTable(name: "dbo.OnlineVouchers", newName: "Vouchers");
        }
    }
}

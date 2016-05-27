namespace Download_Vouchery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class remove_online_vouchers : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OnlineVouchers", "OnlineVoucherFileId_FileId", "dbo.BlobUploadModels");
            DropIndex("dbo.OnlineVouchers", new[] { "OnlineVoucherFileId_FileId" });
            DropTable("dbo.OnlineVouchers");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.OnlineVouchers",
                c => new
                    {
                        OnlineVoucherId = c.Guid(nullable: false, identity: true),
                        OnlineVoucherCode = c.String(nullable: false),
                        OnlineVoucherRedeemed = c.Boolean(nullable: false),
                        OnlineVoucherCreationDate = c.DateTime(nullable: false),
                        OnlineVoucherRedemptionDate = c.DateTime(),
                        OnlineVoucherRedemptionCounter = c.Int(nullable: false),
                        OnlineVoucherShared = c.Boolean(nullable: false),
                        OnlineVoucherFileId_FileId = c.Guid(),
                    })
                .PrimaryKey(t => t.OnlineVoucherId);
            
            CreateIndex("dbo.OnlineVouchers", "OnlineVoucherFileId_FileId");
            AddForeignKey("dbo.OnlineVouchers", "OnlineVoucherFileId_FileId", "dbo.BlobUploadModels", "FileId");
        }
    }
}

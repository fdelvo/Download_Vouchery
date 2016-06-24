namespace Download_Vouchery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class onlinevouchers : DbMigration
    {
        public override void Up()
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
                        OnlineVoucherEmail = c.String(),
                        OnlineVoucherFileId_FileId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.OnlineVoucherId)
                .ForeignKey("dbo.BlobUploadModels", t => t.OnlineVoucherFileId_FileId, cascadeDelete: true)
                .Index(t => t.OnlineVoucherFileId_FileId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OnlineVouchers", "OnlineVoucherFileId_FileId", "dbo.BlobUploadModels");
            DropIndex("dbo.OnlineVouchers", new[] { "OnlineVoucherFileId_FileId" });
            DropTable("dbo.OnlineVouchers");
        }
    }
}

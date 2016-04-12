namespace Download_Vouchery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class voucher_change_1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BlobUploadModels", "FileOwner_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.Vouchers", "VoucherFileId_FileId", c => c.Guid(nullable: false));
            CreateIndex("dbo.BlobUploadModels", "FileOwner_Id");
            CreateIndex("dbo.Vouchers", "VoucherFileId_FileId");
            AddForeignKey("dbo.BlobUploadModels", "FileOwner_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Vouchers", "VoucherFileId_FileId", "dbo.BlobUploadModels", "FileId", cascadeDelete: true);
            DropColumn("dbo.Vouchers", "VoucherFileId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Vouchers", "VoucherFileId", c => c.Guid(nullable: false));
            DropForeignKey("dbo.Vouchers", "VoucherFileId_FileId", "dbo.BlobUploadModels");
            DropForeignKey("dbo.BlobUploadModels", "FileOwner_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Vouchers", new[] { "VoucherFileId_FileId" });
            DropIndex("dbo.BlobUploadModels", new[] { "FileOwner_Id" });
            DropColumn("dbo.Vouchers", "VoucherFileId_FileId");
            DropColumn("dbo.BlobUploadModels", "FileOwner_Id");
        }
    }
}

namespace Download_Vouchery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class blobs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BlobDownloadModels",
                c => new
                    {
                        BlobId = c.Guid(nullable: false),
                        BlobFileName = c.String(),
                        BlobContentType = c.String(),
                        BlobLength = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.BlobId);
            
            CreateTable(
                "dbo.BlobUploadModels",
                c => new
                    {
                        FileId = c.Guid(nullable: false),
                        FileName = c.String(),
                        FileUrl = c.String(),
                        FileSizeInBytes = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.FileId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.BlobUploadModels");
            DropTable("dbo.BlobDownloadModels");
        }
    }
}

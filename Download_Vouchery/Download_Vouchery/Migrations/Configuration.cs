namespace Download_Vouchery.Migrations
{
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Download_Vouchery.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Download_Vouchery.Models.ApplicationDbContext context)
        {
            /*context.Vouchers.AddOrUpdate(
                v => v.VoucherId,
                new Voucher { VoucherId = Guid.NewGuid(), VoucherCode = "ABC123", VoucherCreationDate = DateTime.Now, VoucherFileId = context.BlobUploadModels.Find(new Guid("1afc7d8c-cba7-4917-9a57-3ed7bd71e4a7")), VoucherRedeemed = false },
                new Voucher { VoucherId = Guid.NewGuid(), VoucherCode = "DEF456", VoucherCreationDate = DateTime.Now, VoucherFileId = context.BlobUploadModels.Find(new Guid("1afc7d8c-cba7-4917-9a57-3ed7bd71e4a7")), VoucherRedeemed = false },
                new Voucher { VoucherId = Guid.NewGuid(), VoucherCode = "GHI789", VoucherCreationDate = DateTime.Now, VoucherFileId = context.BlobUploadModels.Find(new Guid("1afc7d8c-cba7-4917-9a57-3ed7bd71e4a7")), VoucherRedeemed = false },
                new Voucher { VoucherId = Guid.NewGuid(), VoucherCode = "JKL123", VoucherCreationDate = DateTime.Now, VoucherFileId = context.BlobUploadModels.Find(new Guid("1afc7d8c-cba7-4917-9a57-3ed7bd71e4a7")), VoucherRedeemed = false },
                new Voucher { VoucherId = Guid.NewGuid(), VoucherCode = "MNO456", VoucherCreationDate = DateTime.Now, VoucherFileId = context.BlobUploadModels.Find(new Guid("1afc7d8c-cba7-4917-9a57-3ed7bd71e4a7")), VoucherRedeemed = false }
                );*/
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}

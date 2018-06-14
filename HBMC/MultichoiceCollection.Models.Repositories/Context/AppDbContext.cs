using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.Validation;
using System.Text;
using System.Threading;
using System.Web;
using MultichoiceCollection.Common.Entities;
using MultichoiceCollection.Common.Entities.Base;

namespace MultichoiceCollection.Models.Repositories.Context
{
    public class AppDbContext: ApplicationDbContext
    {
        public AppDbContext()
        {
        
        }
#region Entities
        public IDbSet<Transaction> Transactions { get; set; }
        public IDbSet<AuditTrail> AuditTrails { get; set; }

        #endregion
    


        #region save

        public override int SaveChanges()
        {
            try
            {

                //var modifiedTransactionHistoryEntries = ChangeTracker.Entries<TransactionHistoryBase>();
                //if (modifiedTransactionHistoryEntries.Any())
                //{
                //    var ctx = ((IObjectContextAdapter)this).ObjectContext;
                //    var appDbContextChangeTracker = (AppDbContext)ctx.InterceptionContext.DbContexts.FirstOrDefault(p => p.GetType() == typeof(AppDbContext));
                //    if (appDbContextChangeTracker != null)
                //        UpdateTransactionHistoryEntries(modifiedTransactionHistoryEntries, appDbContextChangeTracker);

                //}
                var modifiedEntries = ChangeTracker.Entries<IAuditableEntity>();
                UpdateAuditableEntities(modifiedEntries);
                return base.SaveChanges();
            }
            catch
                (DbEntityValidationException dbEx)
            {
               var errorStr=new StringBuilder();
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                         errorStr.Append(string.Format("Property: {0} Error: {1}",
                            validationError.PropertyName,
                            validationError.ErrorMessage));
                    }
                }

                throw new Exception("Entity validation errors: "+ errorStr);  // You can also choose to handle the exception here...
            }
        }



      private void UpdateAuditableEntities(IEnumerable<DbEntityEntry<IAuditableEntity>> modifiedEntries)
        {
            foreach (var entry in modifiedEntries)
            {
                var entity = entry.Entity;
                if (entity == null) continue;
                var identityName = Thread.CurrentPrincipal.Identity.Name;
                var now = DateTime.UtcNow;
                var ip = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress : "";
                if (entry.State == EntityState.Added)
                {
                    entity.CreatedBy = identityName;
                    entity.CreatedDate = now;
                    entity.IP = ip;
                }
                else
                {
                    base.Entry(entity).Property(x => x.CreatedBy).IsModified = false;
                    base.Entry(entity).Property(x => x.CreatedDate).IsModified = false;
                }
             
                entity.UpdatedBy = identityName;
                entity.UpdatedDate = now;
            }
        }
        //private void UpdateTransactionHistoryEntries(IEnumerable<DbEntityEntry<TransactionHistoryBase>> modifiedTransactionHistoryEntries
        //    , AppDbContext appDbContextChangeTracker)
        //{
        //    //if (!HttpContext.Current.User.Identity.IsAuthenticated)
        //    //    throw new Exception("You must be logged in to carry out this transaction.");
        //  //Transaction history Inherited classes
        //    foreach (var entry in modifiedTransactionHistoryEntries)
        //    {
        //        if (entry.Entity == null) continue;
        //        if (entry.Entity.GetType() == typeof(Wallet))
        //        {
        //            var wallet = entry.Entity as Wallet;
        //            if (wallet == null) continue;
        //            var transactionHistory = new TransactionHistory
        //            {
        //                FromUserId = Thread.CurrentPrincipal.Identity.GetUserId(),
        //                TransactionAmount = wallet.WalletType == WalletType.Credit ? wallet.LastCreditedAmount : wallet.WalletType == WalletType.Debit ? wallet.LastDebitedAmount : 0,
        //                Type = wallet.WalletType == WalletType.Credit ? TransactionHistoryType.WalletCredit : TransactionHistoryType.WalletDebit,
        //            };
        //            appDbContextChangeTracker.TransactionHistoryHistories.Add(transactionHistory);
        //        }
        //        else if (entry.Entity.GetType() == typeof(TransferFundAirtimeToWallet))
        //        {

        //        }
        //    }
        //}
        #endregion

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<ApplicationUser>()
                .Property(t => t.Id)
                .IsRequired()
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(
                        new IndexAttribute("IX_ID") {IsUnique = true}));
            //      modelBuilder
            //.Entity<ApplicationUser>().Property(t => t.IdentityCode)
            //.IsRequired()
            //.HasMaxLength(50)
            //.HasColumnAnnotation(
            //    IndexAnnotation.AnnotationName,
            //    new IndexAnnotation(
            //        new IndexAttribute("IX_IdentityCode", 1) { IsUnique = true }));

            //modelBuilder
            //    .Entity<Merchant>()
            //    .HasRequired(marchent => marchent.User)
            //    .WithOptional(user => user.Merchant);

            /*   modelBuilder
        .Entity<Wallet>().Property(t => t.UserId)
        .IsRequired()
        .HasMaxLength(50)
        .HasColumnAnnotation(
            IndexAnnotation.AnnotationName,
            new IndexAnnotation(
                new IndexAttribute("IX_UserId", 1) { IsUnique = true }));*/
            base.OnModelCreating(modelBuilder);

          
        }
        
    }

}
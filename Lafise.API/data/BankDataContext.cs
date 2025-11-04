using System;
using System.Linq;
using System.Threading.Tasks;
using Lafise.API.data.model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Lafise.API.data
{
    // BankDataContext.cs
    public class BankDataContext : DbContext
    {

        public BankDataContext(DbContextOptions<BankDataContext> options) : base(options)
        {
        }

        // Definiciones de las tablas
        public DbSet<Client> Clients { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }




        #region Sobrecargas de SaveChanges (Auditoría)

        public void AddOrUpdate(IEntity entity)
        {
            if (string.IsNullOrWhiteSpace(entity.Id))
                Add(entity);
            else
                Update(entity);
        }

        public override EntityEntry<TEntity> Add<TEntity>(TEntity entity)
        {
            //Asignar Id
            if (entity is IEntity)
                (entity as IEntity).Id = Guid.NewGuid().ToString().ToLower();

            return base.Add(entity);
        }


        public override int SaveChanges()
        {
            AuditChanges();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            AuditChanges();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AuditChanges();
            return await base.SaveChangesAsync(cancellationToken);
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            AuditChanges();
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void AuditChanges()
        {
            var addedAuditedEntities = ChangeTracker.Entries<IAuditable>()
              .Where(p => p.State == EntityState.Added)
              .Select(p => p.Entity);

            var modifiedAuditedEntities = ChangeTracker.Entries<IAuditable>()
              .Where(p => p.State == EntityState.Modified)
              .Select(p => p.Entity);

            var now = DateTime.UtcNow;
            foreach (var added in addedAuditedEntities)
            {
                if (added is IEntity)
                {
                    IEntity addedEntity = (IEntity)added;
                    if (string.IsNullOrEmpty(addedEntity.Id))
                        addedEntity.Id = Guid.NewGuid().ToString().ToLower();
                }

                added.DateCreated = now;
            }

            foreach (var modified in modifiedAuditedEntities)
            {
                modified.DateModified = now;
            }
        }


        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ModelBuilderExtensions.RemovePluralizingTableNameConvention(modelBuilder);

            modelBuilder.Entity<Account>()
                .HasIndex(a => a.AccountNumber)
                .IsUnique();

            modelBuilder.Entity<Client>()
                .HasMany(c => c.Accounts)
                .WithOne(a => a.Client)
                    .HasForeignKey(a => a.ClientId);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.Transactions)
                .WithOne(t => t.Account)
                .HasForeignKey(t => t.AccountId);


        }
    }
}
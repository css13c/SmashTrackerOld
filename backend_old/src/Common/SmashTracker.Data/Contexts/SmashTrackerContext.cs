using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using SmashTracker.Data.Entities;
using SmashTracker.Data.Helpers;
using SmashTracker.Data.Interfaces;

namespace SmashTracker.Data.Contexts
{
    public class SmashTrackerContext : DbContext
    {
        #region Tables
        public DbSet<Player> Players { get; set; }

        public DbSet<Game> Games { get; set; }

        public DbSet<GameCharacter> GameCharacters { get; set; }

        public DbSet<GamePlayerAssociation> GamePlayerAssociations { get; set; }

        public DbSet<GamePlayerCharacter> GamePlayerCharacters { get; set; }
        #endregion

        #region Method Overrides
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Player Indexes
            modelBuilder.Entity<Player>().HasIndex(x => x.Email);

            // GameCharacter Indexes
            modelBuilder.Entity<GameCharacter>().HasIndex(x => x.GameId);

            // GamePlayerAssociation Indexes
            modelBuilder.Entity<GamePlayerAssociation>().HasIndex(x => x.GameId);
            modelBuilder.Entity<GamePlayerAssociation>().HasIndex(x => x.PlayerId);

            // GamePlayerCharacter Indexes
            modelBuilder.Entity<GamePlayerCharacter>().HasIndex(x => x.GamePlayerAssociationId);
            modelBuilder.Entity<GamePlayerCharacter>().HasIndex(x => x.GameCharacterId);
        }

        public override int SaveChanges()
        {
            PreSaveChanges();
            return base.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            CancellationToken cancellationToken = new CancellationToken();
            return await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            PreSaveChanges();
            return await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        #endregion

        #region Private Methods
        private void PreSaveChanges()
        {
            var changes = ChangeTracker.Entries().Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);
            foreach (var change in changes)
            {
                var entity = (IEntity)change.Entity;
                var auditableEntity = (IAuditable)change.Entity;
                if (change.State == EntityState.Added)
                {
                    if (entity.Id == Guid.Empty)
                    {
                        entity.Id = SequentialGuid.NewSequentialGuid();
                    }
                    if (auditableEntity.CreatedOn == null)
                    {
                        auditableEntity.CreatedOn = DateTimeOffset.UtcNow;
                    }
                }

                auditableEntity.ModifiedOn = DateTimeOffset.UtcNow;
            }
        }
        #endregion
    }
}

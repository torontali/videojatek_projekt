using Microsoft.EntityFrameworkCore;

namespace backend.Models
{
    public class Application : DbContext
    {
        public Application(DbContextOptions<Application> options) : base(options)
        {
        }

        public DbSet<Game> Games => Set<Game>();
        public DbSet<GameType> Types => Set<GameType>();
        public DbSet<player> Players => Set<player>();
        public DbSet<Rewiev> Reviews => Set<Rewiev>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Game>(entity =>
            {
                entity.ToTable("Games");
                entity.HasKey(game => game.Id);
                entity.Property(game => game.Id).ValueGeneratedOnAdd();
                entity.Property(game => game.Name).HasMaxLength(150).IsRequired();
                entity.Property(game => game.Company).HasMaxLength(100);
                entity.Property(game => game.Platform).HasMaxLength(50);
                entity.Property(game => game.ReleaseDate).HasMaxLength(30);
                entity.Property(game => game.CoverImage).HasMaxLength(255);
                entity.Property(game => game.Votes).HasDefaultValue(0);

                entity.HasOne(game => game.GameType)
                    .WithMany(type => type.Games)
                    .HasForeignKey(game => game.TypeId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<GameType>(entity =>
            {
                entity.ToTable("Types");
                entity.HasKey(type => type.Id);
                entity.Property(type => type.Id).ValueGeneratedOnAdd();
                entity.Property(type => type.Name).HasMaxLength(50).IsRequired();
                entity.HasIndex(type => type.Name).IsUnique();
            });

            modelBuilder.Entity<player>(entity =>
            {
                entity.ToTable("Players");
                entity.HasKey(item => item.Id);
                entity.Property(item => item.Id).ValueGeneratedOnAdd();
                entity.Property(item => item.Username).HasMaxLength(50).IsRequired();
                entity.Property(item => item.Email).HasMaxLength(100).IsRequired();
                entity.Property(item => item.Level).HasDefaultValue(1);
                entity.HasIndex(item => item.Username).IsUnique();
                entity.HasIndex(item => item.Email).IsUnique();
            });

            modelBuilder.Entity<Rewiev>(entity =>
            {
                entity.ToTable("Reviews");
                entity.HasKey(review => review.Id);
                entity.Property(review => review.Id).ValueGeneratedOnAdd();
                entity.Property(review => review.Comment).HasMaxLength(500);
                entity.Property(review => review.ReviewReleaseDate)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAdd();

                entity.HasOne<player>()
                    .WithMany()
                    .HasForeignKey(review => review.PlayerId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<Game>()
                    .WithMany()
                    .HasForeignKey(review => review.GameId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}

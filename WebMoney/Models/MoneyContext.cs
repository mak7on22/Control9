using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace WebMoney.Models
{
    public class MoneyContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
		public DbSet<TransferHistory> TransferHistories { get; set; }
		public DbSet<Replenish> Replenishs { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Company> Companys { get; set; }
        public MoneyContext(DbContextOptions<MoneyContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TransferHistory>()
                 .HasOne(th => th.Receiver)
                 .WithMany()
                 .HasForeignKey(th => th.ReceiverId)
                 .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Company>().HasData(new Company { Id = 1, Name = "Газпром",Money = 800 });
            modelBuilder.Entity<Company>().HasData(new Company { Id = 2, Name = "Роснефть", Money = 1800 });
            modelBuilder.Entity<Company>().HasData(new Company { Id = 3, Name = "Лукойл", Money = 2800 });
            modelBuilder.Entity<Company>().HasData(new Company { Id = 4, Name = "Сбербанк", Money = 3800 });
            modelBuilder.Entity<Company>().HasData(new Company { Id = 5, Name = "Газпром нефть", Money = 4800 });
            modelBuilder.Entity<Company>().HasData(new Company { Id = 6, Name = "Транснефть", Money = 5800 });
            modelBuilder.Entity<Company>().HasData(new Company { Id = 7, Name = "Россети", Money = 6800 });
            modelBuilder.Entity<Company>().HasData(new Company { Id = 8, Name = "Аэрофлот", Money = 7800 });
            modelBuilder.Entity<Company>().HasData(new Company { Id = 9, Name = "Газпромбанк", Money = 8800 });
            modelBuilder.Entity<Company>().HasData(new Company { Id = 10, Name = "МТС", Money = 9800 });


        }
    }
}

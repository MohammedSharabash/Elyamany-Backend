namespace ElYamanyDashboard.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using ElYamanyDashboard.Models.Views;

    public partial class ElYamanyContext : DbContext
    {
        public ElYamanyContext()
            : base("name=ElYamanyContext")
        {
        }

        public virtual DbSet<Area> Areas { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<ContactUs> ContactUs { get; set; }
        public virtual DbSet<DeliveryType> DeliveryTypes { get; set; }
        public virtual DbSet<Gender> Genders { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<NotificationType> NotificationTypes { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderItem>  OrderItems { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<SystemAdmin> SystemAdmins { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<OrderStatus> OrderStatus { get; set; }
        public virtual DbSet<Cheque>  Cheques { get; set; }
        public virtual DbSet<UserSalary>   UserSalaries { get; set; }
        public virtual DbSet<UserCheque>    UserCheques { get; set; }
        public virtual DbSet<LevelsSetting>     LevelsSettings { get; set; }
        public virtual DbSet<PushToken> PushTokens { get; set; }
        public virtual DbSet<UserChequeViewForTarget> UserChequeViewForTargets { get; set; }
        public virtual DbSet<DailyUserSalary> DailyUserSalaries { get; set; }
        public virtual DbSet<UserChequeNote> UserChequeNotes { get; set; }
        public virtual DbSet<UserFavorite> Favorites { get; set; }







        public virtual DbSet<ProductView> ProductViews { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>()
                .Property(e => e.SubTotal)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Order>()
                .Property(e => e.DeliveryCost)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Order>()
                .Property(e => e.TotalPrice)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Product>()
                .Property(e => e.CurrentPrice)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Product>()
                .Property(e => e.OldPrice)
                .HasPrecision(19, 4);

            
        }

        public System.Data.Entity.DbSet<ElYamanyDashboard.Models.Setting> Settings { get; set; }

        public System.Data.Entity.DbSet<ElYamanyDashboard.Models.Views.UserChequeView> UserChequeViews { get; set; }
    }
}

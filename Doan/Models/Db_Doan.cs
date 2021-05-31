namespace Doan.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Db_Doan : DbContext
    {
        public Db_Doan()
            : base("name=Db_Doan")
        {
        }

        public virtual DbSet<__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<_Order> Orders { get; set; }
        public virtual DbSet<BestSale> BestSales { get; set; }
        public virtual DbSet<Blog> Blogs { get; set; }
        public virtual DbSet<CateCollection> CateCollections { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Colletion> Colletions { get; set; }
        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<Coupon> Coupons { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Credential> Credentials { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Footer> Footers { get; set; }
        public virtual DbSet<History> Histories { get; set; }
        public virtual DbSet<IVC> IVCs { get; set; }
        public virtual DbSet<IVCDetail> IVCDetails { get; set; }
        public virtual DbSet<Shipping> Shippings { get; set; }
        public virtual DbSet<Shipping_unit> Shipping_Units { get; set; }
        public virtual DbSet<Menu> Menus { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Payment> Payment { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Sale> Sales { get; set; }
        public virtual DbSet<Slide> Slides { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<TypeCu> TypeCus { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserGroup> UserGroups { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<_Order>()
                .Property(e => e.Password_cus)
                .IsUnicode(false);

            modelBuilder.Entity<_Order>()
                .Property(e => e.SDT_Cus)
                .IsFixedLength();

            modelBuilder.Entity<Blog>()
                .Property(e => e.ChangesBy)
                .IsFixedLength();

            modelBuilder.Entity<Contact>()
                .Property(e => e.FirstName)
                .IsFixedLength();

            modelBuilder.Entity<Contact>()
                .Property(e => e.LastName)
                .IsFixedLength();

            modelBuilder.Entity<Contact>()
                .Property(e => e.Email)
                .IsFixedLength();

            modelBuilder.Entity<Credential>()
                .Property(e => e.UserGroupID)
                .IsUnicode(false);

            modelBuilder.Entity<Credential>()
                .Property(e => e.RoleId)
                .IsUnicode(false);

            modelBuilder.Entity<Customer>()
                .Property(e => e.Phone_Cus)
                .IsUnicode(false);

            modelBuilder.Entity<Customer>()
                .Property(e => e.Password)
                .IsUnicode(false);

            modelBuilder.Entity<Customer>()
                .Property(e => e.FirstName)
                .IsFixedLength();

            modelBuilder.Entity<Customer>()
                .Property(e => e.LastName)
                .IsFixedLength();

            modelBuilder.Entity<Customer>()
                .HasMany(e => e.Histories)
                .WithOptional(e => e.Customer)
                .HasForeignKey(e => e.Customer_CodeCus);

            modelBuilder.Entity<IVC>()
                .HasMany(e => e.IVCDetails)
                .WithOptional(e => e.IVC)
                .HasForeignKey(e => e.Ivc_Id);

            modelBuilder.Entity<IVCDetail>()
                .Property(e => e.Price)
                .HasPrecision(18, 0);

            modelBuilder.Entity<OrderDetail>()
                .Property(e => e.UnitPriceSale)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Product>()
                .Property(e => e.Price)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Product>()
                .Property(e => e.Entryprice)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Product>()
                .Property(e => e.PromotionPrice)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Product>()
                .Property(e => e.Status)
                .IsFixedLength();

            modelBuilder.Entity<Product>()
                .HasMany(e => e.IVCDetails)
                .WithOptional(e => e.Product)
                .HasForeignKey(e => e.Product_IDProduct);

            modelBuilder.Entity<Role>()
                .Property(e => e.ID)
                .IsUnicode(false);

            modelBuilder.Entity<Supplier>()
                .HasMany(e => e.IVCs)
                .WithOptional(e => e.Supplier)
                .HasForeignKey(e => e.Supplier_SupplierID);

            modelBuilder.Entity<User>()
                .Property(e => e.Password)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.GroupID)
                .IsUnicode(false);

            modelBuilder.Entity<UserGroup>()
                .Property(e => e.Id)
                .IsUnicode(false);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using TechoWorld_DataModels_v2;
using TechoWorld_DataModels_v2.Entities;

#nullable disable

namespace TechnoWorld_API.Data
{
    public partial class TechnoWorldContext : DbContext
    {
        public TechnoWorldContext()
        {
        }

        public TechnoWorldContext(DbContextOptions<TechnoWorldContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Delivery> Deliveries { get; set; }
        public virtual DbSet<DeliveryStatus> DeliveryStatuses { get; set; }
        public virtual DbSet<ElectrnicsType> ElectrnicsTypes { get; set; }
        public virtual DbSet<Electronic> Electronics { get; set; }
        public virtual DbSet<ElectronicsToDelivery> ElectronicsToDeliveries { get; set; }
        public virtual DbSet<ElectronicsToStorage> ElectronicsToStorages { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Manufacturer> Manufacturers { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderElectronic> OrderElectronics { get; set; }
        public virtual DbSet<OrderStatus> OrderStatuses { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Storage> Storages { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=MRBORING\\SQLEXPRESS;Database=TechnoWorld;Trusted_Connection=True;MultipleActiveResultSets=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Cyrillic_General_CI_AS");

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.Property(e => e.Image).HasColumnType("image");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(80)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Delivery>(entity =>
            {
                entity.HasKey(e => e.DelivertId);

                entity.ToTable("Delivery");

                entity.HasIndex(e => e.DateOfDelivery, "IX_DeliveryDateOfDelivery");

                entity.HasIndex(e => e.DateOfDelivery, "IX_DeliveryDateOfOrder");

                entity.HasIndex(e => e.DeliveryNumber, "IX_DeliveryNumber");

                entity.HasIndex(e => e.StatusId, "IX_DeliveryStatus");

                entity.HasIndex(e => e.StorageId, "IX_DeliveryStorage");

                entity.HasIndex(e => e.SupplierId, "IX_DeliverySupplier");

                entity.Property(e => e.DateOfDelivery).HasColumnType("date");

                entity.Property(e => e.DateOfOrder).HasColumnType("date");

                entity.Property(e => e.DeliveryNumber)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.Deliveries)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Delivery_Employee");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Deliveries)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Delivery_DeliveryStatus");

                entity.HasOne(d => d.Storage)
                    .WithMany(p => p.Deliveries)
                    .HasForeignKey(d => d.StorageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Delivery_Storage");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.Deliveries)
                    .HasForeignKey(d => d.SupplierId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Delivery_Supplier");
            });

            modelBuilder.Entity<DeliveryStatus>(entity =>
            {
                entity.ToTable("DeliveryStatus");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ElectrnicsType>(entity =>
            {
                entity.HasKey(e => e.TypeId)
                    .HasName("PK_Type");

                entity.ToTable("ElectrnicsType");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.ElectrnicsTypes)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ElectrnicsType_Category");
            });

            modelBuilder.Entity<Electronic>(entity =>
            {
                entity.HasKey(e => e.ElectronicsId);

                entity.HasIndex(e => e.IsOfferedForSale, "IX_ElectronicsIsOfferedForSale");

                entity.HasIndex(e => e.ManufactrurerId, "IX_ElectronicsManufacturer");

                entity.HasIndex(e => e.Model, "IX_ElectronicsModel")
                    .IsUnique();

                entity.HasIndex(e => e.PurchasePrice, "IX_ElectronicsPurchasePrice");

                entity.HasIndex(e => e.SalePrice, "IX_ElectronicsSalePrice");

                entity.HasIndex(e => e.TypeId, "IX_ElectronicsType");

                entity.Property(e => e.Color)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.Image).HasColumnType("image");

                entity.Property(e => e.ManufacturerСountry)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Model)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PurchasePrice).HasColumnType("money");

                entity.Property(e => e.SalePrice).HasColumnType("money");

                entity.HasOne(d => d.Manufacturer)
                    .WithMany(p => p.Electronics)
                    .HasForeignKey(d => d.ManufactrurerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Electronics_Manufacturer");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.Electronics)
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Electronics_Type");
            });

            modelBuilder.Entity<ElectronicsToDelivery>(entity =>
            {
                entity.HasKey(e => new { e.ElectronicsId, e.DeliveryId });

                entity.ToTable("ElectronicsToDelivery");

                entity.HasOne(d => d.Delivery)
                    .WithMany(p => p.ElectronicsToDeliveries)
                    .HasForeignKey(d => d.DeliveryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ElectronicsToDelivery_Delivery");

                entity.HasOne(d => d.Electronics)
                    .WithMany(p => p.ElectronicsToDeliveries)
                    .HasForeignKey(d => d.ElectronicsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ElectronicsToDelivery_Electronics");
            });

            modelBuilder.Entity<ElectronicsToStorage>(entity =>
            {
                entity.HasKey(e => new { e.ElectronicsId, e.StorageId });

                entity.ToTable("ElectronicsToStorage");

                entity.HasOne(d => d.Electronics)
                    .WithMany(p => p.ElectronicsToStorages)
                    .HasForeignKey(d => d.ElectronicsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ElectronicsToStorage_Electronics");

                entity.HasOne(d => d.Storage)
                    .WithMany(p => p.ElectronicsToStorages)
                    .HasForeignKey(d => d.StorageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ElectronicsToStorage_Storage");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("Employee");

                entity.HasIndex(e => e.Email, "IX_EmployeeEmail");

                entity.HasIndex(e => e.FullName, "IX_EmployeeFullName");

                entity.HasIndex(e => e.Login, "IX_EmployeeLogin")
                    .IsUnique();

                entity.HasIndex(e => e.Password, "IX_EmployeePassword");

                entity.HasIndex(e => e.PhoneNumber, "IX_EmployeePhoneNumber");

                entity.HasIndex(e => e.PostId, "IX_EmployeePost");

                entity.HasIndex(e => e.RoleId, "IX_EmployeeRole");

                entity.Property(e => e.DateOfBirth).HasColumnType("date");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(120)
                    .IsUnicode(false);

                entity.Property(e => e.Login)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Passport)
                    .IsRequired()
                    .HasMaxLength(11)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(17)
                    .IsUnicode(false);

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(d => d.PostId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Employee_Post");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Employee_Role");
            });

            modelBuilder.Entity<Manufacturer>(entity =>
            {
                entity.ToTable("Manufacturer");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");

                entity.HasIndex(e => e.DateOfRegistration, "IX_OrderDateOfRegistration");

                entity.HasIndex(e => e.OrderId, "IX_OrderNumber");

                entity.HasIndex(e => e.StatusId, "IX_OrderStatus");

                entity.Property(e => e.DateOfRegistration).HasColumnType("datetime");

                entity.Property(e => e.OrderNumber)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Order_Employee");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_Status");
            });

            modelBuilder.Entity<OrderElectronic>(entity =>
            {
                entity.HasKey(e => new { e.ElectronicsId, e.OrderId });

                entity.HasOne(d => d.Electronics)
                    .WithMany(p => p.OrderElectronics)
                    .HasForeignKey(d => d.ElectronicsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderElectronics_Electronics");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderElectronics)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_OrderElectronics_Order");
            });

            modelBuilder.Entity<OrderStatus>(entity =>
            {
                entity.ToTable("OrderStatus");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.ToTable("Post");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Storage>(entity =>
            {
                entity.ToTable("Storage");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.ToTable("Supplier");

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(40)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

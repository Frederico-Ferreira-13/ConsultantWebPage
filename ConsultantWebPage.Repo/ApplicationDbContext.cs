using ConsultantWebPage.Core.Models;
using ConsultantWebPage.Core.Models.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace ConsultantWebPage.Repo;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Entidades Principais
    public DbSet<RealEstate> RealEstates { get; set; }
    public DbSet<RealEstateType> RealEstateTypes { get; set; }
    public DbSet<RealEstateMedia> RealEstateMedia { get; set; }
    public DbSet<RealEstateCategories> RealEstateCategories { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<CategoryType> CategoryTypes { get; set; }
    public DbSet<Cities> Cities { get; set; }

    // Interação e Marketing
    public DbSet<AboutMe> AboutMe { get; set; }
    public DbSet<Leads> Leads { get; set; }
    public DbSet<PropertyEvaluations> PropertyEvaluations { get; set; }
    public DbSet<Testimonials> Testimonials { get; set; }
    public DbSet<Posts> Posts { get; set; }
    public DbSet<Comments> Comments { get; set; }

    // Utilizadores e Sistema
    public DbSet<Users> Users { get; set; }
    public DbSet<UsersRole> UsersRoles { get; set; }
    public DbSet<UserSettings> UserSettings { get; set; }
    public DbSet<Favorites> Favorites { get; set; }
    public DbSet<Ratings> Ratings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UsersRole>(entity =>
        {
            entity.ToTable("UserRoles");
            entity.HasKey(r => r.UsersRoleId);
            entity.Property(r => r.RoleName).IsRequired().HasMaxLength(50);
            entity.HasIndex(r => r.RoleName).IsUnique();
        });

        modelBuilder.Entity<Users>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(u => u.UserId);

            entity.Property(u => u.FullName).IsRequired().HasMaxLength(255);
            entity.Property(u => u.UserName).IsRequired().HasMaxLength(255);
            entity.HasIndex(u => u.UserName).IsUnique();

            entity.OwnsOne(u => u.Contact, contact =>
            {
                contact.Property(c => c.Email)
                    .HasColumnName("Email")
                    .HasMaxLength(255)
                    .IsRequired();
                contact.HasIndex(c => c.Email).IsUnique();

                contact.Property(c => c.PhoneNumber)
                    .HasColumnName("Phone")
                    .HasMaxLength(50);
            });

            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.Salt).IsRequired();
            entity.Property(u => u.IsActive).HasDefaultValue(true);
            entity.Property(u => u.IsApproved).HasDefaultValue(false);

            entity.Property(u => u.UsersRoleId).HasColumnName("UserRoleId");

            entity.HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.UsersRoleId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<UserSettings>(entity =>
        {
            entity.ToTable("UserSettings");
            entity.HasKey(s => s.UserSettingsId);
            entity.HasIndex(s => s.UserId).IsUnique();
            entity.Property(s => s.Theme).HasMaxLength(50).HasDefaultValue("Light");
            entity.Property(s => s.Language).HasMaxLength(10).HasDefaultValue("pt-PT");
            entity.Property(s => s.ReceiveNotifications).HasDefaultValue(true);
        });

        modelBuilder.Entity<CategoryType>(entity =>
        {
            entity.ToTable("CategoryType");
            entity.HasKey(ct => ct.CategoryTypeId);
            entity.Property(ct => ct.TypeName).IsRequired().HasMaxLength(100);
            entity.HasIndex(ct => ct.TypeName).IsUnique();
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Category");
            entity.HasKey(c => c.CategoriesId);
            entity.Property(c => c.CategoryName).IsRequired().HasMaxLength(100);

            entity.HasOne(c => c.ParentCategory)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(c => c.CategoryType)
                .WithMany()
                .HasForeignKey(c => c.CategoryTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Cities>(entity =>
        {
            entity.ToTable("Cities");
            entity.HasKey(c => c.CityId);
            entity.Property(c => c.CityName).IsRequired().HasMaxLength(100);
            
            // ✅ ADICIONAR ISTO:
            entity.Property(c => c.DistrictName)
                .IsRequired()
                .HasMaxLength(100)
                .HasDefaultValue(string.Empty);
        });

        modelBuilder.Entity<RealEstateType>(entity =>
        {
            entity.ToTable("RealEstateType");
            entity.HasKey(r => r.RealEstateTypeId);
            entity.Property(r => r.TypeName).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<RealEstate>(entity =>
        {
            entity.ToTable("RealEstate");
            entity.HasKey(e => e.RealEstateId);

            entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(50).HasDefaultValue("Disponível");
            entity.Property(e => e.IsFeatured).HasDefaultValue(false);

            entity.Property(e => e.RealEstateTypeId).HasColumnName("RealEstateTypeId");
            entity.Property(e => e.ConsultantId).HasColumnName("ConsultantId");
            entity.Property(e => e.CityId).HasColumnName("CityId");

            entity.OwnsOne(e => e.Address, address =>
            {
                address.Property(a => a.Street)
                    .HasColumnName("Address")
                    .HasMaxLength(255);
                address.Property(a => a.PostalCode)
                    .HasColumnName("PostalCode")
                    .HasMaxLength(20);

                address.Property(a => a.Number).HasColumnName("Address_Number").HasDefaultValue("N/A");
                address.Property(a => a.City).HasColumnName("Address_City").HasDefaultValue("N/A");
                address.Property(a => a.Country).HasColumnName("Address_Country").HasDefaultValue("Portugal");
                address.Property(a => a.Complement).HasColumnName("Address_Complement").IsRequired(false);
            });

            // Money Value Object → mapeia para colunas Price e Currency da tua BD
            entity.OwnsOne(e => e.Price, price =>
            {
                price.Property(p => p.Amount)
                    .HasColumnName("Price")
                    .HasColumnType("decimal(18,2)");
                price.Property(p => p.Currency)
                    .HasColumnName("Currency")
                    .HasMaxLength(10)
                    .HasDefaultValue("EUR");
            });

            entity.HasOne(e => e.RealEstateType)
                .WithMany(t => t.RealEstates)
                .HasForeignKey(e => e.RealEstateTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Consultant)
                .WithMany()
                .HasForeignKey(e => e.ConsultantId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.City)
                .WithMany(c => c.RealEstates)
                .HasForeignKey(e => e.CityId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<RealEstateCategories>(entity =>
        {
            entity.ToTable("RealEstateCategories");
            
            // Configuração da Chave Composta
            entity.HasKey(re => new { re.RealEstateId, re.CategoriesId });

            // Configuração das relações com as tabelas pai
            entity.HasOne(re => re.RealEstate)
                .WithMany(r => r.RealEstateCategories)
                .HasForeignKey(re => re.RealEstateId)
                .OnDelete(DeleteBehavior.Cascade); // Se o imóvel for apagado, o link desaparece

            entity.HasOne(re => re.Category)
                .WithMany()
                .HasForeignKey(re => re.CategoriesId)
                .OnDelete(DeleteBehavior.Restrict); // Evita apagar a categoria se houver imóveis ligados
        });

        modelBuilder.Entity<RealEstateMedia>(entity =>
        {
            entity.ToTable("RealEstateMedia");
            entity.HasKey(m => m.MediaId);

            // Validação de nomes de colunas conforme o SQL
            entity.Property(m => m.MediaUrl).IsRequired();
            entity.Property(m => m.MediaType).HasMaxLength(50).HasDefaultValue("image");
            entity.Property(m => m.IsMain).HasDefaultValue(false);

            // Relação 1:N (Um Imóvel tem muitas Médias)
            entity.HasOne(m => m.RealEstate)
                .WithMany() // Se quiseres navegar a partir do Imóvel, precisarias de adicionar uma coleção em RealEstate
                .HasForeignKey(m => m.RealEstateId)
                .OnDelete(DeleteBehavior.Cascade); // Se o imóvel for apagado, as fotos/vídeos também são
        });

        modelBuilder.Entity<Leads>(entity =>
        {
            entity.ToTable("Leads");
            entity.HasKey(l => l.InquiryId);

            entity.Property(l => l.ClientName).IsRequired().HasMaxLength(255);
            entity.Property(l => l.Status).HasMaxLength(50).HasDefaultValue("Novo");

            entity.OwnsOne(l => l.Contact, contact =>
            {
                contact.Property(c => c.Email)
                    .HasColumnName("ClientEmail")
                    .HasMaxLength(255);
                contact.Property(c => c.PhoneNumber)
                    .HasColumnName("ClientPhone")
                    .HasMaxLength(50);
            });

            entity.HasOne(l => l.RealEstate)
                .WithMany()
                .HasForeignKey(l => l.RealEstateId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Favorites>(entity =>
        {
            entity.ToTable("Favorites");
            entity.HasKey(f => f.FavoritesId);
            entity.HasIndex(f => new { f.UserId, f.RealEstateId }).IsUnique();

            entity.HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(f => f.RealEstate)
                .WithMany()
                .HasForeignKey(f => f.RealEstateId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Ratings>(entity =>
        {
            entity.ToTable("Ratings");
            entity.HasKey(r => r.RatingsId);
            entity.HasIndex(r => new { r.UserId, r.RealEstateId }).IsUnique();

            entity.Property(r => r.RatingValue)
                .HasConversion(
                    v => v!.Value,
                    v => StarRating.Create(v)
                )
                .HasColumnName("RatingValue")
                .IsRequired();

            entity.HasOne(r => r.RealEstate)
                .WithMany()
                .HasForeignKey(r => r.RealEstateId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Comments>(entity =>
        {
            entity.ToTable("Comments");
            entity.HasKey(c => c.CommentsId);

            entity.HasOne(c => c.RealEstate)
                .WithMany()
                .HasForeignKey(c => c.RealEstateId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(c => c.Users)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Testimonials>(entity =>
        {
            entity.ToTable("Testimonials");
            entity.HasKey(t => t.TestimonialId);
            entity.Property(t => t.ClientName).IsRequired().HasMaxLength(255);
            entity.Property(t => t.IsVisible).HasDefaultValue(false);

            entity.HasOne(t => t.Consultant)
                .WithMany()
                .HasForeignKey(t => t.ConsultantId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Posts>(entity =>
        {
            entity.ToTable("Posts");
            entity.HasKey(p => p.PostId);
            entity.Property(p => p.Title).IsRequired().HasMaxLength(255);
            entity.HasIndex(p => p.Title).IsUnique();
            entity.Property(p => p.Slug).HasMaxLength(255);
            entity.HasIndex(p => p.Slug).IsUnique();
            entity.Property(p => p.Status).HasMaxLength(50).HasDefaultValue("Draft");
            entity.Property(p => p.IsPublished).HasDefaultValue(false);
            entity.Property(p => p.ViewCount).HasDefaultValue(0);
            entity.Property(p => p.Summary).HasMaxLength(500);

            entity.HasOne(p => p.Author)
                .WithMany()
                .HasForeignKey(p => p.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PropertyEvaluations>(entity =>
        {
            entity.ToTable("PropertyEvaluations");
            entity.HasKey(e => e.EvaluationId);

            entity.Property(e => e.ClientContact).IsRequired().HasMaxLength(255);
            entity.Property(e => e.ClientName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Area).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Status).HasMaxLength(50).HasDefaultValue("Pending");

            entity.HasOne(e => e.Consultant)
                .WithMany()
                .HasForeignKey(e => e.ConsultantId)
                .OnDelete(DeleteBehavior.Restrict);
        });
       
        modelBuilder.Entity<AboutMe>(entity =>
        {
            entity.ToTable("AboutMe");
            entity.HasKey(a => a.AboutMeId);

            // Garante que um utilizador tem apenas um registo de "AboutMe"
            entity.HasIndex(a => a.UserId).IsUnique();

            entity.HasOne(a => a.User)
                .WithOne() // Assumindo que a relação é 1:1 com o utilizador
                .HasForeignKey<AboutMe>(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Se o utilizador for eliminado, o AboutMe também é
        });
    }
}
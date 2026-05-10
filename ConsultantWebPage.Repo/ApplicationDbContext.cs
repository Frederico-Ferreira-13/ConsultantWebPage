using Microsoft.EntityFrameworkCore;
using ConsultantWebPage.Core.Models;

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
    
    // Interação e Marketing
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

        // 1. Configuração de Imóveis
        modelBuilder.Entity<RealEstate>(entity => {
            entity.ToTable("RealEstate");
            // Configuração dos Value Objects (Address, Money) se os usares
            entity.OwnsOne(e => e.Address); 
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
        });

        // 2. Tabela de Ligação (Many-to-Many) para Categorias
        modelBuilder.Entity<RealEstateCategories>()
            .HasKey(rc => new { rc.RealEstateId, rc.CategoriesId });

        // 3. Avaliações (Property Evaluation)
        modelBuilder.Entity<PropertyEvaluations>(entity => 
    {
        entity.ToTable("PropertyEvaluation");
        entity.HasKey(e => e.EvaluationId);

        // ClientContact aqui é uma STRING simples na tua classe, não um Value Object
        entity.Property(e => e.ClientContact)
              .IsRequired()
              .HasMaxLength(255);

        entity.Property(e => e.ClientName)
              .IsRequired()
              .HasMaxLength(255);

        // Configuração da Precisão Decimal para a Área
        entity.Property(e => e.Area)
              .HasColumnType("decimal(18,2)");

        entity.Property(e => e.Status)
              .HasMaxLength(50)
              .HasDefaultValue("Pending");

        // Relacionamento com o Consultor (User)
        entity.HasOne(e => e.Consultant)
              .WithMany() // Se o User não tiver uma coleção de Evaluations, deixa vazio
              .HasForeignKey(e => e.ConsultantId)
              .OnDelete(DeleteBehavior.Cascade);
        });

        // 4. Configuração de Utilizadores e Roles
        modelBuilder.Entity<Users>(entity => {
            entity.ToTable("Users");
            entity.HasKey(u => u.UserId);
            entity.OwnsOne(u => u.Contact, contact => 
        {
            contact.Property(c => c.Email).HasMaxLength(255).IsRequired();
            // Adiciona aqui um índice único para o Email dentro do Value Object
            contact.HasIndex(c => c.Email).IsUnique(); 
        });

        entity.Property(u => u.Name).IsRequired().HasMaxLength(255);
        entity.Property(u => u.UserName).IsRequired().HasMaxLength(255);
        
        // Relacionamento com a Role
        entity.HasOne(u => u.Role)
              .WithMany()
              .HasForeignKey(u => u.UsersRoleId)
              .OnDelete(DeleteBehavior.Restrict);
        });

        // 5. Configuração de Favoritos (Chave Composta)
        modelBuilder.Entity<Favorites>()
            .HasKey(f => new { f.UserId, f.RealEstateId });

        // 6. Configuração de Ratings (Chave Composta)
        modelBuilder.Entity<Ratings>()
            .HasKey(r => new { r.UserId, r.RealEstateId });
            
        // 7. Testemunhos e Posts
        modelBuilder.Entity<Testimonials>().ToTable("Testimonials");
        modelBuilder.Entity<Posts>().ToTable("Posts");
    }
}
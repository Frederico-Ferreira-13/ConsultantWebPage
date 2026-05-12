using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Transactions;
using ConsultantWebPage.Contracts.Interfaces.Repo;
using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;
using Microsoft.Extensions.Configuration;

namespace ConsultantWebPage.Repo;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private TransactionScope? _scope = null;

    private bool _disposed = false;
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public IAboutMeRepository AboutMe { get; }
    public ICategoryRepository Categories { get; }
    public ICategoryTypeRepository CategoryTypes { get; }
    public ICommentsRepository Comments { get; }
    public IFavoritesRepository Favorites { get; }
    public ILeadsRepository Leads { get; }
    public IPostsRepository Posts { get; }
    public IPropertyEvaluationRepository PropertiesEvaluations { get; }
    public IRatingsRepository Rating {get;}
    public IRealEstateRepository RealEstates { get; }
    public IRealEstateCategoriesRepository RealEstatesCategories { get; }
    public IRealEstateMediaRepository RealEstatesMedia {get;}
    public IRealEstateTypeRepository RealEstatesTypes {get;}
    public ITestimonialsRepository Testimonials {get;}
    public IUsersRepository Users { get; }
    public IUserSettingRepository UserSettings { get; }
    public IUsersRoleRepository UsersRole { get; }

    public UnitOfWork(
        ApplicationDbContext context,
        IConfiguration configuration,
        IAboutMeRepository aboutMe,
        ICategoryRepository categories,
        ICategoryTypeRepository categoryTypes,
        ICommentsRepository comments,
        IFavoritesRepository favorites,
        ILeadsRepository leads,
        IPostsRepository posts,
        IPropertyEvaluationRepository propertiesEvaluations,
        IRatingsRepository rating,
        IRealEstateRepository realEstates,
        IRealEstateCategoriesRepository realEstatesCategories,
        IRealEstateMediaRepository realEstatesMedia,
        IRealEstateTypeRepository realEstatesTypes,
        ITestimonialsRepository testimonials,
        IUsersRepository users,
        IUserSettingRepository userSettings,
        IUsersRoleRepository usersRole)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        AboutMe = aboutMe;
        Categories = categories;
        CategoryTypes = categoryTypes;
        Comments = comments;
        Favorites = favorites;
        Leads = leads;
        Posts = posts;
        PropertiesEvaluations = propertiesEvaluations;
        Rating = rating;
        RealEstates = realEstates;
        RealEstatesCategories = realEstatesCategories;
        RealEstatesMedia = realEstatesMedia;
        RealEstatesTypes = realEstatesTypes;
        Testimonials = testimonials;
        Users = users;
        UserSettings = userSettings;
        UsersRole = usersRole;
    }

    public async Task BeginTransactionAsync()
    {
        if (_scope != null)
        {
            throw new InvalidOperationException("Já existe uma transação ativa.");
        }

        var options = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted, // Padrão bom para leitura/escrita consistente
            Timeout = TimeSpan.FromSeconds(30) // Ajustar se necessitar de mais tempo
        };



       _scope = new TransactionScope(
            TransactionScopeOption.Required,
            options,
            TransactionScopeAsyncFlowOption.Enabled //Para Async
        );

        await Task.CompletedTask;
    }

    public async Task<int> CommitAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Rollback()
    {
        if (_scope == null)
        {
            throw new InvalidOperationException("Não há uma transação ativa para ser revertida.");
        }

        _scope.Dispose();
        _scope = null;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _scope?.Dispose();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}

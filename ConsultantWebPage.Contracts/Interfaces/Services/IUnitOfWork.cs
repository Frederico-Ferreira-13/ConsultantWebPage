using ConsultantWebPage.Contracts.Interfaces.Repo;

namespace ConsultantWebPage.Contracts.Interfaces.Repo;

public interface IUnitOfWork : IDisposable
{
    IAboutMeRepository AboutMe { get; }
    ICategoryRepository Categories { get; }
    ICategoryTypeRepository CategoryTypes { get; }
    ICommentsRepository Comments { get; }
    IFavoritesRepository Favorites { get; }
    ILeadsRepository Leads { get; }
    IPostsRepository Posts { get; }
    IPropertyEvaluationRepository PropertiesEvaluations { get; }
    IRatingsRepository Rating { get; }
    IRealEstateRepository RealEstates { get; }
    IRealEstateCategoriesRepository RealEstatesCategories { get; }
    IRealEstateMediaRepository RealEstatesMedia { get; }
    IRealEstateTypeRepository RealEstatesTypes { get; }
    ITestimonialsRepository Testimonials { get; }
    IUsersRepository Users { get; }
    IUserSettingRepository UserSettings { get; }
    IUsersRoleRepository UsersRole { get; }
    ICitiesRepository Cities { get; }
    
    Task<int> CommitAsync();
    Task BeginTransactionAsync();
    void Rollback();
}
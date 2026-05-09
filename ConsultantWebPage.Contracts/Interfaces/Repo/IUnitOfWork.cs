using System;

namespace ConsultantWebPage.Contracts.Interfaces.Repo;

public interface IUnitOfWork : IDisposable
{
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

    Task BeginTransactionAsync();
    Task<int> CommitAsync();
    void Rollback();
}

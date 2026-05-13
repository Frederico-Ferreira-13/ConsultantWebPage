CREATE DATABASE ConsultantWebPage;
GO

USE ConsultantWebPage;
GO

CREATE TABLE UserRoles (
    UsersRoleId INT PRIMARY KEY IDENTITY(1,1),
    RoleName NVARCHAR(50) NOT NULL
);
GO

CREATE TABLE RealEstateType (
    RealEstateTypeId INT PRIMARY KEY IDENTITY(1,1),
    TypeName NVARCHAR(100) NOT NULL
);
GO

CREATE TABLE CategoryType (
    CategoryTypeId INT PRIMARY KEY IDENTITY(1,1),
    TypeName NVARCHAR(100) NOT NULL
);
GO

CREATE TABLE Category (
    CategoriesId INT PRIMARY KEY IDENTITY(1,1),
    CategoryName NVARCHAR(100) NOT NULL,
    CategoryTypeId INT NOT NULL,
    ParentCategoryId INT NULL,
    CONSTRAINT FK_Category_Type FOREIGN KEY (CategoryTypeId) REFERENCES CategoryType(CategoryTypeId),
    CONSTRAINT FK_Category_Parent FOREIGN KEY (ParentCategoryId) REFERENCES Category(CategoriesId)
);
GO

CREATE TABLE Cities (
    CityId INT PRIMARY KEY IDENTITY(1,1),
    CityName NVARCHAR(100) NOT NULL,
    DistrictName NVARCHAR(100) NOT NULL
);
GO

CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    UserRoleId INT NOT NULL,
    FullName NVARCHAR(255) NOT NULL,
    UserName NVARCHAR(255) UNIQUE NOT NULL,
    Email NVARCHAR(255) UNIQUE NOT NULL,
    Phone NVARCHAR(50),
    PasswordHash NVARCHAR(MAX) NOT NULL,
    ProfilePicture NVARCHAR(MAX),
    FacebookUrl NVARCHAR(MAX),
    InstagramUrl NVARCHAR(MAX),
    XUrl NVARCHAR(MAX),
    IsActive BIT DEFAULT 1,
    IsApproved BIT DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE(),
    DeletedAt DATETIME NULL,
    CONSTRAINT FK_Users_Roles FOREIGN KEY (UserRoleId) REFERENCES UserRoles(UsersRoleId)
);
GO

CREATE TABLE UserSettings (
    UserSettingsId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL UNIQUE,
    Theme NVARCHAR(50) DEFAULT 'light',
    Language NVARCHAR(10) DEFAULT 'pt-PT',
    ReceiveNotifications BIT DEFAULT 1,
    CONSTRAINT FK_UserSettings_Users FOREIGN KEY (UserId) REFERENCES Users(UserId)
);
GO

CREATE TABLE AboutMe (
    AboutMeId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL UNIQUE,
    Bio NVARCHAR(MAX),
    ProfileImageUrl NVARCHAR(MAX),
    YearsOfExperience INT,
    SocialURL_LinkedIn NVARCHAR(MAX),
    SocialURL_X NVARCHAR(MAX),
    SocialURL_Facebook NVARCHAR(MAX),
    SocialURL_Instagram NVARCHAR(MAX),
    CONSTRAINT FK_AboutMe_Users FOREIGN KEY (UserId) REFERENCES Users(UserId)
);
GO

CREATE TABLE RealEstate (
    RealEstateId INT PRIMARY KEY IDENTITY(1,1),
    RealEstateTypeId INT NOT NULL,
    ConsultantId INT NOT NULL,
    CityId INT NOT NULL,                -- Propriedade de localização corrigida
    Title NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX),           -- Faltava a descrição detalhada
    ThumbnailUrl NVARCHAR(MAX),
    ExternalSourceUrl NVARCHAR(MAX),
    ExternalReferenceId NVARCHAR(100),
    Price DECIMAL(18, 2),
    Currency NVARCHAR(10) DEFAULT 'EUR',
    Address NVARCHAR(255),
    PostalCode NVARCHAR(20),
    Status NVARCHAR(50) DEFAULT 'Disponível',
    IsFeatured BIT DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE(),
    LastCheckDate DATETIME,
    CONSTRAINT FK_RealEstate_Type FOREIGN KEY (RealEstateTypeId) REFERENCES RealEstateType(RealEstateTypeId),
    CONSTRAINT FK_RealEstate_Consultant FOREIGN KEY (ConsultantId) REFERENCES Users(UserId),
    CONSTRAINT FK_RealEstate_City FOREIGN KEY (CityId) REFERENCES Cities(CityId)
);
GO

CREATE TABLE RealEstateCategories (
    RealEstateId INT NOT NULL,
    CategoriesId INT NOT NULL,
    PRIMARY KEY (RealEstateId, CategoriesId),
    CONSTRAINT FK_Link_RealEstate FOREIGN KEY (RealEstateId) REFERENCES RealEstate(RealEstateId),
    CONSTRAINT FK_Link_Category FOREIGN KEY (CategoriesId) REFERENCES Category(CategoriesId)
);
GO

CREATE TABLE RealEstateMedia (
    MediaId INT PRIMARY KEY IDENTITY(1,1),
    RealEstateId INT NOT NULL,
    MediaUrl NVARCHAR(MAX) NOT NULL,
    MediaType NVARCHAR(50) DEFAULT 'image',
    IsMain BIT DEFAULT 0,
    CONSTRAINT FK_Media_RealEstate FOREIGN KEY (RealEstateId) REFERENCES RealEstate(RealEstateId)
);
GO

CREATE TABLE Leads (
    InquiryId INT PRIMARY KEY IDENTITY(1,1),
    RealEstateId INT NULL, 
    ConsultantId INT NULL,
    ClientName NVARCHAR(255) NOT NULL,
    ClientEmail NVARCHAR(255),
    ClientPhone NVARCHAR(50),
    Message NVARCHAR(MAX),
    Status NVARCHAR(50) DEFAULT 'Novo',
    CreatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Leads_RealEstate FOREIGN KEY (RealEstateId) REFERENCES RealEstate(RealEstateId)
);
GO

CREATE TABLE Testimonials (
    TestimonialId INT PRIMARY KEY IDENTITY(1,1),
    ConsultantId INT NOT NULL,
    ClientName NVARCHAR(255),
    TestimonialText NVARCHAR(MAX),
    Rating INT CHECK (Rating BETWEEN 1 AND 5),
    IsVisible BIT DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Testimonials_Consultant FOREIGN KEY (ConsultantId) REFERENCES Users(UserId)
);
GO

CREATE TABLE Comments (
    CommentsId INT PRIMARY KEY IDENTITY(1,1),
    RealEstateId INT NOT NULL,
    UserId INT NOT NULL,
    CommentText NVARCHAR(MAX),
    OriginalComment NVARCHAR(MAX),
    CreatedAt DATETIME DEFAULT GETDATE(),
    LastUpdatedAt DATETIME DEFAULT GETDATE(),
    IsDeleted BIT DEFAULT 0,
    DeletedAt DATETIME NULL,
    IsEdited BIT DEFAULT 0,
    IsApproved BIT DEFAULT 0,
    ParentCommentId INT NULL,           -- Para permitir respostas
    CONSTRAINT FK_Comments_RealEstate FOREIGN KEY (RealEstateId) REFERENCES RealEstate(RealEstateId),
    CONSTRAINT FK_Comments_User FOREIGN KEY (UserId) REFERENCES Users(UserId),
    CONSTRAINT FK_Comments_Parent FOREIGN KEY (ParentCommentId) REFERENCES Comments(CommentsId)
);
GO

CREATE TABLE Ratings (
    RatingsId INT PRIMARY KEY IDENTITY(1,1),
    RealEstateId INT NOT NULL,
    UserId INT NOT NULL,
    RatingValue INT CHECK (RatingValue BETWEEN 1 AND 5),
    CreatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Ratings_RealEstate FOREIGN KEY (RealEstateId) REFERENCES RealEstate(RealEstateId),
    CONSTRAINT FK_Ratings_User FOREIGN KEY (UserId) REFERENCES Users(UserId)
);
GO

CREATE TABLE Favorites (
    FavoritesId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    RealEstateId INT NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Favorites_User FOREIGN KEY (UserId) REFERENCES Users(UserId),
    CONSTRAINT FK_Favorites_RealEstate FOREIGN KEY (RealEstateId) REFERENCES RealEstate(RealEstateId),
    CONSTRAINT UQ_User_Favorite UNIQUE (UserId, RealEstateId)
);
GO

CREATE TABLE PropertyEvaluations (
    EvaluationId INT PRIMARY KEY IDENTITY(1,1),
    ConsultantId INT NOT NULL,
    UserId INT NULL,                    -- FK integrada: permite ligar a utilizador registado
    ClientName NVARCHAR(255),
    ClientContact NVARCHAR(255),
    PropertyType NVARCHAR(100),
    Address NVARCHAR(MAX),
    Typology NVARCHAR(50),
    Area DECIMAL(18, 2),
    Condition NVARCHAR(100),
    Status NVARCHAR(50) DEFAULT 'Pending',
    CreatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Evaluations_Consultant FOREIGN KEY (ConsultantId) REFERENCES Users(UserId),
    CONSTRAINT FK_Evaluations_User FOREIGN KEY (UserId) REFERENCES Users(UserId)
);
GO

CREATE TABLE Posts (
    PostId INT PRIMARY KEY IDENTITY(1,1),
    AuthorId INT NOT NULL,
    CategoryId INT NOT NULL DEFAULT 1,
    ViewCount INT NOT NULL DEFAULT 0,
    Title NVARCHAR(255) NOT NULL UNIQUE,
    Slug NVARCHAR(255) UNIQUE,
    Content NVARCHAR(MAX) NOT NULL,
    Summary NVARCHAR(500),
    ImageUrl NVARCHAR(MAX),
    Status NVARCHAR(50) NOT NULL DEFAULT 'Draft',
    IsPublished BIT DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE(),
    LastUpdatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Posts_Author FOREIGN KEY (AuthorId) REFERENCES Users(UserId),
    CONSTRAINT FK_Posts_Category FOREIGN KEY (CategoryId) REFERENCES Category(CategoriesId)
);
GO
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

CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    UsersRoleId INT NOT NULL,
    LeadsId INT NOT NULL,
    FullName NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) UNIQUE NOT NULL,
    IsActive BIT DEFAULT 1,
    CONSTRAINT FK_Users_Roles FOREIGN KEY (UsersRoleId) REFERENCES UserRoles(UsersRoleId)    
);
GO

CREATE TABLE UserSettings (
    UserSettingsId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    Theme NVARCHAR(50),
    Language NVARCHAR(10),
    ReceiveNotifications BIT DEFAULT 1,
    CONSTRAINT FK_UserSettings_Users FOREIGN KEY (UserId) REFERENCES Users(UserId)
);
GO

CREATE TABLE AboutMe (
    AboutMeId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    Bio NVARCHAR(MAX),
    ProfileImageUrl NVARCHAR(MAX),
    YearsOfExperience INT,
    SocialURL NVARCHAR(MAX),
    CONSTRAINT FK_AboutMe_Users FOREIGN KEY (UserId) REFERENCES Users(UserId)
);
GO

CREATE TABLE RealEstate (
    RealEstateId INT PRIMARY KEY IDENTITY(1,1),
    RealEstateTypeId INT NOT NULL,
    ConsultantId INT NOT NULL,
    Title NVARCHAR(255) NOT NULL,
    ThumbnailUrl NVARCHAR(MAX),
    ExternalSourceUrl NVARCHAR(MAX),
    ExternalReferenceId NVARCHAR(100),
    Price DECIMAL(18, 2),
    City NVARCHAR(100),
    Status NVARCHAR(50),
    IsFeatured BIT DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE(),
    LastCheckDate DATETIME,
    CONSTRAINT FK_RealEstate_Type FOREIGN KEY (RealEstateTypeId) REFERENCES RealEstateType(RealEstateTypeId),
    CONSTRAINT FK_RealEstate_Consultant FOREIGN KEY (ConsultantId) REFERENCES Users(UserId)
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
    MediaType NVARCHAR(50),
    IsMain BIT DEFAULT 0,
    CONSTRAINT FK_Media_RealEstate FOREIGN KEY (RealEstateId) REFERENCES RealEstate(RealEstateId)
);
GO

CREATE TABLE Leads (
    InquiryId INT PRIMARY KEY IDENTITY(1,1),
    RealEstateId INT NOT NULL,
    ClientName NVARCHAR(255) NOT NULL,
    ClientEmail NVARCHAR(255),
    ClientPhone NVARCHAR(50),
    Message NVARCHAR(MAX),
    Status NVARCHAR(50),
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
    CONSTRAINT FK_Testimonials_Consultant FOREIGN KEY (ConsultantId) REFERENCES Users(UserId)
);
GO

CREATE TABLE Comments (
    CommentsId INT PRIMARY KEY IDENTITY(1,1),
    RealEstateId INT NOT NULL,
    UserId INT NOT NULL,
    CommentText NVARCHAR(MAX),
    CreatedAt DATETIME DEFAULT GETDATE(),
    LastUpdatedAt DATETIME,
    IsEdited BIT DEFAULT 0,
    IsDeleted BIT DEFAULT 0,
    OriginalComment NVARCHAR(MAX),
    ParentCommentId INT NULL,
    CONSTRAINT FK_Comments_RealEstate FOREIGN KEY (RealEstateId) REFERENCES RealEstate(RealEstateId),
    CONSTRAINT FK_Comments_User FOREIGN KEY (UserId) REFERENCES Users(UserId)
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
    CONSTRAINT FK_Favorites_RealEstate FOREIGN KEY (RealEstateId) REFERENCES RealEstate(RealEstateId)
);
GO

CREATE TABLE PropertyEvaluations (
    EvaluationId INT PRIMARY KEY IDENTITY(1,1),
    ConsultantId INT NOT NULL,
    ClientName NVARCHAR(255),
    ClientContact NVARCHAR(255),
    PropertyType NVARCHAR(100),
    Address NVARCHAR(MAX),
    Typology NVARCHAR(50),
    Area DECIMAL(18, 2),
    Condition NVARCHAR(100),
    Status NVARCHAR(50),
    CreatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Evaluations_Consultant FOREIGN KEY (ConsultantId) REFERENCES Users(UserId)
);
GO

CREATE TABLE Posts (
    PostId INT PRIMARY KEY IDENTITY(1,1),
    AuthorId INT NOT NULL, -- FK para Users (o consultor que escreveu)
    Title NVARCHAR(255) NOT NULL,
    Slug NVARCHAR(255) UNIQUE, -- Para URLs amigáveis: /blog/como-vender-casa
    Content NVARCHAR(MAX) NOT NULL,
    Summary NVARCHAR(500),
    ImageUrl NVARCHAR(MAX),
    IsPublished BIT DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Posts_Author FOREIGN KEY (AuthorId) REFERENCES Users(UserId)
);
GO

ALTER TABLE Users
ADD CONSTRAINT FK_Users_Leads FOREIGN KEY (LeadsId) REFERENCES Leads(InquiryId);
GO

ALTER TABLE PropertyEvaluations
ADD UserId INT NULL;

ALTER TABLE PropertyEvaluations
ADD CONSTRAINT FK_Evaluations_Client FOREIGN KEY (UserId) REFERENCES Users(UserId);
GO
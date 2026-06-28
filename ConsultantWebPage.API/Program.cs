using Microsoft.EntityFrameworkCore;
using ConsultantWebPage.Repo;
using ConsultantWebPage.Services;
using ConsultantWebPage.Contracts.Interfaces.Services;
using ConsultantWebPage.Contracts.Interfaces.Repo;
using Microsoft.Extensions.Options;
using ConsultantWebPage.Core.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ConsultantWebPage.Services.Import;
using Microsoft.OpenApi;


var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURAÇÃO DA BASE DE DADOS ---
// Certifica-te que a ConnectionString no appsettings.Development.json está correta
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- 2. REGISTO DOS REPOSITÓRIOS ---
builder.Services.AddScoped<IAboutMeRepository, AboutMeRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryTypeRepository, CategoryTypeRepository>();
builder.Services.AddScoped<ICitiesRepository, CitiesRepository>();
builder.Services.AddScoped<ICommentsRepository, CommentsRepository>();
builder.Services.AddScoped<IFavoritesRepository, FavoritesRepository>();
builder.Services.AddScoped<ILeadsRepository, LeadsRepository>();
builder.Services.AddScoped<IPostsRepository, PostsRepository>();
builder.Services.AddScoped<IPropertyEvaluationRepository, PropertyEvaluationRepository>();
builder.Services.AddScoped<IRatingsRepository, RatingsRepository>();
builder.Services.AddScoped<IRealEstateRepository, RealEstateRepository>();
builder.Services.AddScoped<IRealEstateMediaRepository, RealEstateMediaRepository>();
builder.Services.AddScoped<IRealEstateTypeRepository, RealEstateTypeRepository>();
builder.Services.AddScoped<IRealEstateCategoriesRepository, RealEstateCategoriesRepository>();
builder.Services.AddScoped<ITestimonialsRepository, TestimonialsRepository>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IUsersRoleRepository, UsersRoleRepository>();
builder.Services.AddScoped<IUserSettingRepository, UserSettingRepository>();

// Adiciona os outros se o teu colega os terminar (RealEstate, etc.)

// --- 3. REGISTO DA UNIT OF WORK ---
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// --- 4. REGISTO DOS TEUS SERVIÇOS (Injeção de Dependência) ---
// Usando os nomes com "Services" (plural) para bater certo com as interfaces do teu colega
builder.Services.AddScoped<IAboutMeServices, AboutMeServices>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<ICategoryServices, CategoryServices>();
builder.Services.AddScoped<ICitiesServices, CitiesServices>();
builder.Services.AddScoped<ICloudService, CloudService>();
builder.Services.AddScoped<ICommentsServices, CommentsService>();
builder.Services.AddScoped<IFavoritesServices, FavoritesServices>();
builder.Services.AddScoped<ILeadsServices, LeadsServices>();
builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();
builder.Services.AddScoped<IPostsServices, PostsServices>();
builder.Services.AddScoped<IPropertyEvaluationServices, PropertyEvaluationServices>();
builder.Services.AddScoped<IRatingsServices, RatingsServices>();
builder.Services.AddScoped<IRealEstateServices, RealEstateServices>();
builder.Services.AddScoped<ITestimonialsServices, TestimonialsServices>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUsersServices, UsersServices>();

builder.Services.AddScoped<IPropertyImportProvider, MockImportProvider>();
builder.Services.AddScoped<IPropertyImportService, PropertyImportService>();

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IOptions<JwtSettings>>().Value);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecurityKey"]))
        };
    });   

builder.Services.AddCors(options => {
    options.AddPolicy("FrontendPolicy", policy => {
        policy.WithOrigins(
                "http://localhost:5173",
                "http://localhost:5174"
            )
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers()
    .AddApplicationPart(typeof(ConsultantWebPage.API.Controllers.RealEstateController).Assembly);

// --- 5. CONFIGURAÇÃO BASE DA API ---
builder.Services.AddControllers(); 
builder.Services.AddEndpointsApiExplorer();

const string SecuritySchemeId = "Bearer";

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insere o token JWT assim: Bearer {o teu token}"
    });
 
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference(SecuritySchemeId, document)] = new List<string>()
    });
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// --- 6. PIPELINE DE EXECUÇÃO ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("FrontendPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
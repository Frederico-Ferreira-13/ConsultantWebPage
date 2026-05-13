using Microsoft.EntityFrameworkCore;
using ConsultantWebPage.Repo;
using ConsultantWebPage.Services;
using ConsultantWebPage.Contracts.Interfaces.Services;
using ConsultantWebPage.Contracts.Interfaces.Repo;

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
// Adiciona os outros se o teu colega os terminar (RealEstate, etc.)

// --- 3. REGISTO DA UNIT OF WORK ---
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// --- 4. REGISTO DOS TEUS SERVIÇOS (Injeção de Dependência) ---
// Usando os nomes com "Services" (plural) para bater certo com as interfaces do teu colega
builder.Services.AddScoped<IAboutMeServices, AboutMeServices>();
builder.Services.AddScoped<ICategoryServices, CategoryServices>();
builder.Services.AddScoped<ICitiesServices, CitiesServices>();
builder.Services.AddScoped<ILeadsServices, LeadsServices>();
builder.Services.AddScoped<IFavoritesServices, FavoritesServices>();
builder.Services.AddScoped<IPostsServices, PostsServices>();
builder.Services.AddScoped<IRatingsServices, RatingsServices>();
builder.Services.AddScoped<IPropertyEvaluationServices, PropertyEvaluationServices>();
builder.Services.AddScoped<IRealEstateServices, RealEstateServices>();
builder.Services.AddScoped<ITestimonialsServices, TestimonialsServices>();
builder.Services.AddScoped<IUsersServices, UsersServices>();

// --- 5. CONFIGURAÇÃO BASE DA API ---
builder.Services.AddControllers(); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- 6. PIPELINE DE EXECUÇÃO ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Importante para as rotas dos Controllers funcionarem
app.UseAuthorization();
app.MapControllers();

// Podes manter o weatherforecast para testes rápidos se quiseres, ou apagar
app.MapGet("/weatherforecast", () =>
{
    var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
    return Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast(DateOnly.FromDateTime(DateTime.Now.AddDays(index)), Random.Shared.Next(-20, 55), summaries[Random.Shared.Next(summaries.Length)]))
        .ToArray();
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

// Definição do Record para o exemplo
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
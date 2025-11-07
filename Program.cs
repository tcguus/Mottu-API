using System.Reflection;
using System.Text;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Mottu.Api.Data;
using Mottu.Api.ML;
using Mottu.Api.Services;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.ML;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
{
    ["Jwt:Key"] = "minha-nova-chave-super-secreta-para-o-dev-agora-com-mais-de-32-bytes"
});

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite("Data Source=mottu.db"));

builder.Services.AddCors(o => o.AddPolicy("AllowAll",
    p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddScoped<ITokenService, TokenService>();

// Registra ML apenas se não estiver em ambiente de teste
// (Sua excelente adição para os testes!)
if (!builder.Environment.IsEnvironment("Testing"))
{
    var predictionEngine = MLService.CreatePredictionEngine();
    if (predictionEngine != null)
    {
        builder.Services.AddSingleton(predictionEngine);
    }
}

builder.Services.AddHealthChecks()
  .AddDbContextCheck<AppDbContext>( 
    name: "database",
    failureStatus: HealthStatus.Unhealthy,
    tags: new[] { "db", "sqlite" });

builder.Services.AddControllers();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Mottu API",
        Version = "v1",
        Description = "API de Usuários, Motos e Manutenções (paginação, HATEOAS e JWT)."
    });
    
    c.EnableAnnotations();
    
    var xmlName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlName);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    
    c.ExampleFilters(); // Mantém esta linha
    
    // --- INÍCIO DA CORREÇÃO ---
    // Substituída a configuração de segurança por esta mais robusta
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira 'Bearer' [espaço] e então o seu token. \r\n\r\nExemplo: Bearer eyJhbGciOi..."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
    // --- FIM DA CORREÇÃO ---
});
builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    if (!db.Usuarios.Any())
    {
        db.Usuarios.Add(new Mottu.Api.Domain.Usuario
        {
            Nome = "Admin Demo",
            Email = "admin@mottu.com",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("123456")
        });
        db.Motos.AddRange(
            new Mottu.Api.Domain.Moto { Placa = "ABC-1234", Ano = 2022, Modelo = Mottu.Api.Domain.ModeloMoto.Sport },
            new Mottu.Api.Domain.Moto { Placa = "XYZ-0001", Ano = 2024, Modelo = Mottu.Api.Domain.ModeloMoto.Pop }
        );
        db.SaveChanges();
    }
}

app.UseCors("All");
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI(o =>
{
    o.SwaggerEndpoint("/swagger/v1/swagger.json", "Mottu API v1");
    o.DocumentTitle = "Mottu API Docs";
    o.RoutePrefix = "swagger"; 
});

app.MapHealthChecks("/health");
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
app.MapControllers();
app.Run();

public partial class Program { }

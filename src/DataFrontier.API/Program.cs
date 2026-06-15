using System.Text;
using DataFrontier.API.Middleware;
using DataFrontier.Infrastructure;
using DataFrontier.Infrastructure.Persistence.Seed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ── Camada de Infraestrutura ─────────────────────────────────────────
// Registra todos os serviços: ITenantResolver, ITenantConnectionResolver,
// AppDbContext, Interceptors, AuthService, TokenService, ProdutoService
builder.Services.AddInfrastructure(builder.Configuration);

// ── Autenticação JWT ─────────────────────────────────────────────────
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration["Jwt:SecretKey"]
                        ?? throw new InvalidOperationException(
                            "Jwt:SecretKey não configurada no appsettings.")))
        };
    });

builder.Services.AddAuthorization();

// ── Controllers ──────────────────────────────────────────────────────
builder.Services.AddControllers();

// ── Swagger / OpenAPI ────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "DataFrontier ERP API",
        Version = "v1",
        Description = "API do ERP DataFrontier — Multi-Tenant com suporte à Reforma Tributária brasileira."
    });

    // Configuração do botão "Authorize" no Swagger para JWT Bearer
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT no formato: Bearer {seu_token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

// ── CORS ─────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultPolicy", policy =>
    {
        policy
            .WithOrigins(
                builder.Configuration.GetSection("Cors:AllowedOrigins")
                    .Get<string[]>() ?? new[] { "http://localhost:5173" })
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// ── Seed de dados (apenas em Development) ────────────────────────────
if (app.Environment.IsDevelopment())
{
    await DatabaseSeeder.SeedAsync(app.Services);
}

// ── Pipeline HTTP ────────────────────────────────────────────────────
// Ordem crítica: Exception → Swagger → HTTPS → Routing → CORS
//                → Auth → Tenant → Authorization → Controllers

// 0. Tratamento global de exceções (PRIMEIRO no pipeline)
app.UseGlobalExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "DataFrontier ERP API v1");
        options.RoutePrefix = string.Empty; // Swagger na raiz
    });
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("DefaultPolicy");

// 1. Autenticação: decodifica o JWT e popula HttpContext.User com as claims
app.UseAuthentication();

// 2. Resolução de Tenant: lê as claims do JWT e resolve a connection string
//    DEVE estar entre UseAuthentication() e UseAuthorization()
app.UseTenantResolution();

// 3. Autorização: valida policies e roles após o tenant estar resolvido
app.UseAuthorization();

app.MapControllers();

app.Run();

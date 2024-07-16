using APIContagem;
using APIs.Security.JWT;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configurando o uso do Swagger para prever tokens JWT
builder.Services.AddSwaggerGenJwt("v1",
    new OpenApiInfo
    {
        Title = "APIContagem",
        Description = "Exemplo de implementacao de uso de JWT em uma API de contagem de acessos",
        Version = "v1"
    });

// Configurando o uso da classe de contexto para
// acesso as tabelas do ASP.NET Identity Core
builder.Services.AddDbContext<ApiSecurityDbContext>(options =>
    options.UseInMemoryDatabase("InMemoryDatabase"));

var tokenConfigurations = new TokenConfigurations();
new ConfigureFromConfigurationOptions<TokenConfigurations>(
    builder.Configuration.GetSection("TokenConfigurations"))
        .Configure(tokenConfigurations);

// Aciona a extensao que ira configurar o uso de
// autenticacao e autorizacao via tokens
builder.Services.AddJwtSecurity(tokenConfigurations);

// Acionar caso seja necessario criar usuarios para testes
builder.Services.AddScoped<IdentityInitializer>();

builder.Services.AddSingleton<Contador>();

builder.Services.AddCors();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

// Criacao de estruturas, usuarios e permissoes
// na base do ASP.NET Identity Core (caso ainda nao
// existam)
using var scope = app.Services.CreateScope();
scope.ServiceProvider.GetRequiredService<IdentityInitializer>().Initialize();

app.UseAuthorization();

app.MapControllers();

app.Run();
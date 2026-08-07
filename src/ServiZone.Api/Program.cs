using Microsoft.EntityFrameworkCore;
using ServiZone.Api.Endpoints;
using ServiZone.Api.Middleware;
using ServiZone.Api.Services;
using ServiZone.Domain.Interfaces;
using ServiZone.Infrastructure.Data;
using ServiZone.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Configuração de Serviços
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// HttpContextAccessor para acesso ao contexto HTTP
builder.Services.AddHttpContextAccessor();

// CurrentTenant para resolução do tenant (organização) atual
builder.Services.AddScoped<ICurrentTenant, CurrentTenant>();

// Autenticação e Autorização (esquema JWT Bearer a ser configurado no item 4 do NEXT_STEPS.md)
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

// Configuração do DbContext com PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ServiZoneDbContext>(options =>
    options.UseNpgsql(connectionString));

// Repositórios
builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();

// CORS (configurar de acordo com a necessidade)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configuração do Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middlewares
app.UseCorrelationId();
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

// Health Check básico
app.MapHealthEndpoints();

app.Run();

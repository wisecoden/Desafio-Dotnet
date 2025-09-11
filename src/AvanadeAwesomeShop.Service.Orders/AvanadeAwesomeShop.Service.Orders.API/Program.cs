using AvanadeAwesomeShop.Service.Orders.Infrastructure;
using AvanadeAwesomeShop.Service.Orders.Infrastructure.MessageBus.Services;
using AvanadeAwesomeShop.Service.Orders.Application.Commands.Handlers;
using FluentValidation.AspNetCore;
using FluentValidation;
using AvanadeAwesomeShop.Service.Orders.Application.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;

// Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", Serilog.Events.LogEventLevel.Information) // Permite ver as URLs
    .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Service", "Orders")
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341")
    .CreateLogger();

try
{
    Log.Information("🚀 Iniciando AvanadeAwesomeShop Orders Service");

    var builder = WebApplication.CreateBuilder(args);

    // Configurar Serilog
    builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();

// Add MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateOrderCommandHandler).Assembly));

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateOrderDtoValidator>();

// Add Infrastructure services (Database, Repositories)
builder.Services.AddInfrastructureServices(builder.Configuration);

// Configurar JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

/* builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Para desenvolvimento
    options.SaveToken = true;
    // Adicionando evento de erro para debug
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("Token validated successfully");
            return Task.CompletedTask;
        }
    };
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();
*/
// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Inicializar RabbitMQ Exchanges e Queues
using (var scope = app.Services.CreateScope())
{
    var rabbitMQInitializer = scope.ServiceProvider.GetRequiredService<IRabbitMQInitializer>();
    try
    {
        await rabbitMQInitializer.InitializeAsync();
        Console.WriteLine("🎉 RabbitMQ inicializado com sucesso!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️  Aviso: Não foi possível inicializar RabbitMQ: {ex.Message}");
        Console.WriteLine("A aplicação continuará funcionando, mas sem messaging.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

// Add Authentication and Authorization middleware
// app.UseAuthentication();
// app.UseAuthorization();

app.MapControllers();

app.Run();}
catch (Exception ex)
{
    Log.Fatal(ex, "❌ Orders Service falhou ao iniciar");
}
finally
{
    Log.Information("🔄 Encerrando Orders Service...");
    Log.CloseAndFlush();
}

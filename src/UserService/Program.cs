using UserService.Common.Authentication;
using Microsoft.EntityFrameworkCore;
using UserService.Common.Services;
using System.Reflection;
using FluentValidation;
using UserService.Data;
using UserService.Extensions;

var builder = WebApplication.CreateBuilder(args);

{
    var assembly = Assembly.GetExecutingAssembly();

    builder.Services.AddDbContext<AppDbContext>(
        options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
    );

    builder.Services.Configure<JwtSettings>(
        builder.Configuration.GetSection(JwtSettings.SectionName)
    );

    builder.Services
        .AddSingleton<IJwtTokenService, JwtTokenService>()
        .AddSingleton<IDateTimeProvider, DateTimeProvider>();

    builder.Services
        .AddScoped<IUserService, UserServiceImp>()
        .AddScoped<IUserRepository, UserRepository>();

    builder.Services.AddRabbitMq(builder.Configuration);
    
    builder.Services
        .AddValidatorsFromAssembly(assembly)
        .AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies())
        .AddControllers();

    builder.Services.AddCors(options => {
        options.AddPolicy("AllowAllOrigins",
            builder => {
                builder.AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowAnyOrigin();
            });
    });
}

var app = builder.Build();
{
    app.UseAuthorization();
    app.MapControllers();
    app.UseCors("AllowAllOrigins");
    app.Run();
}
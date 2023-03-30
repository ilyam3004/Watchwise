using WatchlistService.Data.DbContext;
using WatchlistService.Data.Repositories;
using System.Reflection;
using FluentValidation;
using WatchlistService.Common.Services;
using WatchlistService.SyncDataServices;

var builder = WebApplication.CreateBuilder(args);
{
    var assembly = Assembly.GetExecutingAssembly();

    builder.Services
        .AddScoped<IWatchlistService, WatchlistServiceImp>()
        .AddScoped<IWatchListRepository, WatchlistRepository>()
        .AddScoped<IWatchlistContext, WatchlistContext>()
        .AddSingleton<IMessageBusClient, MessageBusClient>()
        .AddValidatorsFromAssembly(assembly)
        .AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies())
        .AddControllers();

    builder.Services.Configure<DatabaseSettings>(
        builder.Configuration.GetSection(DatabaseSettings.SectionName)
    );
}

var app = builder.Build();
{
    app.UseAuthorization();
    app.MapControllers();
    app.Run();
}
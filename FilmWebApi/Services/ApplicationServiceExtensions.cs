﻿using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using FilmWebApi.DataBaseAccess;
using FilmWebApi.TmdbAccess;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;


namespace FilmWebApi.Services
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,IConfiguration config)
        {
            services.AddMvc();
            services.AddControllers();
            
            //DynamoDB Access
            var credentials = new BasicAWSCredentials(Constants.ACCESS_KEY, Constants.SECRET_KEY);
            var dynamoDbConfig = new AmazonDynamoDBConfig()
            {
                RegionEndpoint = RegionEndpoint.USEast1
            };    
            var client = new AmazonDynamoDBClient(credentials, dynamoDbConfig);
            
            //Singletons
            services.AddSingleton<IAmazonDynamoDB>(client);
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<IWatchListRepository, WatchListRepository>();
            services.AddSingleton<ITokenService, TokenService>();
            services.AddSingleton<IDynamoDBContext, DynamoDBContext>();
            //TMDBClient
            services.AddHttpClient<ITMDBHttpClient, TMDBHttpClient>();
            //JsonSetup
            services.AddControllers().AddNewtonsoftJson();
            //SwaggerConfiguration
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "dynamodb_sample", 
                    Version = "v1",
                    Description = "TestApi"
                });
            });
            return services;
        }
    }
}
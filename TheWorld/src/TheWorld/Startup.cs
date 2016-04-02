using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.DependencyInjection;
using TheWorld.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.PlatformAbstractions;
using TheWorld.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using AutoMapper;
using TheWorld.ViewModels;
using TheWorld.Controllers.Api;


// This file is the entry point into the App. 
// The key parts are: 
//    1. ConfigureServices()
//       This is where you define exactly which services your App needs to run and add them to
//       a IServiceCollection collection.
//       You can add predefined services within the MS Stack or your own using Add(scoped/transient/etc)
//      
//    2. Configure()
//       This is called by the runtime after ConfigureServices() and is able to use those services.
//       ASP.NET uses dependency injection to get those services, so you can then go on to configure them.
//       This is like defining your middleware in Express, creating your HTTP pipeline.
//
//    3. Constructor
//       This uses dependency injection to gain access to the IApplicationEnvironment to then go on to 
//       build an IConfigurationRoot hierarchical root using the ApplicationBasePath and adding the config JSON
//       file. This is then accessible by the app as Startup.Configuration["key1:subkey2"];

namespace TheWorld
{
    public class Startup
    {
        public static IConfigurationRoot Configuration;

        //       Constructor
        //       This uses dependency injection to gain access to the IApplicationEnvironment to then go on to 
        //       build an IConfigurationRoot hierarchical root using the ApplicationBasePath and adding the config JSON
        //       file. This is then accessible by the app as Startup.Configuration["key1:subkey2"];
        public Startup(IApplicationEnvironment appEnv)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(appEnv.ApplicationBasePath)
                .AddJsonFile("config.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }


        //       ConfigureServices()
        //       This is where you define exactly which services your App needs to run and add them to
        //       a IServiceCollection collection.
        //       You can add predefined services within the MS Stack or your own using Add(scoped/transient/etc)
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddJsonOptions(opt => // We want to return our Json objects in camelcase
                {
                    opt.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });
            // Tell our App to used EF7 alongside Sql and to use our WorldContext for Tables/Properties.
            services.AddEntityFramework()
                .AddSqlServer()
                .AddDbContext<WorldContext>();
            services.AddTransient<WorldContextSeedData>();
            services.AddScoped<IWorldRepository, WorldRepository>();
            services.AddLogging();

            // This strangely formatted if block checks to see if the application is running in DEBUG mode, as opposed to production.
            // If it is, use our DebugMailService which is a concrete implementation of the IMailService interface we defined, otherwise
            // use the as-yet-implemented MailService.
#if DEBUG
            services.AddScoped<IMailService, DebugMailService>();
#else
            services.AddScoped<IMailService, MailService>();
#endif

        }
             
        //    2. Configure()
        //       This is called by the runtime after ConfigureServices() and is able to use those services.
        //       ASP.NET uses dependency injection to get those services, so you can then go on to configure them.
        //       This is like defining your middleware in Express, creating your HTTP pipeline.
        public void Configure(IApplicationBuilder app, WorldContextSeedData seeder, ILoggerFactory loggerFactory)
        {
            // Middleware - called sequentially, order is important!!
            // LOGGING - For systems logs
            loggerFactory.AddDebug(LogLevel.Warning);
            // STATIC FILES - For serving static files e.g. src="~/css/site.css"
            app.UseStaticFiles();
            // AUTOMAPPER - Using AutoMapper NuGet package to map viewmodels to models in our controller - notably the API
            //              Note that you don't need to specify collections containing models here, most of them come bundled
            Mapper.Initialize(config =>
            {
                config.CreateMap<Trip, TripViewModel>().ReverseMap();
                config.CreateMap<Stop, StopViewModel>().ReverseMap();
            });
            // MVC6 CONFIG - breaks down a route into the controller, action(method on controller) and id(optional)
            app.UseMvc(config =>
            {
                config.MapRoute(
                    name: "Default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "App", action = "Index" }
                    
                );
            });
            // This is just a development tool we created to ensure that if the database is empty, seed some data into it.
            seeder.EnsureSeedData();
        }
        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}

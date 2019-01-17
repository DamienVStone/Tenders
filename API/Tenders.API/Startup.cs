using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tenders.API.DAL.Elastic;
using Tenders.API.DAL.Elastic.Interfaces;
using Tenders.API.DAL.Interfaces;
using Tenders.API.DAL.Mongo;
using Tenders.API.DAL.Mongo.Interfaces;
using Tenders.API.Services;
using Tenders.API.Services.Interfaces;
using static Tenders.Core.DI.Container;

namespace Tenders.API
{
    public static class TestEnvHelper
    {
        public static bool IsTestEnv = true;
    }

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            TestEnvHelper.IsTestEnv = false;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Registration.Register(services);
            //services.AddCors();
            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSingleton<IAPIConfigService, APIConfigService>();
            services.AddSingleton<IElasticDbContext, ElasticDBContext>();
            services.AddSingleton<IMongoDbContext, MongoDbContext>();

            services.AddSingleton<IIdProvider, MongoIdProvider>();
            services.AddSingleton<IFTPPathRepo, FTPPathMongoRepo>();
            services.AddSingleton<IFTPEntryRepo, FTPEntryMongoRepo>();
            services.AddSingleton<ITenderPlanIndexRepo, TenderPlanIndexMongoRepo>();

            services.AddSingleton<IPathService, PathService>();
            services.AddSingleton<ITreeLookerService, TreeLookerService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors(builder =>
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
            );
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TenderPlanAPI.Classes;
using TenderPlanAPI.Controllers;
using TenderPlanAPI.Services;
using Tenders.API.Services;
using static Tenders.Core.DI.Container;

namespace TenderPlanAPI
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
            services
                            .AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(opt =>
                {
                    opt.SerializerSettings.Converters.Add(new ObjectIdConverter());
                });
            services.AddSingleton<IAPIConfigService, APIConfigService>();
            services.AddSingleton<IDBConnectContext, DBConnectContext>();
            services.AddSingleton<IPathService, PathService>();
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

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}

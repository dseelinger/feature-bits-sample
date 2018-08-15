using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FeatureBits.Core;
using FeatureBits.Data;
using FeatureBits.Data.AzureTableStorage;
using FeatureBits.Data.EF;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SampleWeb
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json",
                    optional: false,
                    reloadOnChange: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string featureBitsConnectionString = Configuration.GetConnectionString("FeatureBitsDbContext");
            services.AddDbContext<FeatureBitsEfDbContext>(options => options.UseSqlServer(featureBitsConnectionString));
            services.AddTransient<IFeatureBitsRepo, FeatureBitsEfRepo>((serviceProvider) =>
            {
                DbContextOptionsBuilder<FeatureBitsEfDbContext> options = new DbContextOptionsBuilder<FeatureBitsEfDbContext>();
                options.UseSqlServer(featureBitsConnectionString);
                var context = new FeatureBitsEfDbContext(options.Options);
                return new FeatureBitsEfRepo(context);
            });
            services.AddTransient<IFeatureBitEvaluator, FeatureBitEvaluator>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}

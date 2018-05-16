using System.IO;
using Customer.Api.Application.Background;
using Customer.Api.Application.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.Swagger;
using UnitOfWorks.EntityFrameworkCore;

namespace Customer.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHostedService, CustomerHostedService>();

            services.AddMvcCore()
                // Add Api Explorer, so Swagger can find the API versioning in
                // the controller / actions.
                .AddVersionedApiExplorer(o => o.GroupNameFormat = "'v'VVV");

            services.AddMvc();

            // MediatR
            services.AddMediatR(typeof(Startup));

            // Add Api Versioning to the project.
            services.AddApiVersioning();

            // Add swagger documentation.
            services.AddSwaggerGen(c =>
            {
                var provider = services.BuildServiceProvider()
                    .GetRequiredService<IApiVersionDescriptionProvider>();

                foreach (var description in provider.ApiVersionDescriptions)
                {
                    c.SwaggerDoc(
                        description.GroupName,
                        new Info()
                        {
                            Title = $"Customer API {description.ApiVersion}",
                            Version = description.ApiVersion.ToString()
                        });
                }

                var filePath = Path.Combine(
                    PlatformServices.Default.Application.ApplicationBasePath,
                    "Customer.Api.xml");

                c.IncludeXmlComments(filePath);
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                // Seed the database in development mode.
                new CustomerDbContext()
                    .SeedDatabaseWithJson<Application.Entities.Customer>("Seed/Customer.json");
            }

            app.UseMvc();

            // Add Swagger.
            app.UseSwagger();

            // Add Swagger UI.
            app.UseSwaggerUI(
                options =>
                {
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint(
                            $"/swagger/{description.GroupName}/swagger.json",
                            description.GroupName.ToUpperInvariant());
                    }
                });

        }
    }
}

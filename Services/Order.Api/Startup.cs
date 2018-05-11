using System.IO;
using AspNetCore.Validation.Filters;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.PlatformAbstractions;
using Order.Api.Application.Orchestra;
using Swashbuckle.AspNetCore.Swagger;

namespace Order.Api
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
            services.AddSingleton<IHostedService, OrderOrchestraService>();

            services.AddMvcCore()
                // Add Api Explorer, so Swagger can find the API versioning in
                // the controller / actions.
                .AddVersionedApiExplorer(o => o.GroupNameFormat = "'v'VVV");

            services.AddMvc(options => options.Filters.Add(typeof(ValidatorActionFilter)))
                // Add Fluent Validatin to the mvc. This will load all the validators.
                .AddFluentValidation(fvc => fvc.RegisterValidatorsFromAssemblyContaining<Startup>());

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
                            Title = $"Order API {description.ApiVersion}",
                            Version = description.ApiVersion.ToString()
                        });
                }

                var filePath = Path.Combine(
                    PlatformServices.Default.Application.ApplicationBasePath,
                    "Order.Api.xml");

                c.IncludeXmlComments(filePath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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

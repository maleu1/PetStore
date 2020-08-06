using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace PetStore
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
            services.AddControllers();
            
            services.AddVersionedApiExplorer(o =>
            {
                o.GroupNameFormat = "'v'VV";
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.AssumeDefaultVersionWhenUnspecified = true;
            });

            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.AssumeDefaultVersionWhenUnspecified = true;
            });
            
            // swagger registration
            services.AddSwaggerGen(c =>
            {
                const string securityDefinition = "Bearer";
                c.AddSecurityDefinition(securityDefinition, new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme()
                        {
                            Reference = new OpenApiReference{
                                Id = securityDefinition,
                                Type = ReferenceType.SecurityScheme
                            }
                        }, new List<string>()
                    }
                });

                IApiVersionDescriptionProvider provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
                foreach (ApiVersionDescription description in provider.ApiVersionDescriptions)
                {
                    if (!string.IsNullOrWhiteSpace(description.GroupName))
                    {
                        c.SwaggerDoc(description.GroupName,
                            IsFlutterApi(description) 
                                ? CreateFlutterInfo(description.ApiVersion.MajorVersion ?? 1) 
                                : CreateInfoForApiVersion(description));
                    }
                }

                string xmlDocFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlDocFileName);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);   
                }
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(
                options =>
                {
                    // build a swagger endpoint for each discovered API version
                    foreach (ApiVersionDescription description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                            IsFlutterApi(description) 
                                ? "V2.0 - flutter only API"
                                : description.GroupName.ToUpperInvariant());
                    }
                    
                    options.DocExpansion(DocExpansion.None);
                });
            
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
        
        private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new OpenApiInfo()
            {
                Title = $"PetStore API {description.ApiVersion}",
                Version = description.ApiVersion.ToString(),
                Description = "Public APIs of the PetStore application landscape.",
                Contact = new OpenApiContact()
                {
                    Name = "PetStore Company",
                    Url = new Uri("https://www.PetStoreCompany.com")
                }
            };

            if (description.IsDeprecated)
            {
                info.Description += "This API version has been deprecated.";
            }

            return info;
        }
        
        private static OpenApiInfo CreateFlutterInfo(int majorVersion)
        {
            var info = new OpenApiInfo()
            {
                Title = "PetStore Flutter API",
                Version = $"{majorVersion}.0",
                Description = "Public APIs of PetStore's flutter app.",
                Contact = new OpenApiContact()
                {
                    Name = "PetStore Company",
                    Url = new Uri("https://www.PetStoreCompany.com")
                }
            };

            return info;
        }
        
        private static bool IsFlutterApi(ApiVersionDescription description)
        {
            return description.ApiVersion.MinorVersion == 1;
        }
    }
}
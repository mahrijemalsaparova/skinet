using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace API.Extensions
{
    public static class SwaggerServiceExtensions
    {
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo  {Title = "SkiNet API", Version = "v1"});

                var securitySchema = new OpenApiSecurityScheme
                {
                    Description = "JWT Auth Bearer Schema",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };

                c.AddSecurityDefinition("Bearer", securitySchema);
                var securityRequirenment = new OpenApiSecurityRequirement {{securitySchema, new[] {"Bearer"}}};
                c.AddSecurityRequirement(securityRequirenment);
            });

            return services;
        }

        public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
        {
             // Endpointlerimizi Swagger'da göstermek için uygulanan middleware
            
            app.UseSwagger();
            //we're using swagger UI as well which will allow us to browse to a web page
            // which is going to show all of our API endpoints.
            app.UseSwaggerUI(c =>{c
            .SwaggerEndpoint("/swagger/v1/swagger.json", "SkiNet API v1");});  //localhost:5001/swagger/v1/swagger.json

            return app;
        }
    }
}
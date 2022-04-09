using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Web.Api.Models.Interfaz;
using Web.Api.RepositoryTrax.Core;
using Web.Api.RepositoryTrax.Repository;
using Web.Api.DAO.ContextBD;
using Web.Api.Models.Repositorio;
using Web.Api.Models.Settings;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using WEBAPITRAX.Auth;
namespace WEBAPITRAX
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            String currentPath = System.AppContext.BaseDirectory.ToLower();
            var builder = new ConfigurationBuilder().SetBasePath(currentPath).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddScoped<IServices, CoreApi>();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("ServiciosRest", new Microsoft.OpenApi.Models.OpenApiInfo()
                {
                    Title = "API REST SERVICIOS CORE ",
                    Version = "1.0",
                    Description = "Documentación"
                });
                options.CustomSchemaIds(type => type.ToString());
            });
            services.AddMvc(option => option.EnableEndpointRouting = false )
                .SetCompatibilityVersion(CompatibilityVersion.Latest)
                .AddNewtonsoftJson(opt => opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

            //services.AddMvc(option => option.EnableEndpointRouting = false)
            //    .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
            //     .AddNewtonsoftJson(opt => opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

            //services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = Configuration["Jwt:Audience"],
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                };
            });
            services.AddAuthorization();
            //services.AddSingleton<IJwtAuthenticationService>(new JwtAuthenticationService(key));

            services.Configure<AppSettings>(Configuration.GetSection("Keys"));

            services.AddControllers();
            services.AddLogging(loggingBuilder =>
            {
                var loggingSection = Configuration.GetSection("Logging");
                loggingBuilder.AddFile(loggingSection);
            });

        }
      
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/ServiciosRest/swagger.json", "Api Core");
                options.RoutePrefix = string.Empty;
            });
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            
            
        }
    }
}

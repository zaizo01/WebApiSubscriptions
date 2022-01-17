using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WebAPIAutores.Entities;
using WebAPIAutores.Filters;
using WebAPIAutores.Middlewares;
using WebAPIAutores.Services;
using WebAPIAutores.Utilities;

[assembly: ApiConventionType(typeof(DefaultApiConventions))]
namespace WebAPIAutores
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {


            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(ExceptionFilter));
                options.Conventions.Add(new SwaggerGroupByVersion());
            }).AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore).AddNewtonsoftJson()
            .AddXmlDataContractSerializerFormatters();
            
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters { 
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Configuration["keyJwt"])),
                    ClockSkew = TimeSpan.Zero
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "WebAPIAutores",
                    Version = "v1",
                    Description = "This is a little description",
                    Contact = new OpenApiContact
                    {
                        Email = "e.zaizortega@gmail.com",
                        Name = "Cristopher Zaiz Ortega",
                        Url = new Uri("https://czaiz.com")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT"
                    }
                }); 
                c.SwaggerDoc("v2", new OpenApiInfo { Title = "WebAPIAutores", Version = "v2" });
                c.OperationFilter<AddParamtersHATEOAS>();
                c.OperationFilter<AddParameterXVersion>();
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{}
                    }
                });

                var fileXML = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var routeXML = Path.Combine(AppContext.BaseDirectory, fileXML);
                c.IncludeXmlComments(routeXML);

            });

            services.AddAutoMapper(typeof(Startup));

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("IsAdmin", politics => politics.RequireClaim("IsAdmin"));
            });

            services.AddDataProtection();

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
                        .WithExposedHeaders(new string[] { "TotalNumberOfRecords" });
                });
            });

            services.AddScoped<KeysServices>();
            services.AddHostedService<InvoiceHostedService>();
            services.AddTransient<HashService>();
            services.AddTransient<LinkGenerator>();
            services.AddTransient<HATEOASAuthorFilterAttribute>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {

            app.UseHttpResponseLog();

           
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                
            }
            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPIAutores v1");
                c.SwaggerEndpoint("/swagger/v2/swagger.json", "WebAPIAutores v2");
                });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseLimitRequests();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

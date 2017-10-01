using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using HeadRepositoryNet.Entities;
using Dapper.FastCrud;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IO;
using HeadRepositoryNet.Services;

namespace HeadRepositoryNet
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
            services.Configure<DataAccessOptions>(Configuration.GetSection("dataAccessOptions"));
            //services.Configure<AuthOptions>(Configuration.GetSection("authOptions"));
            OrmConfiguration.DefaultDialect = SqlDialect.PostgreSql;
            
            InitAuthOptions();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.RequireHttpsMetadata = false;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            // укзывает, будет ли валидироваться издатель при валидации токена
                            ValidateIssuer = true,
                            // строка, представляющая издателя
                            ValidIssuer = AuthOptions.Issuer,
 
                            // будет ли валидироваться потребитель токена
                            ValidateAudience = true,
                            // установка потребителя токена
                            ValidAudience = AuthOptions.Audience,
                            // будет ли валидироваться время существования
                            ValidateLifetime = true,
 
                            // установка ключа безопасности
                            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(AuthOptions.KeyAccess),
                            // валидация ключа безопасности
                            ValidateIssuerSigningKey = true,
                        };
                    });

            //services.UseMiddleware<ExceptionMiddleware>();
            
            /*services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder.WithOrigins("http://localhost:37885")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());//builder.WithOrigins("http://localhost:4200"));
            });*/
            services.AddCors();


            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            /*
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); 
            }*/
            //app.UseCors("AllowSpecificOrigin");
            app.UseCors(builder =>
                builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());

            app.UseAuthentication();

            app.UseMvc();
        }

        private void InitAuthOptions()
        {            
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

            var conf = builder.Build();
            AuthOptions.Issuer = conf["authOptions:issuer"];
            AuthOptions.Audience = conf["authOptions:audience"];
            AuthOptions.KeyRefresh = conf["authOptions:keyRefresh"];
            AuthOptions.KeyAccess = conf["authOptions:keyAccess"];
            AuthOptions.LifetimeAccess = Convert.ToInt32(conf["authOptions:lifetimeAccess"]);
            AuthOptions.LifetimeRefresh = Convert.ToInt32(conf["authOptions:lifetimeRefresh"]);
        }
    }
}

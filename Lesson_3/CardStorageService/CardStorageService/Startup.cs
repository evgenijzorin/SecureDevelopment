using CardStorageService.Data;
using CardStorageService.Services;
using CardStorageService.Services.Impl;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CardStorageService
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
            #region Configure Options
            services.Configure<DatabaseOptions>(options =>
            {
                // ������� ������ �������� � �������� DatabaseOptions
                Configuration.GetSection("Settings:DatabaseOptions").Bind(options);
            });
            #endregion

            #region ���������� �������� (��������� ����������� DI)
            services.AddSingleton<IAuthenticateService, AuthenticateService>();
            services.AddScoped<IClientRepositoryService, ClientRepository>();
            services.AddScoped<ICardRepositoryService, CardRepository>();
            // AddScoped ������������ ������ � �������� �������� ������� ������������� - �������� ������������� ������ �������. 
            // ��������� ��� ���� ������
            #endregion

            #region Configure logging
            services.AddHttpLogging(logging =>
            {
                logging.LoggingFields = HttpLoggingFields.All | HttpLoggingFields.RequestQuery;
                logging.RequestBodyLogLimit = 4096;
                logging.ResponseBodyLogLimit = 4096;
                logging.RequestHeaders.Add("Authorization");
                logging.RequestHeaders.Add("X-Real-IP");
                logging.RequestHeaders.Add("X-Forwarded-For");
            });
            #endregion

            #region ���������� ��������� ���� ������, � ��� ��������� �� ��������� ConectionString � appsettings.json
            services.AddDbContext<CardStorageServiceDbContext>(options =>
            {
                options.UseSqlServer(Configuration["Settings:DatabaseOptions:ConnectionString"]);
            });
            #endregion

            services.AddControllers();

            #region Instal authentication services
            services.AddAuthentication(x => // Add a service with properties
            {
                x.DefaultAuthenticateScheme =
                JwtBearerDefaults.AuthenticationScheme; // use the defalt scheme for autentication
                x.DefaultChallengeScheme =
                JwtBearerDefaults.AuthenticationScheme;
            })
                // ���������� ������������ � ��������� ��� ���������
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;// ����������� �� http ����������
                    options.SaveToken = true; // ������ �� ����� ������������ ���� �������� � ������� ����������� � ������ ���. �����������
                    options.TokenValidationParameters = new TokenValidationParameters // ��������� ���������
                    {
                        ValidateIssuerSigningKey = true, // ���������� �� �������� ���������� ��������� ����� ������������
                        IssuerSigningKey = // �������� ����� // �������� ���� ������� ����� ����������� ��� ��������
                        new SymmetricSecurityKey(Encoding.ASCII.GetBytes(AuthenticateService.SecretKey)), // �������� ��� ���� � �������� � ASCII

                        ValidateIssuer = false, // �������� ������������ �� ����� �������� ������
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero
                    };
                }); 
            
            #endregion
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CardStoregeService", Version = "v1" });
                c.CustomOperationIds(SwaggerUtils.OperationIdProvider);
                // ����������� ������ �������� �������������� ��� �������
                // ���������� �������� ������:
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "JWT Authorization header using the Bearer scheme(Example: 'Bearer 12345abcdef')",  // ��������� ��� ������������
                    Name = "Authorization", // ��� ���������
                    In = ParameterLocation.Header, // ��� ��������� ����
                    Type = SecuritySchemeType.ApiKey, // ��� �������� �����
                    Scheme = "Bearer" // ��� ����� �����������
                });
                // ����������� ���������� ������.
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme()
                        {
                            Reference = new OpenApiReference()
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CardStoregeService v1"));
            }

            app.UseRouting();
            // ����������� Http ��������:
            app.UseHttpLogging();

            // ����������� � ��������������            
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}

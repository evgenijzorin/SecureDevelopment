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
                // Связать секцию настроек с объектом DatabaseOptions
                Configuration.GetSection("Settings:DatabaseOptions").Bind(options);
            });
            #endregion

            #region инсталяция сервисов (внедрение зависимости DI)
            services.AddSingleton<IAuthenticateService, AuthenticateService>();
            services.AddScoped<IClientRepositoryService, ClientRepository>();
            services.AddScoped<ICardRepositoryService, CardRepository>();
            // AddScoped регистрирует службу с заданной областью времени существования - временем существования одного запроса. 
            // приемлимо для базы данных
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

            #region добавление контекста базы данных, и его настройка по средством ConectionString в appsettings.json
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
                // Подключить предъявителя и настроить его параметры
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;// Запрашивать ли http метаданные
                    options.SaveToken = true; // должен ли токен предъявителя быть сохранен в сервисе авторизации в случае усп. авторизации
                    options.TokenValidationParameters = new TokenValidationParameters // параметры валидации
                    {
                        ValidateIssuerSigningKey = true, // вызывается ли проверка валидности защитного ключа предъявителя
                        IssuerSigningKey = // издатель ключа // содержит ключ который будет использован для проверки
                        new SymmetricSecurityKey(Encoding.ASCII.GetBytes(AuthenticateService.SecretKey)), // передаем наш ключ и кодируем в ASCII

                        ValidateIssuer = false, // Проверка предъявителя во время проверки токена
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero
                    };
                }); 
            
            #endregion
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CardStoregeService", Version = "v1" });
                c.CustomOperationIds(SwaggerUtils.OperationIdProvider);
                // Подключение секций проверки аутентификации для свагера
                // Поключение описания защиты:
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "JWT Authorization header using the Bearer scheme(Example: 'Bearer 12345abcdef')",  // подсказка для пользователя
                    Name = "Authorization", // имя заголовка
                    In = ParameterLocation.Header, // кде находится ключ
                    Type = SecuritySchemeType.ApiKey, // тип защитной схемы
                    Scheme = "Bearer" // имя схемы авторизации
                });
                // Подключение требований защиты.
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
            // Логирование Http запросов:
            app.UseHttpLogging();

            // авторизация а аутентификация            
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}

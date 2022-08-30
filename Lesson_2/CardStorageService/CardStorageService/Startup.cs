using CardStorageService.Data;
using CardStorageService.Services;
using CardStorageService.Services.Impl;
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
                // —в€зать секцию настроек с объектом DatabaseOptions
                Configuration.GetSection("Settings:DatabaseOptions").Bind(options);
            });
            #endregion

            #region инстал€ци€ сервисов (внедрение зависимости DI)
            services.AddSingleton<IAuthenticateService, AuthenticateService>();
            services.AddScoped<IClientRepositoryService, ClientRepository>();
            services.AddScoped<ICardRepositoryService, CardRepository>();
            // AddScoped регистрирует службу с заданной областью времени существовани€ - временем существовани€ одного запроса. 
            // приемлимо дл€ базы данных
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

            #region добавление контекста базы данных, и его настройка по средством ConectionString
            services.AddDbContext<CardStorageServiceDbContext>(options =>
            {
                options.UseSqlServer(Configuration["Settings:DatabaseOptions:ConnectionString"]);
            });
            #endregion

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CardStoregeService", Version = "v1" });
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
            // Ћогирование Http запросов:
            app.UseHttpLogging();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

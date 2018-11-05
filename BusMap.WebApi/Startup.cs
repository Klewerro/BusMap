﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusMap.WebApi.Automapper;
using BusMap.WebApi.Data;
using BusMap.WebApi.Repositories.Abstract;
using BusMap.WebApi.Repositories.Implementations;
using BusMap.WebApi.Services;
using BusMap.WebApi.Services.Abstract;
using BusMap.WebApi.Services.Implementations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BusMap.WebApi
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddDbContext<DatabaseContext>(options =>
                options.UseSqlServer(Connections.GeDbConnectionString()));

            services.AddScoped<IBusStopRepository, BusStopRepository>();
            services.AddScoped<IRouteRepository, RouteRepository>();
            services.AddScoped<ICarrierRepository, CarrierRepository>();
            services.AddScoped<IQueueRepository, QueueRepository>();
            services.AddScoped<ITraceRepository, TraceRepository>();

            services.AddScoped<ICarrierService, CarrierService>();
            services.AddScoped<IBusStopService, BusStopService>();
            services.AddScoped<IRouteService, RouteService>();
            services.AddScoped<IQueueService, QueueService>();
            services.AddScoped<ITraceService, TraceService>();

            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutomapperProfile());
            });
            var mapper = config.CreateMapper();
            services.AddSingleton(mapper);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();

        }
    }
}

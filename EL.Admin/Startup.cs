using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Autofac;
using EL.Common;
using EL.EntityFrameworkCore;
using EL.Repository;
using Microsoft.Extensions.Logging;

namespace EL.Admin
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
            //redis注册
            RedisExt.Init();
            var connection = new JsonConfigManager().GetValue<string>("ELConnection");
            services.AddDbContext<ELDbContext>(options => options.UseLazyLoadingProxies().UseMySql(connection));
            //services.AddTransient(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddDistributedMemoryCache()
                    .AddSession(options =>
                    {
                        options.IdleTimeout = TimeSpan.FromSeconds(10);
                        options.Cookie.HttpOnly = true;
                        options.Cookie.IsEssential = true;
                    });
            services.AddControllersWithViews();
        }
        public void ConfigureContainer(ContainerBuilder builder)
        {
            // 在这里添加服务注册
            builder.RegisterGeneric(typeof(BaseRepository<>)).As(typeof(IBaseRepository<>)).InstancePerDependency();
            builder.RegisterAssemblyTypes(Assembly.Load("EL.Application")).Where(a => a.Name.EndsWith("Service")).AsImplementedInterfaces();
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSession();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

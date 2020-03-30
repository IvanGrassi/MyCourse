using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyCourse.Models.Services.Application;
using MyCourse.Models.Services.Infrastructure;
using Westwind.AspNetCore.LiveReload;

namespace MyCourse
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)          //lega le interfacce ad implementazioni
        {
#if DEBUG
            services.AddLiveReload();
#endif
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddTransient<ICourseService, AdoNetCourseServices>();        //ogni volta che un componente ha una dipendenza da ICourseServices, in realtà la sostituisce e coustruisce un AdoNetCourseServices
            services.AddTransient<IDatabaseAccess, SqlLiteDatabaseAccess>();       //ogni volta che un componente ha una dipendenza da IDatabaseAccess, dotnetcore inietterà un istanza di SqlLiteDatabaseAccess
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())                                        //il middleware viene usato solo quando siamo in ambiente development
            {
                app.UseDeveloperExceptionPage();                            //primo middleware, SEMPRE per primo, produce una pagina informativa in caso di errore
            }

#if DEBUG
            app.UseLiveReload();
#endif

            //l'ambiente (production o development) viene definito nel launchsettings.json

            app.UseStaticFiles();                                           //middleware file statici (immagini ad esempio)

            app.UseRouting();

            app.UseEndpoints(endpoints => //secondo middleware
            {
                //app.UseMvcWithDefaultRoute();
                //configurato una route con il template controller, action e id. ora il middleware é in grado di estrapolare info dal percorso della richiesta utente

                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");  //settati i valori di default, id é opzionale
                //Courses/Detail/5 = Controller/Action/id
            });
        }
    }
}

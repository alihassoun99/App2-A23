using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.Google;

namespace Sanssoussi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddControllersWithViews();

            // A02:2021 : Stockage des donnes sensible
            services.AddAuthentication().AddGoogle(
                GoogleOptions =>
                {
                    GoogleOptions.ClientId = Configuration["Authentication:Google:ClientId"];
                    GoogleOptions.ClientSecret= Configuration["Authentication:Google:ClientSecret"];
                });

            // INFO EN CLAIR JUST POUR La validation et la problematique

            // ID : 387338168029-77jciq0gvlv3ubo7h5886mb46e7d8tgo.apps.googleusercontent.com
            // sercet : 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

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts(); // A02:2021 : Defaillance cryptographique (Protocole de chiffrement Faible) -> Forcer l'utilisation du HTTPS en 
                                // appliquant le mecanisme HSTS

            }

            app.UseHttpsRedirection(); // A02:2021 : Defaillance cryptographique (Protocole de chiffrement Faible) -> Forcer la redirection vers HTTPS quand c'Est un HTTP



            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Home}/{action=Index}/{id?}");
                    endpoints.MapRazorPages();
                });
        }
    }
}
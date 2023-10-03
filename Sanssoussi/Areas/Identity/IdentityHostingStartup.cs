using Microsoft.AspNetCore.Hosting;

using Microsoft.AspNetCore.Identity;


using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Sanssoussi.Areas.Identity;
using Sanssoussi.Areas.Identity.Data;
using Sanssoussi.Data;

[assembly: HostingStartup(typeof(IdentityHostingStartup))]

namespace Sanssoussi.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices(
                (context, services) =>
                {
                    services.AddDbContext<SanssoussiContext>(
                        options =>
                            options.UseSqlite(
                                context.Configuration.GetConnectionString("SanssoussiContextConnection")));

                    // ajout des roles - Ali
                    services.AddDefaultIdentity<SanssoussiUser>(options => options.SignIn.RequireConfirmedAccount = true)

                        .AddRoles<IdentityRole>()

                        .AddEntityFrameworkStores<SanssoussiContext>();
                });
        }
    }
}
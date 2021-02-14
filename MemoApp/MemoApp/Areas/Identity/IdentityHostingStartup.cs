using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(MemoApp.Areas.Identity.IdentityHostingStartup))]
namespace MemoApp.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Download_Vouchery.Startup))]
namespace Download_Vouchery
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

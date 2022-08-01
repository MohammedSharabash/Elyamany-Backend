using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ElYamanyDashboard.Startup))]
namespace ElYamanyDashboard
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

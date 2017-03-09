using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(dbUpdater.Startup))]
namespace dbUpdater
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IranAudioGuide_MainServer.Startup))]
namespace IranAudioGuide_MainServer
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

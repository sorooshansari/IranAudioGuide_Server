using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IranAudioGuide_Server.Startup))]
namespace IranAudioGuide_Server
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

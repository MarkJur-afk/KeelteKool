using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(KeelteKool.Startup))]
namespace KeelteKool
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

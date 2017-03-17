using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(com.yannis.client.Startup))]
namespace com.yannis.client
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GladcherryShopping.Startup))]
namespace GladcherryShopping
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

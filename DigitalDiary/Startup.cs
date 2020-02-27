using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DigitalDiary.Startup))]
namespace DigitalDiary
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

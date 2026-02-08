using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(notesApp.Startup))]
namespace notesApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

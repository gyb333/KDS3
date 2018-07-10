using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wilmar.Service.Security.Infrastructure;

[assembly: OwinStartup(typeof(Wilmar.Service.Security.SecurityConfigure))]

namespace Wilmar.Service.Security
{
    internal class SecurityConfigure
    {
        public void Configuration(IAppBuilder app)
        {
            OAuthBearerAuthenticationOptions options = new OAuthBearerAuthenticationOptions();

            ApplicationSignInManager.Initialize(options);
            

            app.UseOAuthBearerAuthentication(options);
        }
    }
}

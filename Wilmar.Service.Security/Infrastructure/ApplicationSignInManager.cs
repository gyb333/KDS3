using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Wilmar.Service.Security.Model;

namespace Wilmar.Service.Security.Infrastructure
{
    /// <summary>
    /// 登陆管理
    /// </summary>
    public class ApplicationSignInManager : SignInManager<ApplicationUser, int>
    {
        private static OAuthBearerAuthenticationOptions AuthenticationOptions;
        private static readonly TimeSpan ExpiresTime = TimeSpan.FromDays(1);

        /// <summary>
        /// 创建登陆管理对象
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="authenticationManager"></param>
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
            this.AuthenticationType = AuthenticationOptions.AuthenticationType;
        }
        /// <summary>
        /// 创建登陆管理对象
        /// </summary>
        /// <param name="dbcontext"></param>
        /// <param name="owin"></param>
        public ApplicationSignInManager(ApplicationDbContext dbcontext, IOwinContext owin)
            : this(new ApplicationUserManager(dbcontext), owin.Authentication)
        {

        }
        /// <summary>
        /// 创建登陆管理对象
        /// </summary>
        /// <param name="dbcontext"></param>
        public ApplicationSignInManager(ApplicationDbContext dbcontext)
            : this(new ApplicationUserManager(dbcontext), HttpContext.Current.GetOwinContext().Authentication)
        {

        }
        /// <summary>
        /// 初始化认证选项
        /// </summary>
        /// <param name="options"></param>
        public static void Initialize(OAuthBearerAuthenticationOptions options)
        {
            if (AuthenticationOptions == null)
            {
                AuthenticationOptions = options;
            }
        }
        /// <summary>
        /// 创建认证凭证
        /// </summary>
        /// <returns></returns>
        public AuthenticationTicket CreateTicket()
        {
            return CreateTicket(this.AuthenticationManager.AuthenticationResponseGrant.Identity);
        }
        /// <summary>
        /// 创建认证凭证
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public AuthenticationTicket CreateTicket(ClaimsIdentity identity)
        {
            var now = AuthenticationOptions.SystemClock.UtcNow;
            AuthenticationProperties property = new AuthenticationProperties()
            {
                IsPersistent = true,
                IssuedUtc = now,
                AllowRefresh = true,
                ExpiresUtc = now.Add(ExpiresTime)
            };
            return new AuthenticationTicket(identity, property);
        }
        /// <summary>
        /// 加密认证凭证
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public string ProtectTicket(AuthenticationTicket ticket)
        {
            return AuthenticationOptions.AccessTokenFormat.Protect(ticket);
        }
        /// <summary>
        /// 解密认证凭证
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public AuthenticationTicket UnprotectTicket(string content)
        {
            return AuthenticationOptions.AccessTokenFormat.Unprotect(content);
        }
        /// <summary>
        /// 当前成功登陆的用户标识
        /// </summary>
        public ClaimsIdentity SignInIdentity
        {
            get { return this.AuthenticationManager.AuthenticationResponseGrant.Identity; }
        }
    }
}

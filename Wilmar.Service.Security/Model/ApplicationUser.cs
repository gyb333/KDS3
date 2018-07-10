using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wilmar.Service.Security.Model
{
    /// <summary>
    /// 全局应用程序用户
    /// </summary>
    [Table(nameof(ApplicationUser))]
    public class ApplicationUser : IdentityUser<int, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
    {
    }
}

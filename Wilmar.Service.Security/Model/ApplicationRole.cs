using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wilmar.Service.Security.Model
{
    /// <summary>
    /// 角色对象，一个用户可以拥有多个角色
    /// </summary>
    [Table(nameof(ApplicationRole))]
    public class ApplicationRole : IdentityRole<int, ApplicationUserRole>
    {
        /// <summary>
        /// 角色应用权限的优先级
        /// </summary>
        public int Priority { get; set; }
        /// <summary>
        /// 当前角色所在的项目ID
        /// </summary>
        public int ProjectId { get; set; }
    }
}

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wilmar.Service.Security.Model;

namespace Wilmar.Service.Security.Infrastructure
{
    public class ApplicationRoleManager : RoleManager<ApplicationRole, int>
    {
        public ApplicationRoleManager(IRoleStore<ApplicationRole, int> store)
            : base(store)
        {

        }

        public ApplicationRoleManager(ApplicationDbContext dbcontext)
            :this(new RoleStore<ApplicationRole, int, ApplicationUserRole>(dbcontext))
        {

        }
    }
}

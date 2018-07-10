using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wilmar.Service.Security.Model
{
    [Table(nameof(ApplicationUserLogin))]
    public class ApplicationUserLogin : IdentityUserLogin<int>
    {

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Compilation;
using System.Web.Http.Dispatcher;

namespace Wilmar.Service.Core.OData
{
    public class PlatformAssembliesResolver : IAssembliesResolver
    {

        private ICollection<Assembly> Assemblys = null;

        public ICollection<Assembly> GetAssemblies()
        {
            if (Assemblys == null)
            {
                Assemblys = BuildManager.GetReferencedAssemblies().OfType<Assembly>().ToList<Assembly>();
                foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (asm.FullName.StartsWith("Wilmar.Project."))
                    {
                        if (!Assemblys.Contains(asm))
                            Assemblys.Add(asm);
                    }
                }
            }
            return Assemblys;
        }
    }
}

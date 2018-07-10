using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wilmar.Compile.Core.Service.Models
{
    public class MetadataBase
    {
        public string Identity
        {
            get { return Project.Identity; }
        }

        public ProjectMetadata Project { get; private set; }
        
        public virtual void Initialize(ProjectMetadata project)
        {
            this.Project = project;
        }
    }
}

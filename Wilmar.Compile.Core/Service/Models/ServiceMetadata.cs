using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wilmar.Compile.Core.Service.Models
{
    public class ServiceMetadata : MetadataBase
    {

        public List<EntityMetadata> Entitys { get; private set; } = new List<EntityMetadata>();

        public List<ControllerActionMetadata> Actions { get; private set; } = new List<ControllerActionMetadata>();
    }
}

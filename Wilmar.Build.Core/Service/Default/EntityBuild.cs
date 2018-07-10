using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wilmar.Build.Core.Service.Default.ServiceTemplate;
using Wilmar.Compile.Core.Service;
using Wilmar.Foundation.Common;
using Wilmar.Foundation.Projects;
using Wilmar.Model.Core.Definitions;
using Wilmar.Service.Common.Generate;

namespace Wilmar.Build.Core.Service.Default
{
    public class EntityBuild : BuildBase
    {
        public override void Build(CompileBase compile, Foundation.Projects.ProjectDocument doc)
        {
            try
            {
                var serviceCompile = compile as ServiceCompile;
                if (serviceCompile != null)
                {
                    var entity = serviceCompile.Data.Entitys[doc.Id];
                    string result = new EntityTemplate(entity).TransformText();
                    serviceCompile.GenerateCode("Models", entity.ProjectItem.Name, result);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("EntityBuild:" + ex.ToString());
            }

        }

        public override int BuildTypeId
        {
            get { return GlobalIds.BuildType.Entity; }
        }

        public override int Id
        {
            get { return GlobalIds.BuildType.Entity + 1; }
        }

        public override string Title
        {
            get { return "默认实体生成器"; }
        }
    }
}

using System;
using Wilmar.Build.Core.Service.Default.ServiceTemplate;
using Wilmar.Compile.Core.Service;
using Wilmar.Foundation.Common;
using Wilmar.Model.Core.Definitions;
using Wilmar.Service.Common.Generate;

namespace Wilmar.Build.Core.Service.Default
{
    public class EntityControllterBuild : BuildBase
    {
        public override void Build(CompileBase compile, Foundation.Projects.ProjectDocument doc)
        {
            try
            {
                var serviceCompile = compile as ServiceCompile;
                if (serviceCompile != null)
                {
                    var entity = serviceCompile.GetDocumentBody(doc) as EntityDefinition;
                    if (entity != null && entity.Controller != null && entity.Members.Count > 0)
                    {
                        var data = serviceCompile.Data.Controllers[doc.Id];
                        string result = new ControllerTemplate(data).TransformText();
                        serviceCompile.GenerateCode("Controllers", data.ClassName, result);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("DataControllterBuild:" + ex.ToString());
            }
        }

        public override int BuildTypeId
        {
            get { return GlobalIds.BuildType.EntityControllter; }
        }

        public override int Id
        {
            get { return GlobalIds.BuildType.EntityControllter + 1; }
        }

        public override string Title
        {
            get { return "默认实体控制器生成器"; }
        }
    }
}

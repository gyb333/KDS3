using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wilmar.Build.Core.Service.Default.ServiceTemplate;
using Wilmar.Compile.Core.Service;
using Wilmar.Foundation.Common;
using Wilmar.Service.Common.Generate;

namespace Wilmar.Build.Core.Service.Default
{
    public class ServiceBuild : BuildBase
    {

        public override void Build(CompileBase compile, Foundation.Projects.ProjectDocument doc)
        {
            try
            {
                var serviceCompile = compile as ServiceCompile;
                if (serviceCompile != null)
                {
                    string strResult = new ProjectServiceTemplate(compile, serviceCompile.Data.Service).TransformText();
                    serviceCompile.GenerateCode("", serviceCompile.Data.Service.Identity + "ProjectService", strResult);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ProjectServiceBuild:" + ex.ToString());
            }
        }

        public override int BuildTypeId
        {
            get { return GlobalIds.BuildType.Service; }
        }

        public override int Id
        {
            get { return GlobalIds.BuildType.Service + 1; }
        }

        public override string Title
        {
            get { return "默认项目服务"; }
        }
    }
}

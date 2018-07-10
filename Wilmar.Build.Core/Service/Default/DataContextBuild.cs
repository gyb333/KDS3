using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wilmar.Build.Core.Service.Default.ServiceTemplate;
using Wilmar.Compile.Core.Service;
using Wilmar.Foundation.Common;
using Wilmar.Foundation.Projects;
using Wilmar.Foundation.Projects.Configures;
using Wilmar.Service.Common.Generate;

namespace Wilmar.Build.Core.Service.Default
{
    public class DataContextBuild : BuildBase
    {
        public override void Build(CompileBase compile, Foundation.Projects.ProjectDocument doc)
        {
            try
            {
                var serviceCompile = compile as ServiceCompile;
                foreach (var item in serviceCompile.Data.DataContexts.Values)
                {
                    string result = new DataContextTemplate(item).TransformText();
                    serviceCompile.GenerateCode("", item.ClassName, result);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("DataContextBuild:" + ex.ToString());
            }
        }

        public override int BuildTypeId
        {
            get { return GlobalIds.BuildType.DataContext; }
        }

        public override int Id
        {
            get { return GlobalIds.BuildType.DataContext + 1; }
        }

        public override string Title
        {
            get { return "默认数据上下文生成器"; }
        }
    }
}

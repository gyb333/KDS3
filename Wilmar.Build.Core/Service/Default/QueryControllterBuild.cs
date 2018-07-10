using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wilmar.Foundation.Common;
using Wilmar.Service.Common.Generate;

namespace Wilmar.Build.Core.Service.Default
{
    public class QueryControllterBuild : BuildBase
    {
        public override void Build(CompileBase compile, Foundation.Projects.ProjectDocument doc)
        {
            throw new NotImplementedException();
        }

        public override int BuildTypeId
        {
            get { return GlobalIds.BuildType.QueryControllter; }
        }

        public override int Id
        {
            get { return GlobalIds.BuildType.QueryControllter + 1; }
        }

        public override string Title
        {
            get { return "默认查询控制器生成器"; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wilmar.Foundation.Common;
using Wilmar.Service.Common.Generate;

namespace Wilmar.Compile.Core.Service
{
    /// <summary>
    /// 实体控制器生成类型
    /// </summary>
    public class EntityControllterBuildType : BuildTypeBase
    {
        public override int Id
        {
            get { return GlobalIds.BuildType.EntityControllter; }
        }

        public override string Title
        {
            get { return "实体数据控制器"; }
        }

        public override int DocumentType
        {
            get { return GlobalIds.DocumentType.Entity; }
        }
    }

}

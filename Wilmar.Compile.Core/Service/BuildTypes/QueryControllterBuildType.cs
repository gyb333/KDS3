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
    /// 查询控制器生成类型
    /// </summary>
    public class QueryControllterBuildType : BuildTypeBase
    {
        public override int Id
        {
            get { return GlobalIds.BuildType.QueryControllter; }
        }

        public override string Title
        {
            get { return "查询数据控制器"; }
        }

        public override int DocumentType
        {
            get { return GlobalIds.DocumentType.Query; }
        }
    }

}

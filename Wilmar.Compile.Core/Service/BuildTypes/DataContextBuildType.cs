using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wilmar.Foundation.Common;
using Wilmar.Service.Common.Generate;

namespace Wilmar.Compile.Core.Service
{
    public class DataContextBuildType : BuildTypeBase
    {
        public override int Id
        {
            get { return GlobalIds.BuildType.DataContext; }
        }

        public override string Title
        {
            get { return "数据上下文"; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wilmar.Foundation.Common;
using Wilmar.Service.Common.Generate;

namespace Wilmar.Compile.Core.Service
{
    public class EntityBuildType : BuildTypeBase
    {
        public override int Id
        {
            get { return GlobalIds.BuildType.Entity; }
        }

        public override string Title
        {
            get { return "实体定义"; }
        }

        public override int DocumentType
        {
            get { return GlobalIds.DocumentType.Entity; }
        }
    }
}

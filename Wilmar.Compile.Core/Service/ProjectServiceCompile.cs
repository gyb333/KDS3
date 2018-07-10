using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wilmar.Foundation.Common;
using Wilmar.Service.Common.Generate;

namespace Wilmar.Compile.Core.Service
{
    public class ProjectServiceCompile : CompileBase
    {

        public override void BeginCompile()
        {
            
        }

        public override int Id
        {
            get { return GlobalIds.Compile.Service; }
        }

        public override int[] BuildTypes
        {
            get { return _BuildTypes; }
        }
        private static int[] _BuildTypes = new int[] { 

            GlobalIds.BuildType.Service
        };
        public override void EndCompile()
        {
            
        }
 
        public override string Title
        {
            get { return  "默认项目服务编译"; }
        }
    }
}

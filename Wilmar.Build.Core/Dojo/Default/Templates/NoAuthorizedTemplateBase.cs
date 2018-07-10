using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wilmar.Compile.Core.Service;
using Wilmar.Compile.Core.Service.Models;
using Wilmar.Foundation.Common;
using Wilmar.Foundation.Projects;
using Wilmar.Model.Core.Definitions;
using Wilmar.Model.Core.Definitions.Entities;
using Wilmar.Model.Core.Definitions.Entities.Configure;
using Wilmar.Model.Core.Definitions.Entities.DataTypes;
using Wilmar.Service.Common.Generate;

namespace Wilmar.Build.Core.Dojo.Default.Templates
{
    public partial class NoAuthorizedTemplate
    {
        public NoAuthorizedTemplate(CompileBase compile)
        {
            this.Compile = compile;
        }

        /// <summary>
        /// 编译对象
        /// </summary>
        public CompileBase Compile
        {
            get; private set;
        }
        
        /// <summary>
        /// 项目标识
        /// </summary>
        public string Identity
        {
            get { return Compile.Project.Identity; }
        }
    }
}

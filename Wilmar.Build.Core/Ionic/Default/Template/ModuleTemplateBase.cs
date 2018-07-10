using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wilmar.Compile.Core.Service;
using Wilmar.Foundation.Projects;
using Wilmar.Foundation;
using Wilmar.Model.Core.Definitions;
using Wilmar.Model.Core.Definitions.Entities.Configure;
using System.IO;
using Wilmar.Model.Core.Definitions.Querites;
using System.Text.RegularExpressions;
using Wilmar.Compile.Core.Service.Models;
using Wilmar.Model.Core.Definitions.Entities;
using Wilmar.Model.Core.Definitions.Entities.DataTypes;
using Wilmar.Build.Core.Dojo.Default.Builders;
using Wilmar.Model.Core.Definitions.Entities.Members;
using Wilmar.Compile.Core.Ionic;
using System.Collections;
using Wilmar.Foundation.Common;

namespace Wilmar.Build.Core.Ionic.Default.Template
{
    /// <summary>
    /// 控制器模版
    /// </summary>
    public partial class ModuleTemplate
    {
        /// <summary>
        /// 编译对象
        /// </summary>
        public IonicCompile Compile
        {
            get; private set;
        }
        /// <summary>
        /// 文档对象
        /// </summary>
        public ProjectDocument Documnet
        {
            get; private set;
        }
        /// <summary>
        /// 页面集合
        /// </summary>
        private List<string> Pages = new List<string>();

        public ModuleTemplate(IonicCompile ionicCompile, ProjectDocument doc)
        {
            this.Compile = ionicCompile;
            this.Documnet = doc;
            Initial();
        }
        /// <summary>
        /// 初始化数据
        /// </summary>
        private void Initial()
        {
            var items = this.Compile.ProjectItems.Where(a => a.Value.DocumentType == GlobalIds.DocumentType.Screen).ToList();
            foreach (var item in items)
            {
                if (item.Value.Name.ToLower() == "startuppage") continue;
                this.Pages.Add(item.Value.Name);
            }
        }

        /// <summary>
        /// 返回页面字符串
        /// </summary>
        /// <returns></returns>
        public string GenerateCode()
        {
            if (this.Pages.Count > 0)
            {
                return string.Join(",", this.Pages.ToArray());
            }
            return string.Empty;
        }
    }
}

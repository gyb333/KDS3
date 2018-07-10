using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wilmar.Foundation.Projects;
using Wilmar.Model.Core.Definitions;
using System.IO;
using System.Text.RegularExpressions;
using Wilmar.Compile.Core.Ionic;
using System.Collections;
using Wilmar.Model.Core.Definitions.Screens.Members;
using Wilmar.Model.Core.Definitions.Screens;
using Wilmar.Model.Core.Definitions.Entities.DataTypes;
using Wilmar.Build.Core.Dojo.Default.Builders;
using Wilmar.Foundation.Common;

namespace Wilmar.Build.Core.Ionic.Default.Template
{
    /// <summary>
    /// 控制器模版
    /// </summary>
    public partial class PageContainerTemplate
    {
        /// <summary>
        /// 编译对象
        /// </summary>
        public IonicCompile Compile
        {
            get; private set;
        }
        /// <summary>
        /// 模型集合
        /// </summary>
        private List<string> Models = new List<string>();

        public PageContainerTemplate(IonicCompile ionicCompile)
        {
            this.Compile = ionicCompile;
            Initial();
        }
        /// <summary>
        /// 初始化数据
        /// </summary>
        private void Initial()
        {
            var entityItems = this.Compile.ProjectItems.Where(a => a.Value.DocumentType == GlobalIds.DocumentType.Entity).ToList();
            foreach (var entityItem in entityItems)
            {
                this.Models.Add(entityItem.Value.Name);
            }
        }

        /// <summary>
        /// 返回模型字符串
        /// </summary>
        /// <returns></returns>
        public string GenerateCode()
        {
            if (this.Models.Count > 0)
            {
                return string.Join(",", this.Models.ToArray());
            }
            return string.Empty;
        }
    }
}

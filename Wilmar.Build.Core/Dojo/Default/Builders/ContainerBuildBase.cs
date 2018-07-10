using System;
using System.Collections.Generic;
using System.Web.UI;
using Wilmar.Foundation.Projects;
using Wilmar.Model.Core.Definitions;
using Wilmar.Model.Core.Definitions.Screens;
using Wilmar.Service.Common.Generate;

namespace Wilmar.Build.Core.Dojo.Default.Builders
{
    /// <summary>
    /// 控件生成器
    /// </summary>
    public abstract class ContainerBuildBase : ControlBuildBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="controlHost">控件</param>
        /// <param name="compile">编译器对象</param>
        /// <param name="htmlWriter">htmlWriter</param>
        public ContainerBuildBase(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
            : base(isPreview, controlHost, screenDef, compile, doc, permissionData, htmlWriter)
        {

        }

        /// <summary>
        /// 生成HTML
        /// </summary>
        protected override void SetChildElements()
        {
            //渲染子控件
            foreach (var c in this.ControlHost.Children)
            {
                var builder = c.GetBuilder(this.IsPreview, this.ScreenDefinition, this.Compile, this.ProjectDocument, this.PermissionData, this.HtmlWriter);
                builder.Parent = this;
                builder.Build();
            }
        }
    }
}

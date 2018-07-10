using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using Wilmar.Foundation.Projects;
using Wilmar.Model.Core.Definitions;
using Wilmar.Model.Core.Definitions.Screens;
using Wilmar.Model.Core.Definitions.Screens.Controls;
using Wilmar.Service.Common.Generate;

namespace Wilmar.Build.Core.Dojo.Default.Builders
{
    /// <summary>
    /// 标题面板生成器
    /// </summary>
    internal class TitlePaneBuild : ContainerBuildBase
    {
        public TitlePaneBuild(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
            : base(isPreview, controlHost, screenDef, compile, doc, permissionData, htmlWriter)
        {

        }

        /// <summary>
        /// 设置属性
        /// </summary>
        protected override void SetAttributes()
        {
            TitlePane control = (TitlePane)this.ControlHost.Content;
            
            base.SetAttributes();
        }
        /// <summary>
        /// 设置子元素
        /// </summary>
        protected override void SetChildElements()
        {
            TitlePane control = (TitlePane)this.ControlHost.Content;
            this.HtmlWriter.AddAttribute("dojoType", "Controls/TitlePane");
            if (!IsPreview && !string.IsNullOrEmpty(this.ControlHost.Name))
            {
                this.HtmlWriter.AddAttribute("id", this.ControlHost.Name);
                this.HtmlWriter.AddAttribute("name", this.ControlHost.Name);
            }
            this.HtmlWriter.AddAttribute("toggleable", control.CanExpand.ToString().ToLower());
            this.HtmlWriter.AddAttribute("open", control.IsExpanded.ToString().ToLower());
            this.HtmlWriter.AddAttribute("duration", control.Duration.ToString());
            this.HtmlWriter.AddAttribute("title", this.ControlHost.Title);

            StringBuilder sbProps = new StringBuilder();
            StringBuilder returnContent = new StringBuilder();
            string props = control.BuildControlProps(this.ScreenDefinition, this.IsPreview, this.PermissionData, returnContent);
            if (!string.IsNullOrEmpty(props)) sbProps.AppendFormat("{0},", props);
            if (sbProps.ToString().Length > 0)
            {
                this.HtmlWriter.AddAttribute("data-dojo-props", sbProps.ToString().Substring(0, sbProps.ToString().Length - 1), false);
            }

            this.HtmlWriter.RenderBeginTag(this.TagName);
            //子元素
            foreach (var child in this.ControlHost.Children)
            {
                var builder = child.GetBuilder(this.IsPreview, this.ScreenDefinition, this.Compile, this.ProjectDocument, this.PermissionData, this.HtmlWriter);
                builder.Parent = this;
                builder.Build();
            }
            this.HtmlWriter.RenderEndTag();
        }
    }
}

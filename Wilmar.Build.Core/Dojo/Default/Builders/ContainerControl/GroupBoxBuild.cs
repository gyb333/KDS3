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
    /// 组合框生成器
    /// </summary>
    internal class GroupBoxBuild : ContainerBuildBase
    {
        public GroupBoxBuild(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
            : base(isPreview, controlHost, screenDef, compile, doc, permissionData, htmlWriter)
        {

        }

        /// <summary>
        /// 设置属性
        /// </summary>
        protected override void SetAttributes()
        {
            GroupBox control = this.ControlHost.Content as GroupBox;

            base.SetAttributes();
        }
        /// <summary>
        /// 设置子元素
        /// </summary>
        protected override void SetChildElements()
        {
            GroupBox control = this.ControlHost.Content as GroupBox;
            this.HtmlWriter.AddAttribute("dojoType", "Controls/Fieldset");
            if (!IsPreview && !string.IsNullOrEmpty(this.ControlHost.Name))
            {
                this.HtmlWriter.AddAttribute("id", this.ControlHost.Name);
                this.HtmlWriter.AddAttribute("name", this.ControlHost.Name);
            }
            if (!control.CanExpand)
            {
                this.HtmlWriter.AddAttribute("toggleable", "false");
            }
            if (!control.IsExpanded)
            {
                this.HtmlWriter.AddAttribute("open", "false");
            }

            StringBuilder sbProps = new StringBuilder();
            StringBuilder returnContent = new StringBuilder();
            string props = control.BuildControlProps(this.ScreenDefinition, this.IsPreview, this.PermissionData, returnContent);
            if (!string.IsNullOrEmpty(props)) sbProps.AppendFormat("{0},", props);
            if (!IsPreview) sbProps.AppendFormat("{0},", "context:at('rel:','VM')");
            if (sbProps.ToString().Length > 0)
            {
                this.HtmlWriter.AddAttribute("data-dojo-props", sbProps.ToString().Substring(0, sbProps.ToString().Length - 1), false);
            }

            //标题
            this.HtmlWriter.RenderBeginTag(this.TagName);
            this.HtmlWriter.RenderBeginTag("legend");
            if (!string.IsNullOrEmpty(this.ControlHost.Title))
            {
                this.HtmlWriter.WriteEncodedText(this.ControlHost.Title);
            }
            this.HtmlWriter.RenderEndTag();

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

using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Web.UI;
using Wilmar.Foundation.Projects;
using Wilmar.Model.Core.Definitions;
using Wilmar.Model.Core.Definitions.Configure;
using Wilmar.Model.Core.Definitions.Screens;
using Wilmar.Model.Core.Definitions.Screens.Controls;
using Wilmar.Service.Common.Generate;
using System;

namespace Wilmar.Build.Core.Dojo.Default.Builders
{
    /// <summary>
    /// 流布局容器生成器
    /// </summary>
    internal class FluidLayoutBuild : ContainerBuildBase
    {
        public FluidLayoutBuild(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
            : base(isPreview, controlHost, screenDef, compile, doc, permissionData, htmlWriter)
        {

        }

        /// <summary>
        /// 设置属性
        /// </summary>
        protected override void SetAttributes()
        {
            FluidLayout control = this.ControlHost.Content as FluidLayout;
            this.HtmlWriter.AddAttribute("dojoType", "Controls/FluidLayout");
            if (!IsPreview && !string.IsNullOrEmpty(this.ControlHost.Name))
            {
                this.HtmlWriter.AddAttribute("id", this.ControlHost.Name);
                this.HtmlWriter.AddAttribute("name", this.ControlHost.Name);
            }
            if (control.Cols > 0) this.HtmlWriter.AddAttribute("cols", control.Cols.ToString());
            if (control.LabelWidth > 0) this.HtmlWriter.AddAttribute("labelWidth", control.LabelWidth.ToString());
            string[] alignHorizontals = new string[] { "left", "center", "right", "right" };
            string labelHorizontal = alignHorizontals[(int)control.LabelPosition];
            this.HtmlWriter.AddAttribute("labelPosition", labelHorizontal);
            this.HtmlWriter.AddAttribute("showLabels", control.ShowLabels.ToString().ToLower());

            StringBuilder sbProps = new StringBuilder();
            StringBuilder returnContent = new StringBuilder();
            string props = control.BuildControlProps(this.ScreenDefinition, this.IsPreview, this.PermissionData, returnContent);
            if (!string.IsNullOrEmpty(props)) sbProps.AppendFormat("{0},", props);
            if (sbProps.ToString().Length > 0)
            {
                this.HtmlWriter.AddAttribute("data-dojo-props", sbProps.ToString().Substring(0, sbProps.ToString().Length - 1), false);
            }

            base.SetAttributes();
        }

        /// <summary>
        /// 设置子元素
        /// </summary>
        protected override void SetChildElements()
        {
            FluidLayout control = this.ControlHost.Content as FluidLayout;
            foreach (var child in this.ControlHost.Children)
            {
                this.HtmlWriter.AddAttribute("dojoType", "Controls/ContentPane");
                this.HtmlWriter.AddAttribute("label", child.Title);
                string controlName = child.Content.GetType().Name;
                if (controlName == "Textarea")
                {
                    Textarea ctl = child.Content as Textarea;
                    if (ctl.Height != null && ctl.Height > 0) this.HtmlWriter.AddAttribute("customHeight", ctl.Height.ToString());
                }
                else if (controlName == "RichTextBox")
                {
                    RichTextBox ctl = child.Content as RichTextBox;
                    if (ctl.Height != null && ctl.Height > 0) this.HtmlWriter.AddAttribute("customHeight", ctl.Height.ToString());
                }
                FluidLayoutAttach formAttach =  child.AttachObject as FluidLayoutAttach;
                if (formAttach.ColumnSpan > 1) this.HtmlWriter.AddAttribute("colspan", formAttach.ColumnSpan.ToString().ToLower());
                //if (formAttach.CollapsedTitle) this.HtmlWriter.AddAttribute("spanLabel", "true");
                this.HtmlWriter.RenderBeginTag("div");
                var builder = child.GetBuilder(this.IsPreview, this.ScreenDefinition, this.Compile, this.ProjectDocument, this.PermissionData, this.HtmlWriter);
                builder.Parent = this;
                builder.Build();
                this.HtmlWriter.RenderEndTag();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using Wilmar.Foundation.Projects;
using Wilmar.Model.Core.Definitions;
using Wilmar.Model.Core.Definitions.Screens;
using Wilmar.Model.Core.Definitions.Screens.Controls;
using Wilmar.Service.Common.Generate;

namespace Wilmar.Build.Core.Dojo.Default.Builders
{
    /// <summary>
    /// 日历生成器
    /// </summary>
    internal class CalendarBuild : ControlBuildBase
    {
        public CalendarBuild(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
            : base(isPreview, controlHost, screenDef, compile, doc, permissionData, htmlWriter)
        {

        }

        /// <summary>
        /// 重载生成
        /// </summary>
        public override void Build()
        {
            this.SetAttributes();
            //创建开始标签
            this.HtmlWriter.RenderBeginTag(this.TagName);

            this.ControlAttribute();
            this.HtmlWriter.RenderBeginTag(this.TagName);
            this.HtmlWriter.RenderEndTag();

            //创建结束标签
            this.HtmlWriter.RenderEndTag();
        }
        /// <summary>
        /// 设置属性
        /// </summary>
        private void ControlAttribute()
        {
            Calendar control = this.ControlHost.Content as Calendar;
            this.HtmlWriter.AddAttribute("dojoType", "Controls/Calendar");
            if (!IsPreview && !string.IsNullOrEmpty(this.ControlHost.Name))
            {
                this.HtmlWriter.AddAttribute("id", this.ControlHost.Name);
                this.HtmlWriter.AddAttribute("name", this.ControlHost.Name);
            }
            if (!string.IsNullOrEmpty(control.ToolTip) && this.ProjectDocument != null)
            {
                this.HtmlWriter.AddAttribute("tooltip-name", this.ProjectDocument.Name + "_" + control.ToolTip);
            }

            StringBuilder sbProps = new StringBuilder();
            StringBuilder returnContent = new StringBuilder();
            string props = control.BuildControlProps(this.ScreenDefinition, this.IsPreview, this.PermissionData, returnContent);
            if (!string.IsNullOrEmpty(props)) sbProps.AppendFormat("{0},", props);
            if (sbProps.ToString().Length > 0)
            {
                this.HtmlWriter.AddAttribute("data-dojo-props", sbProps.ToString().Substring(0, sbProps.ToString().Length - 1), false);
            }

            this.HtmlWriter.AddAttribute("style", "width:100%;height:100%");
        }
    }
}

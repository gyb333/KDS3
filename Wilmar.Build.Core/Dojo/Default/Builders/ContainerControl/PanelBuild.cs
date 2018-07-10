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
    /// 面板控件生成器
    /// </summary>
    internal class PanelBuild : ContainerBuildBase
    {
        public PanelBuild(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
            : base(isPreview, controlHost, screenDef, compile, doc, permissionData, htmlWriter)
        {

        }

        /// <summary>
        /// 设置属性
        /// </summary>
        protected override void SetAttributes()
        {
            Panel control = this.ControlHost.Content as Panel;
            this.HtmlWriter.AddAttribute("dojoType", "Controls/Panel");
            if (!IsPreview && !string.IsNullOrEmpty(this.ControlHost.Name))
            {
                this.HtmlWriter.AddAttribute("id", this.ControlHost.Name);
                this.HtmlWriter.AddAttribute("name", this.ControlHost.Name);
            }
            
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
        /// 设置Style
        /// </summary>
        protected override string SetControlStyle()
        {
            StringBuilder baseStyle = new StringBuilder();
            Panel control = this.ControlHost.Content as Panel;

            string url = "Images/";
            if (!this.IsPreview)
            {
                url = "./Projects/" + this.Compile.Project.Identity + "/Images/";
            }
            if (!string.IsNullOrEmpty(control.BackGroundImage))
            {
                baseStyle.AppendFormat("background-image: url('{0}');", url + control.BackGroundImage);
            }

            return base.SetControlStyle() + baseStyle.ToString();
        }
    }
}

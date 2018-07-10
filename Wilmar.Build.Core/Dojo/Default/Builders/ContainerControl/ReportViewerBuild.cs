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
using Wilmar.Model.Core.Definitions.Screens.Members;
using Wilmar.Service.Common.Generate;

namespace Wilmar.Build.Core.Dojo.Default.Builders
{
    /// <summary>
    /// 报表控件生成器
    /// </summary>
    internal class ReportViewerBuild : ContainerBuildBase
    {
        public ReportViewerBuild(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
            : base(isPreview, controlHost, screenDef, compile, doc, permissionData, htmlWriter)
        {

        }

        /// <summary>
        /// 设置属性
        /// </summary>
        protected override void SetAttributes()
        {
            ReportViewer control = (ReportViewer)this.ControlHost.Content;
            this.HtmlWriter.AddAttribute("dojoType", "Controls/ReportViewer");
            if (!IsPreview && !string.IsNullOrEmpty(this.ControlHost.Name))
            {
                this.HtmlWriter.AddAttribute("id", this.ControlHost.Name);
                this.HtmlWriter.AddAttribute("name", this.ControlHost.Name);
            }
            this.HtmlWriter.AddAttribute("toolbarVisible", control.Toolbar.ToString().ToLower());
            this.HtmlWriter.AddAttribute("searchVisible", control.Parameters.ToString().ToLower());
            string reprotProps = GetReportProsps(this.ControlHost);
            if (!string.IsNullOrEmpty(reprotProps))
            {
                this.HtmlWriter.AddAttribute("report", reprotProps, false);
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

        private string GetReportProsps(ControlHost controlHost)
        {
            string result = string.Empty;
            ReportViewer control = controlHost.Content as ReportViewer;
            if (!this.IsPreview && control.Bindings.Count > 0)
            {
                Dictionary<string, string> dictProperty = control.GetPropertyBindValue();
                foreach (var item in control.Bindings)
                {
                    string bindPath = item.Path == null ? "" : item.Path;
                    string bindProperty = item.Property == null ? "" : item.Property;
                    if (bindProperty.ToLower() == "value" && !string.IsNullOrEmpty(bindPath))
                    {
                        result = string.Format("at('rel:','{0}').direction(1)", bindPath);
                        break;
                    }
                }
            }
            return result;
        }
    }
}

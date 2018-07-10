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
    /// 图形容器生成器
    /// </summary>
    internal class ChartPaneBuild : ControlBuildBase
    {
        public ChartPaneBuild(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
            : base(isPreview, controlHost, screenDef, compile, doc, permissionData, htmlWriter)
        {

        }

        /// <summary>
        /// 设置属性
        /// </summary>
        protected override void SetAttributes()
        {
            DataGrid control = this.ControlHost.Content as DataGrid;
            this.HtmlWriter.AddAttribute("dojoType", "Controls/Panel");
        }

        /// <summary>
        /// 设置子属性
        /// </summary>
        protected override void SetChildElements()
        {
            ChartPane control = this.ControlHost.Content as ChartPane;
            this.HtmlWriter.AddAttribute("dojoType", "Controls/Charting/Chart");
            if (!IsPreview && !string.IsNullOrEmpty(this.ControlHost.Name))
            {
                this.HtmlWriter.AddAttribute("id", this.ControlHost.Name);
            }
            string chartTheme = control.ChartTheme.ToString();
            if (control.ChartTheme == EChartTheme.PlotKitBlue) chartTheme = "PlotKit.blue";
            if (control.ChartTheme == EChartTheme.PlotKitCyan) chartTheme = "PlotKit.cyan";
            if (control.ChartTheme == EChartTheme.PlotKitGreen) chartTheme = "PlotKit.green";
            if (control.ChartTheme == EChartTheme.PlotKitOrange) chartTheme = "PlotKit.orange";
            if (control.ChartTheme == EChartTheme.PlotKitPurple) chartTheme = "PlotKit.purple";
            if (control.ChartTheme == EChartTheme.PlotKitRed) chartTheme = "PlotKit.red";
            this.HtmlWriter.AddAttribute("theme", "dojox.charting.themes." + chartTheme);

            int width = 0;
            StringBuilder styleStr = new StringBuilder();
            if (control.Width != null && control.Width > 0) width = width - 1;
            if (width > 0) styleStr.AppendFormat("width:{0}px !important;", control.Width.ToString());
            else styleStr.AppendFormat("width:calc(100% - 1px) !important;");
            if (control.Height != null && control.Height > 0) styleStr.AppendFormat("height:{0}px !important;", control.Height.ToString());
            if (!string.IsNullOrEmpty(styleStr.ToString()))
            {
                this.HtmlWriter.AddAttribute("style", styleStr.ToString());
            }


            //
            this.HtmlWriter.RenderBeginTag(this.TagName);
            foreach (var c in this.ControlHost.Children)
            {
                var builder = c.GetBuilder(this.IsPreview, this.ScreenDefinition, this.Compile, this.ProjectDocument, this.PermissionData, this.HtmlWriter);
                builder.Parent = this;
                builder.Build();
            }
            this.HtmlWriter.RenderEndTag();
        }
    }
}

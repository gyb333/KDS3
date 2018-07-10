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
    /// 图表标题生成器
    /// </summary>
    internal class ChartLegendBuild : ControlBuildBase
    {
        public ChartLegendBuild(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
            : base(isPreview, controlHost, screenDef, compile, doc, permissionData, htmlWriter)
        {

        }

        /// <summary>
        /// 设置属性
        /// </summary>
        protected override void SetAttributes()
        {
            ChartLegend control = this.ControlHost.Content as ChartLegend;
            if (control.LegendType == EChartLegendType.Legend) this.HtmlWriter.AddAttribute("dojoType", "Controls/Charting/Legend");
            else if (control.LegendType == EChartLegendType.SelectableLegend) this.HtmlWriter.AddAttribute("dojoType", "Controls/Charting/SelectableLegend");
            if (this.ProjectDocument != null && !string.IsNullOrEmpty(control.ChartRef))
            {
                this.HtmlWriter.AddAttribute("chartRef", this.ProjectDocument.Name + "_" + control.ChartRef);
                this.HtmlWriter.AddAttribute("id", this.ProjectDocument.Name + "_" + control.ChartRef + "_" + control.LegendType.ToString().ToLower());
            }

            base.SetAttributes();
        }
    }
}

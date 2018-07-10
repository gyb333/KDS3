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
    /// 饼图生成器
    /// </summary>
    internal class PieChartBuild : ControlBuildBase
    {
        public PieChartBuild(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
            : base(isPreview, controlHost, screenDef, compile, doc, permissionData, htmlWriter)
        {

        }

        /// <summary>
        /// 设置属性
        /// </summary>
        protected override void SetAttributes()
        {
            PieChart control = this.ControlHost.Content as PieChart;
            this.HtmlWriter.AddAttribute("dojoType", "Controls/Charting/Plot");
            if (!this.IsPreview)
            {
                if (!string.IsNullOrEmpty(this.ControlHost.Name))
                {
                    string name = this.ControlHost.Name.Replace(this.ProjectDocument.Name + "_", "");
                    this.HtmlWriter.AddAttribute("name", name);
                }
                else this.HtmlWriter.AddAttribute("name", "default");
            }

            this.HtmlWriter.AddAttribute("class", "plot");
            this.HtmlWriter.AddAttribute("type", "Pie");
            this.HtmlWriter.AddAttribute("omitLabels", "true");
            if (control.Radius > 0) this.HtmlWriter.AddAttribute("radius", control.Radius.ToString());
            if (control.LabelOffset != null) this.HtmlWriter.AddAttribute("labelOffset", control.LabelOffset.ToString());
            if (control.PieType == EChartPieType.PieLineChart)
            {
                this.HtmlWriter.AddAttribute("radGrad", "linear");
                this.HtmlWriter.AddAttribute("labelStyle", "columns");
            }
            else if (control.PieType == EChartPieType.PieFanChart)
            {
                this.HtmlWriter.AddAttribute("radGrad", "fan");
            }
            this.HtmlWriter.AddAttribute("labels", control.Labels.ToString().ToLower());
            this.HtmlWriter.AddAttribute("animate", control.Animate.ToString().ToLower());
            this.HtmlWriter.AddAttribute("chartRef", this.Parent.ControlHost.Name);
            if (!string.IsNullOrEmpty(control.LinkAxisX)) this.HtmlWriter.AddAttribute("hAxis", control.LinkAxisX);
            if (!string.IsNullOrEmpty(control.LinkAxisY)) this.HtmlWriter.AddAttribute("vAxis", control.LinkAxisY);

            base.SetAttributes();
        }
    }
}

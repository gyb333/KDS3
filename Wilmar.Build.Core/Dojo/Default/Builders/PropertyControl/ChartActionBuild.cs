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
    /// 图形动作生成器
    /// </summary>
    internal class ChartActionBuild : ControlBuildBase
    {
        public ChartActionBuild(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
            : base(isPreview, controlHost, screenDef, compile, doc, permissionData, htmlWriter)
        {

        }
        
        /// <summary>
        /// 设置属性
        /// </summary>
        protected override void SetAttributes()
        {
            ChartAction control = this.ControlHost.Content as ChartAction;
            this.HtmlWriter.AddAttribute("dojoType", "Controls/Charting/Action");
            if (!IsPreview && !string.IsNullOrEmpty(this.ControlHost.Name))
            {
                this.HtmlWriter.AddAttribute("id", this.ControlHost.Name);
            }
            this.HtmlWriter.AddAttribute("class", "action");
            if (control.ActionType != EChartActionType.None) this.HtmlWriter.AddAttribute("type", control.ActionType.ToString());
            if (control.ActionAxis == EChartAxis.xAxis) this.HtmlWriter.AddAttribute("axis", "x");
            else if (control.ActionAxis == EChartAxis.yAxis) this.HtmlWriter.AddAttribute("axis", "y");
            if (control.MaxScale != null) this.HtmlWriter.AddAttribute("maxScale", control.MaxScale.ToString());
            if (control.ScaleFactor != null) this.HtmlWriter.AddAttribute("scaleFactor", control.ScaleFactor.ToString());
            if (control.Shift != null) this.HtmlWriter.AddAttribute("shift", control.Shift.ToString());

            base.SetAttributes();
        }
    }
}

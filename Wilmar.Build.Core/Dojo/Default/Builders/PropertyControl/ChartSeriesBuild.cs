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
    /// 图形级数生成器
    /// </summary>
    internal class ChartSeriesBuild : ControlBuildBase
    {
        public ChartSeriesBuild(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
            : base(isPreview, controlHost, screenDef, compile, doc, permissionData, htmlWriter)
        {

        }
        
        /// <summary>
        /// 设置属性
        /// </summary>
        protected override void SetAttributes()
        {
            ChartSeries control = this.ControlHost.Content as ChartSeries;
            this.HtmlWriter.AddAttribute("dojoType", "Controls/Charting/Series");
            if (!IsPreview && !string.IsNullOrEmpty(this.ControlHost.Name))
            {
                this.HtmlWriter.AddAttribute("id", this.ControlHost.Name);
            }
            this.HtmlWriter.AddAttribute("class", "series");
            this.HtmlWriter.AddAttribute("chartRef", this.Parent.ControlHost.Name);
            if (this.ProjectDocument != null && !string.IsNullOrEmpty(control.ChartPlot))
            {
                this.HtmlWriter.AddAttribute("plot", control.ChartPlot);
            }


            //store/displayMember/valueMember
            string stores = GetStore();
            if (!string.IsNullOrEmpty(stores))
            {
                this.HtmlWriter.AddAttribute("sourceType", "store");
                this.HtmlWriter.AddAttribute("store", stores, false);
            }
            string displayMember = GetDisplayMember();
            if (!string.IsNullOrEmpty(displayMember))
            {
                this.HtmlWriter.AddAttribute("nameField", displayMember, false);
            }
            string valueMember = GetValueMember();
            if (!string.IsNullOrEmpty(valueMember))
            {
                this.HtmlWriter.AddAttribute("dataField", valueMember, false);
            }
            string tooltipMember = GetTooltipMember();
            if (!string.IsNullOrEmpty(tooltipMember))
            {
                this.HtmlWriter.AddAttribute("tooltipField", tooltipMember, false);
            }
            string legendField = GetLegendField();
            if (!string.IsNullOrEmpty(legendField))
            {
                this.HtmlWriter.AddAttribute("legendField", legendField, false);
            }

            base.SetAttributes();
        }

        /// <summary>
        /// 设置Store
        /// </summary>
        /// <returns></returns>
        private string GetStore()
        {
            ChartSeries control = this.ControlHost.Content as ChartSeries;
            StringBuilder result = new StringBuilder();
            bool bindingDataSource = false;
            Dictionary<string, string> dictProperty = control.GetPropertyBindValue();
            if (!IsPreview && control.Bindings.Count > 0)
            {
                foreach (var item in control.Bindings)
                {
                    string bindPath = item.Path == null ? "" : item.Path;
                    string bindProperty = item.Property == null ? "" : item.Property;
                    if (bindProperty.ToLower() == "datasource")
                    {
                        string property = string.Empty;
                        if (dictProperty.ContainsKey(bindProperty))
                        {
                            if (dictProperty.TryGetValue(bindProperty, out property)) bindProperty = property;
                        }
                        if (!string.IsNullOrEmpty(bindPath) && !string.IsNullOrEmpty(bindProperty))
                        {
                            result.AppendFormat("at('rel:{0}', '{1}').direction(1)", "", bindPath);
                            if (bindProperty.ToLower() == "datasource") bindingDataSource = true;
                        }
                    }
                }
            }
            if (!IsPreview && control.ExistProperty("DataSource") && !bindingDataSource)
            {
                string bindPath = control.DataSource;
                string bindProperty = "DataSource";
                if (!string.IsNullOrEmpty(bindPath))
                {
                    string property = string.Empty;
                    if (dictProperty.ContainsKey(bindProperty))
                    {
                        if (dictProperty.TryGetValue(bindProperty, out property)) bindProperty = property;
                    }
                    result.AppendFormat("at('rel:{0}', '{1}').direction(1)", "", bindPath);
                }
            }

            return result.ToString();
        }
        /// <summary>
        /// 设置ValueMember
        /// </summary>
        /// <returns></returns>
        private string GetValueMember()
        {
            ChartSeries control = this.ControlHost.Content as ChartSeries;
            StringBuilder result = new StringBuilder();
            bool bindingDataSource = false;
            Dictionary<string, string> dictProperty = control.GetPropertyBindValue();
            if (!IsPreview && control.ExistProperty("ValueMember") && !bindingDataSource)
            {
                string bindPath = control.ValueMember;
                string bindProperty = "ValueMember";
                if (!string.IsNullOrEmpty(bindPath))
                {
                    string property = string.Empty;
                    if (dictProperty.ContainsKey(bindProperty))
                    {
                        if (dictProperty.TryGetValue(bindProperty, out property)) bindProperty = property;
                    }
                    result.AppendFormat("'{0}'", bindPath);
                }
            }

            return result.ToString();
        }
        /// <summary>
        /// 设置DisplayMember
        /// </summary>
        /// <returns></returns>
        private string GetDisplayMember()
        {
            ChartSeries control = this.ControlHost.Content as ChartSeries;
            StringBuilder result = new StringBuilder();
            bool bindingDataSource = false;
            Dictionary<string, string> dictProperty = control.GetPropertyBindValue();
            if (!IsPreview && control.ExistProperty("DisplayMember") && !bindingDataSource)
            {
                string bindPath = control.DisplayMember;
                string bindProperty = "DisplayMember";
                if (!string.IsNullOrEmpty(bindPath))
                {
                    string property = string.Empty;
                    if (dictProperty.ContainsKey(bindProperty))
                    {
                        if (dictProperty.TryGetValue(bindProperty, out property)) bindProperty = property;
                    }
                    result.AppendFormat("'{0}'", bindPath);
                }
            }

            return result.ToString();
        }
        /// <summary>
        /// 设置TooltipField
        /// </summary>
        /// <returns></returns>
        private string GetTooltipMember()
        {
            ChartSeries control = this.ControlHost.Content as ChartSeries;
            StringBuilder result = new StringBuilder();
            bool bindingDataSource = false;
            Dictionary<string, string> dictProperty = control.GetPropertyBindValue();
            if (!IsPreview && control.ExistProperty("TooltipMember") && !bindingDataSource)
            {
                string bindPath = control.TooltipMember;
                string bindProperty = "TooltipMember";
                if (!string.IsNullOrEmpty(bindPath))
                {
                    string property = string.Empty;
                    if (dictProperty.ContainsKey(bindProperty))
                    {
                        if (dictProperty.TryGetValue(bindProperty, out property)) bindProperty = property;
                    }
                    result.AppendFormat("'{0}'", bindPath);
                }
            }

            return result.ToString();
        }
        /// <summary>
        /// 设置LegendField
        /// </summary>
        /// <returns></returns>
        private string GetLegendField()
        {
            ChartSeries control = this.ControlHost.Content as ChartSeries;
            StringBuilder result = new StringBuilder();
            Dictionary<string, string> dictProperty = control.GetPropertyBindValue();

            var Path = (from t in control.Bindings
                        where t.Property != null && t.Property.ToLower() == "legendfield"
                        select t.Path).FirstOrDefault();
            if (Path != null)
            {
                result.AppendFormat("at('rel:{0}', '{1}').direction(1)", "", Path);
            }
            else
            {
                string bindPath = control.LegendField;
                if (!string.IsNullOrEmpty(bindPath))
                {
                    result.AppendFormat("'{0}'", bindPath);
                }
            }

            return result.ToString();
        }
    }
}

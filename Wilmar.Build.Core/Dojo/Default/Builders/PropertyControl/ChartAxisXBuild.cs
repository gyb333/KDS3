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
    /// 图形坐标X生成器
    /// </summary>
    internal class ChartAxisXBuild : ControlBuildBase
    {
        public ChartAxisXBuild(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
            : base(isPreview, controlHost, screenDef, compile, doc, permissionData, htmlWriter)
        {

        }

        /// <summary>
        /// 设置属性
        /// </summary>
        protected override void SetAttributes()
        {
            ChartAxisX control = this.ControlHost.Content as ChartAxisX;
            this.HtmlWriter.AddAttribute("dojoType", "Controls/Charting/XAxis");
            this.HtmlWriter.AddAttribute("class", "axis");
            if (!this.IsPreview)
            {
                if (!string.IsNullOrEmpty(this.ControlHost.Name))
                {
                    string name = this.ControlHost.Name.Replace(this.ProjectDocument.Name + "_", "");
                    this.HtmlWriter.AddAttribute("name", name);
                }
                else this.HtmlWriter.AddAttribute("name", "x");
            }

            this.HtmlWriter.AddAttribute("title", this.ControlHost.Title);
            this.HtmlWriter.AddAttribute("titleFontColor", control.TitleFontColor.ToString());
            this.HtmlWriter.AddAttribute("titleOrientation", control.TitleOrientation.ToString());
            this.HtmlWriter.AddAttribute("titleGap", control.TitleGap.ToString());
            this.HtmlWriter.AddAttribute("rotation", control.Rotation.ToString());
            string fontStr = string.Empty;
            fontStr = control.FontStyle.ToString() + " " + control.FontWeight.ToString();
            if (control.FontSize > 0) fontStr += " " + control.FontSize.ToString() + "pt";
            if (!string.IsNullOrEmpty(control.FontFamily)) fontStr += " " + control.FontFamily.ToString();
            this.HtmlWriter.AddAttribute("font", fontStr);
            this.HtmlWriter.AddAttribute("fixLower", "minor");
            this.HtmlWriter.AddAttribute("fixUpper", "minor");

            this.HtmlWriter.AddAttribute("majorLabels", control.MajorLabels.ToString().ToLower());
            this.HtmlWriter.AddAttribute("minorTicks", control.MinorTicks.ToString().ToLower());
            this.HtmlWriter.AddAttribute("majorTick", "{length:" + control.MajorTick.ToString() + "}");
            if (control.MajorTickStep != null && control.MajorTickStep > 0) this.HtmlWriter.AddAttribute("majorTickStep", control.MajorTickStep.ToString());

            this.HtmlWriter.AddAttribute("natural", "true");
            this.HtmlWriter.AddAttribute("includeZero", "true");
            this.HtmlWriter.AddAttribute("chartRef", this.Parent.ControlHost.Name);
            if (control.LeftBottom == EChartAxisXPosition.Top) this.HtmlWriter.AddAttribute("leftBottom", "false");

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

            base.SetAttributes();
        }

        /// <summary>
        /// 设置Store
        /// </summary>
        /// <returns></returns>
        private string GetStore()
        {
            ChartAxisX control = this.ControlHost.Content as ChartAxisX;
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
            ChartAxisX control = this.ControlHost.Content as ChartAxisX;
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
            ChartAxisX control = this.ControlHost.Content as ChartAxisX;
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
    }
}

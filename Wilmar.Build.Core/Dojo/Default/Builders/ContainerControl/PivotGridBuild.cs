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
    /// 多维报表控件生成器
    /// </summary>
    internal class PivotGridBuild : ContainerBuildBase
    {
        public PivotGridBuild(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
            : base(isPreview, controlHost, screenDef, compile, doc, permissionData, htmlWriter)
        {

        }

        /// <summary>
        /// 设置属性
        /// </summary>
        protected override void SetAttributes()
        {
            PivotGrid control = this.ControlHost.Content as PivotGrid;
            this.HtmlWriter.AddAttribute("dojoType", "Controls/PivotGrid");
            if (!IsPreview && !string.IsNullOrEmpty(this.ControlHost.Name))
            {
                this.HtmlWriter.AddAttribute("id", this.ControlHost.Name);
                this.HtmlWriter.AddAttribute("name", this.ControlHost.Name);
            }

            StringBuilder sbProps = new StringBuilder();
            StringBuilder returnContent = new StringBuilder();
            string props = control.BuildControlProps(this.ScreenDefinition, this.IsPreview, this.PermissionData, returnContent);
            if (!string.IsNullOrEmpty(props)) sbProps.AppendFormat("{0},", props);

            string stores = GetStore();
            if (!string.IsNullOrEmpty(stores)) sbProps.AppendFormat("{0},", stores.Substring(0, stores.Length - 1));
            string wptOptions = BuildOptions();
            if (!string.IsNullOrEmpty(wptOptions)) sbProps.AppendFormat("{0},", wptOptions);

            if (sbProps.ToString().Length > 0)
            {
                this.HtmlWriter.AddAttribute("data-dojo-props", sbProps.ToString().Substring(0, sbProps.ToString().Length - 1), false);
            }

            base.SetAttributes();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string BuildOptions()
        {
            StringBuilder result = new StringBuilder();
            if (this.ControlHost.Children.Count > 0)
            {
                result.Append("wptOptions:{");
                StringBuilder cellStr = new StringBuilder();
                this.BuildGirdColumn(this.ControlHost, cellStr, 0, 0);
                if (cellStr.ToString().Length > 0)
                {
                    result.Append(cellStr.ToString().Substring(0, cellStr.ToString().Length - 1));
                }
                result.Append("}");
            }
            return result.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="controlHost"></param>
        /// <param name="cellStr"></param>
        /// <param name="dIndex"></param>
        /// <param name="mIndex"></param>
        private void BuildGirdColumn(ControlHost controlHost, StringBuilder cellStr, int dIndex, int mIndex)
        {
            foreach (var child in controlHost.Children)
            {
                string controlName = child.Content.GetType().Name;
                int controlChilds = controlHost.Children.Count;
                string parentControlName = controlHost.Content.GetType().Name;
                if (parentControlName == "DimensionPanel") dIndex++;
                if (parentControlName == "MeasurePanel") mIndex++;

                if (controlName == "DimensionPanel")
                {
                    DimensionPanel dimensionPanel = child.Content as DimensionPanel;
                    cellStr.Append("dimensions:[");
                    BuildGirdColumn(child, cellStr, dIndex, mIndex);
                    cellStr.Append("],");
                }
                else if (controlName == "MeasurePanel")
                {
                    MeasurePanel measurePanelPanel = child.Content as MeasurePanel;
                    cellStr.Append("measures:[");
                    BuildGirdColumn(child, cellStr, dIndex, mIndex);
                    cellStr.Append("],");
                }
                else if (controlName == "Dimension")
                {
                    Dimension dimension = child.Content as Dimension;

                    #region 
                    string fieldName = string.Empty, fieldPath = string.Empty, formatter = string.Empty;
                    var bindData = (from t in child.Content.Bindings where t.Path != null && t.Property != null && t.Property.ToLower() == "value" select t).FirstOrDefault();
                    if (bindData != null)
                    {
                        var bindPath = bindData.Path;
                        fieldName = bindPath.Split('.')[bindPath.Split('.').Length - 1];
                        fieldPath = bindPath;
                    }
                    #endregion

                    cellStr.Append("{");
                    cellStr.AppendFormat("field:'{0}',", fieldName);
                    cellStr.AppendFormat("title:'{0}',", child.Title);
                    cellStr.AppendFormat("type:'{0}'", dimension.DimensionType.ToString().ToLower());

                    if (controlChilds == dIndex) cellStr.Append("}"); else cellStr.Append("},");
                }
                else if (controlName == "Measure")
                {
                    Measure measure = child.Content as Measure;

                    #region 
                    string fieldName = string.Empty, fieldPath = string.Empty, formatter = string.Empty;
                    var bindData = (from t in child.Content.Bindings where t.Path != null && t.Property != null && t.Property.ToLower() == "value" select t).FirstOrDefault();
                    if (bindData != null)
                    {
                        var bindPath = bindData.Path;

                        fieldName = bindPath.Split('.')[bindPath.Split('.').Length - 1];
                        if (bindPath.Split('.').Length >= 4)
                        {
                            formatter = "formatter:function(inDatum){if(inDatum){return inDatum." + fieldName + ";}else{return '';}}";
                            fieldName = bindPath.Split('.')[2];
                        }
                        fieldPath = bindPath;
                    }
                    #endregion

                    cellStr.Append("{");
                    cellStr.AppendFormat("field:'{0}',", fieldName);
                    cellStr.AppendFormat("title:'{0}',", child.Title);
                    cellStr.AppendFormat("stats:'{0}',", measure.Summary.ToString());

                    StringBuilder sbFormt = new StringBuilder();
                    if (!string.IsNullOrEmpty(measure.Symbol)) sbFormt.AppendFormat("symbol:'{0}',", measure.Symbol);
                    sbFormt.AppendFormat("decimal:{0},", measure.Decimal.ToString());
                    sbFormt.AppendFormat("negative:{0},", measure.Negative.ToString());
                    sbFormt.AppendFormat("symbolSuffix:{0},", measure.SymbolSuffix.ToString());
                    sbFormt.AppendFormat("separatorFlag:{0},", measure.SeparatorFlag.ToString().ToLower());
                    sbFormt.AppendFormat("category:'{0}',", measure.Category.ToString().ToUpper());

                    if (sbFormt.ToString().Length > 0)
                    {
                        cellStr.Append("format:{" + sbFormt.ToString().Substring(0, sbFormt.ToString().Length - 1) + "}");
                    }

                    if (controlChilds == mIndex) cellStr.Append("}"); else cellStr.Append("},");
                }
            }
        }
        /// <summary>
        /// 设置Store
        /// </summary>
        /// <returns></returns>
        private string GetStore()
        {
            PivotGrid control = this.ControlHost.Content as PivotGrid;

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
                            result.AppendFormat("store:at('rel:{0}', '{1}').direction(1),", "", bindPath);
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
                    result.AppendFormat("store:at('rel:{0}', '{1}').direction(1),", "", bindPath);
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// 设置子元素
        /// </summary>
        protected override void SetChildElements()
        {
            foreach (var child in this.ControlHost.Children)
            {
            }
        }
    }
}

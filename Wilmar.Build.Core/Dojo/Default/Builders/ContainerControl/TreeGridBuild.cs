using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// 树形网格生成器
    /// </summary>
    internal class TreeGridBuild : ContainerBuildBase
    {
        public TreeGridBuild(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
            : base(isPreview, controlHost, screenDef, compile, doc, permissionData, htmlWriter)
        {

        }

        /// <summary>
        /// 设置属性
        /// </summary>
        protected override void SetAttributes()
        {
            TreeGrid control = this.ControlHost.Content as TreeGrid;

            this.HtmlWriter.AddAttribute("dojoType", "Controls/Panel");
            string stores = GetStore();

            StringBuilder returnContent = new StringBuilder();
            string props = control.BuildControlProps(this.ScreenDefinition, this.IsPreview, this.PermissionData, returnContent);
            string dojoProps = stores + returnContent;
            if (!string.IsNullOrEmpty(dojoProps))
            {
                this.HtmlWriter.AddAttribute("data-dojo-props", dojoProps.ToString().Length == 0 ? "" : dojoProps.ToString().Substring(0, dojoProps.ToString().Length - 1), false);
            }

            base.SetAttributes();
        }
        /// <summary>
        /// 设置子元素
        /// </summary>
        protected override void SetChildElements()
        {
            TreeGrid control = this.ControlHost.Content as TreeGrid;

            this.HtmlWriter.AddAttribute("dojoType", "Controls/Dgrid/TreeGrid");
            if (!IsPreview && !string.IsNullOrEmpty(this.ControlHost.Name))
            {
                this.HtmlWriter.AddAttribute("id", this.ControlHost.Name);
                this.HtmlWriter.AddAttribute("name", this.ControlHost.Name);
            }

            StringBuilder sbProps = new StringBuilder();
            StringBuilder returnContent = new StringBuilder();
            string props = control.BuildControlProps(this.ScreenDefinition, this.IsPreview, this.PermissionData, returnContent);
            if (!string.IsNullOrEmpty(props)) sbProps.AppendFormat("{0},", props);
            if (control.AutoLoadData) sbProps.AppendFormat("{0},", "autoLoadData:false");
            sbProps.AppendFormat("{0},", "hiddenColoumnButton:" + control.HiddenColoumnButton.ToString().ToLower() + "");

            bool remberConfig = false;
            if (!string.IsNullOrEmpty(this.ControlHost.Name)) remberConfig = true;
            sbProps.AppendFormat("{0},", "remberConfig:" + remberConfig.ToString().ToLower());
            sbProps.AppendFormat("{0},", "pagingTextBox:true");
            sbProps.AppendFormat("{0},", "allowTextSelection:true");
            sbProps.AppendFormat("{0},", "allowSelectAll:true");
            sbProps.AppendFormat("{0},", "firstLastArrows:true");
            sbProps.AppendFormat("{0},", "collection:null");
            sbProps.AppendFormat("{0},", "expanded:" + control.Expanded.ToString().ToLower() + "");

            string structures = GetTreeGridStructure();
            if (!string.IsNullOrEmpty(structures))
            {
                sbProps.AppendFormat("{0},", structures);
            }
            if (sbProps.ToString().Length > 0)
            {
                this.HtmlWriter.AddAttribute("data-dojo-props", sbProps.ToString().Substring(0, sbProps.ToString().Length - 1), false);
            }
            this.HtmlWriter.RenderBeginTag(this.TagName);

            foreach (var child in this.ControlHost.Children)
            {
            }

            this.HtmlWriter.RenderEndTag();
        }

        /// <summary>
        /// 设置TreeGrid列
        /// </summary>
        /// <returns></returns>
        private string GetTreeGridStructure()
        {
            StringBuilder sb = new StringBuilder();
            if (this.ControlHost.Children.Count > 0)
            {
                var content = this.ControlHost.Content as TreeGrid;

                sb.Append("columns:{");
                StringBuilder cellStr = new StringBuilder();
                this.BuildTreeGirdColumn(this.ControlHost, cellStr, 0, 0, 0);
                sb.Append(cellStr.ToString().Length > 0 ? cellStr.ToString().Substring(0, cellStr.ToString().Length - 1) : "");
                sb.Append("}");
            }

            return sb.ToString();
        }

        /// <summary>
        /// 设置TreeGrid列
        /// </summary>
        /// <param name="controlHost"></param>
        /// <param name="cellStr"></param>
        /// <param name="dIndex"></param>
        /// <param name="mIndex"></param>
        /// <param name="gIndex"></param>
        private void BuildTreeGirdColumn(ControlHost controlHost, StringBuilder cellStr, int dIndex, int mIndex, int gIndex)
        {
            foreach (var child in controlHost.Children)
            {
                ControlBase control = child.Content;
                dynamic c = control;
                int controlChilds = controlHost.Children.Count;
                string controlName = control.GetType().Name;
                string parentControlName = controlHost.Content.GetType().Name;

                if (parentControlName == "DimensionPane") dIndex++;
                else if (parentControlName == "DimensionGroup") gIndex++;
                else if (parentControlName == "MeasurePane") mIndex++;

                if (controlName == "DimensionPane") //纬度面板
                {
                    cellStr.Append("dimensionPane:[");
                    BuildTreeGirdColumn(child, cellStr, dIndex, mIndex, gIndex);
                    cellStr.Append("],");
                }
                else if (controlName == "DimensionGroup") //纬度组
                {
                    cellStr.Append("{fields:[");
                    BuildTreeGirdColumn(child, cellStr, dIndex, mIndex, gIndex);
                    if (parentControlName == "DimensionPane")
                    {
                        if (controlChilds == dIndex) cellStr.Append("]}"); else cellStr.Append("]},");
                    }
                }
                else if (controlName == "MeasurePane") //度量面板
                {
                    cellStr.Append("measurePane:[");
                    BuildTreeGirdColumn(child, cellStr, dIndex, mIndex, gIndex);
                    cellStr.Append("],");
                }
                else
                {
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
                    #region title
                    cellStr.AppendFormat("label:'{0}',", child.Title);
                    #endregion
                    #region width
                    if (control.ExistProperty("Width"))
                    {
                        if (c.Width > 0) cellStr.AppendFormat("width:{0},", c.Width.ToString());
                    }
                    #endregion
                    #region style
                    StringBuilder sbStyle = new StringBuilder();
                    if (control.ExistProperty("BackColor"))
                    {
                        string backColor = c.BackColor.R.ToString() + "," + c.BackColor.G.ToString() + "," + c.BackColor.B.ToString() + "," + c.BackColor.A.ToString();
                        if (c.BackColor.ToString() != "#FFFFFFFF")
                        {
                            sbStyle.AppendFormat("backgroundColor:'rgba({0})',", backColor);
                        }
                    }
                    if (control.ExistProperty("ForeColor"))
                    {
                        string foreColor = c.ForeColor.R.ToString() + "," + c.ForeColor.G.ToString() + "," + c.ForeColor.B.ToString() + "," + c.ForeColor.A.ToString();
                        if (c.ForeColor.ToString() != "#FF000000")
                        {
                            sbStyle.AppendFormat("color:'rgba({0})',", foreColor);
                        }
                    }
                    if (control.ExistProperty("FontSize"))
                    {
                        if (c.FontSize != null && c.FontSize > 0) sbStyle.AppendFormat("fontSize:{0},", c.FontSize.ToString());
                    }

                    if (sbStyle.ToString().Length > 0)
                    {
                        cellStr.Append("style:{" + sbStyle.ToString().Substring(0, sbStyle.ToString().Length - 1) + "},");
                    }
                    #endregion
                    cellStr.AppendFormat("sortable:false,");
                    cellStr.AppendFormat("field:'{0}',", fieldName);
                    cellStr.AppendFormat("path:'{0}'", fieldPath);
                    if (parentControlName == "DimensionPane")
                    {
                        cellStr.Append(",renderExpando:true");
                        if (controlChilds == dIndex) cellStr.Append("}"); else cellStr.Append("},");
                    }
                    else if (parentControlName == "DimensionGroup")
                    {
                        if (gIndex == 1) cellStr.Append(",renderExpando:true");
                        if (controlChilds == gIndex) cellStr.Append("}"); else cellStr.Append("},");
                    }
                    else if (parentControlName == "MeasurePane")
                    {
                        if (controlChilds == mIndex) cellStr.Append("}"); else cellStr.Append("},");
                    }
                }
            }
        }

        /// <summary>
        /// 设置TreeGrid的Store
        /// </summary>
        /// <returns></returns>
        private string GetStore()
        {
            TreeGrid control = this.ControlHost.Content as TreeGrid;

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
    }
}
using System;
using System.Collections.Generic;
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
    /// 网格生成器
    /// </summary>
    internal class DataGridBuild : ContainerBuildBase
    {
        public DataGridBuild(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
            : base(isPreview, controlHost, screenDef, compile, doc, permissionData, htmlWriter)
        {

        }

        #region 旧版Grid
        ///// <summary>
        ///// 设置属性
        ///// </summary>
        //protected override void SetAttributes()
        //{
        //    DataGrid control = this.ControlHost.Content as DataGrid;
        //    this.HtmlWriter.AddAttribute("dojoType", "Controls/EnhancedGrid");
        //    if (!IsPreview && !string.IsNullOrEmpty(this.ControlHost.Name))
        //    {
        //        this.HtmlWriter.AddAttribute("id", this.ControlHost.Name);
        //        this.HtmlWriter.AddAttribute("name", this.ControlHost.Name);
        //    }
        //    if (control.SingleClickEdit)
        //    {
        //        this.HtmlWriter.AddAttribute("singleClickEdit", control.SingleClickEdit.ToString().ToLower());
        //    }
        //    string pluginsStr = "pagination:{pageSizes:[],defaultPage:1,defaultPageSize:10}";
        //    this.HtmlWriter.AddAttribute("region", "center");
        //    this.HtmlWriter.AddAttribute("columnreordering", "true");
        //    this.HtmlWriter.AddAttribute("nestedsorting", "true");
        //    if (control.SelectionMode != ESelectionMode.None)
        //    {
        //        this.HtmlWriter.AddAttribute("selectionmode", control.SelectionMode.ToString().ToLower());
        //        pluginsStr += ",indirectSelection:{headerSelector:true,styles:'text-align:center;'}";
        //    }
        //    if (!this.IsPreview)
        //    {
        //        this.HtmlWriter.AddAttribute("plugins", "{" + pluginsStr + "}");
        //    }
        //    string structures = GetDataGridStructure();
        //    if (!string.IsNullOrEmpty(structures))
        //    {
        //        this.HtmlWriter.AddAttribute("structure", structures, false);
        //    }
        //    string stores = GetDataGridStore();
        //    if (!string.IsNullOrEmpty(stores))
        //    {
        //        this.HtmlWriter.AddAttribute("store", stores, false);
        //    }

        //    StringBuilder sbProps = new StringBuilder();
        //    StringBuilder returnContent = new StringBuilder();
        //    string props = control.BuildControlProps(this.ScreenDefinition, this.IsPreview, returnContent);
        //    if (!string.IsNullOrEmpty(props)) sbProps.AppendFormat("{0},", props);
        //    if (sbProps.ToString().Length > 0)
        //    {
        //        this.HtmlWriter.AddAttribute("data-dojo-props", sbProps.ToString().Substring(0, sbProps.ToString().Length - 1), false);
        //    }

        //    base.SetAttributes();
        //}

        ///// <summary>
        ///// 设置子元素
        ///// </summary>
        //protected override void SetChildElements()
        //{

        //}

        ///// <summary>
        ///// 设置DataGrid列
        ///// </summary>
        ///// <returns></returns>
        //private string GetDataGridStructure()
        //{
        //    StringBuilder sb = new StringBuilder();
        //    if (this.ControlHost.Children.Count > 0)
        //    {
        //        var content = this.ControlHost.Content as DataGrid;

        //        sb.Append("[{");
        //        sb.Append("cells:[[");
        //        string cellNumber = "{name:'序号',type:dojox.grid.cells.RowIndex,width:4},";
        //        StringBuilder cellStr = new StringBuilder();
        //        foreach (var child in this.ControlHost.Children)
        //        {
        //            StringBuilder cellContent = new StringBuilder();
        //            StringBuilder sbProps = new StringBuilder();
        //            ControlBase control = child.Content;
        //            dynamic c = control;
        //            string controlName = control.GetType().Name;

        //            cellContent.AppendFormat("name:'{0}',", child.Title);
        //            #region editable
        //            bool editable = true;
        //            DataGrid gridControl = (DataGrid)this.ControlHost.Content;
        //            if (gridControl.IsReadOnly || !gridControl.IsEnable) editable = false;
        //            if (editable)
        //            {
        //                if (control.ExistProperty("IsReadOnly"))
        //                {
        //                    if (c.IsReadOnly) editable = false;
        //                }
        //                if (editable)
        //                {
        //                    if (control.ExistProperty("IsEnable"))
        //                    {
        //                        if (!c.IsEnable) editable = false;
        //                    }
        //                }
        //            }
        //            if (controlName == "Hyperlink" || controlName == "Button") editable = false;
        //            if (editable) cellContent.AppendFormat("editable:true,");
        //            #endregion
        //            #region width
        //            if (control.ExistProperty("Width"))
        //            {
        //                if (c.Width > 0) cellContent.AppendFormat("width:{0},", c.Width.ToString());
        //            }
        //            #endregion
        //            #region title
        //            string controlTitle = child.Title == null ? "" : child.Title;
        //            #endregion
        //            if (!this.IsPreview)
        //            {
        //                StringBuilder returnContent = new StringBuilder();
        //                sbProps.AppendFormat("{0},", "context:at('rel:','VM')");
        //                string props = ControlBaseExtend.BuildControlProps(control, this.ScreenDefinition, this.IsPreview, returnContent, true);
        //                if (!string.IsNullOrEmpty(props)) sbProps.AppendFormat("{0},", props);
        //                if (returnContent.ToString().Length > 0) cellContent.Append(returnContent.ToString());
        //                if (sbProps.ToString().Length > 0) cellContent.Append("widgetProps:{" + sbProps.ToString().Substring(0, sbProps.ToString().Length - 1) + "},");
        //            }
        //            cellContent.AppendFormat("{0}", control.GetGridCellType(controlTitle));
        //            cellStr.Append("{" + cellContent.ToString() + "},");
        //        }
        //        if (cellStr.ToString().Length > 0)
        //        {
        //            string cell = cellNumber + cellStr.ToString();
        //            sb.Append(cell.ToString().Substring(0, cell.ToString().Length - 1));
        //        }
        //        sb.Append("]]");
        //        sb.Append("}]");
        //    }

        //    return sb.ToString();
        //}

        ///// <summary>
        ///// 设置DataGird的Store
        ///// </summary>
        ///// <returns></returns>
        //private string GetDataGridStore()
        //{
        //    DataGrid control = (DataGrid)this.ControlHost.Content;

        //    StringBuilder result = new StringBuilder();
        //    bool bindingDataSource = false;
        //    Dictionary<string, string> dictProperty = BuildCommonMethod.GetPropertyBindValue();
        //    if (!IsPreview && control.Bindings.Count > 0)
        //    {
        //        foreach (var item in control.Bindings)
        //        {
        //            string bindPath = item.Path == null ? "" : item.Path;
        //            string bindProperty = item.Property == null ? "" : item.Property;
        //            if (bindProperty.ToLower() == "datasource")
        //            {
        //                string property = string.Empty;
        //                if (dictProperty.ContainsKey(bindProperty))
        //                {
        //                    if (dictProperty.TryGetValue(bindProperty, out property)) bindProperty = property;
        //                }
        //                if (!string.IsNullOrEmpty(bindPath) && !string.IsNullOrEmpty(bindProperty))
        //                {
        //                    result.AppendFormat("at('rel:{0}', '{1}').direction(1),", "", bindPath);
        //                    if (bindProperty.ToLower() == "datasource") bindingDataSource = true;
        //                }
        //            }
        //        }
        //    }
        //    if (!IsPreview && control.ExistProperty("DataSource") && !bindingDataSource)
        //    {
        //        string bindPath = control.DataSource;
        //        string bindProperty = "DataSource";
        //        if (!string.IsNullOrEmpty(bindPath))
        //        {
        //            string property = string.Empty;
        //            if (dictProperty.ContainsKey(bindProperty))
        //            {
        //                if (dictProperty.TryGetValue(bindProperty, out property)) bindProperty = property;
        //            }
        //            result.AppendFormat("at('rel:{0}', '{1}').direction(1),", "", bindPath);
        //        }
        //    }

        //    return result.ToString().Length == 0 ? "" : result.ToString().Substring(0, result.Length - 1);
        //}
        #endregion

        #region 新版Grid
        /// <summary>
        /// 设置属性
        /// </summary>
        protected override void SetAttributes()
        {
            DataGrid control = this.ControlHost.Content as DataGrid;

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
            DataGrid control = this.ControlHost.Content as DataGrid;

            this.HtmlWriter.AddAttribute("dojoType", "Controls/Dgrid/OnDemandGrid");
            if (!IsPreview && !string.IsNullOrEmpty(this.ControlHost.Name))
            {
                this.HtmlWriter.AddAttribute("id", this.ControlHost.Name);
                this.HtmlWriter.AddAttribute("name", this.ControlHost.Name);
            }

            StringBuilder sbProps = new StringBuilder();
            StringBuilder sbConstraints = new StringBuilder();
            StringBuilder returnContent = new StringBuilder();
            string props = control.BuildControlProps(this.ScreenDefinition, this.IsPreview, this.PermissionData, returnContent, sbConstraints);
            if (!string.IsNullOrEmpty(props)) sbProps.AppendFormat("{0},", props);
            if (control.AutoLoadData) sbProps.AppendFormat("{0},", "autoLoadData:false");
            sbProps.AppendFormat("{0},", "hiddenColoumnButton:" + control.HiddenColoumnButton.ToString().ToLower() + "");

            if (control.SelectionMode == ESelectionMode.Multiple) sbProps.AppendFormat("{0},", "selectionMode:'multiple'");
            bool remberConfig = false;
            if (!string.IsNullOrEmpty(this.ControlHost.Name)) remberConfig = true;
            sbProps.AppendFormat("{0},", "remberConfig:" + remberConfig.ToString().ToLower());
            sbProps.AppendFormat("{0},", "pagingTextBox:true");
            sbProps.AppendFormat("{0},", "allowTextSelection:true");
            sbProps.AppendFormat("{0},", "allowSelectAll:true");
            sbProps.AppendFormat("{0},", "firstLastArrows:true");
            sbProps.AppendFormat("{0},", "collection:null");
            sbProps.AppendFormat("{0},", "rowsPerPage:15");
            if (!string.IsNullOrEmpty(control.FormatterRow))
            {
                string formartContent = control.FormatterRow.Replace("\r\n", "").Replace("\"", "\'").Trim();
                sbProps.Append("formatterRow:function(row,rowData){" + formartContent + "},");
            }

            string structures = GetDataGridStructure();
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
        /// 设置DataGrid列
        /// </summary>
        /// <returns></returns>
        private string GetDataGridStructure()
        {
            StringBuilder sb = new StringBuilder();
            if (this.ControlHost.Children.Count > 0)
            {
                var content = this.ControlHost.Content as DataGrid;
                sb.Append("columns:[");
                string cellNumber = string.Empty;
                if (content.SelectionMode == ESelectionMode.Single) cellNumber = "{selector:'radio',width:'100%',label:'',width:20},";
                else if (content.SelectionMode == ESelectionMode.Multiple) cellNumber = "{selector:'checkbox',width:'100%',label:'',width:20},";
                StringBuilder cellStr = new StringBuilder();
                foreach (var child in this.ControlHost.Children)
                {
                    StringBuilder result = new StringBuilder();
                    this.BuildDataGridCell(ref result, content, child);
                    cellStr.Append(result);
                }
                if (cellStr.ToString().Length > 0)
                {
                    string cell = cellNumber + cellStr.ToString();
                    sb.Append(cell.ToString().Substring(0, cell.ToString().Length - 1));
                }
                sb.Append("]");
            }

            return sb.ToString();
        }
        private void BuildDataGridCell(ref StringBuilder result, DataGrid content, ControlHost child)
        {
            ControlBase control = child.Content;
            string controlName = control.GetType().Name;
            if (controlName.ToLower() == "datagridheader")
            {
                string cellContent = this.BuildCellContent(content, child, true);
                result.Append("{" + cellContent.ToString().Substring(0, cellContent.ToString().Length - 1) + "");
                if (child.Children.Count > 0)
                {
                    result.Append(",children:[");
                    foreach (var header in child.Children)
                    {
                        if (header.Content.GetType().Name.ToLower() == "datagridheader")
                        {
                            BuildDataGridCell(ref result, content, header);
                        }
                        else
                        {
                            string cellChildContent = this.BuildCellContent(content, header, true);
                            result.Append("{" + cellChildContent.ToString().Substring(0, cellChildContent.ToString().Length - 1) + "},");
                        }
                    }
                    result = new StringBuilder(result.ToString().Substring(0, result.ToString().Length - 1));
                    result.Append("]");
                }
                result.Append("},");
            }
            else
            {
                string cellContent = this.BuildCellContent(content, child, false);
                if (cellContent.ToString().Length > 0)
                {
                    result.Append("{" + cellContent.ToString().Substring(0, cellContent.ToString().Length - 1) + "},");
                }
            }
        }
        /// <summary>
        /// 生成列内容
        /// </summary>
        /// <param name="content"></param>
        /// <param name="child"></param>
        /// <returns></returns>
        private string BuildCell(DataGrid content, ControlHost child)
        {
            StringBuilder result = new StringBuilder();
            ControlBase control = child.Content;
            string controlName = control.GetType().Name;
            //列分组
            if (controlName.ToLower() == "datagridheader")
            {
                StringBuilder cellGroup = new StringBuilder();
                StringBuilder cellString = new StringBuilder();
                cellGroup.Append("[");
                var dgHeader = child.Content as DataGridHeader;
                foreach (var header in child.Children)
                {
                    string cellContent = this.BuildCellContent(content, header, true);
                    if (cellContent.ToString().Length > 0)
                    {
                        cellString.Append("{" + cellContent.ToString().Substring(0, cellContent.ToString().Length - 1) + "},");
                    }
                }
                if (cellString.ToString().Length > 0)
                {
                    cellGroup.Append(cellString.ToString().Substring(0, cellString.ToString().Length - 1));
                }
                cellGroup.Append("]");
                if (cellGroup.ToString().Length > 0) result.Append(cellGroup.ToString() + ",");
            }
            else
            {
                string cellContent = this.BuildCellContent(content, child, false);
                if (cellContent.ToString().Length > 0)
                {
                    result.Append("{" + cellContent.ToString().Substring(0, cellContent.ToString().Length - 1) + "},");
                }
            }
            return result.ToString();
        }
        /// <summary>
        /// 生成列内容
        /// </summary>
        /// <param name="content"></param>
        /// <param name="child"></param>
        /// <returns></returns>
        private string BuildCellContent(DataGrid content, ControlHost child, bool isGroupHeader)
        {
            StringBuilder cellContent = new StringBuilder();
            StringBuilder sbProps = new StringBuilder();
            ControlBase control = child.Content;
            dynamic c = control;
            string controlName = control.GetType().Name;

            StringBuilder sbStyle = new StringBuilder();
            #region title
            string controlTitle = child.Title == null ? "" : child.Title;
            cellContent.AppendFormat("label:'{0}',", child.Title);
            #endregion
            #region width
            if (control.ExistProperty("Width"))
            {
                if (c.Width > 0) cellContent.AppendFormat("width:{0},", c.Width.ToString());
            }
            if (control.ExistProperty("MinWidth"))
            {
                if (c.MinWidth > 0) cellContent.AppendFormat("minWidth:{0},", c.MinWidth.ToString());
            }
            #endregion
            #region editable
            bool editable = true;
            DataGrid gridControl = (DataGrid)this.ControlHost.Content;
            if (gridControl.IsReadOnly || !gridControl.IsEnable) editable = false;
            if (editable)
            {
                if (control.ExistProperty("IsReadOnly"))
                {
                    if (c.IsReadOnly) editable = false;
                }
                if (editable)
                {
                    if (control.ExistProperty("IsEnable"))
                    {
                        if (!c.IsEnable) editable = false;
                    }
                }
            }
            if (controlName == "CheckBox" || controlName == "RadioBox") editable = true;
            if (controlName == "Hyperlink" || controlName == "Button") editable = false;
            #endregion
            #region sortable
            if (!gridControl.ColumnSort) cellContent.Append("sortable:false,");
            #endregion
            #region AttachObject
            bool editHead = false;
            string formatText = string.Empty;
            if (child.AttachObject != null)
            {
                var dataGridAttach = child.AttachObject as DataGridAttach;
                if (dataGridAttach != null)
                {
                    editHead = dataGridAttach.EditHead;
                    if (editHead) cellContent.AppendFormat("editHead:{0},", editHead.ToString().ToLower());
                    cellContent.AppendFormat("titleDock:'{0}',", dataGridAttach.TitlePosition.ToString().ToLower());
                    cellContent.AppendFormat("contentDock:'{0}',", dataGridAttach.ContentPosition.ToString().ToLower());
                    if (!string.IsNullOrEmpty(dataGridAttach.FormatText))
                    {
                        formatText = dataGridAttach.FormatText.Replace("\r\n", "").Replace("\"", "\'").Replace("this", "inDatum").Trim();
                    }
                }
                else
                {
                    var dataGridHeaderAtt = child.AttachObject as DataGridHeaderAttach;
                    if (dataGridHeaderAtt != null)
                    {
                        cellContent.AppendFormat("titleDock:'{0}',", dataGridHeaderAtt.TitlePosition.ToString().ToLower());
                        cellContent.AppendFormat("contentDock:'{0}',", dataGridHeaderAtt.ContentPosition.ToString().ToLower());
                        #region 
                        //if (dataGridHeaderAtt.ColumnSpan > 1)
                        //{
                        //    cellContent.AppendFormat("colSpan:{0},isgroup:true,sortable:false,", dataGridHeaderAtt.ColumnSpan.ToString().ToLower());
                        //}
                        //if (dataGridHeaderAtt.RowSpan > 1)
                        //{
                        //    isBuildFied = false;
                        //    cellContent.AppendFormat("rowSpan:{0},isgroup:true,sortable:false,", dataGridHeaderAtt.RowSpan.ToString().ToLower());
                        //}
                        #endregion
                    }
                }
            }
            #endregion
            #region style
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
                cellContent.Append("style:{" + sbStyle.ToString().Substring(0, sbStyle.ToString().Length - 1) + "},");
            }
            #endregion
            if (!this.IsPreview)
            {
                StringBuilder sbConstraints = new StringBuilder();
                StringBuilder returnContent = new StringBuilder();
                bool isGridFormat = false;
                if (!string.IsNullOrEmpty(formatText)) isGridFormat = true;
                string props = ControlExtend.BuildControlProps(control, this.ScreenDefinition, this.IsPreview, this.PermissionData, returnContent, sbConstraints, true, false, isGridFormat);
                if (!string.IsNullOrEmpty(props)) sbProps.AppendFormat("{0},", props);
                if (returnContent.ToString().Length > 0) cellContent.Append(returnContent.ToString());
                if (!isGroupHeader)
                {
                    bool arrowButton = false;
                    if (controlName == "DatePicker")
                    {
                        string datePattern = "yyyy-MM-dd";
                        DatePicker dp = control as DatePicker;
                        if (!string.IsNullOrEmpty(dp.DatePattern)) datePattern = dp.DatePattern;
                        string field = string.Empty;
                        foreach (var item in dp.Bindings)
                        {
                            string bindPath = item.Path == null ? "" : item.Path.Replace("CurrentItem", "SelectedItem");
                            string bindProperty = item.Property == null ? "" : item.Property;
                            if (bindProperty.ToLower() == "value" && bindPath.Split('.').Length >= 4)
                            {
                                field = bindPath.Split('.')[bindPath.Split('.').Length - 1];
                                break;
                            }
                        }
                        if (!string.IsNullOrEmpty(field)) cellContent.AppendFormat("{0},", "formatter:function(inDatum){if(inDatum){return dojo.date.locale.format(new Date(inDatum." + field + "),this.constraint);}else{return '';}},constraint:{formatLength:'long',selector:'date',datePattern:'" + datePattern + "'}");
                        else cellContent.AppendFormat("{0},", "formatter:function(inDatum){if(inDatum){return dojo.date.locale.format(new Date(inDatum),this.constraint);}else{return '';}},constraint:{formatLength:'long',selector:'date',datePattern:'" + datePattern + "'}");
                    }
                    if (controlName == "TimePicker")
                    {
                        string timePattern = "MM-dd";
                        TimePicker dp = control as TimePicker;
                        string field = string.Empty;
                        foreach (var item in dp.Bindings)
                        {
                            string bindPath = item.Path == null ? "" : item.Path.Replace("CurrentItem", "SelectedItem");
                            string bindProperty = item.Property == null ? "" : item.Property;
                            if (bindProperty.ToLower() == "value" && bindPath.Split('.').Length >= 4)
                            {
                                field = bindPath.Split('.')[bindPath.Split('.').Length - 1];
                                break;
                            }
                        }
                        if (!string.IsNullOrEmpty(field)) cellContent.AppendFormat("{0},", "formatter:function(inDatum){if(inDatum){return dojo.date.locale.format(new Date(inDatum." + field + "),this.constraint);}else{return '';}},constraint:{formatLength:'short',selector:'time',timePattern:'" + timePattern + "'}");
                        else cellContent.AppendFormat("{0},", "formatter:function(inDatum){if(inDatum){return dojo.date.locale.format(new Date(inDatum),this.constraint);}else{return '';}},constraint:{formatLength:'short',selector:'time',timePattern:'" + timePattern + "'}");
                    }
                    if (controlName == "SelectBox")
                    {
                        SelectBox selectBoxControl = control as SelectBox;
                        sbProps.AppendFormat("isMultiSelect:{0},", selectBoxControl.IsMultiSelect.ToString().ToLower());
                        sbProps.AppendFormat("clearTip:{0},", selectBoxControl.ClearTip.ToString().ToLower());
                        if (selectBoxControl.ScreenTitle != null) sbProps.AppendFormat("screenTitle:'{0}',", selectBoxControl.ScreenTitle);
                        if (selectBoxControl.ScreenName != null) sbProps.AppendFormat("screenName:'{0}',", selectBoxControl.ScreenName);
                        if (selectBoxControl.ScreenInParameter != null) sbProps.AppendFormat("inputParameter:'{0}',", selectBoxControl.ScreenInParameter);
                        if (selectBoxControl.ScreenOutParameter != null) sbProps.AppendFormat("outputParameter:'{0}',", selectBoxControl.ScreenOutParameter);
                        if (selectBoxControl.ScreenOtherParameter != null) sbProps.AppendFormat("otherParameter:'{0}',", selectBoxControl.ScreenOtherParameter);
                        if (selectBoxControl.ScreenWidth != 0) sbProps.AppendFormat("screenWidth:{0},", selectBoxControl.ScreenWidth);
                        if (selectBoxControl.ScreenHeight != 0) sbProps.AppendFormat("screenHeight:{0},", selectBoxControl.ScreenHeight);
                        string showModel = selectBoxControl.ShowModel ? "dialog" : "tooltip";
                        sbProps.AppendFormat("showModel:'{0}',", showModel);
                    }
                    if (controlName == "SelectPage")
                    {
                        SelectPage selectPageControl = control as SelectPage;
                        string searchAttr = GetSearchAttr(selectPageControl);
                        if (!string.IsNullOrEmpty(searchAttr)) sbProps.AppendFormat("{0},", searchAttr);
                        sbProps.AppendFormat("{0},", "multiple:" + selectPageControl.Multiple.ToString().ToLower());
                        sbProps.AppendFormat("{0},", "pagination:" + selectPageControl.Pagination.ToString().ToLower());
                        sbProps.AppendFormat("{0},", "dropButton:" + selectPageControl.DropButton.ToString().ToLower());
                        sbProps.AppendFormat("{0},", "selectToCloseList:" + selectPageControl.SelectToCloseList.ToString().ToLower());
                        sbProps.AppendFormat("{0},", "autoSelectFirst:" + selectPageControl.AutoSelectFirst.ToString().ToLower());
                        sbProps.AppendFormat("{0},", "maxSelectLimit:" + selectPageControl.MaxSelectLimit.ToString());
                        sbProps.AppendFormat("{0},", "inputDelay:" + selectPageControl.InputDelay.ToString());
                        sbProps.AppendFormat("{0},", "showSearch:" + selectPageControl.ShowSearch.ToString().ToLower());
                        if (!string.IsNullOrEmpty(selectPageControl.SelectIndex)) sbProps.AppendFormat("{0},", "selectIndex:'" + selectPageControl.SelectIndex.ToString() + "'");
                        string dropDownButtonName = string.Empty;
                        sbProps.AppendFormat("{0},", "dropDownButtonName:'" + dropDownButtonName + "'");
                    }
                    if (controlName == "Numeric")
                    {
                        Numeric numericCtl = control as Numeric;
                        arrowButton = numericCtl.ArrowButton;
                    }

                    if (editable)
                    {
                        if (controlName == "CheckBox" || controlName == "RadioBox")
                        {
                            if (!string.IsNullOrEmpty(formatText)) cellContent.AppendFormat("{0},", "formatter:function(inDatum){" + formatText + "}");
                            else
                            {
                                #region 
                                bool isReadOnly = false;
                                if (control.ExistProperty("IsReadOnly"))
                                {
                                    if (c.IsReadOnly) isReadOnly = true;
                                }
                                if (!isReadOnly)
                                {
                                    if (control.ExistProperty("IsEnable"))
                                    {
                                        if (!c.IsEnable) isReadOnly = true;
                                    }
                                }
                                if (isReadOnly) sbProps.AppendFormat("{0},", "readOnly:true");
                                #endregion
                                cellContent.AppendFormat("{0},", control.GetGridCellTypeNewGrid(controlTitle, arrowButton));
                            }
                        }
                        else
                        {
                            cellContent.AppendFormat("{0},", control.GetGridCellTypeNewGrid(controlTitle, arrowButton));
                            if (content.SingleClickEdit) cellContent.AppendFormat("editOn:'click',");
                            else cellContent.AppendFormat("editOn:'dblclick',");
                        }
                        if (sbConstraints.ToString().Length > 0)
                        {
                            string constrains = sbConstraints.ToString().Substring(0, sbConstraints.ToString().Length - 1);
                            sbProps.Append("constraints:{" + constrains + "},");
                        }
                        sbProps.AppendFormat("{0},", "context:at('rel:','VM')");
                        if (sbProps.ToString().Length > 0) cellContent.Append("editorArgs:{" + sbProps.ToString().Substring(0, sbProps.ToString().Length - 1) + "},");
                        if (editHead)
                        {
                            StringBuilder batchEditCondition = new StringBuilder();
                            foreach (var item in control.Bindings)
                            {
                                string bindPath = item.Path == null ? "" : item.Path.Replace("CurrentItem", "SelectedItem");
                                string bindProperty = item.Property == null ? "" : item.Property;
                                string path = bindPath;
                                string field = bindPath.Split('.')[bindPath.Split('.').Length - 1];
                                if (bindProperty.ToLower() == "batcheditcondition")
                                {
                                    batchEditCondition.AppendFormat("{0}: at('rel:{1}','{2}')", bindProperty.ToLower(), "", field);
                                    break;
                                }
                            }
                            sbProps.AppendFormat("{0},", batchEditCondition.ToString());
                            cellContent.Append("editorHeadArgs:{ " + sbProps.ToString().Substring(0, sbProps.ToString().Length - 1) + "},");
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(formatText)) cellContent.AppendFormat("{0},", "formatter:function(inDatum){" + formatText + "}");
                    }
                }
            }
            return cellContent.ToString();
        }

        /// <summary>
        /// 设置Store
        /// </summary>
        /// <returns></returns>
        private string GetStore()
        {
            DataGrid control = this.ControlHost.Content as DataGrid;

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
                if (!string.IsNullOrEmpty(bindPath))
                {
                    result.AppendFormat("store:at('rel:{0}', '{1}').direction(1),", "", bindPath);
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// 设置搜索成员
        /// </summary>
        /// <returns></returns>
        private string GetSearchAttr(SelectPage control)
        {
            StringBuilder result = new StringBuilder();
            if (!IsPreview && control.Bindings.Count > 0)
            {
                foreach (var item in control.Bindings)
                {
                    string bindPath = item.Path == null ? "" : item.Path;
                    string bindProperty = item.Property == null ? "" : item.Property;
                    string field = bindPath.Split('.')[bindPath.Split('.').Length - 1];
                    if (bindProperty.ToLower() == "searchvalue" && !string.IsNullOrEmpty(bindPath) && !string.IsNullOrEmpty(bindProperty))
                    {
                        string path = string.Empty;
                        if (bindPath.Split('.').Length > 1) path = bindPath.Split('.')[0];
                        result.AppendFormat("{0}: at('rel:{1}','{2}')", "searchField", path, field);
                    }
                }
            }
            if (result.ToString().Length == 0)
            {
                if (!string.IsNullOrEmpty(control.SearchValue)) result.AppendFormat("{0}:'@{1}',", "searchField", control.SearchValue);
                else if (!string.IsNullOrEmpty(control.DisplayMember)) result.AppendFormat("{0}:'@{1}'", "searchField", control.DisplayMember);
            }
            return result.ToString();
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Web.UI;
using Wilmar.Model.Core.Definitions;
using Wilmar.Service.Common.Generate;
using Wilmar.Foundation.Projects;
using System.Text;
using Wilmar.Model.Core.Definitions.Screens;
using Wilmar.Model.Core.Definitions.Screens.Members;
using Wilmar.Model.Core.Definitions.Entities.DataTypes;
using Wilmar.Foundation;

namespace Wilmar.Build.Core.Dojo.Default.Builders
{
    /// <summary>
    /// 控件基类扩展
    /// </summary>
    public static class ControlExtend
    {
        public static int ctlNumber = 1;

        /// <summary>
        /// 获取生成器
        /// </summary>
        /// <param name="control">控件对象</param>
        /// <returns>返回控件对应的生成器</returns>
        public static ControlBuildBase GetBuilder(this ControlHost controlHost, bool isPreview, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
        {
            //图表控件名字为空自动生成一个默认名字
            var controlName = controlHost.Content.GetType().Name;
            if (controlName == "ChartPane" || controlName == "DropDownButton")
            {
                if (string.IsNullOrEmpty(controlHost.Name)) controlHost.Name = controlName + ctlNumber++;
            }

            if (!isPreview && doc != null && !string.IsNullOrEmpty(controlHost.Name))
            {
                controlHost.Name = doc.Name + "_" + controlHost.Name;
            }
            if (controlHost.Content != null)
            {
                var assembly = Assembly.GetAssembly(typeof(ControlExtend));
                string typeName = string.Format("Wilmar.Build.Core.Dojo.Default.Builders.{0}Build", controlHost.Content.GetType().Name);
                if (assembly.GetType(typeName) == null)
                {
                    throw new Exception(string.Format("类型【{0}】没对应的生成器。", typeName));
                }
                ControlBuildBase builder = Activator.CreateInstance(assembly.GetType(typeName), isPreview, controlHost, screenDef, compile, doc, permissionData, htmlWriter) as ControlBuildBase;
                return builder;
            }
            else return null;
        }

        /// <summary>
        /// 获取data-dojo-props
        /// </summary>
        /// <param name="controlBase"></param>
        /// <param name="screenDef"></param>
        /// <param name="isPreview"></param>
        /// <param name="returnContent"></param>
        /// <param name="isDataGridCell"></param>
        /// <returns></returns>
        public static string BuildControlProps(this ControlBase controlBase, ScreenDefinition screenDef, bool isPreview, Dictionary<int, Tuple<int, string>> permissionData, StringBuilder returnContent, StringBuilder sbConstraints = null, bool isDataGridCell = false, bool isListBox = false, bool isGridFormat = false)
        {
            StringBuilder result = new StringBuilder();
            StringBuilder sbProps = new StringBuilder();
            string controlName = controlBase.GetType().Name;
            dynamic control = controlBase;
            #region 权限
            if (permissionData != null && permissionData.Count > 0)
            {
                if (!string.IsNullOrEmpty(controlBase.Permissions))
                {
                    string permissionStr = string.Empty;
                    string[] pArray = controlBase.Permissions.Split(',');
                    foreach (var pa in pArray)
                    {
                        foreach (var pd in permissionData)
                        {
                            if (pd.Key == int.Parse(pa)) permissionStr += pd.Value.Item2 + ":" + pd.Value.Item1 + ",";
                        }
                    }
                    if (!string.IsNullOrEmpty(permissionStr))
                    {
                        if (isDataGridCell) returnContent.AppendFormat("{0},", permissionStr.Substring(0, permissionStr.Length - 1));
                        else sbProps.AppendFormat("{0},", permissionStr.Substring(0, permissionStr.Length - 1));
                    }
                }
            }
            #endregion

            #region Bindings
            if (!isPreview && controlBase.Bindings.Count > 0)
            {
                Dictionary<string, string> dictProperty = GetPropertyBindValue(controlBase);
                foreach (var item in controlBase.Bindings)
                {
                    string property = string.Empty, contextStr = string.Empty;
                    string bindPath = item.Path == null ? "" : item.Path.Replace("CurrentItem", "SelectedItem");
                    string bindProperty = item.Property == null ? "" : item.Property;
                    string path = bindPath;
                    string field = bindPath.Split('.')[bindPath.Split('.').Length - 1];
                    if (dictProperty.ContainsKey(bindProperty))
                    {
                        if (dictProperty.TryGetValue(bindProperty, out property)) bindProperty = property;
                    }
                    if (!string.IsNullOrEmpty(bindPath) && !string.IsNullOrEmpty(bindProperty))
                    {
                        #region 
                        if (bindProperty.ToLower() == "datasource")
                        {
                            if (controlName == "DataGrid" || controlName == "TreeGrid" || controlName == "PivotGrid" || controlName == "ListBox")
                            {
                                continue;
                            }
                        }
                        else if (bindProperty.ToLower() == "searchvalue" || bindProperty.ToLower() == "searchfield") continue;
                        #endregion
                        #region DisplayText
                        else if (bindProperty.ToLower() == "displaytext")
                        {
                            if (isDataGridCell)
                            {
                                string formatter = string.Empty;
                                if (bindPath.Split('.').Length == 4)
                                {
                                    bool isNavigatorMember = bindNavigatorMember(controlBase, screenDef, item.Path, 4);
                                    if (!isNavigatorMember)
                                    {
                                        if (!isGridFormat && controlName != "DatePicker" && controlName != "TimePicker")
                                        {
                                            string selectedStr = bindPath.Split('.')[0] + "." + bindPath.Split('.')[1] + ".";
                                            string fieldFullName = bindPath.Replace(selectedStr, "");
                                            returnContent.AppendFormat("fieldFullName:'{0}',", fieldFullName);

                                            formatter = "formatter:function(inDatum){if(inDatum){return inDatum." + field + ";}else{return '';}}";
                                        }
                                    }
                                }
                                else if (bindPath.Split('.').Length == 5)
                                {
                                    string selectedStr = bindPath.Split('.')[0] + "." + bindPath.Split('.')[1] + ".";
                                    string fieldFullName = bindPath.Replace(selectedStr, "");
                                    returnContent.AppendFormat("fieldFullName:'{0}',", fieldFullName);
                                    if (!isGridFormat && controlName != "DatePicker" && controlName != "TimePicker")
                                    {
                                        string relStr = bindPath.Split('.')[0] + "." + bindPath.Split('.')[1] + "." + bindPath.Split('.')[2] + ".";
                                        string relName = bindPath.Replace(relStr, "");
                                        string relFirst = relName.Split('.')[0];
                                        formatter = "formatter:function(inDatum){if(inDatum){if(inDatum." + relFirst + "){return inDatum." + relName + ";}else{return '';}}else{return '';}}";
                                    }
                                }
                                if (!string.IsNullOrEmpty(formatter)) returnContent.AppendFormat("{0},", formatter);
                            }
                        }
                        #endregion
                        #region Condition
                        else if (bindProperty.ToLower() == "condition")
                        {
                            if (isDataGridCell)
                            {
                                returnContent.AppendFormat("{0}: at('rel:{1}','{2}'),", bindProperty, "", field);
                                continue;
                            }
                        }
                        else if (bindProperty.ToLower() == "batcheditcondition")
                        {
                            continue;
                        }
                        #endregion
                        #region Value
                        else if (bindProperty.ToLower() == "value")
                        {
                            if (controlName == "ReportViewer") continue;

                            #region isListBox
                            if (isListBox)
                            {
                                if (bindPath.Split('.').Length == 4)
                                {
                                    field = bindPath.Split('.')[2] + "." + bindPath.Split('.')[3];
                                }
                                string itemPro = "${item." + field + "}";
                                sbProps.AppendFormat("{0}: {1},", bindProperty, itemPro);
                                continue;
                            }
                            #endregion
                            #region isDataGridCell
                            if (isDataGridCell)
                            {
                                string formatter = string.Empty;
                                if (bindPath.Split('.').Length == 3)
                                {
                                    bool isNavigatorMember = bindNavigatorMember(controlBase, screenDef, item.Path);
                                    if (!isNavigatorMember) returnContent.AppendFormat("fieldFullName:'{0}',", field);
                                }
                                else if (bindPath.Split('.').Length == 4)
                                {
                                    bool isNavigatorMember = bindNavigatorMember(controlBase, screenDef, item.Path, 4);
                                    if (!isNavigatorMember)
                                    {
                                        if (!isGridFormat && controlName != "DatePicker" && controlName != "TimePicker")
                                        {
                                            string selectedStr = bindPath.Split('.')[0] + "." + bindPath.Split('.')[1] + ".";
                                            string fieldFullName = bindPath.Replace(selectedStr, "");
                                            returnContent.AppendFormat("fieldFullName:'{0}',", fieldFullName);

                                            formatter = "formatter:function(inDatum){if(inDatum){return inDatum." + field + ";}else{return '';}}";
                                        }
                                    }
                                    field = bindPath.Split('.')[2];
                                }
                                else if (bindPath.Split('.').Length == 5)
                                {
                                    if (!isGridFormat && controlName != "DatePicker" && controlName != "TimePicker")
                                    {
                                        string selectedStr = bindPath.Split('.')[0] + "." + bindPath.Split('.')[1] + ".";
                                        string fieldFullName = bindPath.Replace(selectedStr, "");
                                        returnContent.AppendFormat("fieldFullName:'{0}',", fieldFullName);

                                        string relStr = bindPath.Split('.')[0] + "." + bindPath.Split('.')[1] + "." + bindPath.Split('.')[2] + ".";
                                        string relName = bindPath.Replace(relStr, "");
                                        string relFirst = relName.Split('.')[0];
                                        formatter = "formatter:function(inDatum){if(inDatum){if(inDatum." + relFirst + "){return inDatum." + relName + ";}else{return '';}}else{return '';}}";
                                    }
                                    field = bindPath.Split('.')[2];
                                }
                                returnContent.AppendFormat("field:'{0}',", field);
                                if (!string.IsNullOrEmpty(formatter)) returnContent.AppendFormat("{0},", formatter);
                                continue;
                            }
                            #endregion
                            #region ComboBox
                            if (controlName == "ComboBox")
                            {
                                if (bindPath.Split('.').Length == 3)
                                {
                                    bool isNavigatorMember = bindNavigatorMember(controlBase, screenDef, item.Path);
                                    if (isNavigatorMember) bindProperty = "item";
                                }
                            }
                            #endregion
                            else if (controlName == "CheckBox" || controlName == "RadioBox" || controlName == "ToggleButton") bindProperty = "checked";
                            else if (controlName == "SelectBox" || controlName == "CheckedMultiSelect" || controlName == "SearchMultiSelect" || controlName == "SelectPage") bindProperty = "values";
                        }
                        #endregion
                        #region Max/Min
                        if (bindProperty.ToLower() == "maximum")
                        {
                            if (controlName == "Numeric" || controlName == "ProgressBar") bindProperty = "max";
                        }
                        else if (bindProperty.ToLower() == "minimum")
                        {
                            if (controlName == "Numeric" || controlName == "ProgressBar") bindProperty = "min";
                        }
                        #endregion
                        #region OnClick
                        else if (bindProperty.ToLower() == "onclick")
                        {
                            if (controlName == "TreeView") bindProperty = "onNodeClick";
                        }
                        #endregion
                        #region OnRowDoubleClick 网格双击行事件
                        else if (bindProperty.ToLower() == "onrowdblclick")
                        {
                            returnContent.AppendFormat("rowdblclick:at('rel:{0}','{1}').direction(1),", "", path);
                            continue;
                        }
                        #endregion
                        #region Rended 网格渲染完成事件
                        else if (bindProperty.ToLower() == "rended")
                        {
                            returnContent.AppendFormat("rended:at('rel:{0}','{1}').direction(1),", "", path);
                            continue;
                        }
                        #endregion
                        #region GotoPage 网格分页事件
                        else if (bindProperty.ToLower() == "gotopage")
                        {
                            returnContent.AppendFormat("GotoPage:at('rel:{0}','{1}').direction(1),", "", path);
                            continue;
                        }
                        #endregion

                        #region 
                        if (bindPath.Split('.').Length == 1)
                        {
                            if (IsEmptyPathType(controlBase, screenDef, bindPath)) path = "";
                            if (controlName == "DataGrid") path = "";
                        }
                        else if (bindPath.Split('.').Length == 2)
                        {
                            path = bindPath.Split('.')[0];
                        }
                        else
                        {
                            path = bindPath.Replace("." + field, "");
                        }
                        if (!string.IsNullOrEmpty(path) && (controlName == "Button" || controlName == "MenuItem")) contextStr = string.Format("context:at('rel:','{0}'),", path);

                        sbProps.AppendFormat("{0}: at('rel:{1}','{2}'),{3}", bindProperty, path, field, contextStr);
                        #endregion
                    }
                }
            }
            #endregion

            #region DataSource
            if (!isPreview && controlBase.ExistProperty("DataSource"))
            {
                string bindPath = control.DataSource;
                if (isDataGridCell && !string.IsNullOrEmpty(bindPath))
                {
                    sbProps.AppendFormat("store: at('rel:{0}', '{1}').direction(1),", "", bindPath);
                }
            }
            #endregion
            #region ValueMember/DispalyMember/ChildMember/ParentMember/FocusDisplayMember/CheckedAttr/ResultAttr
            if (!isPreview && controlBase.ExistProperty("ValueMember"))
            {
                string bindPath = control.ValueMember;
                if (!string.IsNullOrEmpty(bindPath))
                {
                    string path = string.Empty;
                    //string field = bindPath.Split('.')[bindPath.Split('.').Length - 1];
                    string field = bindPath;
                    if (controlName == "TreeView" || controlName == "Menu") path = "idAttr";
                    else path = "valueAttr";
                    sbProps.AppendFormat("{0}:'{1}',", path, field);
                }
            }
            if (!isPreview && controlBase.ExistProperty("DisplayMember"))
            {
                string bindPath = control.DisplayMember;
                if (!string.IsNullOrEmpty(bindPath))
                {
                    string path = string.Empty;
                    //string field = bindPath.Split('.')[bindPath.Split('.').Length - 1];
                    string field = bindPath;
                    if (controlName == "TreeView" || controlName == "Menu") path = "labelAttr";
                    else if (controlName == "SelectBox" || controlName == "CheckedMultiSelect" || controlName == "SearchMultiSelect" || controlName == "RadioButtonList" || controlName == "SelectPage") path = "displayAttr";
                    else path = "searchAttr";
                    sbProps.AppendFormat("{0}:'{1}',", path, field);
                }
            }
            if (!isPreview && controlBase.ExistProperty("ChildMember"))
            {
                string bindPath = control.ChildMember;
                if (!string.IsNullOrEmpty(bindPath))
                {
                    string path = string.Empty;
                    //string field = bindPath.Split('.')[bindPath.Split('.').Length - 1];
                    string field = bindPath;
                    if (controlName == "TreeView")
                    {
                        sbProps.AppendFormat("{0}:'{1}',", "childrenAttr", field);
                    }
                }
            }
            if (!isPreview && controlBase.ExistProperty("ParentMember"))
            {
                string bindPath = control.ParentMember;
                if (!string.IsNullOrEmpty(bindPath))
                {
                    string path = string.Empty;
                    //string field = bindPath.Split('.')[bindPath.Split('.').Length - 1];
                    string field = bindPath;
                    if (controlName == "TreeView")
                    {
                        sbProps.AppendFormat("{0}:'{1}',", "parentAttr", field);
                    }
                }
            }
            if (!isPreview && controlBase.ExistProperty("FocusDisplayMember"))
            {
                string bindPath = control.FocusDisplayMember;
                if (!string.IsNullOrEmpty(bindPath))
                {
                    string path = string.Empty;
                    //string field = bindPath.Split('.')[bindPath.Split('.').Length - 1];
                    string field = bindPath;
                    path = "focusAttr";
                    sbProps.AppendFormat("{0}:'{1}',", path, field);
                }
            }
            if (!isPreview && controlBase.ExistProperty("CheckedAttr"))
            {
                string bindPath = control.CheckedAttr;
                if (!string.IsNullOrEmpty(bindPath))
                {
                    string path = string.Empty;
                    //string field = bindPath.Split('.')[bindPath.Split('.').Length - 1];
                    string field = bindPath;
                    path = "checkedAttr";
                    sbProps.AppendFormat("{0}:'{1}',", path, field);
                }
            }
            if (!isPreview && controlBase.ExistProperty("ResultAttr"))
            {
                string bindPath = control.ResultAttr;
                if (!string.IsNullOrEmpty(bindPath))
                {
                    string path = string.Empty;
                    //string field = bindPath.Split('.')[bindPath.Split('.').Length - 1];
                    string field = bindPath;
                    path = "resultAttr";
                    sbProps.AppendFormat("{0}:'{1}',", path, field);
                }
            }
            #endregion
            #region Max/Min
            if (controlBase.ExistProperty("Maximum") && control.Maximum != null)
            {
                if (controlName == "DatePicker" || controlName == "ProgressBar")
                {
                    string atName = string.Empty, atValue = string.Empty;
                    if (controlName == "ProgressBar")
                    {
                        atName = "max";
                        atValue = control.Maximum.ToString();
                    }
                    else
                    {
                        atName = "maximum";
                        atValue = "'" + control.Maximum.ToString("yyyy-MM-dd") + "'";
                    }
                    sbProps.AppendFormat("{0}:{1},", atName, atValue);
                }
                else if (controlName == "Numeric")
                {
                    sbConstraints.AppendFormat("max:{0},", control.Maximum.ToString());
                }
            }
            if (controlBase.ExistProperty("Minimum") && control.Minimum != null)
            {
                if (controlName == "DatePicker")
                {
                    string atName = string.Empty, atValue = string.Empty;
                    {
                        atName = "minimum";
                        atValue = "'" + control.Minimum.ToString("yyyy-MM-dd") + "'";
                    }
                    sbProps.AppendFormat("{0}:{1},", atName, atValue);
                }
                else if (controlName == "Numeric")
                {
                    sbConstraints.AppendFormat("min:{0},", control.Minimum.ToString());
                }
            }
            #endregion
            #region Value
            if (controlBase.ExistProperty("Value") && control.Value != null)
            {
                if (!string.IsNullOrEmpty(control.Value.ToString()))
                {
                    #region 
                    if (controlName == "ReportViewer") { }
                    else if (controlName == "DatePicker" || controlName == "Calendar")
                    {
                        sbProps.AppendFormat("value:'{0}',", control.Value.ToString("yyyy-MM-dd"));
                    }
                    else if (controlName == "CheckBox" || controlName == "RadioBox")
                    {
                        if (control.Value) sbProps.AppendFormat("checked:'{0}',", control.Value == true ? "checked" : "");
                    }
                    else if (controlName == "SelectBox" || controlName == "CheckedMultiSelect" || controlName == "SearchMultiSelect")
                    {
                        sbProps.AppendFormat("values:'{0}',", control.Value.ToString());
                    }
                    else if (controlName == "ToggleButton")
                    {
                        sbProps.AppendFormat("checked:'{0}',iconClass:'dijitCheckBoxIcon',", control.Value == true ? "checked" : "");
                    }
                    else if (controlName == "TimePicker")
                    {
                        sbProps.AppendFormat("value:'T{0}',", control.Value.TimeOfDay.ToString().Substring(0, 5));
                    }
                    else if (controlName == "ProgressBar")
                    {
                        sbProps.AppendFormat("value:{0},", control.Value.ToString());
                    }
                    else if (controlName == "Accordion" || controlName == "TabControl")
                    {
                        sbProps.AppendFormat("selectedIndex:{0},", control.Value.ToString());
                    }
                    else
                    {
                        sbProps.AppendFormat("value:'{0}',", control.Value.ToString());
                    }
                    #endregion
                }
            }
            #endregion
            #region PlaceHolder
            if (controlBase.ExistProperty("Watermark") && control.Watermark != null)
            {
                sbProps.AppendFormat("placeHolder:'{0}',", control.Watermark);
            }
            #endregion
            #region Url
            if (controlBase.ExistProperty("Url") && control.Url != null)
            {
                sbProps.AppendFormat("url:'{0}',", control.Url);
            }
            #endregion

            #region 验证器
            string validatorFullName = string.Empty;
            if (controlBase.Bindings.Count > 0)
            {
                var bindValue = (from t in controlBase.Bindings where t.Path != null && t.Property != null && t.Property.ToLower() == "value" select t).FirstOrDefault();
                if (bindValue != null)
                {
                    validatorFullName = bindValue.Path;
                    bool isBindNavigatorMember = bindNavigatorMember(controlBase, screenDef, validatorFullName);
                    if (isBindNavigatorMember)
                    {
                        var validatorMember = (from t in controlBase.Bindings where t.Path != null && t.Property != null && t.Property.ToLower() == "validatormember" select t).FirstOrDefault();
                        if (validatorMember != null)
                        {
                            validatorFullName = validatorMember.Path;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(validatorFullName))
                {
                    string validatorsStr = BuildValidators(controlBase, screenDef, validatorFullName);
                    sbProps.Append(validatorsStr);
                }
            }
            #endregion
            if (sbProps.ToString().Length > 0) result.Append(sbProps.ToString().Substring(0, sbProps.ToString().Length - 1));
            return result.ToString();
        }
        /// <summary>
        /// 判断控件中是否存属性
        /// </summary>
        /// <param  name="control">控件</param>
        /// <param name="propertyName">属性名称</param>
        /// <returns></returns>
        public static bool ExistProperty(this ControlBase controlBase, string propertyName)
        {
            var property = controlBase.GetType().GetProperty(propertyName);
            return property != null;
        }
        /// <summary>
        /// 获取网格列类型
        /// </summary>
        /// <param name="controlBase"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public static string GetGridCellTypeNewGrid(this ControlBase controlBase, string title, bool arrowButton = false)
        {
            string controlTypeName = controlBase.GetType().Name;
            switch (controlTypeName)
            {
                case "CheckBox":
                    return "editor:Controls.CheckBox";
                case "RadioBox":
                    return "editor:Controls.RadioButton";
                case "Numeric":
                    if (arrowButton) return "editor:Controls.NumberSpinner";
                    else return "editor:Controls.NumberTextBox";
                case "DatePicker":
                    return "editor:Controls.DateTextBox";
                case "TimePicker":
                    return "editor:Controls.TimeTextBox";
                case "ComboBox":
                    return "editor:Controls.FilteringSelect";
                case "CheckedMultiSelect":
                    return "editor:Controls.CheckedMultiSelect";
                case "Hyperlink":
                    return "";
                case "Button":
                    return "";
                case "SelectBox":
                    return "editor:Controls.SelectBox";
                case "SelectPage":
                    return "editor:Controls.SelectPage";
                default:
                    return "editor:Controls.TextBox";
            }
        }
        /// <summary>
        /// 获取对应的绑定属性字典
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetPropertyBindValue(this ControlBase controlBase)
        {
            Dictionary<string, string> dicts = new Dictionary<string, string>();
            string[] keys = new string[] {
                "Value","OnClick","OnRowDoubleClick","OnDoubleClick","OnSelected","DisplayText","Watermark","SelectedIndex",
                "BackColor","ForeColor","BorderColor","BorderThickness","Opacity","IsEnable","Visibility",
                "Margin","FontFamily","FontSize","FontWeight","FontStyle","IsReadOnly","CheckBoxes",
                "Width","Height","MinWidth","MinHeight","MaxWidth","MaxHeight",
                "HorizontalScrollBar","VerticalScrollBar","Maximum","Minimum",
                "ScreenTitle","ScreenName","ScreenInParameter","ScreenOutParameter","ScreenWidth","ScreenHeight","ShowModel",
                "Decimals","OnLoaded","CustomClose","OnBeforeOpen","Rended","SearchTextBox","ValidatorMember",
                "FormatItem","EOpen","ESelect","EClear","ETagRemove","ResultAttr","SearchValue","OnClear","LeafEvent",
                "ShowSearch","SelectIndex","CustomExpand","Condition","GotoPage","OnSelectedBefore"
            };
            string[] vals = new string[] {
                "value","onClick","onRowDblClick","onDblClick","onSelect","displayText","placeHolder","selectedIndex",
                "backColor","foreColor","borderColor","borderThickness","opacity","disabled","visibility",
                "margin","fontFamily","fontSize","fontWeight","fontStyle","readonly","checkBoxes",
                "width","height","minWidth","minHeight","maxWidth","maxHeight",
                "horizontalScrollBar","verticalScrollBar","maximum","minimum",
                "screenTitle","screenName","inputParameter","outputParameter","screenWidth","screenHeight","showModel",
                "decimals","Loaded","customClose","beforeOpen","rended","searchTextBox","validatorMember",
                "formatItem","eOpen","eSelect","eClear","eTagRemove","resultAttr","searchField","onClear","leafEvent",
                "showSearch","selectIndex","customExpand","condition","GotoPage","onSelectBefore"
            };
            for (int i = 0; i < keys.Length; i++) dicts.Add(keys[i], vals[i]);

            return dicts;
        }
        /// <summary>
        /// 当前绑定的属性是否是导航属性
        /// </summary>
        /// <param name="screenDef"></param>
        /// <param name="bindPath"></param>
        /// <returns></returns>
        public static bool bindNavigatorMember(this ControlBase controlBase, ScreenDefinition screenDef, string bindPath, int pathLevel = 3)
        {
            bool isNavigatorMember = false;
            string datasetName = bindPath.Split('.')[0];
            var dataDataSet = (from t in screenDef.Children where (t.MemberType == EMemberType.DataSet || t.MemberType == EMemberType.Objects) && t.Name == datasetName select t).FirstOrDefault();
            if (dataDataSet != null)
            {
                var childItems = (from t in dataDataSet.Children where t.MemberType == EMemberType.CurrentItem || t.MemberType == EMemberType.SelectedItem select t.Children).FirstOrDefault();
                if (childItems != null)
                {
                    var path = bindPath;
                    if (pathLevel == 4) path = bindPath.Split('.')[0] + "." + bindPath.Split('.')[1] + "." + bindPath.Split('.')[2];
                    var dataMember = (from t in childItems where t.FullName == path select t).FirstOrDefault();
                    if (dataMember != null)
                    {
                        if (pathLevel == 4)
                        {
                            MemberBase childMB = dataMember as MemberBase;
                            var childMBs = (from t in childMB.Children where t.FullName == bindPath select t).FirstOrDefault();
                            if (childMBs != null)
                            {
                                if (childMBs.MemberType == EMemberType.NavigateMember || childMBs.MemberType == EMemberType.DataSet) return isNavigatorMember = true;
                            }
                        }
                        else if (dataMember.MemberType == EMemberType.NavigateMember) return isNavigatorMember = true;
                    }
                }
            }
            return isNavigatorMember;
        }
        /// <summary>
        /// 返回所有验证
        /// </summary>
        /// <param name="controlBase"></param>
        /// <param name="screenDef"></param>
        /// <param name="validatorFullName"></param>
        /// <param name="dataTypeName"></param>
        /// <param name="dataTypeContent"></param>
        /// <returns></returns>
        public static void GetValidators(this ControlBase controlBase, ScreenDefinition screenDef, string validatorFullName, ref List<Model.Core.Definitions.Entities.ValidatorBase> listValidators, ref EDataBaseType baseType, ref CommonDataType dataTypeContent)
        {
            if (!string.IsNullOrEmpty(validatorFullName))
            {
                //验证成员来自 1/绑定数据集中的数据项. 2/属性
                string datasetName = validatorFullName.Split('.')[0];
                var pathLength = validatorFullName.Split('.').Length;
                if (pathLength == 1)
                {
                    //属性
                    var dataDataSet = (from t in screenDef.Children where t.MemberType == EMemberType.Property && t.Name == datasetName select t).FirstOrDefault();
                    if (dataDataSet != null)
                    {
                        BuildMemberValidatorsContent(dataDataSet, ref listValidators, ref baseType, ref dataTypeContent);
                    }
                }
                else
                {
                    //绑定数据集中的数据项
                    var dataDataSet = (from t in screenDef.Children where (t.MemberType == EMemberType.DataSet || t.MemberType == EMemberType.Objects) && t.Name == datasetName select t).FirstOrDefault();
                    if (dataDataSet != null)
                    {
                        var childItems = (from t in dataDataSet.Children
                                          where t.MemberType == EMemberType.CurrentItem || t.MemberType == EMemberType.SelectedItem
                                          select t.Children).FirstOrDefault();
                        if (childItems != null)
                        {
                            BuildMemberValidators(childItems, validatorFullName, true, ref listValidators, ref baseType, ref dataTypeContent);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取验证器
        /// </summary>
        /// <param name="controlBase"></param>
        /// <param name="screenDef"></param>
        /// <returns></returns>
        private static string BuildValidators(this ControlBase controlBase, ScreenDefinition screenDef, string validatorFullName)
        {
            StringBuilder result = new StringBuilder();
            StringBuilder sbValidator = new StringBuilder();

            List<Model.Core.Definitions.Entities.ValidatorBase> listValidators = new List<Model.Core.Definitions.Entities.ValidatorBase>();
            EDataBaseType baseType = EDataBaseType.String;
            CommonDataType dataTypeContent = null;
            GetValidators(controlBase, screenDef, validatorFullName, ref listValidators, ref baseType, ref dataTypeContent);
            string dataTypeName = BuildCommonMethod.GetTypeName(baseType);

            if (listValidators.Count > 0 && dataTypeContent != null)
            {
                string defaultValue = string.Empty, maxValue = string.Empty, minValue = string.Empty, maxLength = string.Empty, minLength = string.Empty;
                BuildCommonMethod.GetDataTypeValue(dataTypeContent, ref defaultValue, ref maxValue, ref minValue, ref maxLength, ref minLength);
                foreach (var validator in listValidators)
                {
                    string validatorType = validator.ValidatorType.ToString().ToLower();
                    if (validator.ValidatorType == Model.Core.Definitions.Entities.EValidatorType.Phone) validatorType = "tel";
                    else if (validator.ValidatorType == Model.Core.Definitions.Entities.EValidatorType.EmailAddress) validatorType = "email";
                    else if (validator.ValidatorType == Model.Core.Definitions.Entities.EValidatorType.RegularExpression) validatorType = "regexp";
                    else if (dataTypeName == "date" && validator.ValidatorType == Model.Core.Definitions.Entities.EValidatorType.Range) validatorType = "daterange";

                    if (validator.ValidatorType != Model.Core.Definitions.Entities.EValidatorType.Compare)
                    {
                        sbValidator.Append("" + validatorType + ":{");
                        sbValidator.AppendFormat("message:'{0}'", validator.ErrorMessage);

                        switch (validator.ValidatorType)
                        {
                            case Model.Core.Definitions.Entities.EValidatorType.MinLength:
                                if (dataTypeName == "string")
                                {
                                    sbValidator.AppendFormat(",min:{0}", minLength);
                                }
                                break;
                            case Model.Core.Definitions.Entities.EValidatorType.MaxLength:
                                if (dataTypeName == "string")
                                {
                                    sbValidator.AppendFormat(",max:{0}", maxLength);
                                }
                                break;
                            case Model.Core.Definitions.Entities.EValidatorType.Range:
                                if (dataTypeName == "number")
                                {
                                    sbValidator.AppendFormat(",min:{0}", minValue);
                                    sbValidator.AppendFormat(",max:{0}", maxValue);
                                }
                                else if (dataTypeName == "date")
                                {
                                    if (minValue != null)
                                    {
                                        DateTime dtMin = Convert.ToDateTime(minValue);
                                        sbValidator.AppendFormat(",min:'{0}'", dtMin.ToString("yyyy-MM-dd"));
                                    }
                                    if (maxValue != null)
                                    {
                                        DateTime dtMax = Convert.ToDateTime(maxValue);
                                        sbValidator.AppendFormat(",max:'{0}'", dtMax.ToString("yyyy-MM-dd"));
                                    }
                                }
                                break;
                            case Model.Core.Definitions.Entities.EValidatorType.RegularExpression:
                                var cValidator = validator as Model.Core.Definitions.Entities.Validators.RegularExpressionValidator;
                                sbValidator.AppendFormat(",pattern:'{0}'", cValidator.Pattern.Replace(@"\", @"\\"));
                                break;
                        }

                        sbValidator.Append("},");
                    }
                }
            }

            string _sbValidator = sbValidator.ToString().Length > 0 ? sbValidator.ToString().Substring(0, sbValidator.ToString().Length - 1) : "";
            if (!string.IsNullOrEmpty(_sbValidator))
            {
                result.Append("validators:{");
                result.Append(_sbValidator);
                result.Append("},");
            }

            return result.ToString();
        }
        /// <summary>
        /// 绑定属性Path为空
        /// </summary>
        /// <param name="screenDef"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static bool IsEmptyPathType(this ControlBase controlBase, ScreenDefinition screenDef, string name)
        {
            bool isEmpty = false;
            if (screenDef != null)
            {
                var datas = from t in screenDef.Children where (t.MemberType == EMemberType.Property || t.MemberType == EMemberType.Method || t.MemberType == EMemberType.BuiltMethod) && t.FullName == name select t;
                if (datas != null && datas.ToList().Count > 0) return true;
                List<MemberBase> list = screenDef.Children;
                foreach (var item in list)
                {
                    var itemDatas = from t in item.Children where (t.MemberType == EMemberType.Property || t.MemberType == EMemberType.Method || t.MemberType == EMemberType.BuiltMethod) && t.FullName == name select t;
                    if (itemDatas != null && itemDatas.ToList().Count > 0)
                    {
                        isEmpty = true;
                        break;
                    }
                }
            }
            return isEmpty;
        }
        /// <summary>
        /// 生成返回验证器
        /// </summary>
        /// <param name="childItems"></param>
        /// <param name="fullName"></param>
        /// <param name="listValidators"></param>
        /// <param name="baseType"></param>
        /// <param name="dataTypeContent"></param>
        private static void BuildMemberValidators(MemberBase[] childItems, string fullName, bool isParent, ref List<Model.Core.Definitions.Entities.ValidatorBase> listValidators, ref EDataBaseType baseType, ref CommonDataType dataTypeContent)
        {
            string path = fullName;
            if (isParent && fullName.Split('.').Length == 4) path = fullName.Replace("." + fullName.Split('.')[3], "");

            var dataMember = (from t in childItems where t.FullName == path select t).FirstOrDefault();
            if (dataMember != null)
            {
                if (dataMember.MemberType == EMemberType.NavigateMember)
                {
                    var _NavigateMember = dataMember as NavigateMember;
                    if (_NavigateMember.Children.Count() > 0)
                    {
                        BuildMemberValidators(_NavigateMember.Children, fullName, false, ref listValidators, ref baseType, ref dataTypeContent);
                    }
                }
                else
                {
                    BuildMemberValidatorsContent(dataMember, ref listValidators, ref baseType, ref dataTypeContent);
                }
            }
        }
        /// <summary>
        /// 生成返回验证器
        /// </summary>
        /// <param name="mb"></param>
        /// <param name="listValidators"></param>
        /// <param name="baseType"></param>
        /// <param name="dataTypeContent"></param>
        private static void BuildMemberValidatorsContent(MemberBase mb, ref List<Model.Core.Definitions.Entities.ValidatorBase> listValidators, ref EDataBaseType baseType, ref CommonDataType dataTypeContent)
        {
            switch (mb.MemberType)
            {
                case EMemberType.PrimaryMember:
                    var _PrimaryMember = mb as PrimaryMember;
                    if (_PrimaryMember.Content != null)
                    {
                        dataTypeContent = _PrimaryMember.Content as CommonDataType;
                        baseType = dataTypeContent.BaseType;
                        listValidators = dataTypeContent.Validators;
                    }
                    break;
                case EMemberType.CommonMember:
                    var _CommonMember = mb as CommonMember;
                    if (_CommonMember.Content != null)
                    {
                        dataTypeContent = _CommonMember.Content as CommonDataType;
                        baseType = dataTypeContent.BaseType;
                        listValidators = dataTypeContent.Validators;
                    }
                    break;
                case EMemberType.DataMember:
                    var _DataMember = mb as DataMember;
                    if (_DataMember.Content != null)
                    {
                        dataTypeContent = _DataMember.Content as CommonDataType;
                        baseType = dataTypeContent.BaseType;
                        listValidators = dataTypeContent.Validators;
                    }
                    break;
                case EMemberType.CalculateMember:
                    var _CalculateMember = mb as CalculateMember;
                    if (_CalculateMember.Content != null)
                    {
                        dataTypeContent = _CalculateMember.Content as CommonDataType;
                        baseType = dataTypeContent.BaseType;
                        listValidators = dataTypeContent.Validators;
                    }
                    break;
                case EMemberType.Property:
                    var _PropertyMember = mb as Property;
                    if (_PropertyMember.Content != null)
                    {
                        dataTypeContent = _PropertyMember.Content as CommonDataType;
                        baseType = dataTypeContent.BaseType;
                        listValidators = dataTypeContent.Validators;
                    }
                    break;
            }
        }
    }
}
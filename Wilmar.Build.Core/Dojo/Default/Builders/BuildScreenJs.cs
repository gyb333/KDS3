using System.Linq;
using Wilmar.Model.Core.Definitions;
using Wilmar.Model.Core.Definitions.Screens;
using Wilmar.Service.Common.Generate;
using Wilmar.Foundation.Projects;
using System.Web.UI;
using Wilmar.Model.Core.Definitions.Screens.Members;
using System.Text;
using Wilmar.Model.Core.Definitions.Querites;
using Wilmar.Foundation.Common;
using System.Collections.Generic;
using Wilmar.Model.Core.Definitions.Entities.DataTypes;
using Wilmar.Foundation;
using Wilmar.Model.Core.Definitions.Entities.Members;

namespace Wilmar.Build.Core.Dojo.Default.Builders
{
    /// <summary>
    /// 屏幕JS生成
    /// </summary>
    public static class BuildScreenJs
    {
        /// <summary>
        /// 生成JS
        /// </summary>
        /// <param name="controlHost">控制基类</param>
        /// <param name="compile">编译对象</param>
        /// <param name="screenDef">屏幕定义</param>
        /// <param name="htmlWriter">输出</param>
        /// <param name="doc">文档</param>
        public static void BuildJs(ControlHost controlHost, CompileBase compile, ScreenDefinition screenDef, HtmlTextWriter htmlWriter, ProjectDocument doc)
        {
            htmlWriter.WriteLine();
            htmlWriter.WriteLine("<script id=\"" + doc.Name + "\" type=\"text/javascript\">");
            htmlWriter.WriteLine("function " + compile.Project.Identity.ToLower() + "_" + doc.Name + "_init(screen) {");
            if (screenDef.Children.Count > 0)
            {
                RegisterMembers(controlHost, compile, screenDef, htmlWriter);
            }
            htmlWriter.WriteLine("screen.Initialize(0);");
            htmlWriter.WriteLine("}");
            htmlWriter.WriteLine("</script>");
        }
        private static void RegisterMembers(ControlHost controlHost, CompileBase compile, ScreenDefinition screenDef, HtmlTextWriter htmlWriter)
        {
            htmlWriter.WriteLine("var VM = screen.Proxy;");
            htmlWriter.Write("screen.Members([");

            string methodString = RegisterMethod(screenDef);
            string properString = RegisterProperty(screenDef);
            string datasetString = RegisterDataSet(compile, screenDef);
            string screenParaString = RegisterScreenParameter(screenDef);
            string eventsString = RegisterEvents(screenDef);
            string reportviewString = RegisterReportView(screenDef);
            string content = methodString + properString + datasetString + screenParaString + eventsString + reportviewString;
            if (content.Length > 0) content = content.Substring(0, content.Length - 1);
            htmlWriter.WriteLine(content);

            htmlWriter.WriteLine("]);");
        }

        #region ===================================== 生成方法 =====================================
        /// <summary>
        /// 生成方法
        /// </summary>
        /// <param name="controlHost">控制基类</param>
        /// <param name="compile">编译器</param>
        /// <param name="screenDef">屏幕定义器</param>
        private static string RegisterMethod(ScreenDefinition screenDef)
        {
            StringBuilder resultStr = new StringBuilder();
            var dataMethod = from t in screenDef.Children where t.MemberType == EMemberType.Method || t.MemberType == EMemberType.BuiltMethod select t;
            if (dataMethod != null && dataMethod.ToList().Count > 0)
            {
                resultStr.AppendLine();
                resultStr.AppendLine("{");
                resultStr.Append("MemberType:\"Method\",");
                resultStr.Append("MemberMeta:{");

                StringBuilder sbContent = new StringBuilder();
                foreach (var item in dataMethod.ToList())
                {
                    sbContent.Append(GetMethodContent(item, null, "Method"));
                }
                string _sbContent = sbContent.ToString().Length > 0 ? sbContent.ToString().Substring(0, sbContent.ToString().Length - 1) : "";
                resultStr.Append(_sbContent);

                resultStr.AppendLine("}");
                resultStr.Append("},");
            }

            return resultStr.ToString();
        }
        /// <summary>
        /// 获取方法生成内容
        /// </summary>
        /// <param name="mb"></param>
        /// <returns></returns>
        private static string GetMethodContent(MemberBase mb, MemberBase parent, string methodType)
        {
            StringBuilder result = new StringBuilder();

            string methodArg = string.Empty, methodBody = string.Empty, shortcuts = string.Empty;
            StringBuilder methodPara = new StringBuilder();
            StringBuilder methodReturnPara = new StringBuilder();
            if (mb.MemberType == EMemberType.Method)
            {
                methodBody = (mb as Method).Body;
                shortcuts = (mb as Method).Shortcuts;
            }
            else if (mb.MemberType == EMemberType.BuiltMethod)
            {
                shortcuts = (mb as BuiltMethod).Shortcuts;
            }
            foreach (var child in mb.Children)
            {
                if (child.MemberType == EMemberType.Parameter)
                {
                    var para = child as Parameter;
                    methodPara.Append("{");
                    methodPara.AppendFormat("Name:\"{0}\",", para.Name);
                    methodPara.AppendFormat("Type:{0},", (int)para.ParameterType);
                    methodPara.AppendFormat("Binding:\"{0}\"", para.Path == null ? "" : para.Path.Replace("CurrentItem", "SelectedItem"));
                    methodPara.Append("},");
                    methodArg += para.Name + ",";
                }
                else if (child.MemberType == EMemberType.ReturnParameter)
                {
                    var para = child as ReturnParameter;
                    methodReturnPara.Append("Result: {");
                    methodReturnPara.AppendFormat("Name:\"{0}\",", para.Name);
                    methodReturnPara.AppendFormat("Binding:\"{0}\"", para.Path == null ? "" : para.Path.Replace("CurrentItem", "SelectedItem"));
                    methodReturnPara.Append("},");
                }
            }
            string _methodPara = methodPara.ToString().Length > 0 ? methodPara.ToString().Substring(0, methodPara.ToString().Length - 1) : "";
            string _methodReturnPara = methodReturnPara.ToString().Length > 0 ? methodReturnPara.ToString().Substring(0, methodReturnPara.ToString().Length - 1) : "Result: null";
            string _methodArg = methodArg.Length > 0 ? methodArg.Substring(0, methodArg.Length - 1) : "";

            result.Append("" + mb.Name + ": {");
            result.Append("Type:\"" + methodType + "\"");
            result.Append("," + "Title:\"" + mb.Title + "\"");
            result.Append("," + "Shortcuts:\"" + shortcuts + "\"");
            if (mb.MemberType == EMemberType.Method || mb.MemberType == EMemberType.RemoteMethod)
            {
                result.Append("," + "Parameters:[" + _methodPara + "]");
                result.Append("," + _methodReturnPara);
                result.AppendLine("," + "Action: function(" + _methodArg + ") {");
                if (!string.IsNullOrEmpty(methodBody))
                {
                    string path = mb.Title;
                    if (parent != null) path = parent.Title + "->" + path;
                    result.AppendLine("//路径:" + path);
                }
                result.AppendLine(methodBody);
                result.Append("}");
            }
            result.Append("},");

            return result.ToString();
        }
        #endregion

        #region ===================================== 生成属性 =====================================
        /// <summary>
        /// 生成属性
        /// </summary>
        /// <param name="controlHost">控制基类</param>
        /// <param name="compile">编译器</param>
        /// <param name="screenDef">屏幕定义器</param>
        private static string RegisterProperty(ScreenDefinition screenDef)
        {
            StringBuilder resultStr = new StringBuilder();
            var dataProperty = from t in screenDef.Children where t.MemberType == EMemberType.Property select t;
            if (dataProperty != null && dataProperty.ToList().Count > 0)
            {
                resultStr.AppendLine();
                resultStr.AppendLine("{");
                resultStr.Append("MemberType:\"Property\",");
                resultStr.Append("MemberMeta:{");

                StringBuilder sbContent = new StringBuilder();
                foreach (var item in dataProperty.ToList())
                {
                    sbContent.Append(GetPropertyContent(item));
                }
                string _sbContent = sbContent.ToString().Length > 0 ? sbContent.ToString().Substring(0, sbContent.ToString().Length - 1) : "";
                resultStr.Append(_sbContent);

                resultStr.AppendLine("}");
                resultStr.Append("},");
            }

            return resultStr.ToString();
        }
        /// <summary>
        /// 获取属性生成内容
        /// </summary>
        /// <param name="mb"></param>
        /// <returns></returns>
        private static string GetPropertyContent(MemberBase mb)
        {
            StringBuilder result = new StringBuilder();
            var objProperty = mb as Property;
            if (objProperty == null) return string.Empty;

            var content = objProperty.Content;
            var getType = content.GetType();
            var typeContent = (CommonDataType)content;
            string defaultValue = string.Empty, maxValue = string.Empty, minValue = string.Empty, maxLength = string.Empty, minLength = string.Empty;
            BuildCommonMethod.GetDataTypeValue(typeContent, ref defaultValue, ref maxValue, ref minValue, ref maxLength, ref minLength);

            result.Append("" + objProperty.Name + ": {");
            result.Append("Title:\"" + objProperty.Title + "\",");
            result.Append("IsRequired:" + objProperty.IsRequired.ToString().ToLower() + ",");
            result.Append("IsCollection:" + objProperty.IsCollection.ToString().ToLower() + ",");
            result.Append("Type:\"" + BuildCommonMethod.GetTypeName(typeContent.BaseType) + "\",");
            result.Append("DataType:\"" + typeContent.BaseType.ToString() + "\",");

            if (getType.GetProperty("DefaultValue") != null)
            {
                if (string.IsNullOrEmpty(defaultValue)) result.Append("DefaultValue:\"" + defaultValue + "\",");
                else
                {
                    if (typeContent.BaseType == EDataBaseType.Boolean || (typeContent.BaseType == EDataBaseType.Int16 || typeContent.BaseType == EDataBaseType.Int32 || typeContent.BaseType == EDataBaseType.Int64 || typeContent.BaseType == EDataBaseType.Decimal || typeContent.BaseType == EDataBaseType.Double || typeContent.BaseType == EDataBaseType.Byte || typeContent.BaseType == EDataBaseType.Single))
                    {
                        result.Append("DefaultValue: " + defaultValue + ",");
                    }
                    else result.Append("DefaultValue:\"" + defaultValue + "\",");
                }
            }
            if (getType.GetProperty("MaxValue") != null)
            {
                if (string.IsNullOrEmpty(maxValue)) result.Append("MaxValue:\"" + maxValue + "\",");
                else
                {
                    if (typeContent.BaseType == EDataBaseType.Int16 || typeContent.BaseType == EDataBaseType.Int32 || typeContent.BaseType == EDataBaseType.Int64 || typeContent.BaseType == EDataBaseType.Decimal || typeContent.BaseType == EDataBaseType.Double || typeContent.BaseType == EDataBaseType.Byte || typeContent.BaseType == EDataBaseType.Single)
                    {
                        result.Append("MaxValue: " + maxValue + ",");
                    }
                    else result.Append("MaxValue:\"" + maxValue + "\",");
                }
            }
            if (getType.GetProperty("MinValue") != null)
            {
                if (string.IsNullOrEmpty(minValue)) result.Append("MinValue:\"" + minValue + "\",");
                else
                {
                    if (typeContent.BaseType == EDataBaseType.Int16 || typeContent.BaseType == EDataBaseType.Int32 || typeContent.BaseType == EDataBaseType.Int64 || typeContent.BaseType == EDataBaseType.Decimal || typeContent.BaseType == EDataBaseType.Double || typeContent.BaseType == EDataBaseType.Byte || typeContent.BaseType == EDataBaseType.Single)
                    {
                        result.Append("MinValue: " + minValue + ",");
                    }
                    else result.Append("MinValue:\"" + minValue + "\",");
                }
            }
            if (getType.GetProperty("MaxLength") != null) result.Append("MaxLength:\"" + maxLength + "\",");
            if (getType.GetProperty("MinLength") != null) result.Append("MinLength:\"" + minLength + "\"");
            result.Append("},");

            return result.ToString();
        }
        #endregion

        #region ===================================== 生成参数 =====================================
        /// <summary>
        /// 生成屏幕参数
        /// </summary>
        /// <param name="controlHost">控制基类</param>
        /// <param name="compile">编译器</param>
        /// <param name="screenDef">屏幕定义器</param>
        private static string RegisterScreenParameter(ScreenDefinition screenDef)
        {
            StringBuilder resultStr = new StringBuilder();
            var dataParamenter = from t in screenDef.Children where t.MemberType == EMemberType.ScreenParameters select t;
            if (dataParamenter != null && dataParamenter.ToList().Count > 0)
            {
                resultStr.AppendLine();
                resultStr.AppendLine("{");
                resultStr.Append("MemberType:\"Parameter\",");
                resultStr.Append("MemberMeta:{");

                StringBuilder sbContent = new StringBuilder();
                foreach (var item in dataParamenter.ToList())
                {
                    sbContent.Append(GetParameterContent(item));
                }
                string _sbContent = sbContent.ToString().Length > 0 ? sbContent.ToString().Substring(0, sbContent.ToString().Length - 1) : "";
                resultStr.Append(_sbContent);

                resultStr.AppendLine("}");
                resultStr.Append("},");
            }

            return resultStr.ToString();
        }
        /// <summary>
        /// 获取参数生成内容(屏幕参数/查询参数/报表参数)
        /// </summary>
        /// <param name="mb"></param>
        /// <returns></returns>
        private static string GetParameterContent(MemberBase mb)
        {
            StringBuilder result = new StringBuilder();
            switch (mb.MemberType)
            {
                case EMemberType.ScreenParameters:
                    var screenParameters = mb as ScreenParameters;
                    foreach (var child in screenParameters.Children)
                    {
                        string paras = GetParameterString(child);
                        result.Append(paras);
                    }
                    break;
                case EMemberType.QueryParameters:
                    var queryParameters = mb as QueryParameters;
                    foreach (var child in queryParameters.Children)
                    {
                        string paras = GetParameterString(child);
                        result.Append(paras);
                    }
                    break;
                case EMemberType.ReportParameters:
                    var reportParameters = mb as ReportParameters;
                    foreach (var child in reportParameters.Children)
                    {
                        string paras = GetParameterString(child);
                        result.Append(paras);
                    }
                    break;
            }
            return result.ToString();
        }
        /// <summary>
        /// 获取参数生成内容(屏幕参数/查询参数/报表参数)
        /// </summary>
        /// <param name="mb"></param>
        /// <returns></returns>
        private static string GetParameterString(MemberBase mb)
        {
            StringBuilder result = new StringBuilder();
            if (mb.MemberType == EMemberType.Parameter)
            {
                var para = mb as Parameter;
                result.Append("" + para.Name + ": {");
                result.AppendFormat("Title:\"{0}\",", para.Title);
                result.AppendFormat("Type:{0},", (int)para.ParameterType);
                result.AppendFormat("IsOptional:{0},", para.IsOptional.ToString().ToLower());
                result.AppendFormat("Binding:\"{0}\"", para.Path == null ? "" : para.Path.Replace("CurrentItem", "SelectedItem"));
                result.Append("},");
            }
            return result.ToString();
        }
        #endregion

        #region ===================================== 生成事件 =====================================
        /// <summary>
        /// 生成事件
        /// </summary>
        /// <param name="screenDef"></param>
        /// <returns></returns>
        private static string RegisterEvents(ScreenDefinition screenDef)
        {
            StringBuilder resultStr = new StringBuilder();
            var dataEvents = from t in screenDef.Children where t.MemberType == EMemberType.Events select t;
            if (dataEvents != null && dataEvents.ToList().Count > 0)
            {
                resultStr.AppendLine();
                resultStr.AppendLine("{");
                resultStr.Append("MemberType:\"Events\",");
                resultStr.Append("MemberMeta:{");

                StringBuilder sbContent = new StringBuilder();
                foreach (var item in dataEvents.ToList())
                {
                    sbContent.Append(GetEventsContent(item, null));
                }
                string _sbContent = sbContent.ToString().Length > 0 ? sbContent.ToString().Substring(0, sbContent.ToString().Length - 1) : "";
                resultStr.Append(_sbContent);

                resultStr.AppendLine("}");
                resultStr.Append("},");
            }
            return resultStr.ToString();
        }
        /// <summary>
        /// 获取事件集合生成内容
        /// </summary>
        /// <param name="mb"></param>
        /// <returns></returns>
        private static string GetEventsContent(MemberBase mb, MemberBase parent)
        {
            StringBuilder result = new StringBuilder();
            var objEvents = mb as Events;
            if (objEvents == null) return string.Empty;
            foreach (var child in objEvents.Children)
            {
                if (child.MemberType == EMemberType.Event)
                {
                    var objEvent = child as Event;
                    result.Append("" + objEvent.Name + ": {");
                    result.Append("Title:\"" + objEvent.Title + "\",");
                    result.AppendLine("Action: function(e) {");
                    if (!string.IsNullOrEmpty(objEvent.Body))
                    {
                        string path = mb.Title + "->" + objEvent.Title;
                        if (parent != null) path = parent.Title + "->" + path;
                        result.AppendLine("//路径:" + path);
                    }
                    result.AppendLine(objEvent.Body);
                    result.Append("}");
                    result.Append("},");
                }
            }
            return result.ToString();
        }
        #endregion

        #region ===================================== 生成报表 =====================================
        /// <summary>
        /// 生成报表
        /// </summary>
        /// <param name="screenDef"></param>
        /// <returns></returns>
        private static string RegisterReportView(ScreenDefinition screenDef)
        {
            StringBuilder resultStr = new StringBuilder();
            var dataReportViews = from t in screenDef.Children where t.MemberType == EMemberType.Report select t;
            if (dataReportViews != null && dataReportViews.ToList().Count > 0)
            {
                resultStr.AppendLine();
                resultStr.AppendLine("{");
                resultStr.Append("MemberType:\"Report\",");
                resultStr.Append("MemberMeta:{");

                StringBuilder sbContent = new StringBuilder();
                foreach (var item in dataReportViews.ToList())
                {
                    var report = item as Report;
                    sbContent.Append(item.Name + ": {");

                    sbContent.AppendFormat("Path:\"{0}\",", report.Path == null ? "" : report.Path);

                    StringBuilder sbParas = new StringBuilder();
                    StringBuilder sbReportParas = new StringBuilder();
                    foreach (var child in item.Children)
                    {
                        if (child.MemberType == EMemberType.ReportParameters)
                        {
                            sbReportParas.Append(GetParameterContent(child));
                            string _sbReportParas = sbReportParas.ToString().Length > 0 ? sbReportParas.ToString().Substring(0, sbReportParas.ToString().Length - 1) : "";
                            sbParas.Append(" _Parameters: {");
                            sbParas.Append(_sbReportParas);
                            sbParas.Append("},");
                        }
                    }
                    string _sbParas = sbParas.ToString().Length > 0 ? sbParas.ToString().Substring(0, sbParas.ToString().Length - 1) : "";
                    sbContent.Append(_sbParas);

                    sbContent.Append("},");
                }
                string _sbContent = sbContent.ToString().Length > 0 ? sbContent.ToString().Substring(0, sbContent.ToString().Length - 1) : "";
                resultStr.Append(_sbContent);

                resultStr.AppendLine("}");
                resultStr.Append("},");
            }
            return resultStr.ToString();
        }
        #endregion

        #region ===================================== 生成排序 =====================================
        /// <summary>
        /// 获取排序成员生成内容
        /// </summary>
        /// <param name="mb"></param>
        /// <returns></returns>
        private static string GetSortContent(MemberBase mb)
        {
            StringBuilder result = new StringBuilder();
            switch (mb.MemberType)
            {
                case EMemberType.DataSet:
                    var dataSet = mb as DataSet;
                    foreach (var item in dataSet.Sorts)
                    {
                        result.Append("{");
                        result.AppendFormat("Field:\"{0}\",", item.Name);
                        result.AppendFormat("Dir:\"{0}\"", item.Direction == 0 ? "asc" : "desc");
                        result.Append("},");
                    }
                    break;
                case EMemberType.Objects:
                    var objects = mb as Objects;
                    foreach (var item in objects.Sorts)
                    {
                        result.Append("{");
                        result.AppendFormat("Field:\"{0}\",", item.Name);
                        result.AppendFormat("Dir:\"{0}\"", item.Direction == 0 ? "asc" : "desc");
                        result.Append("},");
                    }
                    break;
            }
            return result.ToString();
        }
        #endregion

        #region ===================================== 生成过滤 =====================================
        /// <summary>
        /// 获取过滤成员生成内容
        /// </summary>
        /// <param name="mb"></param>
        /// <returns></returns>
        private static string GetFilterContent(CompileBase compile, MemberBase mb)
        {
            StringBuilder result = new StringBuilder();
            switch (mb.MemberType)
            {
                case EMemberType.DataSet:
                    var dataSet = mb as DataSet;
                    //数据集是集合
                    if (dataSet.IsCollection)
                    {
                        foreach (var item in dataSet.Filters)
                        {
                            string filterStr = GetFilterString(compile, mb, item);
                            result.Append(filterStr);
                        }
                    }
                    else
                    {
                        //非集合，默认一条查询过滤为当前数据集的主键查询
                        var QueryParametersObj = (from t in dataSet.Children.OfType<QueryParameters>() select t).FirstOrDefault();
                        if (QueryParametersObj != null)
                        {
                            var queryParameters = QueryParametersObj as QueryParameters;
                            foreach (var child in queryParameters.Children)
                            {
                                var parameters = child as Parameter;
                                StringBuilder filterStr = new StringBuilder();
                                filterStr.Append("{");
                                filterStr.Append("Type:\"Standard\",");
                                filterStr.Append("Logic:\"and\",");
                                filterStr.AppendFormat("Field:\"{0}\",", parameters.Name);
                                filterStr.Append("FieldType:\"number\",");
                                filterStr.Append("FunctionType:\"none\",");
                                filterStr.Append("FunctionParemeter:\"\",");
                                filterStr.Append("Operator:\"equal\",");
                                filterStr.Append("ValueType:1,");
                                filterStr.AppendFormat("Value:\"{0}\"", parameters.Name);
                                filterStr.Append("},");
                                result.Append(filterStr.ToString());
                            }
                        }
                    }
                    break;
                case EMemberType.Objects:
                    var objects = mb as Objects;
                    foreach (var item in objects.Filters)
                    {
                        string filterStr = GetFilterString(compile, mb, item);
                        result.Append(filterStr);
                    }
                    break;
            }
            return result.ToString();
        }
        /// <summary>
        /// 获取过滤成员生成内容
        /// </summary>
        /// <param name="mb"></param>
        /// <param name="filterItemBase"></param>
        /// <returns></returns>
        private static string GetFilterString(CompileBase compile, MemberBase mb, FilterItemBase filterItemBase)
        {
            StringBuilder result = new StringBuilder();
            if (filterItemBase.FilterType == EFilterType.Standard || filterItemBase.FilterType == EFilterType.MultiValue)
            {
                string filterItemStr = GetFilterItemData(compile, mb, filterItemBase);
                result.Append(filterItemStr);
            }
            else if (filterItemBase.FilterType == EFilterType.Group)
            {
                StringBuilder sbContent = new StringBuilder();
                var filter = filterItemBase as FilterGroup;
                result.Append("{");
                result.AppendFormat("Type:\"{0}\",", filterItemBase.FilterType.ToString());
                result.AppendFormat("Logic:\"{0}\",", filter.Connection.ToString().ToLower());
                result.Append("Items:[");
                foreach (var child in filter.Filters)
                {
                    string filterItemStr = GetFilterItemData(compile, mb, child);
                    sbContent.Append(filterItemStr);
                }
                result.Append(sbContent.ToString().Length > 0 ? sbContent.ToString().Substring(0, sbContent.ToString().Length - 1) : "");
                result.Append("]");
                result.Append("},");
            }
            return result.ToString();
        }
        /// <summary>
        /// 获取过滤项内容
        /// </summary>
        /// <param name="filterItemBase"></param>
        /// <returns></returns>
        private static string GetFilterItemData(CompileBase compile, MemberBase mb, FilterItemBase filterItemBase)
        {
            StringBuilder result = new StringBuilder();
            switch (filterItemBase.FilterType)
            {
                case EFilterType.Standard:
                    var standard = filterItemBase as FilterStandard;
                    string fieldTypeStandard = GetDataMemberType(compile, mb, standard.Name);
                    string standardString = GetFilterItemString(EFilterType.Standard, standard.Name, fieldTypeStandard, standard.FunctionParemeter,
                        standard.FilterType, standard.Connection, standard.Function, standard.Operator,
                        (int)standard.ValueType, standard.Value, 0, "", 0, "");
                    result.Append(standardString);
                    break;
                case EFilterType.MultiValue:
                    var multiValue = filterItemBase as FilterMultiValue;
                    string fieldTypeMultiValue = GetDataMemberType(compile, mb, multiValue.Name);
                    string multiValueString = GetFilterItemString(EFilterType.MultiValue, multiValue.Name, fieldTypeMultiValue, multiValue.FunctionParemeter,
                        multiValue.FilterType, multiValue.Connection, multiValue.Function, multiValue.Operator,
                        0, "", (int)multiValue.Value1Type, multiValue.Value1, (int)multiValue.Value2Type, multiValue.Value2);
                    result.Append(multiValueString);
                    break;
                case EFilterType.Group:
                    StringBuilder sbContent = new StringBuilder();
                    var filter = filterItemBase as FilterGroup;
                    result.Append("{");
                    result.AppendFormat("Type:\"{0}\",", filterItemBase.FilterType.ToString());
                    result.AppendFormat("Logic:\"{0}\",", filter.Connection.ToString().ToLower());
                    result.Append("Items:[");
                    foreach (var child in filter.Filters)
                    {
                        string filterItemStr = GetFilterItemData(compile, mb, child);
                        sbContent.Append(filterItemStr);
                    }
                    result.Append(sbContent.ToString().Length > 0 ? sbContent.ToString().Substring(0, sbContent.ToString().Length - 1) : "");
                    result.Append("]");
                    result.Append("},");
                    break;
            }
            return result.ToString();
        }
        /// <summary>
        /// 获取过滤项内容
        /// </summary>
        /// <param name="FilterItemType"></param>
        /// <param name="Name"></param>
        /// <param name="FieldType"></param>
        /// <param name="FunctionParemeter"></param>
        /// <param name="FilterType"></param>
        /// <param name="Connection"></param>
        /// <param name="Function"></param>
        /// <param name="Operator"></param>
        /// <param name="ValueType"></param>
        /// <param name="Value"></param>
        /// <param name="Value1Type"></param>
        /// <param name="Value1"></param>
        /// <param name="Value2Type"></param>
        /// <param name="Value2"></param>
        /// <returns></returns>
        private static string GetFilterItemString(EFilterType FilterItemType, string Name, string FieldType, string FunctionParemeter,
             EFilterType FilterType, EConnection Connection, EFunction Function, EOperator Operator,
             int ValueType, string Value, int Value1Type, string Value1, int Value2Type, string Value2)
        {
            StringBuilder result = new StringBuilder();

            result.Append("{");
            if (FilterItemType != EFilterType.Group) result.AppendFormat("Type:\"{0}\",", FilterType.ToString());
            result.AppendFormat("Logic:\"{0}\",", Connection.ToString().ToLower());
            result.AppendFormat("Field:\"{0}\",", Name);
            result.AppendFormat("FieldType:\"{0}\",", FieldType);
            result.AppendFormat("FunctionType:\"{0}\",", Function.ToString().ToLower());
            result.AppendFormat("FunctionParemeter:\"{0}\",", FunctionParemeter);
            result.AppendFormat("Operator:\"{0}\",", Operator.ToString().ToLower());
            if (FilterItemType == EFilterType.MultiValue)
            {
                result.AppendFormat("Value1Type:{0},", Value1Type);
                result.AppendFormat("Value1:\"{0}\",", Value1);
                result.AppendFormat("Value2Type:{0},", Value2Type);
                result.AppendFormat("Value2:\"{0}\"", Value2);
            }
            else
            {
                result.AppendFormat("ValueType:{0},", ValueType);
                result.AppendFormat("Value:\"{0}\"", Value);
            }
            result.Append("},");

            return result.ToString();
        }

        #region 根据路径获取数据类型
        /// <summary>
        /// 根据路径获取数据类型
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string GetDataMemberType(CompileBase compile, MemberBase parent, string path)
        {
            string fieldType = string.Empty;
            switch (parent.MemberType)
            {
                case EMemberType.DataSet:
                    var navMember = parent as DataSet;
                    fieldType = GetMemberFieldType(compile, navMember.EntityId, path, false);
                    break;
                case EMemberType.Objects:
                    var objMember = parent as Objects;
                    fieldType = GetMemberFieldType(compile, objMember.EntityId, path, false);
                    break;
            }
            if (!string.IsNullOrEmpty(fieldType)) fieldType = fieldType.Substring(0, fieldType.Length - 1);
            return fieldType;
        }
        /// <summary>
        /// 根据路径获取数据类型
        /// </summary>
        /// <param name="compile"></param>
        /// <param name="entityId"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string GetMemberFieldType(CompileBase compile, int entityId, string path, bool isQuery)
        {
            StringBuilder sbFieldType = new StringBuilder();
            var projectItem = compile.ProjectItems.Where(a => a.Value.DocumentType == GlobalIds.DocumentType.Entity && a.Key == entityId).FirstOrDefault();
            if (projectItem.Value != null)
            {
                var defItem = compile.GetDocumentBody(projectItem.Value) as EntityDefinition;
                string[] names = path.Split('.');
                for (int i = 0; i < names.Length; i++)
                {
                    string fieldPath = names[i];
                    GetMemberFieldTypeContent(defItem, compile, fieldPath, path, sbFieldType);

                    if (string.IsNullOrEmpty(sbFieldType.ToString()) && !isQuery)
                    {
                        //继承实体
                        var inheritEntityId = (from t in projectItem.Value.Propertys where t.Key == "InheritEntityId" select t.Value).FirstOrDefault();
                        if (inheritEntityId != null)
                        {
                            StringBuilder _sbFieldType = new StringBuilder();
                            GetInheritMemberFieldType(defItem, compile, projectItem.Value.Propertys, fieldPath, path, sbFieldType);
                        }
                    }
                }
            }
            return sbFieldType.ToString();
        }
        /// <summary>
        /// 根据路径获取数据类型
        /// </summary>
        /// <param name="def"></param>
        /// <param name="compile"></param>
        /// <param name="itemPropertys"></param>
        /// <param name="fieldPath"></param>
        /// <param name="fullPath"></param>
        /// <param name="sbFieldType"></param>
        private static void GetInheritMemberFieldType(EntityDefinition def, CompileBase compile, Dictionary<string, object> itemPropertys, string fieldPath, string fullPath, StringBuilder sbFieldType)
        {
            var inheritEntityId = (from t in itemPropertys where t.Key == "InheritEntityId" select t.Value).FirstOrDefault();
            if (inheritEntityId != null)
            {
                var entityItem = compile.ProjectItems.Where(a => a.Value.DocumentType == GlobalIds.DocumentType.Entity && a.Key == int.Parse(inheritEntityId.ToString())).FirstOrDefault();
                if (entityItem.Value != null)
                {
                    var defItem = compile.GetDocumentBody(entityItem.Value) as EntityDefinition;
                    GetMemberFieldTypeContent(defItem, compile, fieldPath, fullPath, sbFieldType);

                    GetInheritMemberFieldType(defItem, compile, entityItem.Value.Propertys, fieldPath, fullPath, sbFieldType);
                }
            }
        }
        /// <summary>
        /// 根据路径获取数据类型
        /// </summary>
        /// <param name="def"></param>
        /// <param name="compile"></param>
        /// <param name="fieldPath"></param>
        /// <param name="fullPath"></param>
        /// <param name="sbFieldType"></param>
        private static void GetMemberFieldTypeContent(EntityDefinition def, CompileBase compile, string fieldPath, string fullPath, StringBuilder sbFieldType)
        {
            var member = def.Members.Where(a => a.Name == fieldPath).FirstOrDefault();
            if (member != null)
            {
                if (member.MemberType == Model.Core.Definitions.Entities.EMemberType.Navigation)
                {
                    var navigationMember = member as NavigationMember;
                    string fieldType = navigationMember.ToCardinality == Model.Core.Definitions.Entities.EMappingCardinality.Many ? "array" : "object";
                    if (!string.IsNullOrEmpty(fieldType)) sbFieldType.Append(fieldType + "|");

                    int entityId = navigationMember.ToEntityId;
                    string _fieldPath = fullPath.Replace(navigationMember.Name + ".", "");
                    string _fieldType = GetMemberFieldType(compile, entityId, _fieldPath, true);
                    if (!string.IsNullOrEmpty(_fieldType)) sbFieldType.Append(_fieldType);
                }
                else
                {
                    var typeContent = (CommonDataType)member.Content;
                    string fieldType = BuildCommonMethod.GetTypeName(typeContent.BaseType);
                    if (!string.IsNullOrEmpty(fieldType)) sbFieldType.Append(fieldType + "|");
                }
            }
        }
        #endregion
        #endregion

        #region ===================================== 生成分页 =====================================
        /// <summary>
        /// 获取Page成员生成内容
        /// </summary>
        /// <param name="mb"></param>
        /// <returns></returns>
        private static string GetPageContent(MemberBase mb)
        {
            StringBuilder result = new StringBuilder();
            int pageSize = 60;
            switch (mb.MemberType)
            {
                case EMemberType.DataSet:
                    var dataSet = mb as DataSet;
                    pageSize = dataSet.PageSize;
                    result.AppendFormat("PageSize:{0},", pageSize <= 0 ? 60 : pageSize);
                    break;
                case EMemberType.Objects:
                    var objects = mb as Objects;
                    pageSize = objects.PageSize;
                    result.AppendFormat("PageSize:{0},", pageSize <= 0 ? 60 : pageSize);
                    break;
            }
            return result.ToString();
        }
        #endregion

        #region ===================================== 生成集合 =====================================
        /// <summary>
        /// 生成集合
        /// </summary>
        /// <param name="controlHost">控制基类</param>
        /// <param name="compile">编译器</param>
        /// <param name="screenDef">屏幕定义器</param>
        private static string RegisterDataSet(CompileBase compile, ScreenDefinition screenDef)
        {
            StringBuilder resultStr = new StringBuilder();
            var dataDataSet = from t in screenDef.Children where t.MemberType == EMemberType.DataSet || t.MemberType == EMemberType.Objects select t;
            if (dataDataSet != null && dataDataSet.ToList().Count() > 0)
            {
                string memberType = "RemoteCollection";
                StringBuilder sbRemoteCollectionContent = new StringBuilder();
                StringBuilder sbRemoteObjectContent = new StringBuilder();
                StringBuilder sbLocalCollectionContent = new StringBuilder();
                StringBuilder sbLocalObjectContent = new StringBuilder();
                foreach (var item in dataDataSet.ToList())
                {
                    if (item.MemberType == EMemberType.DataSet)
                    {
                        DataSet ds = item as DataSet;
                        if (ds.IsCollection) sbRemoteCollectionContent.Append(GetDataSetAndObjectContent(compile, screenDef, item));
                        else sbRemoteObjectContent.Append(GetDataSetAndObjectContent(compile, screenDef, item));

                        //2017-01-03
                        var entityItem = compile.ProjectItems.Where(a => a.Value.DocumentType == GlobalIds.DocumentType.Entity && a.Key == ds.EntityId).FirstOrDefault();
                        if (entityItem.Value != null)
                        {
                            string entityName = entityItem.Value.Name;
                            string bindStr = ds.Binding == null ? "" : ds.Binding.Replace("CurrentItem", "SelectedItem");
                            string bindField = bindStr.Split('.').Length > 2 ? bindStr.Split('.')[2] : "";
                            if (!string.IsNullOrEmpty(bindField) && !string.IsNullOrEmpty(entityName))
                            {
                                var def = compile.GetDocumentBody(entityItem.Value) as EntityDefinition;
                                var entityNavigation = def.Members.Where(a => a.MemberType == Model.Core.Definitions.Entities.EMemberType.Navigation).ToList();
                                foreach (var nav in entityNavigation)
                                {
                                    var navigationMember = nav as Model.Core.Definitions.Entities.Members.NavigationMember;
                                    string toName = navigationMember.Name;
                                    string toEntity = (from t in compile.ProjectItems where t.Key == navigationMember.ToEntityId select t.Value).FirstOrDefault().Name;
                                    if (toName == bindField && toEntity == entityName)
                                    {
                                        memberType = "TreeCollection";
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else if (item.MemberType == EMemberType.Objects)
                    {
                        Objects obj = item as Objects;
                        if (obj.IsCollection) sbLocalCollectionContent.Append(GetDataSetAndObjectContent(compile, screenDef, item));
                        else sbLocalObjectContent.Append(GetDataSetAndObjectContent(compile, screenDef, item));
                    }
                }
                string _sbRemoteCollectionContent = sbRemoteCollectionContent.ToString().Length > 0 ? sbRemoteCollectionContent.ToString().Substring(0, sbRemoteCollectionContent.ToString().Length - 1) : "";
                string _sbLocalCollectionContent = sbLocalCollectionContent.ToString().Length > 0 ? sbLocalCollectionContent.ToString().Substring(0, sbLocalCollectionContent.ToString().Length - 1) : "";
                string _sbRemoteObjectContent = sbRemoteObjectContent.ToString().Length > 0 ? sbRemoteObjectContent.ToString().Substring(0, sbRemoteObjectContent.ToString().Length - 1) : "";
                string _sbLocalObjectContent = sbLocalObjectContent.ToString().Length > 0 ? sbLocalObjectContent.ToString().Substring(0, sbLocalObjectContent.ToString().Length - 1) : "";
                if (!string.IsNullOrEmpty(_sbRemoteCollectionContent))
                {
                    resultStr.AppendLine();
                    resultStr.AppendLine("{");
                    resultStr.Append("MemberType:\"" + memberType + "\",");
                    resultStr.Append("MemberMeta:{");
                    resultStr.Append(_sbRemoteCollectionContent);
                    resultStr.AppendLine("}");
                    resultStr.Append("},");
                }
                if (!string.IsNullOrEmpty(_sbLocalCollectionContent))
                {
                    resultStr.AppendLine();
                    resultStr.AppendLine("{");
                    resultStr.Append("MemberType:\"LocalCollection\",");
                    resultStr.Append("MemberMeta:{");
                    resultStr.Append(_sbLocalCollectionContent);
                    resultStr.AppendLine("}");
                    resultStr.Append("},");
                }
                if (!string.IsNullOrEmpty(_sbRemoteObjectContent))
                {
                    resultStr.AppendLine();
                    resultStr.AppendLine("{");
                    resultStr.Append("MemberType:\"RemoteObject\",");
                    resultStr.Append("MemberMeta:{");
                    resultStr.Append(_sbRemoteObjectContent);
                    resultStr.AppendLine("}");
                    resultStr.Append("},");
                }
                if (!string.IsNullOrEmpty(_sbLocalObjectContent))
                {
                    resultStr.AppendLine();
                    resultStr.AppendLine("{");
                    resultStr.Append("MemberType:\"LocalObject\",");
                    resultStr.Append("MemberMeta:{");
                    resultStr.Append(_sbLocalObjectContent);
                    resultStr.AppendLine("}");
                    resultStr.Append("},");
                }
            }
            return resultStr.ToString();
        }
        /// <summary>
        /// 获取数据集和对象生成内容
        /// </summary>
        /// <param name="compile"></param>
        /// <param name="screenDef"></param>
        /// <param name="mb"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string GetDataSetAndObjectContent(CompileBase compile, ScreenDefinition screenDef, MemberBase mb)
        {
            StringBuilder result = new StringBuilder();
            StringBuilder sbExpand = new StringBuilder();
            StringBuilder sbExpandRef = new StringBuilder();
            StringBuilder sbSort = new StringBuilder();
            StringBuilder sbFilter = new StringBuilder();
            StringBuilder sbPage = new StringBuilder();
            StringBuilder sbQueryParameter = new StringBuilder();
            StringBuilder sbEntityContent = new StringBuilder();
            StringBuilder sbAllForeigns = new StringBuilder();
            GetDataSetAndObjectBase(compile, mb, sbEntityContent, sbAllForeigns);
            result.Append("" + mb.Name + ": {");
            #region Entity
            result.Append(sbEntityContent.ToString());
            #endregion
            #region Children
            StringBuilder sbMethod = new StringBuilder();
            StringBuilder sbParas = new StringBuilder();
            StringBuilder sbProperty = new StringBuilder();
            StringBuilder sbEvents = new StringBuilder();
            StringBuilder sbFields = new StringBuilder();
            foreach (var child in mb.Children)
            {
                if (child.MemberType == EMemberType.SelectedItem || child.MemberType == EMemberType.CurrentItem)
                {
                    foreach (var itemChild in child.Children)
                    {
                        if (itemChild.MemberType == EMemberType.RemoteMethod)
                        {
                            sbMethod.Append(GetMethodContent(itemChild, mb, "RemoteObjectMethod"));
                            continue;
                        }
                        else if (itemChild.MemberType == EMemberType.NavigateMember)
                        {
                            GetExpandContent(itemChild, sbExpandRef, "");
                        }
                        sbFields.Append(GetDataMemberContent(compile, mb, itemChild, sbAllForeigns.ToString()));
                    }
                }
                else if (child.MemberType == EMemberType.Method || child.MemberType == EMemberType.BuiltMethod) sbMethod.Append(GetMethodContent(child, mb, "Method"));
                else if (child.MemberType == EMemberType.RemoteMethod) sbMethod.Append(GetMethodContent(child, mb, "RemoteCollectionMethod"));
                else if (child.MemberType == EMemberType.QueryParameters) sbParas.Append(GetParameterContent(child));
                else if (child.MemberType == EMemberType.Property) sbProperty.Append(GetPropertyContent(child));
                else if (child.MemberType == EMemberType.Events) sbEvents.Append(GetEventsContent(child, mb));
            }
            #endregion
            #region Expand
            string _sbExpand = sbExpandRef.ToString();
            if (!string.IsNullOrEmpty(_sbExpand))
            {
                string expandStr = _sbExpand.Length > 0 ? _sbExpand.Substring(0, _sbExpand.Length - 1) : "";
                sbExpand.Append("Expand:\"" + expandStr + "\",");
            }
            #endregion
            #region Sort
            string sortContent = GetSortContent(mb);
            string _sbSort = sortContent.ToString().Length > 0 ? sortContent.ToString() : "";
            if (!string.IsNullOrEmpty(_sbSort))
            {
                sbSort.Append("Sort:[");
                sbSort.Append(_sbSort.ToString().Length > 0 ? _sbSort.ToString().Substring(0, _sbSort.ToString().Length - 1) : "");
                sbSort.Append("]" + ",");
            }
            #endregion
            #region Filter
            string filterContent = GetFilterContent(compile, mb);
            string _sbFilter = filterContent.ToString().Length > 0 ? filterContent.ToString() : "";
            if (!string.IsNullOrEmpty(_sbFilter))
            {
                sbFilter.Append("Filters:[");
                sbFilter.Append(_sbFilter.ToString().Length > 0 ? _sbFilter.ToString().Substring(0, _sbFilter.ToString().Length - 1) : "");
                sbFilter.Append("]" + ",");
            }
            #endregion
            #region Page
            string pageContent = GetPageContent(mb);
            string _sbPage = pageContent.ToString().Length > 0 ? pageContent.ToString() : "";
            if (!string.IsNullOrEmpty(_sbPage))
            {
                sbPage.Append("Page:{");
                sbPage.Append(_sbPage.ToString().Length > 0 ? _sbPage.ToString().Substring(0, _sbPage.ToString().Length - 1) : "");
                sbPage.Append("}" + ",");
            }
            #endregion
            #region QueryParameters
            string _sbParas = sbParas.ToString().Length > 0 ? sbParas.ToString().Substring(0, sbParas.ToString().Length - 1) : "";
            if (!string.IsNullOrEmpty(_sbParas))
            {
                sbQueryParameter.Append("Parameters: {");
                sbQueryParameter.Append(_sbParas);
                sbQueryParameter.Append("}" + ",");
            }
            #endregion
            #region Query
            string _sbQuery = sbExpand.ToString() + sbSort.ToString() + sbFilter.ToString() + sbPage.ToString() + sbQueryParameter.ToString();
            if (!string.IsNullOrEmpty(_sbQuery))
            {
                result.Append("," + "_Query:{" + _sbQuery.Substring(0, _sbQuery.Length - 1) + "}");
            }
            #endregion
            #region Methods
            string _sbMethod = sbMethod.ToString().Length > 0 ? sbMethod.ToString().Substring(0, sbMethod.ToString().Length - 1) : "";
            if (!string.IsNullOrEmpty(_sbMethod))
            {
                result.Append("," + "_Methods: {");
                result.Append(_sbMethod);
                result.Append("}");
            }
            #endregion
            #region Property
            string _sbProperty = sbProperty.ToString().Length > 0 ? sbProperty.ToString().Substring(0, sbProperty.ToString().Length - 1) : "";
            if (!string.IsNullOrEmpty(_sbProperty))
            {
                result.Append("," + "_Propertys: {");
                result.Append(_sbProperty);
                result.Append("}");
            }
            #endregion
            #region Events
            string _sbEvents = sbEvents.ToString().Length > 0 ? sbEvents.ToString().Substring(0, sbEvents.ToString().Length - 1) : "";
            if (!string.IsNullOrEmpty(_sbEvents))
            {
                result.Append("," + "_Events: {");
                result.Append(_sbEvents);
                result.Append("}");
            }
            #endregion
            #region Fields
            string _sbFields = sbFields.ToString().Length > 0 ? sbFields.ToString().Substring(0, sbFields.ToString().Length - 1) : "";
            if (!string.IsNullOrEmpty(_sbFields))
            {
                result.Append("," + "_Fields: {");
                result.Append(_sbFields);
                result.Append("}");
            }
            #endregion
            result.Append("},");
            return result.ToString();
        }
        private static void GetDataSetAndObjectBase(CompileBase compile, MemberBase mb, StringBuilder sbEntityContent, StringBuilder sbAllForeigns)
        {
            if (mb.MemberType == EMemberType.DataSet)
            {
                StringBuilder sbPrimaryContent = new StringBuilder();
                DataSet ds = (DataSet)mb;
                var entityItem = compile.ProjectItems.Where(a => a.Value.DocumentType == GlobalIds.DocumentType.Entity && a.Key == ds.EntityId).FirstOrDefault();
                if (entityItem.Value != null)
                {
                    #region DataSet-Property
                    var def = compile.GetDocumentBody(entityItem.Value) as EntityDefinition;
                    BuildPrimaryContent(mb, entityItem.Value.Propertys, def, compile, sbPrimaryContent);

                    string bindStr = ds.Binding == null ? "" : ds.Binding.Replace("CurrentItem", "SelectedItem");
                    sbEntityContent.AppendFormat("Entity:\"{0}\",", entityItem.Value.Name);
                    sbEntityContent.AppendFormat("EntityId:{0},", ds.EntityId);
                    sbEntityContent.AppendFormat("AutoRefresh:{0},", ds.AutoRefresh.ToString().ToLower());
                    sbEntityContent.AppendFormat("Binding:\"{0}\",", bindStr);
                    sbEntityContent.AppendFormat("IdAttribute:\"{0}\",", sbPrimaryContent.ToString().Length == 0 ? "" : sbPrimaryContent.ToString().Substring(0, sbPrimaryContent.ToString().Length - 1));
                    sbEntityContent.AppendFormat("Name:\"{0}\"", mb.Name);

                    #region 获取所有自定义外键
                    StringBuilder sbForeignKey = new StringBuilder();
                    BuildCommonMethod.GetAllRelationForeignContent(entityItem.Value.Propertys, def, compile, sbForeignKey);
                    if (sbForeignKey.ToString().Length > 0)
                    {
                        sbAllForeigns.Append(sbForeignKey.ToString());
                    }
                    #endregion
                    #endregion
                }
            }
            else if (mb.MemberType == EMemberType.Objects)
            {
                StringBuilder sbPrimaryContent = new StringBuilder();
                Objects ds = (Objects)mb;
                var entityItem = compile.ProjectItems.Where(a => a.Value.DocumentType == GlobalIds.DocumentType.Entity && a.Key == ds.EntityId).FirstOrDefault();
                if (entityItem.Value != null)
                {
                    #region Objects-Property
                    var def = compile.GetDocumentBody(entityItem.Value) as EntityDefinition;
                    BuildPrimaryContent(mb, entityItem.Value.Propertys, def, compile, sbPrimaryContent);
                    sbEntityContent.AppendFormat("Entity:\"{0}\",", entityItem.Value.Name);

                    //获取所有自定义外键
                    StringBuilder sbForeignKey = new StringBuilder();
                    BuildCommonMethod.GetAllRelationForeignContent(entityItem.Value.Propertys, def, compile, sbForeignKey);
                    if (sbForeignKey.ToString().Length > 0)
                    {
                        sbAllForeigns.Append(sbForeignKey.ToString());
                    }
                    #endregion
                }
                else
                {
                    string primaryStr = BuildPrimaryFieldsContent(mb);
                    if (!string.IsNullOrEmpty(primaryStr)) sbPrimaryContent.AppendFormat("{0},", primaryStr.Substring(0, primaryStr.Length - 1));
                }
                sbEntityContent.AppendFormat("IdAttribute:\"{0}\",", sbPrimaryContent.ToString().Length == 0 ? "" : sbPrimaryContent.ToString().Substring(0, sbPrimaryContent.ToString().Length - 1));
                sbEntityContent.AppendFormat("Name:\"{0}\"", mb.Name);
            }
        }

        /// <summary>
        /// 获取展开成员内容
        /// </summary>
        /// <param name="mb"></param>
        /// <param name="expandStr"></param>
        private static void GetExpandContent(MemberBase mb, StringBuilder expandStr, string parentName)
        {
            NavigateMember nm = mb as NavigateMember;
            if (nm != null)
            {
                if (nm.IsExpand)
                {
                    string expandName = nm.Name;
                    if (!string.IsNullOrEmpty(parentName)) expandName = parentName + "($expand=" + nm.Name + ")";
                    expandStr.AppendFormat("{0},", expandName);
                }
                if (nm.Children.Count() > 0)
                {
                    foreach (var child in nm.Children)
                    {
                        if (child.MemberType == EMemberType.NavigateMember) GetExpandContent(child, expandStr, nm.Name);
                    }
                }
            }
        }
        /// <summary>
        /// 获取数据成员生成内容
        /// </summary>
        /// <param name="mb"></param>
        /// <returns></returns>
        private static string GetDataMemberContent(CompileBase compile, MemberBase parent, MemberBase mb, string AllForeigns)
        {
            StringBuilder result = new StringBuilder();
            switch (mb.MemberType)
            {
                case EMemberType.PrimaryMember:
                    var primaryMember = mb as Model.Core.Definitions.Screens.Members.PrimaryMember;
                    string memberStr = GetDataMemberString(compile, parent, mb, primaryMember.Name, primaryMember.Title, primaryMember.IsRequired, "", primaryMember.Content, "", false, false, AllForeigns);
                    result.Append(memberStr);
                    break;
                case EMemberType.DataMember:
                    var dataMember = mb as DataMember;
                    string dataMemberStr = GetDataMemberString(compile, parent, mb, dataMember.Name, dataMember.Title, dataMember.IsRequired, "", dataMember.Content, "", false, true, AllForeigns);
                    result.Append(dataMemberStr);
                    break;
                case EMemberType.CommonMember:
                    var commonMember = mb as Model.Core.Definitions.Screens.Members.CommonMember;
                    string commonMemberStr = GetDataMemberString(compile, parent, mb, commonMember.Name, commonMember.Title, commonMember.IsRequired, "", commonMember.Content, "", false, false, AllForeigns);
                    result.Append(commonMemberStr);
                    break;
                case EMemberType.CalculateMember:
                    var calculateMember = mb as CalculateMember;
                    string calculateMemberStr = GetDataMemberString(compile, parent, mb, calculateMember.Name, calculateMember.Title, calculateMember.IsRequired, calculateMember.Body, calculateMember.Content, "", false, true, AllForeigns);
                    result.Append(calculateMemberStr);
                    break;
                case EMemberType.NavigateMember:
                    var navigateMember = mb as NavigateMember;
                    string navigateMemberStr = GetDataMemberString(compile, parent, mb, navigateMember.Name, navigateMember.Title, false, "", null, navigateMember.Children.Count() > 0 ? "object" : "array", true, false, AllForeigns);
                    result.Append(navigateMemberStr);
                    break;
            }
            return result.ToString();
        }
        /// <summary>
        /// 获取数据成员生成内容
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Title"></param>
        /// <param name="IsRequired"></param>
        /// <param name="Body"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        private static string GetDataMemberString(CompileBase compile, MemberBase parent, MemberBase mb, string Name, string Title, bool IsRequired, string Body, Wilmar.Model.Core.Definitions.Entities.DataTypeBase content, string Type, bool isNav, bool isDataMember, string AllForeigns)
        {
            StringBuilder result = new StringBuilder();
            StringBuilder sbNav = new StringBuilder();
            #region 根据字段获取服务端刷新和单向关系属性
            if (parent.MemberType == EMemberType.DataSet)
            {
                DataSet ds = (DataSet)parent;
                var entityItems = compile.ProjectItems.Where(a => a.Value.DocumentType == GlobalIds.DocumentType.Entity && a.Key == ds.EntityId).FirstOrDefault();
                if (entityItems.Value != null)
                {
                    //服务端刷新和单向关系属性
                    StringBuilder sbServerRefresh = new StringBuilder();
                    var def = compile.GetDocumentBody(entityItems.Value) as EntityDefinition;
                    BuildServerRefreshItems(entityItems.Value.Propertys, def, compile, Name, sbServerRefresh);
                    if (sbServerRefresh.ToString().Length > 0)
                    {
                        sbNav.Append(sbServerRefresh.ToString());
                    }
                }
            }
            else if (parent.MemberType == EMemberType.Objects)
            {
                Objects ds = (Objects)parent;
                var entityItems = compile.ProjectItems.Where(a => a.Value.DocumentType == GlobalIds.DocumentType.Entity && a.Key == ds.EntityId).FirstOrDefault();
                if (entityItems.Value != null)
                {
                    //服务端刷新和单向关系属性
                    StringBuilder sbServerRefresh = new StringBuilder();
                    var def = compile.GetDocumentBody(entityItems.Value) as EntityDefinition;
                    BuildServerRefreshItems(entityItems.Value.Propertys, def, compile, Name, sbServerRefresh);
                    if (sbServerRefresh.ToString().Length > 0)
                    {
                        sbNav.Append(sbServerRefresh.ToString());
                    }
                }
            }
            #endregion
            if (isNav) //导航属性
            {
                result.Append("" + Name + ": {");
                result.Append(sbNav.ToString());
                result.Append("Title:\"" + Title + "\",");
                result.Append("IsCustom:" + isDataMember.ToString().ToLower() + ",");
                result.Append("Type:\"" + Type + "\",");
                result.Append("DataType:\"" + Type + "\"");
                result.Append("},");
            }
            else
            {
                #region 
                //标识该字段是否是自定义外键
                bool IsForeignKey = false;
                if (AllForeigns.Length > 0)
                {
                    string[] foreigns = AllForeigns.Split(',');
                    for (var i = 0; i < foreigns.Length; i++)
                    {
                        if (foreigns[i] == Name)
                        {
                            IsForeignKey = true;
                            break;
                        }
                    }
                }
                if (content != null)
                {
                    var getType = content.GetType();
                    var typeContent = (CommonDataType)content;
                    if (typeContent.BaseType == EDataBaseType.Timestamp) return string.Empty;
                }
                result.Append("" + Name + ": {");
                result.Append("Title:\"" + Title + "\",");
                if (IsForeignKey) result.Append("IsForeignKey:" + IsForeignKey.ToString().ToLower() + ",");
                result.Append("IsCustom:" + isDataMember.ToString().ToLower() + ",");
                result.Append("IsRequired:" + IsRequired.ToString().ToLower() + ",");
                if (content != null)
                {
                    var getType = content.GetType();
                    var typeContent = (CommonDataType)content;
                    string defaultValue = string.Empty, maxValue = string.Empty, minValue = string.Empty, maxLength = string.Empty, minLength = string.Empty;
                    BuildCommonMethod.GetDataTypeValue(typeContent, ref defaultValue, ref maxValue, ref minValue, ref maxLength, ref minLength);

                    result.Append(sbNav.ToString());

                    #region 默认值设置
                    if (getType.GetProperty("DefaultValue") != null)
                    {
                        if (string.IsNullOrEmpty(defaultValue)) result.Append("DefaultValue:\"" + defaultValue + "\",");
                        else
                        {
                            if (typeContent.BaseType == EDataBaseType.Boolean || (typeContent.BaseType == EDataBaseType.Int16 || typeContent.BaseType == EDataBaseType.Int32 || typeContent.BaseType == EDataBaseType.Int64 || typeContent.BaseType == EDataBaseType.Decimal || typeContent.BaseType == EDataBaseType.Double || typeContent.BaseType == EDataBaseType.Byte || typeContent.BaseType == EDataBaseType.Single))
                            {
                                result.Append("DefaultValue: " + defaultValue + ",");
                            }
                            else result.Append("DefaultValue:\"" + defaultValue + "\",");
                        }
                    }
                    if (getType.GetProperty("MaxValue") != null)
                    {
                        if (string.IsNullOrEmpty(maxValue)) result.Append("MaxValue:\"" + maxValue + "\",");
                        else
                        {
                            if (typeContent.BaseType == EDataBaseType.Int16 || typeContent.BaseType == EDataBaseType.Int32 || typeContent.BaseType == EDataBaseType.Int64 || typeContent.BaseType == EDataBaseType.Decimal || typeContent.BaseType == EDataBaseType.Double || typeContent.BaseType == EDataBaseType.Byte || typeContent.BaseType == EDataBaseType.Single)
                            {
                                result.Append("MaxValue: " + maxValue + ",");
                            }
                            else result.Append("MaxValue:\"" + maxValue + "\",");
                        }
                    }
                    if (getType.GetProperty("MinValue") != null)
                    {
                        if (string.IsNullOrEmpty(minValue)) result.Append("MinValue:\"" + minValue + "\",");
                        else
                        {
                            if (typeContent.BaseType == EDataBaseType.Int16 || typeContent.BaseType == EDataBaseType.Int32 || typeContent.BaseType == EDataBaseType.Int64 || typeContent.BaseType == EDataBaseType.Decimal || typeContent.BaseType == EDataBaseType.Double || typeContent.BaseType == EDataBaseType.Byte || typeContent.BaseType == EDataBaseType.Single)
                            {
                                result.Append("MinValue: " + minValue + ",");
                            }
                            else result.Append("MinValue:\"" + minValue + "\",");
                        }
                    }
                    if (getType.GetProperty("MaxLength") != null) result.Append("MaxLength:\"" + maxLength + "\",");
                    if (getType.GetProperty("MinLength") != null) result.Append("MinLength:\"" + minLength + "\",");

                    if (getType.GetProperty("Precision") != null)
                    {
                        var decimalData = typeContent as DecimalType;
                        if (decimalData != null)
                        {
                            var Precision = decimalData.Precision;
                            if (Precision > 0) result.Append("Precision: " + Precision + ","); ;
                        }
                    }
                    if (getType.GetProperty("Scale") != null)
                    {
                        var decimalData = typeContent as DecimalType;
                        if (decimalData != null)
                        {
                            var Scale = decimalData.Scale;
                            if (Scale > 0) result.Append("Scale: " + Scale + ","); ;
                        }
                    }


                    if (!string.IsNullOrEmpty(Body))
                    {
                        result.Append("Type:\"function\",");
                        result.Append("Action: function(e) {");
                        result.Append(Body);
                        result.Append("}");
                    }
                    else
                    {
                        result.Append("Type:\"" + BuildCommonMethod.GetTypeName(typeContent.BaseType) + "\",");
                        result.Append("DataType:\"" + typeContent.BaseType.ToString() + "\"");
                    }
                    #endregion
                }
                #endregion

                //
                string relationStr = BuildRelationObj(compile, parent, mb, isDataMember, AllForeigns);
                if (!string.IsNullOrEmpty(relationStr)) result.Append(relationStr);

                result.Append("},");
            }
            return result.ToString();
        }

        private static string BuildRelationObj(CompileBase compile, MemberBase parent, MemberBase mb, bool isDataMember, string AllForeigns)
        {
            StringBuilder result = new StringBuilder();
            dynamic member = null;
            if (mb.MemberType == EMemberType.CommonMember) member = mb as Model.Core.Definitions.Screens.Members.CommonMember;
            else if (mb.MemberType == EMemberType.DataMember) member = mb as DataMember;

            if (member != null && member.Children.Length > 0)
            {
                result.Append(",_RelationObj: {");
                StringBuilder sbEntityContent = new StringBuilder();
                StringBuilder sbAllForeigns = new StringBuilder();
                GetDataSetAndObjectBase(compile, member.Children[0], sbEntityContent, sbAllForeigns);

                StringBuilder relationStr = new StringBuilder();
                StringBuilder sbFields = new StringBuilder();
                StringBuilder sbExpand = new StringBuilder();
                StringBuilder sbSort = new StringBuilder();
                StringBuilder sbFilter = new StringBuilder();
                StringBuilder sbPage = new StringBuilder();
                StringBuilder sbExpandRef = new StringBuilder();
                #region EntityContent
                if (!string.IsNullOrEmpty(sbEntityContent.ToString()))
                {
                    relationStr.Append(sbEntityContent.ToString() + ",");
                }
                #endregion
                #region Field
                foreach (var itemChild in member.Children[0].Children)
                {
                    if (itemChild.MemberType == EMemberType.NavigateMember)
                    {
                        GetExpandContent(itemChild, sbExpandRef, "");
                    }
                    sbFields.Append(GetDataMemberContent(compile, parent, itemChild, AllForeigns));
                }
                #endregion
                #region Query
                #region Expand
                string _sbExpand = sbExpandRef.ToString();
                if (!string.IsNullOrEmpty(_sbExpand))
                {
                    string expandStr = _sbExpand.Length > 0 ? _sbExpand.Substring(0, _sbExpand.Length - 1) : "";
                    sbExpand.Append("Expand:\"" + expandStr + "\",");
                }
                #endregion
                #region Page
                string pageContent = GetPageContent(member.Children[0]);
                string _sbPage = pageContent.ToString().Length > 0 ? pageContent.ToString() : "";
                if (!string.IsNullOrEmpty(_sbPage))
                {
                    sbPage.Append("Page:{");
                    sbPage.Append(_sbPage.ToString().Length > 0 ? _sbPage.ToString().Substring(0, _sbPage.ToString().Length - 1) : "");
                    sbPage.Append("}" + ",");
                }
                #endregion
                string _sbQuery = sbExpand.ToString() + sbSort.ToString() + sbFilter.ToString() + sbPage.ToString();
                if (!string.IsNullOrEmpty(_sbQuery))
                {
                    relationStr.Append("_Query:{" + _sbQuery.Substring(0, _sbQuery.Length - 1) + "},");
                }
                #endregion
                #region Fields
                string _sbFields = sbFields.ToString().Length > 0 ? sbFields.ToString().Substring(0, sbFields.ToString().Length - 1) : "";
                if (!string.IsNullOrEmpty(_sbFields))
                {
                    relationStr.Append("_Fields: {");
                    relationStr.Append(_sbFields);
                    relationStr.Append("},");
                }
                #endregion
                if (!string.IsNullOrEmpty(relationStr.ToString()))
                {
                    result.Append(relationStr.ToString().Substring(0, relationStr.ToString().Length - 1));
                }
                result.Append("}");
            }
            return result.ToString();
        }

        #region 生成主键（Html生成）
        /// <summary>
        /// 生成主键内容
        /// </summary>
        /// <param name="type"></param>
        /// <param name="mb"></param>
        /// <param name="itemPropertys"></param>
        /// <param name="def"></param>
        /// <param name="compile"></param>
        /// <param name="jsonWriter"></param>
        private static void BuildPrimaryContent(MemberBase mb, Dictionary<string, object> itemPropertys, EntityDefinition def, CompileBase compile, StringBuilder jsonWriter)
        {
            var inheritEntityId = (from t in itemPropertys where t.Key == "InheritEntityId" select t.Value).FirstOrDefault();
            if (inheritEntityId != null)
            {
                var entityItems = compile.ProjectItems.Where(a => a.Value.DocumentType == GlobalIds.DocumentType.Entity && a.Key == int.Parse(inheritEntityId.ToString())).ToList();
                foreach (var item in entityItems)
                {
                    var defItem = compile.GetDocumentBody(item.Value) as EntityDefinition;
                    BuildPrimaryFields(mb, defItem, compile, jsonWriter);

                    //递归处理
                    BuildPrimaryContent(mb, item.Value.Propertys, def, compile, jsonWriter);
                }
            }
            else
            {
                BuildPrimaryFields(mb, def, compile, jsonWriter);
            }
        }
        /// <summary>
        /// 生成主键字段(实体主键)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="mb"></param>
        /// <param name="def"></param>
        /// <param name="dojoCompile"></param>
        /// <param name="jsonWriter"></param>
        private static void BuildPrimaryFields(MemberBase mb, EntityDefinition def, CompileBase dojoCompile, StringBuilder jsonWriter)
        {
            if (def.Members.OfType<Model.Core.Definitions.Entities.Members.PrimaryMember>().Count() > 0)
            {
                foreach (var primary in def.Members.OfType<Model.Core.Definitions.Entities.Members.PrimaryMember>())
                {
                    jsonWriter.AppendFormat("{0},", primary.Name);
                }
            }
            string primaryStr = BuildPrimaryFieldsContent(mb);
            if (!string.IsNullOrEmpty(primaryStr)) jsonWriter.AppendFormat("{0},", primaryStr.Substring(0, primaryStr.Length - 1));
        }
        /// <summary>
        /// 生成主键字段(自定义主键)
        /// </summary>
        /// <param name="mb"></param>
        /// <returns></returns>
        private static string BuildPrimaryFieldsContent(MemberBase mb)
        {
            StringBuilder sbResult = new StringBuilder();
            var item = (from t in mb.Children
                        where t.MemberType == EMemberType.SelectedItem || t.MemberType == EMemberType.CurrentItem
                        select t.Children).FirstOrDefault();
            if (item == null) return string.Empty;
            foreach (var it in item)
            {
                if (it.MemberType == EMemberType.DataMember)
                {
                    DataMember dataMember = it as DataMember;
                    if (dataMember != null)
                    {
                        if (dataMember.IsKey) sbResult.AppendFormat("{0},", dataMember.Name);
                    }
                }
            }
            return sbResult.ToString();
        }
        #endregion

        #region 生成服务端刷新和单向关系属性
        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemPropertys"></param>
        /// <param name="def"></param>
        /// <param name="compile"></param>
        /// <param name="fieldName"></param>
        /// <param name="sb"></param>
        private static void BuildServerRefreshItems(Dictionary<string, object> itemPropertys, EntityDefinition def, CompileBase compile, string fieldName, StringBuilder sb)
        {
            var inheritEntityId = (from t in itemPropertys where t.Key == "InheritEntityId" select t.Value).FirstOrDefault();
            if (inheritEntityId != null)
            {
                var entityItems = compile.ProjectItems.Where(a => a.Value.DocumentType == GlobalIds.DocumentType.Entity && a.Key == int.Parse(inheritEntityId.ToString())).FirstOrDefault();
                if (entityItems.Value != null)
                {
                    var defItem = compile.GetDocumentBody(entityItems.Value) as EntityDefinition;
                    BuildServerRefreshFileds(defItem, compile, fieldName, sb);

                    //递归处理
                    BuildServerRefreshItems(entityItems.Value.Propertys, def, compile, fieldName, sb);
                }
            }
            else
            {
                BuildServerRefreshFileds(def, compile, fieldName, sb);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="def"></param>
        /// <param name="compile"></param>
        /// <param name="fieldName"></param>
        /// <param name="sb"></param>
        private static void BuildServerRefreshFileds(EntityDefinition def, CompileBase compile, string fieldName, StringBuilder sb)
        {
            foreach (var member in def.Members)
            {
                if (member.Name == fieldName)
                {
                    if (member.MemberType == Wilmar.Model.Core.Definitions.Entities.EMemberType.Column)
                    {
                        bool isServerRefresh = false;
                        if ((member as ColumnMember).GenerateMode != Model.Core.Definitions.Entities.EColumnGenerateMode.None)
                        {
                            isServerRefresh = true;
                        }
                        sb.Append("ServerRefresh:" + isServerRefresh.ToString().ToLower() + ",");
                        break;
                    }
                    else if (member.MemberType == Model.Core.Definitions.Entities.EMemberType.Navigation)
                    {
                        bool isSingleRelation = false;
                        var navigationMember = member as NavigationMember;
                        string toName = navigationMember.ToName;
                        string toEntity = (from t in compile.ProjectItems where t.Key == navigationMember.ToEntityId select t.Value).FirstOrDefault().Name;
                        if (string.IsNullOrEmpty(navigationMember.ToName)) { isSingleRelation = true; }
                        sb.Append("ToName:\"" + toName + "\",");
                        sb.Append("ToEntity:\"" + toEntity + "\",");
                        sb.Append("SingleRelation:" + isSingleRelation.ToString().ToLower() + ",");
                        if (navigationMember.ToCardinality != Model.Core.Definitions.Entities.EMappingCardinality.Many)
                        {
                            #region RelationForeignKey不生成
                            //string navPrimaryKey = BuildCommonMethod.GetNavPrimaryContent(def, compile, toEntity);
                            //string relationForeignKey = BuildCommonMethod.GetRealForeignKey(navigationMember, toEntity, navPrimaryKey);
                            //sb.Append("RelationForeignKey:[\"" + relationForeignKey + "\"],");
                            #endregion
                        }
                        break;
                    }
                }
            }
        }
        #endregion
        #endregion
    }
}
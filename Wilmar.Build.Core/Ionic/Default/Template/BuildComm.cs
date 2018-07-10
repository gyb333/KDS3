using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wilmar.Build.Core.Dojo.Default.Builders;
using Wilmar.Compile.Core.Ionic;
using Wilmar.Foundation.Common;
using Wilmar.Foundation.Projects;
using Wilmar.Model.Core.Definitions;
using Wilmar.Model.Core.Definitions.Entities.DataTypes;
using Wilmar.Model.Core.Definitions.Querites;
using Wilmar.Model.Core.Definitions.Screens;
using Wilmar.Model.Core.Definitions.Screens.Members;

namespace Wilmar.Build.Core.Ionic.Default.Template
{
    public partial class BuildComm
    {
        #region 属性
        /// <summary>
        /// 生成属性
        /// </summary>
        public static Dictionary<string, MetaDataProperty> RegisterPropertys(IonicCompile ionicCompile, ScreenDefinition screenDef, ProjectDocument doc)
        {
            Dictionary<string, MetaDataProperty> PropertyData = new Dictionary<string, MetaDataProperty>();
            var items = screenDef.Children.Where(a => a.MemberType == EMemberType.Property).ToList();
            foreach (var item in items)
            {
                MetaDataProperty propertyMember = GetPropertyContent(item);
                PropertyData.Add(item.Name, propertyMember);
            }
            return PropertyData;
        }
        private static MetaDataProperty GetPropertyContent(MemberBase mb)
        {
            var item = mb as Property;
            var typeContent = item.Content as CommonDataType;
            string defaultValue = string.Empty, maxValue = string.Empty, minValue = string.Empty, maxLength = string.Empty, minLength = string.Empty;
            BuildCommonMethod.GetDataTypeValue(typeContent, ref defaultValue, ref maxValue, ref minValue, ref maxLength, ref minLength);
            var propertyMember = new MetaDataProperty()
            {
                Name = item.Name,
                Title = item.Title,
                IsRequired = item.IsRequired,
                IsCollection = item.IsCollection,
                DefaultValue = defaultValue,
                MaxValue = maxValue,
                MinValue = minValue,
                MaxLength = maxLength,
                MinLength = minLength,
                DataType = BuildCommonMethod.GetTypeNameByIon(typeContent.BaseType)
            };
            return propertyMember;
        }
        private static MetaDataProperty GetDataMemberContent(MemberBase mb)
        {
            var item = mb as DataMember;
            var typeContent = item.Content as CommonDataType;
            string defaultValue = string.Empty, maxValue = string.Empty, minValue = string.Empty, maxLength = string.Empty, minLength = string.Empty;
            BuildCommonMethod.GetDataTypeValue(typeContent, ref defaultValue, ref maxValue, ref minValue, ref maxLength, ref minLength);
            var propertyMember = new MetaDataProperty()
            {
                Name = item.Name,
                Title = item.Title,
                IsRequired = item.IsRequired,
                IsCollection = false,
                DefaultValue = defaultValue,
                MaxValue = maxValue,
                MinValue = minValue,
                MaxLength = maxLength,
                MinLength = minLength,
                DataType = BuildCommonMethod.GetTypeNameByIon(typeContent.BaseType)
            };
            return propertyMember;
        }
        #endregion

        #region 方法
        /// <summary>
        /// 生成方法
        /// </summary>
        public static Dictionary<string, MetaDataMethod> RegisterMethods(IonicCompile ionicCompile, ScreenDefinition screenDef, ProjectDocument doc)
        {
            Dictionary<string, MetaDataMethod> MethodData = new Dictionary<string, MetaDataMethod>();
            var items = screenDef.Children.Where(a => a.MemberType == EMemberType.Method).ToList();
            foreach (var item in items)
            {
                MetaDataMethod methodMember = GetMethodContent(item);
                MethodData.Add(item.Name, methodMember);
            }
            return MethodData;
        }
        private static MetaDataMethod GetMethodContent(MemberBase mb)
        {
            var item = mb as Method;
            string parms = string.Empty, parmsBind = string.Empty;
            #region 
            StringBuilder methodArg = new StringBuilder();
            StringBuilder methodArgBind = new StringBuilder();
            StringBuilder argBind = new StringBuilder();
            foreach (var child in item.Children)
            {
                var para = child as Parameter;
                //var DataType = BuildCommonMethod.GetTypeNameByIon(para.DataType.Value);
                var DataType = "any";
                if (child.MemberType == EMemberType.Parameter)
                {
                    methodArg.Append(para.Name + ":" + DataType + ",");

                    string bindProp = para.Path == null ? "" : para.Path.Replace("CurrentItem", "SelectedItem");
                    argBind.Append("{ name:\"" + para.Name + "\"," + "bindProperty:\"" + bindProp + "\" },");
                }
            }
            if (!string.IsNullOrEmpty(argBind.ToString()))
            {
                methodArgBind.Append(argBind.ToString().Substring(0, argBind.ToString().Length - 1));
                parmsBind = methodArgBind.ToString();
            }
            if (!string.IsNullOrEmpty(methodArg.ToString()))
            {
                parms = methodArg.ToString().Substring(0, methodArg.Length - 1);
            }
            #endregion
            var MethodMember = new MetaDataMethod
            {
                Name = item.Name,
                Title = item.Title,
                Body = item.Body == null ? "" : item.Body,
                Params = parms,
                ParamsBind = parmsBind
            };
            return MethodMember;
        }
        #endregion

        #region 事件
        /// <summary>
        /// 事件生成
        /// </summary>
        /// <param name="ionicCompile"></param>
        /// <param name="screenDef"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static Dictionary<string, MetaDataMethod> RegisterEvents(IonicCompile ionicCompile, ScreenDefinition screenDef, ProjectDocument doc)
        {
            Dictionary<string, MetaDataMethod> EventData = new Dictionary<string, MetaDataMethod>();
            var items = screenDef.Children.Where(a => a.MemberType == EMemberType.Events).ToList();
            foreach (var item in items)
            {
                foreach (var child in item.Children)
                {
                    MetaDataMethod eventMember = GetEventContent(child);
                    if (eventMember != null) EventData.Add(child.Name, eventMember);
                }
            }
            return EventData;
        }
        private static MetaDataMethod GetEventContent(MemberBase mb)
        {
            var item = mb as Event;
            #region 
            string methodName = item.Name;
            if (item.Name.ToLower() == "loading") methodName = "viewWillEnter";
            else if (item.Name.ToLower() == "loaded") methodName = "viewDidLoad";
            else if (item.Name.ToLower() == "closeing") methodName = "viewWillLeave";
            else if (item.Name.ToLower() == "closed") methodName = "viewDidLeave";
            else if (item.Name.ToLower() == "saveing") methodName = "onSaving";
            else if (item.Name.ToLower() == "saved") methodName = "onSaved";
            else if (item.Name.ToLower() == "datachange") methodName = "onDataChanged";
            else if (item.Name.ToLower() == "loading") methodName = "onDataLoading";
            else if (item.Name.ToLower() == "loaded") methodName = "onDataLoaded";
            string body = item.Body == null ? "" : item.Body;
            if (!string.IsNullOrEmpty(body))
            {
                StringBuilder methodBody = new StringBuilder();
                methodBody.AppendLine("super." + methodName + "();");
                methodBody.AppendLine(body);
                var EventMember = new MetaDataMethod
                {
                    Name = methodName,
                    Title = item.Title,
                    Body = methodBody.ToString(),
                    Params = "",
                    ParamsBind = ""
                };
                return EventMember;
            }
            #endregion
            return null;
        }
        #endregion

        #region 屏幕参数
        /// <summary>
        /// 生成屏幕参数
        /// </summary>
        /// <param name="ionicCompile"></param>
        /// <param name="screenDef"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static Dictionary<string, MetaDataScreenParam> RegisterScreenParams(IonicCompile ionicCompile, ScreenDefinition screenDef, ProjectDocument doc)
        {
            Dictionary<string, MetaDataScreenParam> ScreenParamData = new Dictionary<string, MetaDataScreenParam>();
            var items = screenDef.Children.Where(a => a.MemberType == EMemberType.ScreenParameters).ToList();
            StringBuilder screenParams = new StringBuilder();
            foreach (var item in items)
            {
                string parasStr = GetParameterContent(item);
                if (!string.IsNullOrEmpty(parasStr)) parasStr = parasStr.Substring(0, parasStr.Length - 3);
                screenParams.Append(parasStr);
            }
            var ScreenParamMember = new MetaDataScreenParam
            {
                Content = screenParams.ToString()
            };
            ScreenParamData.Add(doc.Name, ScreenParamMember);
            return ScreenParamData;
        }
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
        private static string GetParameterString(MemberBase mb)
        {
            StringBuilder result = new StringBuilder();
            if (mb.MemberType == EMemberType.Parameter)
            {
                var para = mb as Parameter;
                string bindPath = para.Path == null ? "" : para.Path.Replace("CurrentItem", "SelectedItem");
                string paraType = "InOut";
                if (para.ParameterType == Model.Core.Definitions.Screens.Members.EParameterType.InArgument) paraType = "In";
                else if (para.ParameterType == Model.Core.Definitions.Screens.Members.EParameterType.OutArgument) paraType = "Out";
                result.AppendLine("" + para.Name + ": { name:\"" + para.Name + "\",type:ParameterType." + paraType + ",bindProperty:\"" + bindPath + "\" },");
            }
            return result.ToString();
        }
        #endregion

        #region 数据集
        /// <summary>
        /// 生成数据集
        /// </summary>
        /// <param name="ionicCompile"></param>
        /// <param name="screenDef"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static Dictionary<string, MetaDataDataSet> RegisterDataSets(IonicCompile ionicCompile, ScreenDefinition screenDef, ProjectDocument doc)
        {
            Dictionary<string, MetaDataDataSet> DataSetData = new Dictionary<string, MetaDataDataSet>();
            var items = screenDef.Children.Where(a => a.MemberType == EMemberType.DataSet || a.MemberType == EMemberType.Objects).ToList();
            foreach (var item in items)
            {
                if (item.MemberType == EMemberType.DataSet)
                {
                    #region 远程数据集(有实体)
                    DataSet dats = item as DataSet;
                    var entityItem = ionicCompile.ProjectItems.Where(a => a.Value.DocumentType == GlobalIds.DocumentType.Entity && a.Key == dats.EntityId).FirstOrDefault();
                    if (entityItem.Value != null)
                    {
                        MetaDataDataSet metaDataSet = BuildMetaDataDataSet(entityItem.Value.Name, item);
                        if (metaDataSet != null) DataSetData.Add(item.Name, metaDataSet);
                    }
                    #endregion
                }
                else
                {
                    Objects objs = item as Objects;
                    var entityItem = ionicCompile.ProjectItems.Where(a => a.Value.DocumentType == GlobalIds.DocumentType.Entity && a.Key == objs.EntityId).FirstOrDefault();
                    if (entityItem.Value != null)
                    {
                        #region 本地对象(有实体)
                        MetaDataDataSet metaDataSet = BuildMetaDataDataSet(entityItem.Value.Name, item);
                        if (metaDataSet != null) DataSetData.Add(item.Name, metaDataSet);
                        #endregion
                    }
                    else
                    {
                        #region 本地对象(无实体)
                        MetaDataDataSet metaDataSet = BuildMetaDataDataSet("", item);
                        if (metaDataSet != null) DataSetData.Add(item.Name, metaDataSet);
                        #endregion
                    }
                }
            }
            return DataSetData;
        }
        private static MetaDataDataSet BuildMetaDataDataSet(string entityName, MemberBase memberBase)
        {
            string optionStr = string.Empty, dataSetType = string.Empty;
            StringBuilder options = new StringBuilder();

            dynamic dats = memberBase as DataSet;
            if (memberBase.MemberType == EMemberType.DataSet)
            {
                dats = memberBase as DataSet;
                if (dats.EntityId > 0)
                {
                    if (dats.IsCollection) dataSetType = "RemoteDataSet";
                    else dataSetType = "RemoteDataObject";
                }
                else
                {
                    if (dats.IsCollection) dataSetType = "LocalDataSet";
                    else dataSetType = "LocalDataObject";
                }
                options.AppendLine("refreshOnLoaded:true,");
            }
            else
            {
                dats = memberBase as Objects;
                if (dats.EntityId > 0)
                {
                    if (dats.IsCollection) dataSetType = "RemoteDataSet";
                    else dataSetType = "RemoteDataObject";
                }
                else
                {
                    if (dats.IsCollection) dataSetType = "LocalDataSet";
                    else dataSetType = "LocalDataObject";
                }
            }

            #region 过滤条件
            string filters = GetFilterContent(dats);
            if (!string.IsNullOrEmpty(filters)) options.AppendLine($"where:{filters},");
            #endregion
            #region 数据集中的属性，方法，事件，选中项中的数据成员/方法/展开成员
            StringBuilder expand = new StringBuilder();
            List<MetaDataProperty> listProperty = new List<MetaDataProperty>();
            List<MetaDataMethod> listMethod = new List<MetaDataMethod>();
            List<MetaDataMethod> listEvent = new List<MetaDataMethod>();
            List<MetaDataProperty> listSelectedItemMember = new List<MetaDataProperty>();
            List<MetaDataMethod> listSelectedItemMethod = new List<MetaDataMethod>();
            foreach (var child in dats.Children)
            {
                if (child.MemberType == EMemberType.Property)
                {
                    MetaDataProperty propertyMember = GetPropertyContent(child);
                    listProperty.Add(propertyMember);
                }
                else if (child.MemberType == EMemberType.Method)
                {
                    MetaDataMethod methodMember = GetMethodContent(child);
                    listMethod.Add(methodMember);
                }
                else if (child.MemberType == EMemberType.Events)
                {
                    foreach (var evt in child.Children)
                    {
                        MetaDataMethod eventMember = GetEventContent(evt);
                        if (eventMember != null) listEvent.Add(eventMember);
                    }
                }
                else if (child.MemberType == EMemberType.SelectedItem)
                {
                    foreach (var item in child.Children)
                    {
                        if (item.MemberType == EMemberType.DataMember)
                        {
                            MetaDataProperty propertyMember = GetDataMemberContent(item);
                            listSelectedItemMember.Add(propertyMember);
                        }
                        else if (item.MemberType == EMemberType.Method)
                        {
                            MetaDataMethod methodMember = GetMethodContent(item);
                            listSelectedItemMethod.Add(methodMember);
                        }
                        else if (item.MemberType == EMemberType.NavigateMember)
                        {
                            GetExpandContent(item, expand, "");
                        }
                    }
                }

                else if (child.MemberType == EMemberType.QueryParameters)
                {
                    StringBuilder queryParas = new StringBuilder();
                    string parasStr = GetParameterContent(child);
                    if (!string.IsNullOrEmpty(parasStr)) parasStr = parasStr.Substring(0, parasStr.Length - 3);
                    queryParas.AppendLine("parameters:{");
                    queryParas.AppendLine(parasStr);
                    queryParas.AppendLine("},");
                    options.Append(queryParas);
                }
            }
            if (!string.IsNullOrEmpty(expand.ToString()))
            {
                string expandStr = expand.ToString().Substring(0, expand.ToString().Length - 1);
                options.AppendLine($"orderBy:[{expandStr.ToString()}],");
            }
            #endregion
            #region 排序
            StringBuilder orderByStr = new StringBuilder();
            foreach (var sort in dats.Sorts)
            {
                string sortStr = string.Format("{0} {1}", sort.Name, sort.Direction == 0 ? "asc" : "desc");
                if (orderByStr.ToString().Length == 0) orderByStr.Append("\"" + sortStr + "\"");
                else orderByStr.Append(",\"" + sortStr + "\"");
            }
            if (!string.IsNullOrEmpty(orderByStr.ToString())) options.AppendLine($"expand:[{orderByStr.ToString()}],");
            #endregion
            if (!string.IsNullOrEmpty(options.ToString())) optionStr = options.ToString().Substring(0, options.ToString().Length - 3);
            var DataSetMember = new MetaDataDataSet
            {
                Type = dataSetType,
                Name = dats.Name,
                Title = dats.Title,
                EntityName = entityName,
                Options = optionStr,
                DataPropertys = listProperty,
                DataMethods = listMethod,
                DataEvents = listEvent,
                SelectedItemMembers = listSelectedItemMember,
                SelectedItemMethods = listSelectedItemMethod
            };
            return DataSetMember;
        }
        #region Expand
        private static void GetExpandContent(MemberBase mb, StringBuilder expandStr, string parentName)
        {
            NavigateMember nm = mb as NavigateMember;
            if (nm != null)
            {
                if (nm.IsExpand)
                {
                    string expandName = nm.Name;
                    if (!string.IsNullOrEmpty(parentName)) expandName = parentName + "($expand=" + nm.Name + ")";
                    expandStr.AppendFormat("\"{0}\",", expandName);
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
        #endregion
        #region Filter
        private static string GetFilterContent(MemberBase memberBase)
        {
            StringBuilder result = new StringBuilder();
            switch (memberBase.MemberType)
            {
                case EMemberType.DataSet:
                    DataSet dats = memberBase as DataSet;
                    if (dats.IsCollection)
                    {
                        #region 集合
                        FilterItemBase parentItemBase = null;
                        for (var i = 0; i < dats.Filters.Count; i++)
                        {
                            var item = dats.Filters[i];
                            if (i == 0) BuildFilterDataSet(ref result, item, null);
                            else BuildFilterDataSet(ref result, item, parentItemBase);
                            parentItemBase = item;
                        }
                        #endregion
                    }
                    else
                    {
                        #region 对象
                        var QueryParametersObj = (from t in dats.Children.OfType<QueryParameters>() select t).FirstOrDefault();
                        if (QueryParametersObj != null)
                        {
                            result.Append(GetFilterObjectItem(QueryParametersObj));
                        }
                        #endregion
                    }
                    break;
                case EMemberType.Objects:
                    Objects objs = memberBase as Objects;
                    if (objs.IsCollection)
                    {
                        #region 集合
                        FilterItemBase parentItemBase = null;
                        for (var i = 0; i < objs.Filters.Count; i++)
                        {
                            var item = objs.Filters[i];
                            if (i == 0) BuildFilterDataSet(ref result, item, null);
                            else BuildFilterDataSet(ref result, item, parentItemBase);
                            parentItemBase = item;
                        }
                        #endregion
                    }
                    else
                    {
                        #region 对象
                        var QueryParametersObj = (from t in objs.Children.OfType<QueryParameters>() select t).FirstOrDefault();
                        if (QueryParametersObj != null)
                        {
                            result.Append(GetFilterObjectItem(QueryParametersObj));
                        }
                        #endregion
                    }
                    break;
            }
            return result.ToString();
        }
        private static void BuildFilterDataSet(ref StringBuilder result, FilterItemBase filterItemBase, FilterItemBase preItemBase)
        {
            var tempResult = new StringBuilder();
            var curFilterStr = BuildFilterDataSetContent(filterItemBase);
            if (preItemBase != null)
            {
                var preFilterStr = result.ToString();
                dynamic filterObj = filterItemBase as FilterStandard;
                if (filterItemBase.FilterType == EFilterType.Standard) filterObj = filterItemBase as FilterStandard;
                else if (filterItemBase.FilterType == EFilterType.MultiValue) filterObj = filterItemBase as FilterMultiValue;
                else if (filterItemBase.FilterType == EFilterType.Group) filterObj = filterItemBase as FilterGroup;
                tempResult.Append("{");
                tempResult.Append($"{filterObj.Connection.ToString().ToLower()}: [{curFilterStr},{preFilterStr}]");
                tempResult.Append("}");
            }
            else tempResult.Append(curFilterStr);
            result = tempResult;
        }
        private static string BuildFilterDataSetContent(FilterItemBase filterBase)
        {
            StringBuilder result = new StringBuilder();
            switch (filterBase.FilterType)
            {
                case EFilterType.Standard:
                    var standard = filterBase as FilterStandard;
                    string staResult = BuildFilterStringItem(EFilterType.Standard, standard.Name, standard.Connection, standard.Function, standard.Operator, standard.ValueType.ToString().ToLower(), standard.Value, "", "", "", "");
                    result.Append(staResult);
                    break;
                case EFilterType.MultiValue:
                    var multiValue = filterBase as FilterMultiValue;
                    string mulResult = BuildFilterStringItem(EFilterType.MultiValue, multiValue.Name, multiValue.Connection, multiValue.Function, multiValue.Operator, "", "", multiValue.Value1Type.ToString().ToLower(), multiValue.Value1, multiValue.Value2Type.ToString().ToLower(), multiValue.Value2);
                    result.Append(mulResult);
                    break;
                case EFilterType.Group:
                    var filter = filterBase as FilterGroup;
                    foreach (var child in filter.Filters)
                    {
                        string grpResult = BuildFilterDataSetContent(child);
                        result.Append(grpResult);
                    }
                    break;
            }
            return result.ToString();
        }
        private static string BuildFilterStringItem(EFilterType FilterItemType, string Name, EConnection Connection, EFunction Function, EOperator Operator, string ValueType, string Value, string Value1Type, string Value1, string Value2Type, string Value2)
        {
            StringBuilder current = new StringBuilder();
            current.Append("{");
            current.Append("" + Name + ":{");
            current.Append($"{ Operator.ToString().ToLower()}" + ":{");
            if (FilterItemType == EFilterType.MultiValue)
            {
                current.AppendFormat("Value1Type:\"{0}\",", Value1Type);
                current.AppendFormat("Value1:\"{0}\",", Value1);
                current.AppendFormat("Value2Type:\"{0}\",", Value2Type);
                current.AppendFormat("Value2:\"{0}\"", Value2);
            }
            else
            {
                current.AppendFormat("ValueType:\"{0}\",", ValueType);
                current.AppendFormat("Value:\"{0}\"", Value);
            }
            current.Append("}");
            current.Append("}");
            current.Append("}");
            return current.ToString();
        }
        private static string GetFilterObjectItem(QueryParameters queryParameters)
        {
            StringBuilder result = new StringBuilder();
            foreach (var child in queryParameters.Children)
            {
                var parameters = child as Parameter;
                StringBuilder filterStr = new StringBuilder();
                filterStr.Append("{");
                filterStr.Append($"{parameters.Name}:");
                filterStr.Append("{equal:{");
                filterStr.Append("ValueType:\"parameter\",");
                filterStr.Append($"Value:\"{parameters.Name}\"");
                filterStr.Append("}");
                filterStr.Append("}");
                filterStr.Append("}");
                result.Append(filterStr.ToString());
            }
            return result.ToString();
        }
        #endregion
        #endregion
    }
}

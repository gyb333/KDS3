using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wilmar.Foundation;
using Wilmar.Foundation.Common;
using Wilmar.Model.Core.Definitions;
using Wilmar.Model.Core.Definitions.Entities;
using Wilmar.Model.Core.Definitions.Entities.DataTypes;
using Wilmar.Model.Core.Definitions.Entities.Members;
using Wilmar.Service.Common.Generate;

namespace Wilmar.Build.Core.Dojo.Default.Builders
{
    public static class BuildCommonMethod
    {
        #region 获取前端支持的数据类型
        /// <summary>
        /// 获取前端支持的数据类型
        /// </summary>
        /// <param name="baseType">数据基础类型</param>
        /// <returns></returns>
        public static string GetTypeName(EDataBaseType baseType)
        {
            switch (baseType)
            {
                case EDataBaseType.Int16:
                case EDataBaseType.Int32:
                case EDataBaseType.Int64:
                case EDataBaseType.Byte:
                case EDataBaseType.Single:
                case EDataBaseType.Double:
                case EDataBaseType.Decimal:
                    return "number";
                case EDataBaseType.Boolean:
                    return "boolean";
                case EDataBaseType.String:
                case EDataBaseType.Guid:
                case EDataBaseType.Timestamp:
                    return "string";
                case EDataBaseType.TimeSpan:
                case EDataBaseType.DateTime:
                case EDataBaseType.DateTimeOffset:
                    return "date";
                case EDataBaseType.Binary:
                    return "array";
            }
            return "";
        }
        public static string GetTypeNameByIon(EDataBaseType baseType)
        {
            switch (baseType)
            {
                case EDataBaseType.Int16:
                case EDataBaseType.Int32:
                case EDataBaseType.Int64:
                case EDataBaseType.Byte:
                case EDataBaseType.Single:
                case EDataBaseType.Double:
                case EDataBaseType.Decimal:
                    return "number";
                case EDataBaseType.Boolean:
                    return "boolean";
                case EDataBaseType.String:
                case EDataBaseType.Guid:
                case EDataBaseType.Timestamp:
                    return "string";
                case EDataBaseType.TimeSpan:
                case EDataBaseType.DateTime:
                case EDataBaseType.DateTimeOffset:
                case EDataBaseType.Binary:
                    return "any";
            }
            return "";
        }
        #endregion

        #region 获取数据类型返回的值
        /// <summary>
        /// 获取数据类型返回的值
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="defaultValue"></param>
        /// <param name="maxValue"></param>
        /// <param name="minValue"></param>
        /// <param name="maxLength"></param>
        /// <param name="minLength"></param>
        public static void GetDataTypeValue(CommonDataType dataType, ref string defaultValue, ref string maxValue, ref string minValue, ref string maxLength, ref string minLength)
        {
            switch (dataType.BaseType)
            {
                case EDataBaseType.Int16:
                    var Int16 = dataType as Int16Type;
                    defaultValue = Int16.DefaultValue;
                    maxValue = Int16.MaxValue.ToString();
                    minValue = Int16.MinValue.ToString();
                    break;
                case EDataBaseType.Int32:
                    var Int32 = dataType as Int32Type;
                    defaultValue = Int32.DefaultValue;
                    maxValue = Int32.MaxValue.ToString();
                    minValue = Int32.MinValue.ToString();
                    break;
                case EDataBaseType.Int64:
                    var Int64 = dataType as Int64Type;
                    defaultValue = Int64.DefaultValue;
                    maxValue = Int64.MaxValue.ToString();
                    minValue = Int64.MinValue.ToString();
                    break;
                case EDataBaseType.Byte:
                    var Byte = dataType as ByteType;
                    defaultValue = Byte.DefaultValue;
                    maxValue = Byte.MaxValue.ToString();
                    minValue = Byte.MinValue.ToString();
                    break;
                case EDataBaseType.Single:
                    var Single = dataType as SingleType;
                    defaultValue = Single.DefaultValue;
                    maxValue = Single.MaxValue.ToString();
                    minValue = Single.MinValue.ToString();
                    break;
                case EDataBaseType.Double:
                    var Double = dataType as DoubleType;
                    defaultValue = Double.DefaultValue;
                    maxValue = Double.MaxValue.ToString();
                    minValue = Double.MinValue.ToString();
                    break;
                case EDataBaseType.Decimal:
                    var Decimal = dataType as DecimalType;
                    defaultValue = Decimal.DefaultValue;
                    maxValue = Decimal.MaxValue.ToString();
                    minValue = Decimal.MinValue.ToString();
                    break;
                case EDataBaseType.TimeSpan:
                    var TimeSpan = dataType as TimeSpanType;
                    defaultValue = TimeSpan.DefaultValue;
                    maxValue = TimeSpan.MaxValue.ToString();
                    minValue = TimeSpan.MinValue.ToString();
                    break;
                case EDataBaseType.DateTime:
                    var DateTime = dataType as DateTimeType;
                    defaultValue = DateTime.DefaultValue;
                    maxValue = DateTime.MaxValue.ToString();
                    minValue = DateTime.MinValue.ToString();
                    break;
                case EDataBaseType.DateTimeOffset:
                    var DateTimeOffset = dataType as DateTimeOffsetType;
                    defaultValue = DateTimeOffset.DefaultValue;
                    maxValue = DateTimeOffset.MaxValue.ToString();
                    minValue = DateTimeOffset.MinValue.ToString();
                    break;
                case EDataBaseType.Binary:
                    var Binary = dataType as BinaryType;
                    break;
                case EDataBaseType.Boolean:
                    var Boolean = dataType as BooleanType;
                    defaultValue = Boolean.DefaultValue;
                    break;
                case EDataBaseType.String:
                    var String = dataType as StringType;
                    defaultValue = String.DefaultValue;
                    maxLength = String.MaxLength.ToString();
                    minLength = String.MinLength.ToString();
                    break;
                case EDataBaseType.Guid:
                    var Guid = dataType as GuidType;
                    break;
                case EDataBaseType.Timestamp:
                    var Timestamp = dataType as TimestampType;
                    break;
            }
        }
        #endregion

        #region 获取所有自定义外键
        /// <summary>
        /// 获取所有自定义外键
        /// </summary>
        /// <param name="itemPropertys"></param>
        /// <param name="def"></param>
        /// <param name="compile"></param>
        /// <param name="sb"></param>
        public static void GetAllRelationForeignContent(Dictionary<string, object> itemPropertys, EntityDefinition def, CompileBase compile, StringBuilder sb)
        {
            var inheritEntityId = (from t in itemPropertys where t.Key == "InheritEntityId" select t.Value).FirstOrDefault();
            if (inheritEntityId != null)
            {
                var entityItems = compile.ProjectItems.Where(a => a.Value.DocumentType == GlobalIds.DocumentType.Entity && a.Key == int.Parse(inheritEntityId.ToString())).FirstOrDefault();
                if (entityItems.Value != null)
                {
                    var defItem = compile.GetDocumentBody(entityItems.Value) as EntityDefinition;
                    GetAllRelationForeignString(defItem, compile, sb);

                    //递归处理
                    GetAllRelationForeignContent(entityItems.Value.Propertys, def, compile, sb);
                }
            }
            else
            {
                GetAllRelationForeignString(def, compile, sb);
            }
        }
        /// <summary>
        /// 获取所有自定义外键
        /// </summary>
        /// <param name="def"></param>
        /// <param name="compile"></param>
        /// <param name="sb"></param>
        private static void GetAllRelationForeignString(EntityDefinition def, CompileBase compile, StringBuilder sb)
        {
            foreach (var member in def.Members)
            {
                if (member.MemberType == EMemberType.Navigation)
                {
                    var navigationMember = member as NavigationMember;
                    string toEntity = (from t in compile.ProjectItems where t.Key == navigationMember.ToEntityId select t.Value).FirstOrDefault().Name;
                    if (navigationMember.ToCardinality != EMappingCardinality.Many)
                    {
                        string navPrimaryKey = GetNavPrimaryContent(def, compile, toEntity);
                        string relationForeignKey = GetRealForeignKey(navigationMember, toEntity, navPrimaryKey);
                        sb.Append(relationForeignKey + ",");
                    }
                }
            }
        }
        #endregion

        #region 获取外键
        /// <summary>
        /// 获取外键
        /// </summary>
        /// <param name="member"></param>
        /// <param name="toEntity"></param>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        public static string GetRealForeignKey(NavigationMember member, string toEntity, string primaryKey)
        {
            EMappingCardinality map = member.ToCardinality;
            switch (map)
            {
                case EMappingCardinality.One:
                case EMappingCardinality.ZeroOrOne:
                    string foreignName = member.ForeignKeyName;
                    if (string.IsNullOrEmpty(foreignName))
                    {
                        foreignName = toEntity + primaryKey;
                    }
                    return foreignName;
            }
            return string.Empty;
        }
        #endregion

        #region 获取导航属性所属的主键
        /// <summary>
        /// 获取导航属性所属的主键
        /// </summary>
        /// <param name="def"></param>
        /// <param name="dojoCompile"></param>
        /// <param name="toEntity"></param>
        /// <returns></returns>
        public static string GetNavPrimaryContent(EntityDefinition def, CompileBase compile, string toEntity)
        {
            string navPrimaryKey = string.Empty;
            var navEntityItems = compile.ProjectItems.Where(a => a.Value.DocumentType == GlobalIds.DocumentType.Entity && a.Value.Name == toEntity).FirstOrDefault();
            if (navEntityItems.Value != null)
            {
                //继承实体
                var inheritEntityId = (from t in navEntityItems.Value.Propertys where t.Key == "InheritEntityId" select t.Value).FirstOrDefault();
                if (inheritEntityId != null)
                {
                    StringBuilder sbPrimary = new StringBuilder();
                    GetNavPrimaryString(def, compile, navEntityItems.Value.Propertys, sbPrimary);
                    navPrimaryKey = sbPrimary.ToString();
                }
                else
                {
                    //非继承实体
                    var nvaDef = compile.GetDocumentBody(navEntityItems.Value) as EntityDefinition;
                    foreach (var navPrimary in nvaDef.Members.OfType<PrimaryMember>())
                    {
                        navPrimaryKey = navPrimary.Name;
                    }
                }
            }
            return navPrimaryKey;
        }
        /// <summary>
        /// 获取导航属性所属的主键
        /// </summary>
        /// <param name="def"></param>
        /// <param name="dojoCompile"></param>
        /// <param name="itemPropertys"></param>
        /// <param name="sb"></param>
        private static void GetNavPrimaryString(EntityDefinition def, CompileBase compile, Dictionary<string, object> itemPropertys, StringBuilder sb)
        {
            var inheritEntityId = (from t in itemPropertys where t.Key == "InheritEntityId" select t.Value).FirstOrDefault();
            if (inheritEntityId != null)
            {
                var entityItems = compile.ProjectItems.Where(a => a.Value.DocumentType == GlobalIds.DocumentType.Entity && a.Key == int.Parse(inheritEntityId.ToString())).FirstOrDefault();
                if (entityItems.Value != null)
                {
                    var defItem = compile.GetDocumentBody(entityItems.Value) as EntityDefinition;
                    if (defItem.Members.OfType<PrimaryMember>().Count() > 0)
                    {
                        foreach (var primary in defItem.Members.OfType<PrimaryMember>())
                        {
                            sb.AppendFormat("{0}", primary.Name);
                        }
                    }
                    //递归处理
                    GetNavPrimaryString(def, compile, entityItems.Value.Propertys, sb);
                }
            }
        }
        #endregion

        #region 获取控件的父控件是否有ListBox控件
        public static bool GetIsListBox(ControlBuildBase controlBuildBase)
        {
            string controlName = controlBuildBase.ControlHost.Content.GetType().Name;
            if (controlName == "ListBox") return true;
            else
            {
                if (controlBuildBase.Parent != null) return GetIsListBox(controlBuildBase.Parent);
            }
            return false;
        }
        #endregion
        #region 获取控件的父控件是否有DropDownButton控件
        public static string GetIsDropDownButton(ControlBuildBase controlBuildBase)
        {
            string controlName = controlBuildBase.ControlHost.Content.GetType().Name;
            if (controlName == "DropDownButton") return controlBuildBase.ControlHost.Name;
            else
            {
                if (controlBuildBase.Parent != null) return GetIsDropDownButton(controlBuildBase.Parent);
            }
            return string.Empty;
        }
        #endregion
    }
}
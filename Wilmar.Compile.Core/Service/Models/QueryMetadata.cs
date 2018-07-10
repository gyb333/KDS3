using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wilmar.Foundation;
using Wilmar.Foundation.Common;
using Wilmar.Model.Core.Definitions.Entities.DataTypes;
using Wilmar.Model.Core.Definitions.Querites;

namespace Wilmar.Compile.Core.Service.Models
{
    public class QueryMetadata
    {

        public static EDataBaseType GetLeftDataType(EntityMetadata entity, string path)
        {
            string[] names = path.Split(',');

            if (names.Length > 1)
            {
                if (entity.NavigationPropertys.ContainsKey(names[0]))
                    return GetLeftDataType(entity.NavigationPropertys[names[0]].Entity, names[1]);
            }
            else
            {
                var member = entity.Definition.Members.Where(a => a.Name == names[0]).FirstOrDefault();
                if (member != null)
                {
                    var content = member.Content as CommonDataType;
                    return content.BaseType;
                }
            }
            throw UtilityException.NotSupported("查询表达式数据类型检索失败");
        }

        public static string GetRightExpression(EDataBaseType dataType, EValueType valueType, string value, List<string> parameters, bool isarray)
        {
            switch (valueType)
            {
                case EValueType.Const:
                    return DataTypeMetadata.GenerateValueExpression(dataType, value, false);
                case EValueType.Parameter:
                    parameters.Add(value);
                    return value;
                case EValueType.Property:
                    return "item." + value;
            }
            throw UtilityException.NotSupported("不支持查询值类型");
        }

        public readonly static Dictionary<EOperator, string> Operators = new Dictionary<EOperator, string>()
        {
            { EOperator.IN                  , "{1}.Contains({0)"},
            { EOperator.Equal               , "{0} == {1}"},
            { EOperator.NotEqual            , "{0} != {1}"},
            { EOperator.Greater             , "{0} > {1}"},
            { EOperator.Less                , "{0} < {1}"},
            { EOperator.GreaterThanEqual    , "{0} >= {1}"},
            { EOperator.LessThanEqual       , "{0} <= {1}"},
            { EOperator.Contains            , "{0}.Contains({1})"},
            { EOperator.NotContains         , "!{0}.Contains({1})"},
            { EOperator.IsNull              , "{0} == null"},
            { EOperator.NotIsNull           , "{0} != null"},
            { EOperator.Between             , "{0} >= {1} && {0} <= {2}"},
            { EOperator.NotBetween          , "!({0} >= {1} && {0} <= {2})"},
        };

        public readonly static Dictionary<EFunction, FunctionInfo> Functions = new Dictionary<EFunction, FunctionInfo>()
        {
            //字符串函数
            { EFunction.Contains    , new FunctionInfo(EDataBaseType.Boolean,"{0}.Contains({1})") },
            { EFunction.Indexof     , new FunctionInfo(EDataBaseType.Int32,"{0}.Indexof({1})") },
            { EFunction.Length      , new FunctionInfo(EDataBaseType.Int32,"{0}.Length") },
            { EFunction.EndsWith    , new FunctionInfo(EDataBaseType.Boolean,"{0}.EndsWith({1})") },
            { EFunction.StartsWith  , new FunctionInfo(EDataBaseType.Boolean,"{0}.EndsWith({1})") },
            { EFunction.SubString   , new FunctionInfo(EDataBaseType.String,"{0}.SubString({1})") },
            { EFunction.Trim        , new FunctionInfo(EDataBaseType.String,"{0}.Trim({1})") },
            { EFunction.ToUpper     , new FunctionInfo(EDataBaseType.String,"{0}.ToUpper()") },
            { EFunction.ToLower     , new FunctionInfo(EDataBaseType.String,"{0}.ToLower()") },
            //日期函数
            { EFunction.Year        , new FunctionInfo(EDataBaseType.Int32,"{0}.Year") },
            { EFunction.Month       , new FunctionInfo(EDataBaseType.Int32,"{0}.Month") },
            { EFunction.Day         , new FunctionInfo(EDataBaseType.Int32,"{0}.Day") },
            { EFunction.Hour        , new FunctionInfo(EDataBaseType.Int32,"{0}.Hour") },
            { EFunction.Minute      , new FunctionInfo(EDataBaseType.Int32,"{0}.Minute") },
            { EFunction.Second      , new FunctionInfo(EDataBaseType.Int32,"{0}.Second") },
            { EFunction.Date        , new FunctionInfo(EDataBaseType.DateTime,"{0}.Date") },
            { EFunction.Time        , new FunctionInfo(EDataBaseType.TimeSpan,"{0}.Time") },
        };

        //case EFunction.Concat:
        //    break;
        //case EFunction.TotalOffsetMinutes:
        //    break;
        //case EFunction.TotalSeconds:
        //    break;
        //case EFunction.FractionalSeconds:
        //    break;
        //case EFunction.Ceiling:
        //    break;
        //case EFunction.Floor:
        //    break;
        //case EFunction.Round:
        //    break;


        public struct FunctionInfo
        {
            public FunctionInfo(EDataBaseType returnType, string template)
            {
                this.Template = template;
                this.ReturnType = returnType;
            }

            public string Template { get; private set; }
            public EDataBaseType ReturnType { get; private set; }
        }
    }
}

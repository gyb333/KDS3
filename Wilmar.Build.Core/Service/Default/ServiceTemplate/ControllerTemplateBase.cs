using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wilmar.Compile.Core.Service;
using Wilmar.Foundation.Projects;
using Wilmar.Foundation;
using Wilmar.Model.Core.Definitions;
using Wilmar.Model.Core.Definitions.Entities;
using Wilmar.Model.Core.Definitions.Entities.Configure;
using Wilmar.Model.Core.Definitions.Entities.DataTypes;
using Wilmar.Model.Core.Definitions.Entities.Members;
using Wilmar.Service.Common.Generate;
using System.IO;
using Wilmar.Model.Core.Definitions.Querites;
using System.Text.RegularExpressions;
using Wilmar.Compile.Core.Service.Models;

namespace Wilmar.Build.Core.Service.Default.ServiceTemplate
{
    /// <summary>
    /// 控制器模版
    /// </summary>
    public partial class ControllerTemplate
    {
        /// <summary>
        /// 文档对象
        /// </summary>
        public ProjectDocument Documnet
        {
            get { return Data.Entity.ProjectItem; }
        }
        /// <summary>
        /// 编译器对象
        /// </summary>
        public ServiceCompile Compiler
        {
            get { return Data.Project.Compiler; }
        }
        /// <summary>
        /// 当前实体对象
        /// </summary>
        public EntityDefinition Entity
        {
            get { return Data.Entity.Definition; }
        }
        /// <summary>
        /// 控制器配置对象
        /// </summary>
        public ControllerConfigure Configure { get; private set; }
        /// <summary>
        /// 控制器数据对象
        /// </summary>
        public ControllerMetadata Data { get; private set; }

        public ControllerTemplate(ControllerMetadata data)
        {
            this.Data = data;
            this.Configure = Entity.Controller;
        }

        public string GenerateNavigateFindParameter(PropertyNavigationData member)
        {
            List<string> result = new List<string>();
            int index = 0;
            foreach (var mem in member.ToEntity.PrimaryPropertys.Values)
            {
                if (!mem.PropertyType.IsEntityType)
                {
                    if (mem.PropertyType.BaseClrType != typeof(string))
                        result.Add($"System.Convert.ChangeType(relatedKeys[{index++}],typeof({mem.PropertyType.TypeName}))");
                    else
                        result.Add($"relatedKeys[{index++}]");
                }
            }
            return String.Join(",", result.ToArray());
        }

        public string WriteFilter(EntityQuery query)
        {
            StringWriter w = new StringWriter();
            foreach (var para in query.Parameters)
            {
                var isrequire = para.ParameterType == EParameterType.Common;
                var datatype = new DataTypeMetadata(para.Content, Data.Project, isrequire);
                var isPermission = (para.ParameterType == EParameterType.Permission).ToString().ToLower();
                if (datatype.IsCollection)
                    w.WriteLine($"var {para.Name}=_builder.GetParameterArrayValue<{datatype.TypeName}>(\"para.Name\",{isPermission});");
                else if (isrequire && datatype.BaseClrType.IsValueType)
                    w.WriteLine($"var {para.Name}=_builder.GetParameterNullableValue<{datatype.TypeName}>(\"para.Name\",{isPermission});");
                else
                    w.WriteLine($"var {para.Name}=_builder.GetParameterValue<{datatype.TypeName}>(\"para.Name\",{isPermission});");
            }
            foreach (var para in query.Parameters)
                w.WriteLine($"_builder.Parameters.Add(\"{para.Name}\", {para.Name});");

            int varindex = 1;
            this.WriteFilter(w, query.Filters, "_g0", ref varindex);

            return w.GetStringBuilder().ToString();
        }

        private void WriteFilter(StringWriter w, IEnumerable<FilterItemBase> items, string parent, ref int varindex)
        {
            foreach (var item in items)
            {
                if (item.FilterType == EFilterType.Group)
                {
                    var group = (FilterGroup)item;
                    string groupname = "_g" + varindex++;
                    w.WriteLine($"var {groupname} = {parent}.AddGroup(EExpressionConnect.{group.Connection});");
                    WriteFilter(w, group.Filters, groupname, ref varindex);
                }
                else
                    WriteFilter(w, item, parent, ref varindex);
            }
        }

        private void WriteFilter(StringWriter w, FilterItemBase filter, string parent, ref int varindex)
        {
            List<string> parameters = new List<string>();
            switch (filter.FilterType)
            {
                case EFilterType.Custom:
                    {
                        w.Write(parent + ".AddExpression(item => ");
                        FilterCustom custom = (FilterCustom)filter;
                        var result = CodeParameterPattern.Matches(custom.Code);
                        foreach (Match item in result)
                        {
                            var name = item.Groups[1].Value;
                            if (!parameters.Contains(name))
                                parameters.Add(name);
                        }
                        if (parameters.Count == 0)
                            w.Write(custom.Code);
                        else
                            w.Write(CodeParameterPattern.Replace(custom.Code, m => m.Groups[1].Value));
                    }
                    break;
                case EFilterType.MultiValue:
                    {
                        FilterMultiValue stan = (FilterMultiValue)filter;
                        string expression;
                        EDataBaseType expressionDataType;
                        ProcessExpression(stan.Name, stan.Function, stan.FunctionParemeter, out expression, out expressionDataType);
                        switch (stan.Operator)
                        {
                            case EOperator.Between:
                            case EOperator.NotBetween:
                                string value1 = QueryMetadata.GetRightExpression(expressionDataType, stan.Value1Type, stan.Value1, parameters, false);
                                List<string> valueNames = new List<string>();
                                if (stan.Value1Type != EValueType.Property)
                                {
                                    value1 = ProcessVariable(w, value1, ref varindex);
                                    if (stan.Value1Type == EValueType.Const && stan.Value1.StartsWith("@"))
                                        valueNames.Add(value1);
                                }
                                string value2 = QueryMetadata.GetRightExpression(expressionDataType, stan.Value2Type, stan.Value2, parameters, false);
                                if (stan.Value2Type != EValueType.Property)
                                {
                                    value2 = ProcessVariable(w, value2, ref varindex);
                                    if (stan.Value2Type == EValueType.Const && stan.Value2.StartsWith("@"))
                                        valueNames.Add(value2);
                                }
                                if (valueNames.Count > 0)
                                    w.WriteLine($"if({string.Join(" && ", valueNames.Select(a => a + " != null").ToArray())})");
                                w.Write(parent + ".AddExpression(item => ");
                                w.Write(QueryMetadata.Operators[stan.Operator], expression, value1, value2);
                                break;
                        }
                    }
                    break;
                case EFilterType.Standard:
                    {
                        FilterStandard stan = (FilterStandard)filter;
                        string expression;
                        EDataBaseType expressionDataType;
                        ProcessExpression(stan.Name, stan.Function, stan.FunctionParemeter, out expression, out expressionDataType);

                        switch (stan.Operator)
                        {
                            case EOperator.IsNull:
                            case EOperator.NotIsNull:
                                w.Write(parent + ".AddExpression(item => ");
                                w.Write(QueryMetadata.Operators[stan.Operator], expression);
                                break;
                            default:
                                bool isArray = stan.Operator == EOperator.IN;
                                string value = QueryMetadata.GetRightExpression(expressionDataType, stan.ValueType, stan.Value, parameters, isArray);
                                if (stan.ValueType != EValueType.Property)
                                {
                                    value = ProcessVariable(w, value, ref varindex);
                                    if (stan.ValueType == EValueType.Const && stan.Value.StartsWith("@"))
                                        w.WriteLine($"if({value}!=null)");
                                }
                                w.Write(parent + ".AddExpression(item => ");
                                w.Write(QueryMetadata.Operators[stan.Operator], expression, value);
                                break;
                        }
                    }
                    break;
            }
            w.Write(",EExpressionConnect." + filter.Connection.ToString());
            if (parameters.Count > 0)
            {
                w.Write(",new string[] { " + string.Join(",", parameters.Distinct().Select(a => "\"" + a + "\"").ToArray()) + " }");
            }
            w.WriteLine(");");
        }

        private void ProcessExpression(string name, EFunction function, string parameter, out string expression, out EDataBaseType expressionDataType)
        {
            expression = "item." + name;
            expressionDataType = QueryMetadata.GetLeftDataType(Data.Entity, name);
            if (QueryMetadata.Functions.ContainsKey(function))
            {
                var functionInfo = QueryMetadata.Functions[function];
                expression = string.Format(functionInfo.Template, expression, parameter);
                expressionDataType = functionInfo.ReturnType;
            }
        }

        private string ProcessVariable(StringWriter w, string value, ref int varindex)
        {
            string name = "_var" + (varindex++);
            w.WriteLine("var " + name + " = " + value + ";");
            return name;
        }

        private static Regex CodeParameterPattern = new Regex(@"@(?<name>\w+)");

        private string WriteDimFastQuery()
        {
            StringBuilder writer = new StringBuilder();
            var fast = this.Data.Entity;
            var entitys = fast.DimMemberPropertys.Values.Select(a => a.DimEntity).Distinct().ToList();
            entitys.Add(fast);
            writer.AppendLine("var context = DataContext;");
            writer.AppendLine($"var query = (context.{entitys[0].ClassName}s");
            for (int i = 1; i < entitys.Count; i++)
            {
                writer.AppendFormat("\t.SelectMany(a => context.{0}s, ({1}, {2}) => new {{ {1}, {2} }})",
                    entitys[i].ClassName, new string(alias, 0, i), alias[i]);
                writer.AppendLine();
            }
            writer.AppendLine(") as IQueryable;");

            if (Configure.GetInterceptor != null)
            {
                //query = ssasHelper.OlapWhere(query,
                //    entity => contrys.Contains(entity.Country)
                //);
                writer.AppendLine(Configure.GetInterceptor);
            }

            writer.AppendLine("return " + SsasHelperFieldName + ".OlapQuery(query, queryOptions);");
            return writer.ToString();
        }

        private string WriteDimFastHelper()
        {
            var writer = new StringBuilder();

            var fast = this.Data.Entity;
            var dimensions = fast.DimMemberPropertys.Values.Select(a => a.DimEntity).Distinct().ToList();

            writer.AppendFormat($"private static SsasQueryEngine<{fast.ClassName}DimProxy> {SsasHelperFieldName};");

            writer.AppendLine($"public {Data.ClassName}() {{");
            writer.AppendLine("try { ");
            
            writer.AppendFormat($"{SsasHelperFieldName} = new SsasQueryEngine<{fast.ClassName}DimProxy>(new Dictionary<string, string>()\r\n");
            writer.AppendLine("{");
            foreach (var item in fast.DimMemberPropertys.Values)
            {
                var index = dimensions.IndexOf(item.DimEntity);
                var path = GeneratePath(index, dimensions.Count);
                writer.AppendFormat("{{ \"{0}\" ,\"{2}.{1}\" }},", item.Name, item.DimMember, path);
                writer.AppendLine();
            }
            foreach (var item in fast.ColumnPropertys.Values)
            {
                if (!item.Annotations.Any(a => a.Contains("DimensionPropertyAttribute")))
                {
                    writer.AppendFormat("{{ \"{0}\" ,\"{1}.{0}\" }},", item.Name, alias[dimensions.Count]);
                    writer.AppendLine();
                }
            }
            writer.Append("}");
            var forcemembers = Data.Entity.Definition.Configure?.ForceSelectMembers;
            if (!string.IsNullOrEmpty(forcemembers))
            {
                writer.Append(", \"");
                writer.Append(forcemembers.Trim());
                writer.Append("\"");
            }
            writer.AppendLine(");");

            writer.AppendLine(@"}
            catch (Exception ex)
            {
                Logger.Error(ex, """");
                throw;
            }
}");


            return writer.ToString();
        }
        //ab.a  ab.b
        //abc.ab.a  abc.ab.b    abc.c
        //abcd.abc.ab.a 0    abcd.abc.ab.b 1   abcd.abc.c 2  abcd.d 3
        private static string GeneratePath(int index, int count)
        {
            var words = new string[Math.Min(count - index + 1, count)];
            words[0] = new string(alias, 0, count);
            int i = 1, jend = index == 0 ? 1 : index;
            for (int j = count - 1; j > jend; j--)
            {
                words[i++] = new string(alias, 0, j);
            }
            words[i] = alias[index].ToString();
            return string.Join(".", words);
        }
        private const string SsasHelperFieldName = "ssasHelper";
        private static char[] alias = new char[]
        {
            'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
            'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
        };
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wilmar.Foundation;
using Wilmar.Model.Core.Definitions.Entities;
using Wilmar.Model.Core.Definitions.Entities.DataTypes;

namespace Wilmar.Compile.Core.Service.Models
{
    public class DataTypeMetadata
    {
        public DataTypeMetadata(DataTypeBase type, ProjectMetadata compile, bool isrequired)
        {
            Source = type;
            var commontype = type as CommonDataType;
            if (commontype != null)
            {
                switch (commontype.BaseType)
                {
                    case EDataBaseType.Binary:
                    case EDataBaseType.Timestamp:
                        BaseClrType = ClrType = typeof(System.Byte[]);
                        return;
                }
                string name = "System." + commontype.BaseType.ToString();
                Source.Name = name;
                ClrType = BaseClrType = Type.GetType(name);
                if (commontype.Id < 0)
                    ClrType = Type.GetType(BaseClrType.FullName + "[]");
                else if (!isrequired && BaseClrType.IsValueType)
                    ClrType = typeof(Nullable<>).MakeGenericType(BaseClrType);
            }
            else
            {
                var entitytype = type as EntityDataType;
                var entityid = Math.Abs(entitytype.Id) - byte.MaxValue;
                this.EntityType = compile.Entitys[entityid];
                entitytype.Name = this.EntityType.ClassName;
                
            }
        }

        public bool IsEntityType
        {
            get { return EntityType != null; }
        }

        public bool IsCollection
        {
            get
            {
                if (BaseClrType != null && BaseClrType.IsArray)
                    return true;
                return Source.Id < 0;
            }
        }

        public Type BaseClrType { get; private set; }

        public Type ClrType { get; private set; }

        public EntityMetadata EntityType { get; private set; }

        public DataTypeBase Source { get; private set; }

        public string TypeName
        {
            get
            {
                if (IsEntityType)
                {
                    return EntityType.ProjectItem.Name;
                }
                else
                {
                    if (ClrType.Name.StartsWith("Nullable"))
                        return "System.Nullable<" + BaseClrType.FullName + ">";
                    return ClrType.FullName;
                }
            }
        }

        public string GetValueExpression(string express)
        {
            var com = Source as CommonDataType;
            return GenerateValueExpression(com.BaseType, express, ClrType.IsArray);
        }



        public static string GenerateValueExpression(EDataBaseType datatype, string value, bool isArray)
        {
            if (value.StartsWith("@") && value.Length > 2)
                return value.Substring(1);
            if (!isArray)
            {
                if (VerificationValue(datatype, value))
                {
                    switch (datatype)
                    {
                        case EDataBaseType.String:
                            return "@\"" + value + "\"";
                        case EDataBaseType.Guid:
                        case EDataBaseType.DateTime:
                        case EDataBaseType.DateTimeOffset:
                        case EDataBaseType.TimeSpan:
                            return "System." + datatype.ToString() + ".Parse(\"" + value + "\")";
                        case EDataBaseType.Int16:
                        case EDataBaseType.Int32:
                        case EDataBaseType.Int64:
                        case EDataBaseType.Double:
                        case EDataBaseType.Single:
                        case EDataBaseType.Byte:
                        case EDataBaseType.Boolean:
                        case EDataBaseType.Decimal:
                            return value;
                    }
                }
            }
            else
            {
                switch (datatype)
                {
                    case EDataBaseType.String:
                        return "new string[] { " + value + " } ";
                    case EDataBaseType.Guid:
                    case EDataBaseType.DateTime:
                    case EDataBaseType.DateTimeOffset:
                    case EDataBaseType.TimeSpan:
                        var values = value.Split(',').Where(a => VerificationValue(datatype, a)).Select(a => "System." + datatype.ToString() + ".Parse(" + a + ")").ToArray();
                        return "new " + "System." + datatype.ToString() + "[] { " + string.Join(",", values) + " } ";
                    case EDataBaseType.Int16:
                    case EDataBaseType.Int32:
                    case EDataBaseType.Int64:
                    case EDataBaseType.Double:
                    case EDataBaseType.Single:
                    case EDataBaseType.Byte:
                    case EDataBaseType.Boolean:
                    case EDataBaseType.Decimal:
                        return "new " + "System." + datatype.ToString() + "[] { " + value + " } ";
                }
            }
            return "";
        }


        private static bool VerificationValue(EDataBaseType datatype, string value)
        {
            switch (datatype)
            {
                case EDataBaseType.Guid:
                case EDataBaseType.DateTime:
                case EDataBaseType.DateTimeOffset:
                case EDataBaseType.TimeSpan:
                case EDataBaseType.Int16:
                case EDataBaseType.Int32:
                case EDataBaseType.Int64:
                case EDataBaseType.Double:
                case EDataBaseType.Single:
                case EDataBaseType.Byte:
                case EDataBaseType.Boolean:
                case EDataBaseType.Decimal:
                    if (string.IsNullOrEmpty(value))
                        return false;
                    break;
            }
            switch (datatype)
            {
                case EDataBaseType.Guid:
                    Guid value1;
                    return Guid.TryParse(value, out value1);
                case EDataBaseType.DateTime:
                    DateTime value2;
                    return DateTime.TryParse(value, out value2);
                case EDataBaseType.DateTimeOffset:
                    DateTimeOffset value3;
                    return DateTimeOffset.TryParse(value, out value3);
                case EDataBaseType.TimeSpan:
                    TimeSpan value4;
                    return TimeSpan.TryParse(value, out value4);
            }
            return true;
        }
    }
}

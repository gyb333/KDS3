using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wilmar.Compile.Core.Service;
using Wilmar.Foundation.Common;
using Wilmar.Foundation.Projects;
using Wilmar.Foundation.Projects.Configures;
using Wilmar.Foundation;
using Wilmar.Model.Core.Definitions;
using Wilmar.Model.Core.Definitions.Entities;
using Wilmar.Model.Core.Definitions.Entities.Members;
using Wilmar.Service.Common.Generate;
using Wilmar.Compile.Core.Service.Models;
using static Wilmar.Compile.Core.Service.Models.DataContextMetadata;
using Wilmar.Model.Core.Definitions.Entities.DataTypes;

namespace Wilmar.Build.Core.Service.Default.ServiceTemplate
{
    /// <summary>
    /// 数据上下文模版
    /// </summary>
    public partial class DataContextTemplate
    {

        public class EntityStoreProduceItem
        {
            public string Name { get; set; }

            public string[] InsertParameters { get; set; }

            public string[] UpdateParameters { get; set; }

            public string[] DeleteParameters { get; set; }
        }



        public DataContextTemplate(DataContextMetadata data)
        {
            Data = data;
            Documnet = Data.Source;
            EntityStoreProduceNames = data.Entitys.Values.Where(a => !a.Definition.Configure.IsView).Select(a => a.ClassName).ToArray();

            AllowRenameProcedureParameter = data.DatabaseType == EDatabaseType.MySql;
        }

        private readonly string[] EntityStoreProduceNames;

        private string GetHasName(RelationDataItem item)
        {
            switch (item.FromNavigate.Member.ToCardinality)
            {
                case EMappingCardinality.ZeroOrOne:
                    return "Optional";
                case EMappingCardinality.One:
                    return "Required";
                case EMappingCardinality.Many:
                    return "Many";
            }
            return "";
        }

        private string GetWithName(RelationDataItem item)
        {
            string result = "";
            var member = item.FromNavigate.Member;
            switch (member.Cardinality)
            {
                case EMappingCardinality.ZeroOrOne:
                    result = "Optional";
                    break;
                case EMappingCardinality.One:
                    result = "Required";
                    break;
                case EMappingCardinality.Many:
                    result = "Many";
                    break;
            }
            if (member.Cardinality != EMappingCardinality.Many && member.ToCardinality != EMappingCardinality.Many)
            {
                if (member.Cardinality == member.ToCardinality)
                {
                    if (item.ToNavigate != null && item.ToNavigate.Member.IsMain)
                        result += "Dependent";
                    else
                        result += "Principal";
                }
            }

            return result;
        }

        private string GetHasProperty(RelationDataItem item)
        {
            return $"a => a.{item.FromNavigate.Name}";
        }

        private string GetWithProperty(RelationDataItem item)
        {
            if (item.ToNavigate != null)
            {
                return $"a => a.{item.ToNavigate.Name}";
            }
            return "";
        }

        public string GetForeignKey(RelationDataItem item)
        {
            if (item.AllownForeignKey)
            {
                return $".HasForeignKey(a => a.{item.ForeignKeyName})";
            }
            return "";
        }

        private string GetCascadeOnDelete(RelationDataItem item)
        {
            if (item.AllowCascadeOnDelete)
            {
                return $".WillCascadeOnDelete({item.CascadeOnDelete.ToString().ToLower()})";
            }
            return "";
        }

        /// <summary>
        /// 文档对象
        /// </summary>
        public DatabaseConfigureItem Documnet { get; private set; }

        public DataContextMetadata Data { get; private set; }

        public bool AllowRenameProcedureParameter { get; }

        public List<EntityStoreProduceItem> StoreProduceItems { get; } = new List<EntityStoreProduceItem>();

        private void InititalStoreProduceItems(DataContextMetadata metadata)
        {
            foreach (var entity in metadata.Entitys.Values.Where(a => !a.Definition.Configure.IsView))
            {
                var keys = GetEntityKeys(entity, metadata);
                var columns = GetEntityColumns(entity, metadata);
                var item = new EntityStoreProduceItem()
                {
                    Name = entity.ClassName,
                    InsertParameters = keys.Where(a => !IsCompute(a, keys)).Select(a => a.Name).Concat(
                        columns.Where(a => !IsCompute(a)).Select(a => a.Name)).ToArray(),
                    UpdateParameters = keys.Select(a => a.Name).Concat(
                        columns.Where(a => !IsCompute(a)).Select(a => a.Name)).ToArray(),
                    DeleteParameters = keys.Select(a => a.Name).ToArray()
                };
                var timestamp = GetTimestampColumn(columns);
                if (timestamp != null)
                {
                    var name = timestamp.Name + "_Original";
                    item.UpdateParameters = item.UpdateParameters.Concat(new string[] { name }).ToArray();
                    item.DeleteParameters = item.DeleteParameters.Concat(new string[] { name }).ToArray();
                }
                Compare(item.InsertParameters, entity.ProjectItem.Name, "Insert");
                Compare(item.UpdateParameters, entity.ProjectItem.Name, "Update");
                Compare(item.DeleteParameters, entity.ProjectItem.Name, "Delete");


                StoreProduceItems.Add(item);
            }
        }

        private void Compare(string[] left, string name, string o)
        {
            var right = GetParameters(name + "_" + o);
            left = left.OrderBy(a => a).ToArray();
            System.Diagnostics.Debug.Assert(left.Length == right.Length);
            for (int i = 0; i < left.Length; i++)
            {
                System.Diagnostics.Debug.Assert(left[i].Equals(right[i]));
            }
        }
        private string[] GetParameters(string name)
        {
            using (System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection("Data Source=10.227.201.150;Initial Catalog=KDS3A;User Id=sa;Password=qwer.1234;"))
            {
                var sql = string.Format("SELECT o.id ,o.name , c.name FROM sysobjects o JOIN syscolumns c ON c.id = o.id WHERE o.xtype = 'p' AND o.name='{0}';", name);
                var com = con.CreateCommand();
                com.CommandText = sql;
                con.Open();
                List<string> result = new List<string>();
                using (var reader = com.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(reader.GetString(2).Replace("@", ""));
                    }
                }
                con.Close();
                return result.OrderBy(a => a).ToArray();
            }
        }


        private bool IsCompute(PropertyPrimaryData key, PropertyPrimaryData[] keys)
        {
            if (key.Member.GenerateMode == EPrimaryGenerateMode.Database ||
                key.Member.GenerateMode == EPrimaryGenerateMode.Identity)
                return true;
            if (keys.Length == 1 &&
                key.Member.GenerateMode == EPrimaryGenerateMode.None &&
                key.Name.ToLower().Contains("id"))
                return true;
            return false;
        }

        private bool IsCompute(PropertyColumnData column)
        {
            var datatype = column.Member.Content as CommonDataType;
            if (datatype != null && datatype.BaseType == EDataBaseType.Timestamp)
                return true;
            var mode = column.Member.GenerateMode;
            return mode == EColumnGenerateMode.Database || mode == EColumnGenerateMode.DatabaseCreate || mode == EColumnGenerateMode.DatabaseUpdate;
        }

        private PropertyColumnData GetTimestampColumn(PropertyColumnData[] columns)
        {
            foreach (var column in columns)
            {
                var datatype = column.Member.Content as CommonDataType;
                if (datatype != null && datatype.BaseType == EDataBaseType.Timestamp)
                    return column;
            }
            return null;
        }

        private PropertyColumnData[] GetEntityColumns(EntityMetadata entity, DataContextMetadata metadata)
        {
            return entity.ColumnPropertys.Select(a => a.Value).ToArray();
        }

        private PropertyPrimaryData[] GetEntityKeys(EntityMetadata entity, DataContextMetadata metadata)
        {
            while (entity.Definition.Configure.InheritEntityId > 0)
            {
                entity = metadata.Entitys[entity.Definition.Configure.InheritEntityId];
            }
            return entity.PrimaryPropertys.Select(a => a.Value).ToArray();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wilmar.Foundation;
using System.Threading.Tasks;
using Wilmar.Foundation.Projects.Configures;
using Wilmar.Model.Core.Definitions.Entities;
using Wilmar.Model.Core.Definitions.Entities.Members;

namespace Wilmar.Compile.Core.Service.Models
{
    public class DataContextMetadata : MetadataBase
    {
        public DataContextMetadata(DatabaseConfigureItem source)
        {
            this.Source = source;
            this.Entitys = new Dictionary<int, EntityMetadata>();
            if (source.ProviderName.ToLower().Contains("mysql"))
                DatabaseType = EDatabaseType.MySql;
        }

        public string ClassName
        {
            get { return Source.Name + "DataContext"; }
        }

        public EDatabaseType DatabaseType { get; } = EDatabaseType.SqlServer;

        public DatabaseConfigureItem Source { get; private set; }

        public Dictionary<int, EntityMetadata> Entitys { get; private set; }

        public List<RelationDataItem> Relations { get; private set; } = new List<RelationDataItem>();

        public override void Initialize(ProjectMetadata project)
        {
            base.Initialize(project);
            if (Entitys.Values.Any(a => a.Definition.CubeType == Model.Definitions.Entities.EEntityCubeType.Fact))
                return;
            HashSet<PropertyNavigationData> readyPropertys = new HashSet<PropertyNavigationData>();
            foreach (var entity in this.Entitys.Values)
            {
                foreach (var pro in entity.NavigationPropertys.Values.Where(a => a.IsInherit == false))
                {
                    readyPropertys.Add(pro);
                }
            }
            while (readyPropertys.Count > 0)
            {
                var navi = readyPropertys.First();
                var item = new RelationDataItem(navi);

                readyPropertys.Remove(item.FromNavigate);
                if (item.ToNavigate != null)
                    readyPropertys.Remove(item.ToNavigate);

                this.Relations.Add(item);
            }
        }

        public class RelationDataItem
        {
            public RelationDataItem(PropertyNavigationData navi)
            {
                FromEntity = navi.Entity;
                FromNavigate = navi;
                ToEntity = navi.ToEntity;
                ToNavigate = navi.ToNavigation;

                var member = navi.Member;
                if (ToNavigate != null && ToNavigate.Member.IsMain)
                    member = ToNavigate.Member;
                CascadeOnDelete = member.CascadeOnDelete;

                AllowCascadeOnDelete = !(member.Cardinality == EMappingCardinality.Many && member.ToCardinality == EMappingCardinality.Many);
                if (member.Cardinality == EMappingCardinality.Many && member.ToCardinality != EMappingCardinality.Many)
                    AllownForeignKey = true;
                if (member.Cardinality != EMappingCardinality.Many && member.ToCardinality == EMappingCardinality.Many)
                    AllownForeignKey = true;

                if (AllownForeignKey)
                {
                    //目前只支持单个外键
                    var foreignkey = member.ForeignKeyName;
                    var targetEntity = navi.Member.Cardinality == EMappingCardinality.Many ? navi.Entity : navi.ToEntity;
                    var primaryEntity = navi.Member.Cardinality == EMappingCardinality.Many ? navi.ToEntity : navi.Entity;
                    PropertyColumnData foreigncolumn = null;
                    PropertyPrimaryData foreigncolumn2 = null;
                    if (string.IsNullOrEmpty(foreignkey)
                        || !(targetEntity.ColumnPropertys.TryGetValue(foreignkey, out foreigncolumn)
                        || targetEntity.PrimaryPropertys.TryGetValue(foreignkey, out foreigncolumn2)))
                    {
                        var primaryColumn = primaryEntity.PrimaryPropertys.Values.First();
                        string foreignName = GenerateForeignName(targetEntity, primaryEntity.ClassName + primaryColumn.Name);

                        var content = primaryColumn.Member.Content;
                        content = (DataTypeBase)content.GetType().GetMethod("MemberwiseClone", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(content, new object[] { });

                        bool isrequired = true;
                        if (navi.Member.Cardinality != EMappingCardinality.Many)
                        {
                            isrequired = navi.Member.Cardinality == EMappingCardinality.One;
                        }
                        else if (navi.Member.ToCardinality != EMappingCardinality.Many)
                        {
                            isrequired = navi.Member.ToCardinality == EMappingCardinality.One;
                        }

                        ColumnMember foreignMember = new ColumnMember()
                        {
                            Content = content,
                            Name = foreignName,
                            Title = navi.Member.Title + "外键",
                            IsRequired = isrequired
                        };
                        foreigncolumn = new PropertyColumnData(foreignMember, targetEntity, navi.IsInherit);
                        foreigncolumn.Initialize(navi.Project);
                        targetEntity.ColumnPropertys.Add(foreignMember.Name, foreigncolumn);
                    }
                    if (foreigncolumn != null)
                    {
                        foreigncolumn.Index(false);
                        ForeignKeyName = foreigncolumn.Name;
                    }
                    else if (foreigncolumn2 != null)
                    {
                        ForeignKeyName = foreigncolumn2.Name;
                    }
                }
            }

            private string GenerateForeignName(EntityMetadata targetEntity, string name)
            {
                return targetEntity.Definition.Members.Select(a => a.Name).Union(targetEntity.ColumnPropertys.Keys).UniqueName(a => a, name, true);
            }

            public EntityMetadata FromEntity { get; set; }

            public PropertyNavigationData FromNavigate { get; set; }

            public EntityMetadata ToEntity { get; set; }

            public PropertyNavigationData ToNavigate { get; set; }

            public bool AllowCascadeOnDelete { get; private set; }

            public bool AllownForeignKey { get; private set; }

            public bool CascadeOnDelete { get; private set; }

            public string ForeignKeyName { get; private set; }


        }
    }

    public enum EDatabaseType
    {
        SqlServer = 0,
        MySql = 1
    }
}

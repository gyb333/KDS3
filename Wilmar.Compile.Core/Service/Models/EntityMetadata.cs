using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wilmar.Foundation;
using Wilmar.Foundation.Projects;
using Wilmar.Model.Core.Definitions;
using Wilmar.Model.Core.Definitions.Entities;
using Wilmar.Model.Core.Definitions.Entities.DataTypes;
using Wilmar.Model.Core.Definitions.Entities.Members;
using Wilmar.Model.Core.Definitions.Entities.Validators;

namespace Wilmar.Compile.Core.Service.Models
{
    public class EntityMetadata : AnnotationMetadataBase
    {
        public EntityMetadata(ProjectDocument item, DataContextMetadata context, EntityDefinition def)
        {
            this.IsDimFast = def.CubeType == Model.Definitions.Entities.EEntityCubeType.Fact;
            this.Definition = def;
            this.ProjectItem = item;
            this.DataContext = context;
            if (def.Configure != null)
            {
                string name = "System.ComponentModel.DataAnnotations.Schema.TableDescriptor";
                var list = new List<string>();
                list.Add($"IsView = {def.Configure.IsView.ToString().ToLower()}");
                if (!string.IsNullOrEmpty(def.Configure.TableName))
                {
                    list.Add($"TableName = \"{def.Configure.TableName}\"");
                    if (context.DatabaseType != EDatabaseType.MySql)
                    {
                        if (!string.IsNullOrEmpty(def.Configure.SchemaName))
                            list.Add($"Schema = \"{def.Configure.SchemaName}\"");
                    }
                }
                else
                {
                    list.Add($"TableName = \"{item.Name}s\"");
                }
                Write(name, list.ToArray());
            }
        }

        public string ClassName
        {
            get { return ProjectItem.Name; }
        }

        public string BaseClassName { get; private set; }

        public ProjectDocument ProjectItem { get; private set; }

        public EntityDefinition Definition { get; private set; }

        public DataContextMetadata DataContext { get; private set; }

        public Dictionary<string, PropertyPrimaryData> PrimaryPropertys { get; private set; } = new Dictionary<string, PropertyPrimaryData>();

        public Dictionary<string, PropertyColumnData> ColumnPropertys { get; private set; } = new Dictionary<string, PropertyColumnData>();

        public Dictionary<string, PropertyNavigationData> NavigationPropertys { get; private set; } = new Dictionary<string, PropertyNavigationData>();

        public Dictionary<string, PropertyCalculateData> CalculatePropertys { get; private set; } = new Dictionary<string, PropertyCalculateData>();

        public Dictionary<string, PropertyCommonData> CommonPropertys { get; private set; } = new Dictionary<string, PropertyCommonData>();

        public Dictionary<string, PropertyColumnData> DimEntityPropertys { get; private set; } = new Dictionary<string, PropertyColumnData>();

        public Dictionary<string, PropertyDimMemberData> DimMemberPropertys { get; private set; } = new Dictionary<string, PropertyDimMemberData>();


        public override void Initialize(ProjectMetadata project)
        {
            base.Initialize(project);
            List<MetadataBase> intialize = new List<MetadataBase>();
            if (Definition.CubeType == Model.Definitions.Entities.EEntityCubeType.Fact)
            {
                base.Write("AgileDesign.SsasEntityFrameworkProvider.Attributes.MeasureGroupAttribute");
            }
            InitialMembers(Definition.Members, intialize, false);
            if (Definition.Configure != null)
                InitialInheritMembers(Definition.Configure.InheritEntityId, intialize);
            foreach (var item in intialize)
            {
                item.Initialize(project);
            }
            if (Definition.Configure != null && Definition.Configure.InheritEntityId > 0)
            {
                EntityMetadata baseEntity;
                if (project.Entitys.TryGetValue(Definition.Configure.InheritEntityId, out baseEntity))
                {
                    BaseClassName = baseEntity.ProjectItem.Name;
                }
            }
            if (Definition.CubeType != Model.Definitions.Entities.EEntityCubeType.Fact)
            {
                foreach (var col in PrimaryPropertys.Values)
                {
                    col.Column(col.Member.ColumnName, DataColumnIndex++);
                }
                foreach (var col in ColumnPropertys.Values)
                {
                    col.Column(col.Member.ColumnName, DataColumnIndex++);
                }
            }
        }

        private void InitialInheritMembers(int entityId, List<MetadataBase> intialize)
        {
            if (entityId > 0)
            {
                EntityMetadata baseEntity;
                if (Project.Entitys.TryGetValue(entityId, out baseEntity))
                {
                    InitialMembers(baseEntity.Definition.Members, intialize, true);
                    if (baseEntity.Definition.Configure != null)
                    {
                        InitialInheritMembers(baseEntity.Definition.Configure.InheritEntityId, intialize);
                    }
                }
            }
        }

        private Stack<PropertyPrimaryData> PrimaryColumnStack = new Stack<PropertyPrimaryData>();
        private Stack<PropertyColumnData> DataColumnStack = new Stack<PropertyColumnData>();

        private void InitialMembers(List<MemberBase> members, List<MetadataBase> intialize, bool isInherit)
        {
            foreach (var member in members.Where(a => a.MemberType == EMemberType.Primary))
            {
                var item1 = new PropertyPrimaryData(member, this, isInherit);
                if (PrimaryPropertys.ContainsKey(member.Name))
                {
                    throw new Exception($"实体[{ProjectItem.Title}]中已存在成员[{member.Name}]。");
                }
                PrimaryPropertys.Add(member.Name, item1);
                intialize.Add(item1);
            }
            var prefix = Definition.Configure?.FastPrefixStr;
            foreach (var member in members.Where(a => a.MemberType == EMemberType.Column))
            {
                var item2 = new PropertyColumnData(member, this, isInherit);
                if (ColumnPropertys.ContainsKey(member.Name))
                {
                    throw new Exception($"实体[{ProjectItem.Title}]中已存在成员[{member.Name}]。");
                }
                ColumnPropertys.Add(member.Name, item2);
                if (Definition.CubeType == Model.Definitions.Entities.EEntityCubeType.Fact)
                {
                    if (!string.IsNullOrEmpty(prefix))
                    {
                        var col = (ColumnMember)member;
                        var name = string.IsNullOrEmpty(col.ColumnName) ? member.Name : col.ColumnName;
                        item2.Column(prefix + name, DataColumnIndex++);
                    }
                }
                intialize.Add(item2);
            }
            if (this.Definition.CubeType == Model.Definitions.Entities.EEntityCubeType.None)
            {
                foreach (var member in members.Where(a => a.MemberType == EMemberType.Navigation))
                {

                    if (NavigationPropertys.ContainsKey(member.Name))
                    {
                        throw new Exception($"实体[{ProjectItem.Title}]中已存在成员[{member.Name}]。");
                    }

                    try
                    {
                        var item3 = new PropertyNavigationData(member, this, isInherit);
                        NavigationPropertys.Add(member.Name, item3);
                        intialize.Add(item3);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"实体[{ProjectItem.Title}]中成员[{member.Name}]存在问题!");
                    }
                }
            }
            else if (Definition.CubeType == Model.Definitions.Entities.EEntityCubeType.Fact)
            {
                foreach (var member in members.OfType<NavigationMember>())
                {
                    if (member.Cardinality == EMappingCardinality.Many && member.ToCardinality != EMappingCardinality.Many)
                    {
                        var toitem = Project.Entitys[member.ToEntityId];
                        var toentity = Project.Entitys[member.ToEntityId].Definition;
                        var primary = toentity.Members.OfType<PrimaryMember>().Single();
                        var foreignMember = new ColumnMember()
                        {
                            Content = primary.Content,
                            Name = member.Name + primary.Name,
                            Title = member.Title + "外键",
                            IsRequired = member.ToCardinality == EMappingCardinality.One
                        };
                        var foreigncolumn = new PropertyColumnData(foreignMember, this, false);
                        foreigncolumn.Write("AgileDesign.SsasEntityFrameworkProvider.Attributes.DimensionPropertyAttribute"
                            , "\"" + toitem.ClassName + "s\"");
                        this.ColumnPropertys.Add(foreigncolumn.Name, foreigncolumn);
                        this.DimEntityPropertys.Add(foreignMember.Name, foreigncolumn);
                        intialize.Add(foreigncolumn);
                    }
                }
                foreach (var member in members.OfType<DimensionMember>())
                {
                    if (!Project.Entitys.ContainsKey(member.DimensionEntityId))
                    {
                        throw new Exception($"实体[{ProjectItem.Title}]中维度成员[{member.Title}]所关联的实体[{member.DimensionEntityId}]不存在。");
                    }
                    var dimmember = new PropertyDimMemberData(member, this, false);
                    this.DimMemberPropertys.Add(dimmember.Name, dimmember);
                    intialize.Add(dimmember);
                }
            }
            foreach (var member in members.Where(a => a.MemberType == EMemberType.Calculate))
            {
                var item4 = new PropertyCalculateData(member, this, isInherit);
                CalculatePropertys.Add(member.Name, item4);
                intialize.Add(item4);
            }
            foreach (var member in members.Where(a => a.MemberType == EMemberType.Common))
            {
                var item5 = new PropertyCommonData(member, this, isInherit);
                CommonPropertys.Add(member.Name, item5);
                intialize.Add(item5);
            }
        }

        public int DataColumnIndex { get; set; } = 1;

        public bool IsDimFast { get; }
    }

    public class PropertyPrimaryData : PropertyDataBase<PrimaryMember>
    {
        public PropertyPrimaryData(MemberBase member, EntityMetadata entity, bool isInherit)
            : base(member, entity, isInherit)
        {
            Key();
            switch (Member.GenerateMode)
            {
                case EPrimaryGenerateMode.Identity:
                    DatabaseGenerated("Identity");
                    break;
                case EPrimaryGenerateMode.Database:
                    DatabaseGenerated("Identity");
                    GeneratedProperty(Member.DatabaseGenerate, "");
                    break;
                case EPrimaryGenerateMode.Disabled:
                    DatabaseGenerated("None");
                    break;
            }
            DecimalType dec = member.Content as DecimalType;
            if (dec != null)
                Precision(dec.Precision, dec.Scale);
        }

        public override bool IsRequired
        {
            get { return true; }
        }
    }

    public class PropertyColumnData : PropertyDataBase<ColumnMember>
    {
        public PropertyColumnData(MemberBase member, EntityMetadata entity, bool isInherit)
            : base(member, entity, isInherit)
        {
            switch (Member.GenerateMode)
            {
                case EColumnGenerateMode.Database:
                    DatabaseGenerated("Computed");
                    GeneratedProperty(Member.DatabaseGenerate);
                    break;
                case EColumnGenerateMode.DatabaseCreate:
                    DatabaseGenerated("Computed");
                    GeneratedProperty(Member.DatabaseGenerate, "");
                    break;
                case EColumnGenerateMode.DatabaseUpdate:
                    DatabaseGenerated("Computed");
                    GeneratedProperty("", Member.DatabaseGenerate);
                    break;
            }
            DecimalType dec = member.Content as DecimalType;
            if (dec != null)
                Precision(dec.Precision, dec.Scale);
        }

        public override bool IsRequired
        {
            get { return Member.IsRequired; }
        }
    }

    public class PropertyNavigationData : PropertyDataBase<NavigationMember>
    {
        public PropertyNavigationData(MemberBase member, EntityMetadata entity, bool isInherit)
            : base(member, entity, isInherit)
        {
            this.ToEntity = entity.DataContext.Entitys[Member.ToEntityId];

        }

        public override bool IsRequired
        {
            get { return Member.ToCardinality == EMappingCardinality.One; }
        }

        public EntityMetadata ToEntity { get; private set; }

        public PropertyNavigationData ToNavigation
        {
            get
            {
                if (_ToNavigation == null)
                {
                    PropertyNavigationData temp;
                    if (this.ToEntity.NavigationPropertys.TryGetValue(Member.ToName, out temp))
                    {
                        _ToNavigation = temp;
                    }
                }
                return _ToNavigation;
            }
        }
        private PropertyNavigationData _ToNavigation;
    }

    public class PropertyCalculateData : PropertyDataBase<CalculatedMember>
    {
        public PropertyCalculateData(MemberBase member, EntityMetadata entity, bool isInherit)
            : base(member, entity, isInherit)
        {
            this.NotMapped();
        }

        public override bool IsRequired
        {
            get { return Member.IsRequired; }
        }
    }

    public class PropertyDimMemberData : PropertyDataBase<DimensionMember>
    {
        public PropertyDimMemberData(DimensionMember member, EntityMetadata entity, bool isInherit)
            : base(member, entity, isInherit)
        {
            DimEntity = entity.Project.Entitys[member.DimensionEntityId];
            DimMember = member.PropertyMember;
        }

        public EntityMetadata DimEntity { get; }

        public string DimMember { get; }

        public override bool IsRequired
        {
            get { return true; }
        }
    }

    public class PropertyCommonData : PropertyDataBase<CommonMember>
    {
        public PropertyCommonData(MemberBase member, EntityMetadata entity, bool isInherit)
            : base(member, entity, isInherit)
        {
            //this.NotMapped();
        }

        public override bool IsRequired
        {
            get { return Member.IsRequired; }
        }
    }
}

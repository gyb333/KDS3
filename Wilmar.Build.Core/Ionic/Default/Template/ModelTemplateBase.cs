using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wilmar.Compile.Core.Service;
using Wilmar.Foundation.Projects;
using Wilmar.Foundation;
using Wilmar.Model.Core.Definitions;
using Wilmar.Model.Core.Definitions.Entities.Configure;
using System.IO;
using Wilmar.Model.Core.Definitions.Querites;
using System.Text.RegularExpressions;
using Wilmar.Compile.Core.Service.Models;
using Wilmar.Model.Core.Definitions.Entities;
using Wilmar.Model.Core.Definitions.Entities.DataTypes;
using Wilmar.Build.Core.Dojo.Default.Builders;
using Wilmar.Model.Core.Definitions.Entities.Members;
using Wilmar.Compile.Core.Ionic;

namespace Wilmar.Build.Core.Ionic.Default.Template
{
    /// <summary>
    /// 控制器模版
    /// </summary>
    public partial class ModelTemplate
    {
        /// <summary>
        /// 编译对象
        /// </summary>
        public IonicCompile Compile
        {
            get; private set;
        }
        /// <summary>
        /// 文档对象
        /// </summary>
        public ProjectDocument Documnet
        {
            get; private set;
        }
        /// <summary>
        /// 实体对象
        /// </summary>
        public EntityDefinition EntityDef
        {
            get; private set;
        }
        public Dictionary<string, Tuple<string, string, string>> PrimaryPropertys = new Dictionary<string, Tuple<string, string, string>>();
        public Dictionary<string, Tuple<string, string, string>> ColumnPropertys = new Dictionary<string, Tuple<string, string, string>>();
        public Dictionary<string, Tuple<string, string, string>> CommonPropertys = new Dictionary<string, Tuple<string, string, string>>();
        public Dictionary<string, Tuple<string, string, string>> CalculatePropertys = new Dictionary<string, Tuple<string, string, string>>();
        public Dictionary<string, Tuple<string, string, string, bool>> NavigationPropertys = new Dictionary<string, Tuple<string, string, string, bool>>();
        public Dictionary<string, string> ImportMember = new Dictionary<string, string>();

        public ModelTemplate(IonicCompile ionicCompile, ProjectDocument doc, EntityDefinition def)
        {
            this.Compile = ionicCompile;
            this.Documnet = doc;
            this.EntityDef = def;
            InitialMembers();
        }
        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitialMembers()
        {
            foreach (var member in this.EntityDef.Members.Where(a => a.MemberType == EMemberType.Primary))
            {
                var typeContent = member.Content as CommonDataType;
                var dateType = typeContent.BaseType.ToString();
                var dateTypeFront = BuildCommonMethod.GetTypeNameByIon(typeContent.BaseType);
                PrimaryPropertys.Add(member.Name, new Tuple<string, string, string>(member.Title, dateType, dateTypeFront));
            }
            foreach (var member in this.EntityDef.Members.Where(a => a.MemberType == EMemberType.Column))
            {
                var typeContent = member.Content as CommonDataType;
                if (typeContent.BaseType == EDataBaseType.Timestamp) continue;
                var dateType = typeContent.BaseType.ToString();
                var dateTypeFront = BuildCommonMethod.GetTypeNameByIon(typeContent.BaseType);
                ColumnPropertys.Add(member.Name, new Tuple<string, string, string>(member.Title, dateType, dateTypeFront));
            }
            foreach (var member in this.EntityDef.Members.Where(a => a.MemberType == EMemberType.Common))
            {
                var typeContent = member.Content as CommonDataType;
                if (typeContent.BaseType == EDataBaseType.Timestamp) continue;
                var dateType = typeContent.BaseType.ToString();
                var dateTypeFront = BuildCommonMethod.GetTypeNameByIon(typeContent.BaseType);
                CommonPropertys.Add(member.Name, new Tuple<string, string, string>(member.Title, dateType, dateTypeFront));
            }
            foreach (var member in this.EntityDef.Members.Where(a => a.MemberType == EMemberType.Calculate))
            {
                var typeContent = member.Content as CommonDataType;
                if (typeContent.BaseType == EDataBaseType.Timestamp) continue;
                var dateType = typeContent.BaseType.ToString();
                var dateTypeFront = BuildCommonMethod.GetTypeNameByIon(typeContent.BaseType);
                CalculatePropertys.Add(member.Name, new Tuple<string, string, string>(member.Title, dateType, dateTypeFront));
            }
            foreach (var member in this.EntityDef.Members.Where(a => a.MemberType == EMemberType.Navigation))
            {
                var navigationMember = member as NavigationMember;
                string toEntity = (from t in this.Compile.ProjectItems where t.Key == navigationMember.ToEntityId select t.Value).FirstOrDefault().Name;
                bool isScalar = navigationMember.ToCardinality == EMappingCardinality.Many ? false : true;
                string foreignKeyNames = navigationMember.ForeignKeyName == null ? "" : navigationMember.ForeignKeyName;
                #region 
                //foreach (var primaryMember in this.EntityDef.Members.OfType<PrimaryMember>())
                //{
                //    string primaryKey = primaryMember.Name;
                //    string fkeyNames = GetRealForeignKey(navigationMember, toEntity, primaryKey);
                //    if (string.IsNullOrEmpty(foreignKeyNames)) foreignKeyNames = fkeyNames;
                //    else foreignKeyNames += "," + fkeyNames;
                //}
                #endregion
                NavigationPropertys.Add(member.Name, new Tuple<string, string, string, bool>(member.Title, toEntity, foreignKeyNames, isScalar));
                
                ImportMember.Add(toEntity, toEntity);
            }
        }

        private string GetRealForeignKey(NavigationMember member, string toEntity, string primaryKey)
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
    }
}

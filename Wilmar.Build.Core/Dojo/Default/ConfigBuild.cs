using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Text;
using Wilmar.Compile.Core.Dojo;
using Wilmar.Foundation.Common;
using Wilmar.Service.Common.Generate;
using Wilmar.Model.Core.Definitions;
using Wilmar.Model.Core.Definitions.Entities.Members;
using Wilmar.Model.Core.Definitions.Entities;
using Wilmar.Build.Core.Dojo.Default.Builders;
using Wilmar.Model.Core.Definitions.Configure;
using Wilmar.Foundation.Projects;
using System.Collections.Generic;
using Wilmar.Model.Core.Definitions.Entities.DataTypes;

namespace Wilmar.Build.Core.Dojo.Default
{
    /// <summary>
    /// 配置生成器
    /// </summary>
    public class ConfigBuild : BuildBase
    {
        #region 
        /// <summary>
        /// 生成类型
        /// </summary>
        public override int BuildTypeId
        {
            get { return GlobalIds.BuildType.DojoConfig; }
        }

        /// <summary>
        /// 唯一标识
        /// </summary>
        public override int Id
        {
            get { return GlobalIds.BuildType.DojoConfig + 1; }
        }

        /// <summary>
        /// 标题
        /// </summary>
        public override string Title
        {
            get { return "Dojo配置文件生成器"; }
        }
        #endregion

        /// <summary>
        /// 生成
        /// </summary>
        /// <param name="compile">DOJO编译器</param>
        /// <param name="doc">文档对象模型</param>
        public override void Build(CompileBase compile, ProjectDocument doc)
        {
            DojoCompile dojoCompile = (DojoCompile)compile;

            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            {
                JsonWriter jsonWriter = new JsonTextWriter(sw);
                jsonWriter.Formatting = Formatting.Indented;

                var frontConfig = dojoCompile.Project.Configures.OfType<FrontEndConfigure>().FirstOrDefault();

                //创建配置对象
                #region 
                jsonWriter.WriteStartObject();
                jsonWriter.WritePropertyName("ProjectId");
                jsonWriter.WriteValue(compile.ProjectId);
                jsonWriter.WritePropertyName("Name");
                jsonWriter.WriteValue(compile.Project.Identity);
                jsonWriter.WritePropertyName("Title");
                jsonWriter.WriteValue(compile.Project.Root.Title);
                jsonWriter.WritePropertyName("Desc");
                jsonWriter.WriteValue(compile.Project.Description);
                jsonWriter.WritePropertyName("Version");
                jsonWriter.WriteValue("1.0.0");
                jsonWriter.WritePropertyName("ServiceBaseUrl");
                jsonWriter.WriteValue(frontConfig.ServerUrl);
                jsonWriter.WritePropertyName("AuthServiceBaseUrl");
                jsonWriter.WriteValue(frontConfig.ServerUrl);
                jsonWriter.WritePropertyName("DefaultPage");
                jsonWriter.WriteValue(frontConfig.StartupScreen == null ? "MainScreen" : frontConfig.StartupScreen);
                jsonWriter.WritePropertyName("LoginPage");
                jsonWriter.WriteValue("Login");
                jsonWriter.WritePropertyName("DefaultThemes");
                jsonWriter.WriteValue("");
                #endregion
                #region 生成报表配置节
                var reportConfig = dojoCompile.Project.Configures.OfType<ReportServerConfigure>().FirstOrDefault();
                if (reportConfig != null)
                {
                    jsonWriter.WritePropertyName("ReportBaseUrl");
                    jsonWriter.WriteValue(reportConfig.Reports);

                    jsonWriter.WritePropertyName("ReportServerBaseUrl");
                    jsonWriter.WriteValue(reportConfig.ReportServer);
                }
                #endregion
                #region 生成所有屏幕数据
                jsonWriter.WritePropertyName("Screens");
                jsonWriter.WriteStartArray();
                this.BuildAllScreens(compile, jsonWriter);
                jsonWriter.WriteEndArray();
                #endregion
                #region 生成模型数据定义
                jsonWriter.WritePropertyName("Definition");
                //jsonWriter.WriteStartArray(); //1
                jsonWriter.WriteStartObject();
                this.BuildAllDefinition(compile, jsonWriter);
                jsonWriter.WriteEndObject();
                //jsonWriter.WriteEndArray(); //1
                #endregion

                jsonWriter.WriteEndObject();

                //获取JSON串
                string output = sw.ToString();
                jsonWriter.Close();
                sw.Close();

                //创建配置文件
                var file = Path.Combine(((DojoCompile)compile).OutputPath, "Config.json");
                if (File.Exists(file)) File.Delete(file);
                File.WriteAllText(file, output, System.Text.UTF8Encoding.UTF8);

                //全局生成
                var cFile = Path.Combine(((DojoCompile)compile).OutputPath, "Common.js");
                if (File.Exists(cFile)) File.Delete(cFile);
                string content = "define(function(){return {}});";
                File.WriteAllText(cFile, content, System.Text.UTF8Encoding.UTF8);
            }
        }

        #region 生成所有屏幕名称数据
        /// <summary>
        /// 生成所有屏幕名称数据
        /// </summary>
        /// <param name="compile">编译对象</param>
        /// <param name="jsonWriter">JsonWriter</param>
        private void BuildAllScreens(CompileBase compile, JsonWriter jsonWriter)
        {
            string retrunValue = string.Empty;
            DojoCompile dojoCompile = (DojoCompile)compile;
            Dictionary<int, int> screenPermissionData = dojoCompile.ScreenPermissionData;

            var screenItems = dojoCompile.ProjectItems.Where(a => a.Value.DocumentType == GlobalIds.DocumentType.Screen).ToList();
            foreach (var item in screenItems)
            {
                var doc = item.Value as ProjectDocument;
                if (doc != null)
                {
                    jsonWriter.WriteStartObject();
                    jsonWriter.WritePropertyName("Id");
                    jsonWriter.WriteValue(doc.Id);
                    jsonWriter.WritePropertyName("Name");
                    jsonWriter.WriteValue(doc.Name);
                    jsonWriter.WritePropertyName("Title");
                    jsonWriter.WriteValue(doc.Title);

                    if (screenPermissionData.ContainsKey(doc.Id))
                    {
                        var permissionId = (from t in screenPermissionData where t.Key == doc.Id select t.Value).FirstOrDefault();
                        jsonWriter.WritePropertyName("PermissionId");
                        jsonWriter.WriteValue(permissionId);
                    }

                    jsonWriter.WriteEndObject();
                }
            }
        }
        #endregion

        #region 生成模型数据定义
        /// <summary>
        /// 生成模型数据定义
        /// </summary>
        /// <param name="compile">编译对象</param>
        /// <param name="jsonWriter">JsonWriter</param>
        private void BuildAllDefinition(CompileBase compile, JsonWriter jsonWriter)
        {
            DojoCompile dojoCompile = compile as DojoCompile;
            var entityItem = dojoCompile.ProjectItems.Where(a => a.Value.DocumentType == GlobalIds.DocumentType.Entity).ToList();
            foreach (var item in entityItem)
            {
                //jsonWriter.WriteStartObject(); //1
                jsonWriter.WritePropertyName(item.Value.Name); jsonWriter.WriteStartObject();

                var def = dojoCompile.GetDocumentBody(item.Value) as EntityDefinition;

                #region 获取所有自定义外键
                StringBuilder sbAllForeigns = new StringBuilder();
                StringBuilder sbForeignKey = new StringBuilder();
                BuildCommonMethod.GetAllRelationForeignContent(item.Value.Propertys, def, dojoCompile, sbForeignKey);
                if (sbForeignKey.ToString().Length > 0)
                {
                    sbAllForeigns.Append(sbForeignKey.ToString());
                }
                #endregion

                #region EntityID
                jsonWriter.WritePropertyName("entityId"); jsonWriter.WriteValue(item.Key);
                #endregion

                #region Primary
                jsonWriter.WritePropertyName("primaryKey");
                jsonWriter.WriteStartObject();
                BuildPrimaryContent(item.Value.Propertys, def, dojoCompile, jsonWriter);
                jsonWriter.WriteEndObject();
                #endregion

                #region Fields 
                jsonWriter.WritePropertyName("fields");
                jsonWriter.WriteStartObject();
                BuildItemsContent(item.Value.Propertys, def, dojoCompile, jsonWriter, sbAllForeigns.ToString());
                jsonWriter.WriteEndObject();
                #endregion

                jsonWriter.WriteEndObject();
                //jsonWriter.WriteEndObject(); //1
            }
        }

        #region 生成主键（Config生成）
        /// <summary>
        /// 生成主键
        /// </summary>
        /// <param name="itemPropertys"></param>
        /// <param name="def"></param>
        /// <param name="dojoCompile"></param>
        /// <param name="jsonWriter"></param>
        private void BuildPrimaryContent(Dictionary<string, object> itemPropertys, EntityDefinition def, DojoCompile dojoCompile, JsonWriter jsonWriter)
        {
            var inheritEntityId = (from t in itemPropertys where t.Key == "InheritEntityId" select t.Value).FirstOrDefault();
            if (inheritEntityId != null)
            {
                var entityItems = dojoCompile.ProjectItems.Where(a => a.Value.DocumentType == GlobalIds.DocumentType.Entity && a.Key == int.Parse(inheritEntityId.ToString())).ToList();
                foreach (var item in entityItems)
                {
                    var defItem = dojoCompile.GetDocumentBody(item.Value) as EntityDefinition;
                    BuildPrimaryString(defItem, dojoCompile, jsonWriter);

                    //递归处理
                    BuildPrimaryContent(item.Value.Propertys, def, dojoCompile, jsonWriter);
                }
            }
            else
            {
                BuildPrimaryString(def, dojoCompile, jsonWriter);
            }
        }
        /// <summary>
        /// 生成主键
        /// </summary>
        /// <param name="def"></param>
        /// <param name="dojoCompile"></param>
        /// <param name="jsonWriter"></param>
        private void BuildPrimaryString(EntityDefinition def, DojoCompile dojoCompile, JsonWriter jsonWriter)
        {
            foreach (var primary in def.Members.OfType<PrimaryMember>())
            {
                dynamic type = primary.Content;
                jsonWriter.WritePropertyName(primary.Name);
                jsonWriter.WriteStartObject();
                jsonWriter.WritePropertyName("Title"); jsonWriter.WriteValue(primary.Title);
                jsonWriter.WritePropertyName("Type"); jsonWriter.WriteValue(BuildCommonMethod.GetTypeName(type.BaseType));
                jsonWriter.WriteEndObject();
            }
        }
        #endregion

        #region 生成字段
        /// <summary>
        /// 生成字段
        /// </summary>
        /// <param name="itemPropertys"></param>
        /// <param name="def"></param>
        /// <param name="dojoCompile"></param>
        /// <param name="jsonWriter"></param>
        private void BuildItemsContent(Dictionary<string, object> itemPropertys, EntityDefinition def, DojoCompile dojoCompile, JsonWriter jsonWriter, string AllForeigns)
        {
            var inheritEntityId = (from t in itemPropertys where t.Key == "InheritEntityId" select t.Value).FirstOrDefault();
            if (inheritEntityId != null)
            {
                var entityItems = dojoCompile.ProjectItems.Where(a => a.Value.DocumentType == GlobalIds.DocumentType.Entity && a.Key == int.Parse(inheritEntityId.ToString())).ToList();
                foreach (var item in entityItems)
                {
                    var defItem = dojoCompile.GetDocumentBody(item.Value) as EntityDefinition;
                    BuildFiledsString(defItem, dojoCompile, jsonWriter, AllForeigns);

                    //递归处理
                    BuildItemsContent(item.Value.Propertys, def, dojoCompile, jsonWriter, AllForeigns);
                }
            }
            else
            {
                BuildFiledsString(def, dojoCompile, jsonWriter, AllForeigns);
            }
        }
        /// <summary>
        /// 生成字段
        /// </summary>
        /// <param name="def"></param>
        /// <param name="dojoCompile"></param>
        /// <param name="jsonWriter"></param>
        private void BuildFiledsString(EntityDefinition def, DojoCompile dojoCompile, JsonWriter jsonWriter, string AllForeigns)
        {
            foreach (var member in def.Members)
            {
                if (member.MemberType == EMemberType.Calculate || member.MemberType == EMemberType.Column || member.MemberType == EMemberType.Common)
                {
                    var typeContent = (CommonDataType)member.Content;

                    //时间戳格式不生成
                    if (typeContent.BaseType == Foundation.EDataBaseType.Timestamp) continue;

                    //标识该字段是否是自定义外键
                    bool IsForeignKey = false;
                    if (AllForeigns.Length > 0)
                    {
                        string[] foreigns = AllForeigns.Split(',');
                        for (var i = 0; i < foreigns.Length; i++)
                        {
                            if (foreigns[i] == member.Name)
                            {
                                IsForeignKey = true;
                                break;
                            }
                        }
                    }
                    jsonWriter.WritePropertyName(member.Name);
                    jsonWriter.WriteStartObject();
                    jsonWriter.WritePropertyName("Title"); jsonWriter.WriteValue(member.Title);
                    jsonWriter.WritePropertyName("Type"); jsonWriter.WriteValue(BuildCommonMethod.GetTypeName(typeContent.BaseType));
                    jsonWriter.WritePropertyName("DataType"); jsonWriter.WriteValue(typeContent.BaseType.ToString());
                    #region IsRequired
                    if (member.MemberType == EMemberType.Calculate)
                    {
                        var memberBase = member as ColumnMember;
                        jsonWriter.WritePropertyName("IsRequired"); jsonWriter.WriteValue(memberBase.IsRequired);
                    }
                    else if (member.MemberType == EMemberType.Column)
                    {
                        var memberBase = member as ColumnMember;
                        jsonWriter.WritePropertyName("IsRequired"); jsonWriter.WriteValue(memberBase.IsRequired);
                    }
                    else if (member.MemberType == EMemberType.Common)
                    {
                        var memberBase = member as CommonMember;
                        jsonWriter.WritePropertyName("IsRequired"); jsonWriter.WriteValue(memberBase.IsRequired);
                    }
                    #endregion

                    if (IsForeignKey)
                    {
                        jsonWriter.WritePropertyName("IsForeignKey"); jsonWriter.WriteValue(IsForeignKey);
                    }
                    bool serverRefresh = false;
                    if (member.MemberType == EMemberType.Column)
                    {
                        if ((member as ColumnMember).GenerateMode != EColumnGenerateMode.None)
                        {
                            serverRefresh = true;
                        }
                    }
                    jsonWriter.WritePropertyName("ServerRefresh"); jsonWriter.WriteValue(serverRefresh);

                    jsonWriter.WriteEndObject();
                }
                else if (member.MemberType == EMemberType.Navigation)
                {
                    var navigationMember = member as NavigationMember;
                    string toEntity = (from t in dojoCompile.ProjectItems where t.Key == navigationMember.ToEntityId select t.Value).FirstOrDefault().Name;
                    jsonWriter.WritePropertyName(navigationMember.Name);
                    jsonWriter.WriteStartObject();
                    jsonWriter.WritePropertyName("Title"); jsonWriter.WriteValue(navigationMember.Title);
                    jsonWriter.WritePropertyName("Type"); jsonWriter.WriteValue(navigationMember.ToCardinality == EMappingCardinality.Many ? "array" : "object");
                    jsonWriter.WritePropertyName("DataType"); jsonWriter.WriteValue(navigationMember.ToCardinality == EMappingCardinality.Many ? "array" : "object");
                    jsonWriter.WritePropertyName("ToName"); jsonWriter.WriteValue(navigationMember.ToName);
                    jsonWriter.WritePropertyName("ToEntity"); jsonWriter.WriteValue(toEntity);
                    bool isSingleRelation = false;
                    if (string.IsNullOrEmpty(navigationMember.ToName)) { isSingleRelation = true; }
                    jsonWriter.WritePropertyName("SingleRelation"); jsonWriter.WriteValue(isSingleRelation);
                    if (navigationMember.ToCardinality != EMappingCardinality.Many)
                    {
                        string navPrimaryKey = BuildCommonMethod.GetNavPrimaryContent(def, dojoCompile, toEntity);
                        string foreignKey = BuildCommonMethod.GetRealForeignKey(navigationMember, toEntity, navPrimaryKey);
                        if (navigationMember.Cardinality == EMappingCardinality.One && navigationMember.ToCardinality == EMappingCardinality.One)
                        {
                            foreignKey = navPrimaryKey;
                        }
                        jsonWriter.WritePropertyName("RelationForeignKey");
                        jsonWriter.WriteStartArray();
                        jsonWriter.WriteValue("" + foreignKey + "");
                        jsonWriter.WriteEndArray();
                    }
                    jsonWriter.WriteEndObject();
                }
            }
        }
        #endregion
        #endregion
    }
}
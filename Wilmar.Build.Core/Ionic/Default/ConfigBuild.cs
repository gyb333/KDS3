using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Wilmar.Build.Core.Ionic.Default.Template;
using Wilmar.Compile.Core.Ionic;
using Wilmar.Foundation.Common;
using Wilmar.Foundation.Projects;
using Wilmar.Model.Core.Definitions;
using Wilmar.Model.Core.Definitions.Configure;
using Wilmar.Service.Common.Generate;

namespace Wilmar.Build.Core.Ionic.Default
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
            get { return GlobalIds.BuildType.IonicConfig; }
        }

        /// <summary>
        /// 唯一标识
        /// </summary>
        public override int Id
        {
            get { return GlobalIds.BuildType.IonicConfig + 1; }
        }

        /// <summary>
        /// 标题
        /// </summary>
        public override string Title
        {
            get { return "Ionic配置文件生成器"; }
        }
        #endregion

        public override void Build(CompileBase compile, ProjectDocument doc)
        {
            IonicCompile ionicCompile = compile as IonicCompile;
            BuildProjectConfig(ionicCompile);
            BuildPlatformConfig(ionicCompile);
            BuildEntityDefinition(ionicCompile);
        }

        #region 基础配置文件
        /// <summary>
        /// 生成项目配置文件
        /// </summary>
        /// <param name="ionicCompile"></param>
        private void BuildProjectConfig(IonicCompile ionicCompile)
        {
            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            {
                JsonWriter jsonWriter = new JsonTextWriter(sw);
                jsonWriter.Formatting = Formatting.Indented;

                var frontConfig = ionicCompile.Project.Configures.OfType<FrontEndConfigure>().FirstOrDefault();
                //创建配置对象
                jsonWriter.WriteStartObject();
                #region 
                jsonWriter.WritePropertyName("projectid");
                jsonWriter.WriteValue(ionicCompile.ProjectId);
                jsonWriter.WritePropertyName("name");
                jsonWriter.WriteValue(ionicCompile.Project.Identity);
                jsonWriter.WritePropertyName("title");
                jsonWriter.WriteValue(ionicCompile.Project.Root.Title);
                jsonWriter.WritePropertyName("desc");
                jsonWriter.WriteValue(ionicCompile.Project.Description);
                jsonWriter.WritePropertyName("api_endpoint");
                jsonWriter.WriteValue(frontConfig.ServerUrl+"odata"+"/"+ ionicCompile.Project.Identity);
                #endregion
                jsonWriter.WriteEndObject();

                //获取JSON串
                string output = sw.ToString();
                jsonWriter.Close();
                sw.Close();

                var targetDirectory = new System.IO.FileInfo(new Uri(this.GetType().Assembly.CodeBase).AbsolutePath).Directory.FullName;
                string outputPath = Path.Combine(targetDirectory.Replace("\\Wilmar.Service\\bin\\Extension", ""), @"Wilmar.Mobile\src\config", ionicCompile.Project.Identity);
                outputPath = HttpUtility.UrlDecode(outputPath);
                if (!Directory.Exists(outputPath)) Directory.CreateDirectory(outputPath);

                var file = Path.Combine(outputPath, ionicCompile.Project.Identity + ".config.ts");
                if (File.Exists(file)) File.Delete(file);
                string content = string.Format("export const {0}_CONFIG=", ionicCompile.Project.Identity.ToUpper()) + output;
                File.WriteAllText(file, content, System.Text.UTF8Encoding.UTF8);

                var devFile = Path.Combine(outputPath, ionicCompile.Project.Identity + ".config.dev.ts");
                if (File.Exists(devFile)) File.Delete(devFile);
                string devContent = string.Format("export const {0}_CONFIG=", ionicCompile.Project.Identity.ToUpper()) + output;
                File.WriteAllText(devFile, devContent, System.Text.UTF8Encoding.UTF8);
            }
        }
        /// <summary>
        /// 生成平台配置文件
        /// </summary>
        /// <param name="ionicCompile"></param>
        private void BuildPlatformConfig(IonicCompile ionicCompile)
        {
            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            {
                JsonWriter jsonWriter = new JsonTextWriter(sw);
                jsonWriter.Formatting = Formatting.Indented;

                var frontConfig = ionicCompile.Project.Configures.OfType<FrontEndConfigure>().FirstOrDefault();
                //创建配置对象
                jsonWriter.WriteStartObject();
                #region 
                jsonWriter.WritePropertyName("name");
                jsonWriter.WriteValue("kds3.0 Mobile");
                jsonWriter.WritePropertyName("title");
                jsonWriter.WriteValue("kds3.0 移动端");
                jsonWriter.WritePropertyName("version");
                jsonWriter.WriteValue("1.0.0");
                jsonWriter.WritePropertyName("desc");
                jsonWriter.WriteValue("kds3.0 移动端平台系统");
                jsonWriter.WritePropertyName("logo");
                jsonWriter.WriteValue("kds3.0 logo");

                jsonWriter.WritePropertyName("startup_page");
                jsonWriter.WriteValue(ionicCompile.Project.Identity);
                jsonWriter.WritePropertyName("login_page");
                jsonWriter.WriteValue("Login");

                jsonWriter.WritePropertyName("auth_endpoint");
                jsonWriter.WriteValue(frontConfig.ServerUrl);
                #endregion
                jsonWriter.WriteEndObject();

                //获取JSON串
                string output = sw.ToString();
                jsonWriter.Close();
                sw.Close();

                var targetDirectory = new System.IO.FileInfo(new Uri(this.GetType().Assembly.CodeBase).AbsolutePath).Directory.FullName;
                string outputPath = Path.Combine(targetDirectory.Replace("\\Wilmar.Service\\bin\\Extension", ""), @"Wilmar.Mobile\src\config", "platform");
                outputPath = HttpUtility.UrlDecode(outputPath);
                var file = Path.Combine(outputPath, "platform.config.ts");
                if (!Directory.Exists(outputPath)) Directory.CreateDirectory(outputPath);
                if (File.Exists(file)) File.Delete(file);
                string content = string.Format("export const PLATFORM_CONFIG=") + output;
                File.WriteAllText(file, content, System.Text.UTF8Encoding.UTF8);
            }
        }
        #endregion

        #region 实体对应模型
        /// <summary>
        /// 生成实体对应模型
        /// </summary>
        /// <param name="ionicCompile"></param>
        private void BuildEntityDefinition(IonicCompile ionicCompile)
        {
            var targetDirectory = new System.IO.FileInfo(new Uri(this.GetType().Assembly.CodeBase).AbsolutePath).Directory.FullName;
            string outputPath = Path.Combine(targetDirectory.Replace("\\Wilmar.Service\\bin\\Extension", ""), @"Wilmar.Mobile\src\projects", ionicCompile.Project.Identity, "models");
            outputPath = HttpUtility.UrlDecode(outputPath);
            if (!Directory.Exists(outputPath)) Directory.CreateDirectory(outputPath);
            //生成实体文件
            var entityItems = ionicCompile.ProjectItems.Where(a => a.Value.DocumentType == GlobalIds.DocumentType.Entity).ToList();
            foreach (var entityItem in entityItems)
            {
                ProjectDocument doc = entityItem.Value as ProjectDocument;
                var def = ionicCompile.GetDocumentBody(entityItem.Value) as EntityDefinition;
                var file = Path.Combine(outputPath, doc.Name + ".ts");
                if (File.Exists(file)) File.Delete(file);
                string content = new ModelTemplate(ionicCompile, doc, def).TransformText();
                File.WriteAllText(file, content, System.Text.UTF8Encoding.UTF8);
            }
            //生成Container文件
            var ContainerFile = Path.Combine(outputPath, ionicCompile.Project.Identity + ".container.ts");
            if (File.Exists(ContainerFile)) File.Delete(ContainerFile);
            string Container = new PageContainerTemplate(ionicCompile).TransformText();
            File.WriteAllText(ContainerFile, Container, System.Text.UTF8Encoding.UTF8);
        }
        #endregion
    }
}

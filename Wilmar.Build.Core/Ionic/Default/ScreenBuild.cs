using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using Wilmar.Compile.Core.Ionic;
using Wilmar.Foundation.Common;
using Wilmar.Foundation.Projects;
using Wilmar.Model.Core.Definitions;
using Wilmar.Model.Core.Definitions.Screens;
using Wilmar.Service.Common.Generate;
using Wilmar.Build.Core.Ionic.Default.Builders;
using Wilmar.Build.Core.Ionic.Default.Template;
using System.Web;

namespace Wilmar.Build.Core.Ionic.Default
{
    /// <summary>
    /// 屏幕生成器
    /// </summary>
    public class ScreenBuild : BuildBase
    {
        #region 
        /// <summary>
        /// 生成类型
        /// </summary>
        public override int BuildTypeId
        {
            get { return GlobalIds.BuildType.IonicScreen; }
        }

        /// <summary>
        /// 唯一标识
        /// </summary>
        public override int Id
        {
            get { return GlobalIds.BuildType.IonicScreen + 1; }
        }

        /// <summary>
        /// 标题
        /// </summary>
        public override string Title
        {
            get { return "Ionic屏幕生成器"; }
        }
        #endregion

        /// <summary>
        /// 生成逻辑
        /// </summary>
        /// <param name="compile">DOJO编译器</param>
        /// <param name="doc">文档对象模型</param>
        public override void Build(CompileBase compile, ProjectDocument doc)
        {
            var docBody = compile.GetDocumentBody(doc);
            var screenDefinition = docBody as ScreenDefinition;
            if (screenDefinition != null)
            {
                ControlHost controlHost = screenDefinition.Root;
                BuildModuleHtml(compile, screenDefinition, doc);
                BuildHtml(controlHost, compile, screenDefinition, doc);
            }
        }

        /// <summary>
        /// 生成Module页面HTML
        /// </summary>
        /// <param name="compile"></param>
        /// <param name="doc"></param>
        private void BuildModuleHtml(CompileBase compile, ScreenDefinition screenDef, ProjectDocument doc)
        {
            if (doc.Name.ToLower() == "startuppage")
            {
                var ionicCompile = compile as IonicCompile;
                var targetDirectory = new System.IO.FileInfo(new Uri(this.GetType().Assembly.CodeBase).AbsolutePath).Directory.FullName;
                string outputPath = Path.Combine(targetDirectory.Replace("\\Wilmar.Service\\bin\\Extension", ""), @"Wilmar.Mobile\src\projects", ionicCompile.Project.Identity);
                outputPath = HttpUtility.UrlDecode(outputPath);
                if (!Directory.Exists(outputPath)) Directory.CreateDirectory(outputPath);

                //Module页面
                var fileModulePage = Path.Combine(outputPath, ionicCompile.Project.Identity + ".module.ts");
                if (File.Exists(fileModulePage)) File.Delete(fileModulePage);
                var contentModulePage = new ModuleTemplate(ionicCompile, doc).TransformText();
                File.WriteAllText(fileModulePage, contentModulePage, System.Text.UTF8Encoding.UTF8);

                //生成Mian页面TS
                var fileMainPage = Path.Combine(outputPath, ionicCompile.Project.Identity + ".ts");
                if (File.Exists(fileMainPage)) File.Delete(fileMainPage);
                var contentMainPage = new PageMainTemplate(ionicCompile, screenDef, doc).TransformText();
                File.WriteAllText(fileMainPage, contentMainPage, System.Text.UTF8Encoding.UTF8);
                //生成Mian页面CSS
                var fileMainCss = Path.Combine(outputPath, ionicCompile.Project.Identity + ".scss");
                if (File.Exists(fileMainCss)) File.Delete(fileMainCss);
                string contentMainCss = string.Empty;
                File.WriteAllText(fileMainCss, contentMainCss, System.Text.UTF8Encoding.UTF8);
            }
        }

        /// <summary>
        /// 生成HTML
        /// </summary>
        /// <param name="name">屏幕名称</param>
        /// <param name="controlHost">控件基类</param>
        /// <param name="compile">编译基类</param>
        /// <param name="screenDef">屏幕定义</param>
        /// <param name="doc">项目文档</param>
        private void BuildHtml(ControlHost controlHost, CompileBase compile, ScreenDefinition screenDef, ProjectDocument doc)
        {
            using (var writer = new StringWriter())
            {
                var ionicCompile = compile as IonicCompile;
                Dictionary<int, Tuple<int, string>> itemPermissionData = new Dictionary<int, Tuple<int, string>>();

                var xmlWriter = new HtmlTextWriter(writer);
                var builder = controlHost.GetBuilder(false, screenDef, compile, doc, itemPermissionData, xmlWriter);
                builder.Build();

                //生成文件全路径
                string fileName = doc.Name;
                string outputPath = ionicCompile.OutputPath + "\\" + fileName;
                if (!Directory.Exists(outputPath)) Directory.CreateDirectory(outputPath);
                var file = Path.Combine(outputPath, fileName + ".html");
                if (File.Exists(file)) File.Delete(file);
                //创建HTML文件
                File.WriteAllText(file, writer.ToString(), System.Text.UTF8Encoding.UTF8);

                //启动屏幕名字暂时固定
                //projets/pages中启动屏幕不生成TS文件，TS文件在外部生成
                if (fileName.ToLower() != "startuppage")
                {
                    //生成JS
                    this.BuildJs(compile, screenDef, doc);
                    //生成CSS
                    this.BuildCss(compile, screenDef, doc);
                }
            }
        }
        /// <summary>
        /// 生成JS
        /// </summary>
        /// <param name="compile"></param>
        /// <param name="doc"></param>
        private void BuildJs(CompileBase compile, ScreenDefinition screenDef, ProjectDocument doc)
        {
            var ionicCompile = compile as IonicCompile;
            string fileName = doc.Name;
            string outputPath = ionicCompile.OutputPath + "\\" + fileName;
            if (!Directory.Exists(outputPath)) Directory.CreateDirectory(outputPath);
            var file = Path.Combine(outputPath, fileName + ".ts");
            if (File.Exists(file)) File.Delete(file);

            string content = new PageScriptTemplate(ionicCompile, screenDef, doc).TransformText();
            //创建HTML文件
            File.WriteAllText(file, content, System.Text.UTF8Encoding.UTF8);
        }
        /// <summary>
        /// 生成CSS
        /// </summary>
        /// <param name="compile"></param>
        /// <param name="screenDef"></param>
        /// <param name="doc"></param>
        private void BuildCss(CompileBase compile, ScreenDefinition screenDef, ProjectDocument doc)
        {
            var ionicCompile = compile as IonicCompile;
            string fileName = doc.Name;
            string outputPath = ionicCompile.OutputPath + "\\" + fileName;
            if (!Directory.Exists(outputPath)) Directory.CreateDirectory(outputPath);
            var file = Path.Combine(outputPath, fileName + ".scss");
            if (File.Exists(file)) File.Delete(file);
            string content = string.Empty;
            //创建HTML文件
            File.WriteAllText(file, content, System.Text.UTF8Encoding.UTF8);
        }
    }
}

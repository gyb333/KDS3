using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using Wilmar.Foundation.Common;
using Wilmar.Foundation.Projects;
using Wilmar.Foundation.Projects.Configures;
using Wilmar.Service.Common.Generate;
using Wilmar.Service.Common.ProjectBase;
using Wilmar.Service.Common;
using Wilmar.Compile.Core.Service.Models;
using System.Text.RegularExpressions;
using System.Configuration;

namespace Wilmar.Compile.Core.Service
{
    /// <summary>
    /// 项目服务编译器
    /// </summary>
    public class ServiceCompile : CompileBase
    {
        #region Field
        private static int[] _BuildTypes = new int[] {
            GlobalIds.BuildType.Entity,
            GlobalIds.BuildType.DataContext,
            GlobalIds.BuildType.EntityControllter,
            GlobalIds.BuildType.Service,
            GlobalIds.BuildType.QueryControllter,
        };
        private readonly static string RootPath = HostingEnvironment.MapPath("~/bin");
        private readonly static string ProjectPath = HostingEnvironment.MapPath("~/bin/Projects");
        private readonly static DirectoryInfo ExtensionDirecotry = new DirectoryInfo(HostingEnvironment.MapPath("~/bin/Dependency"));
        private static string[] ReferencedAssemblies = new string[]
        {
            "System.Xml.dll"
            , "System.dll"
            , "System.Web.Extensions.dll"
            , "System.Core.dll"
            , "System.Data.dll"
            , "System.Net.Http.dll"
            , "System.Activities.dll"
            , "System.ComponentModel.DataAnnotations.dll"
            , Path.Combine(RootPath, "NLog.dll")
            , Path.Combine(RootPath, "Wilmar.Foundation.dll")
            , Path.Combine(RootPath, "Wilmar.Service.Common.dll")
            , Path.Combine(RootPath, "Microsoft.AspNet.Identity.Core.dll")
            , Path.Combine(RootPath, "System.Web.OData.dll")
            , Path.Combine(RootPath, "System.Web.Http.dll")
            , Path.Combine(RootPath, "EntityFramework.dll") 
            , Path.Combine(RootPath, "Microsoft.OData.Edm.dll")
            , Path.Combine(RootPath, "Newtonsoft.Json.dll")
            , Path.Combine(RootPath, "SsasEntityFrameworkProvider.Attributes.dll")
        };

        static ServiceCompile()
        {
            var outDir = ConfigurationManager.AppSettings["ProjectOutDir"];
            if (outDir != null)
            {
                ProjectPath = outDir.ToString();
            }
        }
        /// <summary>
        /// Action权限
        /// </summary>
        public const short PermissionAction = 0x11;
        /// <summary>
        /// 实体修改属性权限
        /// </summary>
        public const short PermissionModify = 0x12;
        /// <summary>
        /// 实体查询属性权限
        /// </summary>
        public const short PermissionSelect = 0x13;
        /// <summary>
        /// 编译文件集合
        /// </summary>
        private readonly List<CompileFile> CompileFiles = new List<CompileFile>();

        private readonly static bool IsFileCompile = true;
        #endregion

        #region Override
        /// <summary>
        /// 编译器ID
        /// </summary>
        public override int Id
        {
            get { return GlobalIds.Compile.Service; }
        }
        /// <summary>
        /// 文档生成类型ID集合
        /// </summary>
        public override int[] BuildTypes
        {
            get { return _BuildTypes; }
        }
        /// <summary>
        /// 编译器标题
        /// </summary>
        public override string Title
        {
            get { return "项目服务编译器"; }
        }
        /// <summary>
        /// 开始编译（系统自动调用）
        /// </summary>
        public override void BeginCompile()
        {
            if (!Directory.Exists(ProjectPath))
                Directory.CreateDirectory(ProjectPath);
            this.Identity = Project.Identity.ToUpper();
            this.Configure = new ProjectConfigure();
            this.Configure.DataSources = new Dictionary<int, DatabaseConfigureItem>();
            foreach (var dbConfig in Project.Configures.OfType<DatabaseConfigure>())
            {
                foreach (var temp in dbConfig.Children)
                {
                    this.Configure.DataSources.Add(temp.Id, temp);
                }
            }
            Permission = GlobalServices.SecurityService.CreatePermissionContext(this);
            Data = new ProjectMetadata(this);
        }
        /// <summary>
        /// 完成编译（系统自动调用）
        /// </summary>
        public override void EndCompile()
        {
            try
            {
                var compiler = new CSharpCodeProvider();
                var parameters = new CompilerParameters()
                {
                    GenerateExecutable = false,
                    GenerateInMemory = false,
                    IncludeDebugInformation = true,
                    OutputAssembly = Path.Combine(ProjectPath, string.Format("Wilmar.Project.{0}.dll", this.Project.Identity.ToUpper()))
                };
                parameters.TempFiles.KeepFiles = true;
                parameters.ReferencedAssemblies.AddRange(ReferencedAssemblies);
                if (ExtensionDirecotry.Exists)
                {
                    parameters.ReferencedAssemblies.Add(ExtensionDirecotry.FullName+"\\Wilmar.Interface.Common.dll");
                   
                    foreach (var file in ExtensionDirecotry.GetFiles($"Wilmar.Custom.{Project.Identity.ToUpper()}*.dll"))
                    {
                        if (Regex.IsMatch(file.Name, $"Wilmar\\.Custom\\.{Project.Identity.ToUpper()}\\d+\\.dll"))
                            parameters.ReferencedAssemblies.Add(file.FullName);
                    }
                }
                CompilerResults result = null;
                if (IsFileCompile)
                {
                    List<string> filenames = new List<string>();
                    string root = Path.Combine(Path.GetTempPath(), "PlatformGenerateCode", Project.Identity + DateTime.Now.Ticks.ToString());
                    if (Directory.Exists(root))
                        Directory.Delete(root, true);
                    foreach (var item in CompileFiles)
                    {
                        var file = new FileInfo(Path.Combine(root, item.Category, item.Name + ".cs"));
                        if (!file.Directory.Exists)
                            file.Directory.Create();
                        File.WriteAllText(file.FullName, item.Content);
                        filenames.Add(file.FullName);
                    }
                    result = compiler.CompileAssemblyFromFile(parameters, filenames.ToArray());
                }
                else
                {
                    result = compiler.CompileAssemblyFromSource(parameters, CompileFiles.Select(a => a.Content).ToArray());
                }

                if (result.Errors.Count > 0)
                {
                    StringBuilder errsb = new StringBuilder();
                    foreach (CompilerError item in result.Errors)
                    {
                        if (!item.IsWarning)
                            errsb.AppendLine(string.Format("{0} {1}", item.FileName, item.ErrorText));
                    }
                    throw UtilityException.Compile(errsb.ToString());
                }
                else
                {
                    File.WriteAllText(
                        Path.Combine(ProjectPath,
                        string.Format("Wilmar.Project.{0}.configure",
                        this.Project.Identity.ToUpper())),
                        Utility.Serialize(Configure),
                        Encoding.UTF8);
                    Permission.Commit();
                }
            }
            catch (Exception ex)
            {
                throw UtilityException.Compile(ex.Message);
            }
        }
        #endregion

        /// <summary>
        /// 生成代码项目
        /// </summary>
        /// <param name="category"></param>
        /// <param name="name"></param>
        /// <param name="content"></param>
        public void GenerateCode(string category, string name, string content)
        {
            CompileFiles.Add(new CompileFile()
            {
                Category = category,
                Name = name,
                Content = content
            });
        }

        /// <summary>
        /// 项目标识
        /// </summary>
        public string Identity { get; private set; }
        /// <summary>
        /// 权限上下文内容
        /// </summary>
        public IPermissionContext Permission { get; private set; }
        /// <summary>
        /// 项目配置信息
        /// </summary>
        public ProjectConfigure Configure { get; private set; }
        /// <summary>
        /// 编译数据对象
        /// </summary>
        public ProjectMetadata Data { get; private set; }

        #region Private Type
        /// <summary>
        /// 编译文件对象
        /// </summary>
        private struct CompileFile
        {
            public string Category;
            public string Name;
            public string Content;
        }
        #endregion
    }
}

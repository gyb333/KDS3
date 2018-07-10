using System;
using System.IO;
using System.Web;
using System.Collections.Generic;
using Wilmar.Foundation.Common;
using Wilmar.Service.Common;
using Wilmar.Service.Common.Generate;
using Wilmar.Model.Core.Definitions.Entities;
using Wilmar.Foundation.Projects;
using Newtonsoft.Json;
using Wilmar.Model.Core.Definitions;
using Wilmar.Model.Core.Definitions.Entities.Members;
using System.Text;
using System.Linq;
using Wilmar.Model.Core.Definitions.Configure;
using Wilmar.Foundation;

namespace Wilmar.Compile.Core.Dojo
{
    /// <summary>
    /// DOJO编译器
    /// </summary>
    public class DojoCompile : TerminalCompileBase
    {
        #region Field
        private readonly static int[] _BuildTypes = new int[] {
            GlobalIds.BuildType.DojoIndex,
            GlobalIds.BuildType.DojoPreviewScreen,
            GlobalIds.BuildType.DojoPreviewIndex,
            GlobalIds.BuildType.DojoScreen,
            GlobalIds.BuildType.DojoConfig
        };
        /// <summary>
        /// 屏幕访问权限
        /// </summary>
        public const short PermissionScreen = 0x21;
        /// <summary>
        /// 控件可见权限
        /// </summary>
        public const short PermissionVisible = 0x22;
        /// <summary>
        /// 控件可写权限
        /// </summary>
        public const short PermissionOperate = 0x23;
        /// <summary>
        /// 屏幕范围自定义权限
        /// </summary>
        public const short PermissionCustom = 0x24;
        #endregion
        #region Override
        /// <summary>
        /// 唯一标识
        /// </summary>
        public override int Id
        {
            get { return GlobalIds.Compile.DojoPC; }
        }
        /// <summary>
        /// 标题
        /// </summary>
        public override string Title
        {
            get { return "Dojo桌面框架编译器"; }
        }
        /// <summary>
        /// 生成类型
        /// </summary>
        public override int[] BuildTypes
        {
            get { return _BuildTypes; }
        }
        /// <summary>
        /// 编译器器开始变以前处理逻辑
        /// </summary>
        public override void BeginCompile()
        {
            ScreenPermissionData = new Dictionary<int, int>();
            var targetDirectory = new System.IO.FileInfo(new Uri(this.GetType().Assembly.CodeBase).AbsolutePath).Directory.FullName;
            OutputPath = Path.Combine(targetDirectory.Replace("\\Wilmar.PlatformService\\bin\\Extension", "").Replace("\\Wilmar.Service\\bin\\Extension", ""), @"Wilmar.Terminate\Projects", this.Project.Identity);
            OutputPath = HttpUtility.UrlDecode(OutputPath);
            if (!Directory.Exists(OutputPath))
            {
                Directory.CreateDirectory(OutputPath);
            }
            Permission = GlobalServices.SecurityService.CreatePermissionContext(this);

            //屏幕权限操作
            var screenItems = ProjectItems.Where(a => a.Value.DocumentType == GlobalIds.DocumentType.Screen).ToList();
            foreach (var item in screenItems)
            {
                var screen = GetDocumentBody(item.Value) as ScreenDefinition;
                if (screen != null && screen.PermissionConfigure.EnableAuth)
                {
                    var doc = item.Value as ProjectDocument;
                    int screenPermissionId = this.Permission.Write(doc.Id, DojoCompile.PermissionScreen, doc.Id.ToString(), doc.Title + "的屏幕权限", doc.Title);
                    this.ScreenPermissionData.Add(doc.Id, screenPermissionId);
                }
            }
        }
        /// <summary>
        /// 编译器编译完成后处理逻辑
        /// </summary>
        public override void EndCompile()
        {
            Permission.Commit();
        }
        #endregion
        /// <summary>
        /// 权限上下文内容
        /// </summary>
        public IPermissionContext Permission { get; private set; }
        /// <summary>
        /// 输出路径
        /// </summary>
        public string OutputPath { get; private set; }
        /// <summary>
        /// 屏幕权限数据 //屏幕ID，权限ID
        /// </summary>
        public Dictionary<int, int> ScreenPermissionData { get; private set; }
    }
}

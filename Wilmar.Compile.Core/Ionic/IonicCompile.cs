using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Wilmar.Foundation.Common;
using Wilmar.Service.Common.Generate;

namespace Wilmar.Compile.Core.Ionic
{
    public class IonicCompile : CompileBase
    {
        private readonly static int[] _BuildTypes = new int[] {
            GlobalIds.BuildType.IonicIndex,
            GlobalIds.BuildType.IonicPreviewScreen,
            GlobalIds.BuildType.IonicPreviewIndex,
            GlobalIds.BuildType.IonicScreen,
            GlobalIds.BuildType.IonicConfig
        };

        public override int[] BuildTypes
        {
            get { return _BuildTypes; }
        }

        public override int Id
        {
            get { return GlobalIds.Compile.IonicMobile; }
        }

        public override string Title
        {
            get { return "Ionic移动框架编译器"; }
        }

        public override void BeginCompile()
        {
            var targetDirectory = new System.IO.FileInfo(new Uri(this.GetType().Assembly.CodeBase).AbsolutePath).Directory.FullName;
            OutputPath = Path.Combine(targetDirectory.Replace("\\Wilmar.Service\\bin\\Extension", ""), @"Wilmar.Mobile\src\projects", this.Project.Identity, "pages");
            OutputPath = HttpUtility.UrlDecode(OutputPath);
            if (!Directory.Exists(OutputPath))
            {
                Directory.CreateDirectory(OutputPath);
            }
        }

        public override void EndCompile()
        {
            
        }

        /// <summary>
        /// 输出路径
        /// </summary>
        public string OutputPath { get; private set; }
    }
}

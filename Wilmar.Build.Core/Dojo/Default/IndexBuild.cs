using System.IO;
using Wilmar.Build.Core.Dojo.Default.Templates;
using Wilmar.Compile.Core.Dojo;
using Wilmar.Foundation.Common;
using Wilmar.Service.Common.Generate;

namespace Wilmar.Build.Core.Dojo.Default
{
    /// <summary>
    /// 主页生成器
    /// </summary>
    public class IndexBuild : BuildBase
    {
        /// <summary>
        /// 生成
        /// </summary>
        /// <param name="compile">DOJO编译器</param>
        /// <param name="doc">文档对象模型</param>
        public override void Build(CompileBase compile, Foundation.Projects.ProjectDocument doc)
        {
            var indexPath = Path.Combine(((DojoCompile)compile).OutputPath, "Index.html");
            File.WriteAllText(indexPath, new IndexTemplate().TransformText(), System.Text.UTF8Encoding.UTF8);

            //Error
            var errorPath = Path.Combine(((DojoCompile)compile).OutputPath, "Error.html");
            File.WriteAllText(errorPath, new ErrorTemplate(compile).TransformText(), System.Text.UTF8Encoding.UTF8);

            //NoAuthorized
            var noAuthorizedPath = Path.Combine(((DojoCompile)compile).OutputPath, "NoAuthorized.html");
            File.WriteAllText(noAuthorizedPath, new NoAuthorizedTemplate(compile).TransformText(), System.Text.UTF8Encoding.UTF8);
        }

        /// <summary>
        /// 生成类型
        /// </summary>
        public override int BuildTypeId
        {
            get { return GlobalIds.BuildType.DojoIndex; }
        }

        /// <summary>
        /// 唯一标识
        /// </summary>
        public override int Id
        {
            get { return GlobalIds.BuildType.DojoIndex + 1; }
        }

        /// <summary>
        /// 标题
        /// </summary>
        public override string Title
        {
            get { return "Dojo项目主页生成器"; }
        }
    }
}

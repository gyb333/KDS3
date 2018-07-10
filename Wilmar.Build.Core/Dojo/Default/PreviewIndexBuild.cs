using System.IO;
using Wilmar.Build.Core.Dojo.Default.Templates;
using Wilmar.Compile.Core.Dojo;
using Wilmar.Foundation.Common;
using Wilmar.Service.Common.Generate;

namespace Wilmar.Build.Core.Dojo.Default
{
    /// <summary>
    /// 预览主页生成器
    /// </summary>
    public class PreviewIndexBuild : BuildBase
    {
        public override void Build(CompileBase compile, Foundation.Projects.ProjectDocument doc)
        {
            var previewPath = Path.Combine(((DojoCompile)compile).OutputPath, "Preview.html");
            File.WriteAllText(previewPath, new PreviewIndexTemplate().TransformText(), System.Text.UTF8Encoding.UTF8);
        }

     
        /// <summary>
        /// 生成类型
        /// </summary>
        public override int BuildTypeId
        {
            get { return GlobalIds.BuildType.DojoPreviewIndex; }
        }
        
        /// <summary>
        /// 唯一标识
        /// </summary>
        public override int Id
        {
            get { return GlobalIds.BuildType.DojoPreviewIndex + 1; }
        }

        /// <summary>
        /// 标题
        /// </summary>
        public override string Title
        {
            get { return "Dojo预览主页生成器"; }
        }
    }
}

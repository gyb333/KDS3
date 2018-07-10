using Wilmar.Foundation.Common;
using Wilmar.Service.Common.Generate;

namespace Wilmar.Compile.Core.Dojo
{
    /// <summary>
    /// 预览主页生成类型
    /// </summary>
    public class PreviewIndexBuildType : BuildTypeBase
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public override int Id
        {
            get { return GlobalIds.BuildType.DojoPreviewIndex; }
        }

        /// <summary>
        /// 标题
        /// </summary>
        public override string Title
        {
            get { return "预览主页生成"; }
        }
    }
}

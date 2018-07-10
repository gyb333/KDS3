using Wilmar.Foundation.Common;
using Wilmar.Service.Common.Generate;

namespace Wilmar.Compile.Core.Dojo
{
    /// <summary>
    /// 屏幕预览生成类型
    /// </summary>
    public class PreviewScreenBuildType : BuildTypeBase
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public override int Id
        {
            get { return GlobalIds.BuildType.DojoPreviewScreen; }
        }

        /// <summary>
        /// 标题
        /// </summary>
        public override string Title
        {
            get { return "预览屏幕生成"; }
        }

        /// <summary>
        /// 文档类型
        /// </summary>
        public override int DocumentType
        {
            get { return GlobalIds.DocumentType.Screen; }
        }
    }
}

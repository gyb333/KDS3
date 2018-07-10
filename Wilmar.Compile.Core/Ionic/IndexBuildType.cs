using Wilmar.Foundation.Common;
using Wilmar.Service.Common.Generate;

namespace Wilmar.Compile.Core.Ionic
{
    /// <summary>
    /// 首页生成类型
    /// </summary>
    public class IndexBuildType : BuildTypeBase
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public override int Id
        {
            get { return GlobalIds.BuildType.IonicIndex; }
        }

        /// <summary>
        /// 标题
        /// </summary>
        public override string Title
        {
            get { return "项目主页生成"; }
        }
    }
}

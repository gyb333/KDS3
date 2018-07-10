using Wilmar.Foundation.Common;
using Wilmar.Service.Common.Generate;

namespace Wilmar.Compile.Core.Dojo
{
    /// <summary>
    /// 配置文件生成类型
    /// </summary>
    public class ConfigBuildType : BuildTypeBase
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public override int Id
        {
            get { return GlobalIds.BuildType.DojoConfig; }
        }

        /// <summary>
        /// 标题
        /// </summary>
        public override string Title
        {
            get { return "配置文件生成"; }
        }
    }
}

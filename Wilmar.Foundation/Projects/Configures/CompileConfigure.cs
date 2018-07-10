using System.Collections.Generic;

namespace Wilmar.Foundation.Projects.Configures
{
    /// <summary>
    /// 项目编译方案配置
    /// </summary>
    public class CompileConfigure : ConfigureBase
    {
        /// <summary>
        /// 当前项目编译方案集合
        /// </summary>
        public List<CompilePlanItem> Children { get; set; }
    }
}

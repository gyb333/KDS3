using System.Collections.Generic;

namespace Wilmar.Foundation.Projects
{
    /// <summary>
    /// 编译方案项
    /// </summary>
    public class CompilePlanItem: BuildPlan
    {
        /// <summary>
        /// 编译器
        /// </summary>
        public int Compile { get; set; }
        /// <summary>
        /// 文档生成方案集合
        /// </summary>
        public List<BuildPlan> BuildPlans { get; set; }
    }
}

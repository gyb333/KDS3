using System.Collections.Generic;

namespace Wilmar.Foundation.Projects
{
    /// <summary>
    /// 生成方案
    /// </summary>
    public class BuildPlan
    {
        /// <summary>
        /// 项编号
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 生成方案设置项集合
        /// </summary>
        public List<BuildPlanItem> Children { get; set; }
    }
}

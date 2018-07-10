using System.Collections.Generic;

namespace Wilmar.Foundation.Projects
{
    /// <summary>
    /// 项目项基类
    /// </summary>
    public abstract class ProjectItemBase
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 子节点
        /// </summary>
        public List<ProjectItemBase> Children { get; set; }
    }
}

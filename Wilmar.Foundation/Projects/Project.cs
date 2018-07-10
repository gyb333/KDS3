using System.Collections.Generic;

namespace Wilmar.Foundation.Projects
{
    /// <summary>
    /// 项目 Model 对象
    /// </summary>
    public class Project
    {
        /// <summary>
        /// 项目标识
        /// </summary>
        public string Identity { get; set; }
        /// <summary>
        /// 项目描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 项目树根节点
        /// </summary>
        public ProjectRoot Root { get; set; }
        /// <summary>
        /// 子节点
        /// </summary>
        public List<ConfigureBase> Configures { get; set; }
    }
}

using System.Collections.Generic;

namespace Wilmar.Foundation.Projects
{
    /// <summary>
    /// 项目文档节点对象
    /// </summary>
    public class ProjectDocument : ProjectItemBase
    {
        /// <summary>
        /// 文档编号
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 文档名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 文档类型
        /// </summary>
        public int DocumentType { get; set; }
        /// <summary>
        /// 编译方案
        /// </summary>
        public int ComilePlan { get; set; }
        /// <summary>
        /// 生成方案
        /// </summary>
        public int BuildPlan { get; set; }
        /// <summary>
        /// 文档扩展属性
        /// </summary>
        public Dictionary<string, object> Propertys { get; set; }
    }
}

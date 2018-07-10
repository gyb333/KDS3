namespace Wilmar.Foundation.Projects
{
    /// <summary>
    /// 生成方案设置项
    /// </summary>
    public class BuildPlanItem
    {
        /// <summary>
        /// 生成器类型
        /// </summary>
        public int BuildType { get; set; }
        /// <summary>
        /// 生成器
        /// </summary>
        public int Build { get; set; }
        /// <summary>
        /// 对应生成文档类型，为0则表示是全局生成器
        /// </summary>
        public int DocumentType { get; set; }
    }
}

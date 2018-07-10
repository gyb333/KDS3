using System.Collections.Generic;

namespace Wilmar.Foundation.Projects.Configures
{
    /// <summary>
    /// 数据库配置
    /// </summary>
    public class DatabaseConfigure : ConfigureBase
    {
        /// <summary>
        /// 数据配置项集合
        /// </summary>
        public List<DatabaseConfigureItem> Children { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wilmar.Foundation.Projects.Configures;

namespace Wilmar.Service.Common.ProjectBase
{
    /// <summary>
    /// 项目配置，该对象会以.configure的文件形式和服务DLL文件一起存储，在加载项目中使用其中的配置项
    /// </summary>
    public sealed class ProjectConfigure
    {
        /// <summary>
        /// 数据源配置集合
        /// </summary>
        public Dictionary<int, DatabaseConfigureItem> DataSources { get; set; }
    }
}

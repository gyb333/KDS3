using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wilmar.Service.Core.Projects
{
    /// <summary>
    /// 编译数据
    /// </summary>
    public class CompileData
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 数据类型0-编译器，1-生成类型，2-生成器
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public int[] Data { get; set; }
    }
}

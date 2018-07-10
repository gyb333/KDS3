using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wilmar.Service.Common.Generate
{
    /// <summary>
    /// 生成器类型基类
    /// </summary>
    public abstract class BuildTypeBase
    {
        /// <summary>
        /// 生成器类型ID
        /// </summary>
        public abstract int Id { get; }
        /// <summary>
        /// 生成文档类型，如果为0则表示这是全局文档生成类型，默认为0
        /// </summary>
        public virtual int DocumentType
        {
            get { return 0; }
        }
        /// <summary>
        /// 生成类型标题
        /// </summary>
        public abstract string Title { get; }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wilmar.Foundation.Projects;
using Wilmar.Service.Common.Generate;

namespace Wilmar.Service.Common.Services
{
    /// <summary>
    /// 编译引擎服务
    /// </summary>
    public interface ICompileEngineService
    {
        /// <summary>
        /// 生成器集合
        /// </summary>
        ReadOnlyDictionary<int, BuildBase> Builds { get; }
        /// <summary>
        /// 生成器类型集合
        /// </summary>
        ReadOnlyDictionary<int, BuildTypeBase> BuildTypes { get; }
        /// <summary>
        /// 编译器集合
        /// </summary>
        ReadOnlyDictionary<int, CompileBase> Compiles { get; }
    }
}

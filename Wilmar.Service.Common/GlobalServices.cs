using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wilmar.Foundation;
using Wilmar.Foundation.Common;
using Wilmar.Service.Common.Services;

namespace Wilmar.Service.Common
{
    /// <summary>
    /// 全局服务
    /// </summary>
    public static class GlobalServices
    {
        /// <summary>
        /// 项目引擎服务
        /// </summary>
        public static IProjectEngineService ProjectEngineService { get; } = Utility.GetService<IProjectEngineService>();
        /// <summary>
        /// 编译引擎服务
        /// </summary>
        public static ICompileEngineService CompileEngineService { get; } = Utility.GetService<ICompileEngineService>();
        /// <summary>
        /// 扩展服务
        /// </summary>
        public static IExtensionService ExtensionService { get; } = Utility.GetService<IExtensionService>();
        /// <summary>
        /// 安全服务
        /// </summary>
        public static ISecurityService SecurityService { get; } = Utility.GetService<ISecurityService>();
    }
}

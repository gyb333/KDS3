using System.ComponentModel.Design;

namespace Wilmar.Foundation
{
    /// <summary>
    /// 平台服务引擎
    /// </summary>
    public sealed class PlatformServices : ServiceContainer
    {
        /// <summary>
        /// 平台服务引擎实例
        /// </summary>
        public static PlatformServices Instance { get; } = new PlatformServices();

        private PlatformServices() { }
    }
}

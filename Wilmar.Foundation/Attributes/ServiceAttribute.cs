using System;
using Wilmar.Foundation.Common;
using Wilmar.Foundation.Properties;

namespace Wilmar.Foundation.Attributes
{
    /// <summary>
    /// 指定当前为指定服务的实现类
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ServiceAttribute : Attribute
    {
        /// <summary>
        /// 通过指定的服务接口类型，初始化RegisterServiceAttribute新实例
        /// </summary>
        /// <param name="serviceType">指定的服务接口类型</param>
        public ServiceAttribute(Type serviceType)
        {
            if (serviceType == null)
            {
                throw UtilityException.ArgumentNull("serviceType");
            }
            if (serviceType.IsInterface == false)
            {
                throw UtilityException.Argument("serviceType", Resources.ServiceTypeIsNotInterface, serviceType.FullName);
            }
            this.ServiceType = serviceType;
        }
        /// <summary>
        /// 服务接口类型
        /// </summary>
        public Type ServiceType
        {
            get;
            private set;
        }
    }
}

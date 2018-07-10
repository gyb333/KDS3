using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Wilmar.Service.Common.Manager;

namespace Wilmar.Service.Common
{
    /// <summary>
    /// 服务内容
    /// </summary>
    public class ServiceContext
    {
        //上下文主键
        private const string ServiceContextKey = "Wilmar_PlatformServiceContext";

        /// <summary>
        /// 私有化构造
        /// </summary>
        private ServiceContext() { }
        /// <summary>
        /// 当前请求的服务内容
        /// </summary>
        public static ServiceContext Current
        {
            get
            {
                lock (HttpContext.Current)
                {
                    var items = HttpContext.Current.Items;
                    if (!items.Contains(ServiceContextKey))
                        items.Add(ServiceContextKey, new ServiceContext());
                    return (ServiceContext)items[ServiceContextKey];
                }
            }
        }


        private Lazy<DataContextManager> _DataContext = new Lazy<DataContextManager>(() => { return new DataContextManager(); }, true);
        /// <summary>
        /// 数据内容上下文管理
        /// </summary>
        public DataContextManager DataContext
        {
            get { return _DataContext.Value; }
        }

    }
}

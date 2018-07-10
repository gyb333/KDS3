using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Wilmar.Service.Common.ProjectBase;

namespace Wilmar.Service.Common.Services
{
    /// <summary>
    /// 项目引擎服务
    /// </summary>
    public interface IProjectEngineService
    {
        /// <summary>
        /// 启动引擎
        /// </summary>
        /// <param name="config"></param>
        void StartUp(HttpConfiguration config);
        /// <summary>
        /// 通过项目标识获取项目服务对象
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        ProjectServiceBase GetProjectService(string identity);
        /// <summary>
        /// 通过项目所在程序集获取项目服务对象
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        ProjectServiceBase GetProjectService(Assembly assembly);
    }
}

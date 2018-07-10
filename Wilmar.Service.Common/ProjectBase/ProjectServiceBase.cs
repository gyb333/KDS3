using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.OData.Builder;
using Wilmar.Foundation.Common;

namespace Wilmar.Service.Common.ProjectBase
{
    /// <summary>
    /// 项目服务基类
    /// </summary>
    public abstract class ProjectServiceBase
    {
        private const string ProjectAssemblyPattern = "Wilmar.Project.*.dll";
        private const string ProjectAssemblyPath = "~/bin/Projects/Wilmar.Project.*.dll";

        /// <summary>
        /// 项目标识
        /// </summary>
        public abstract string Identity { get; }
        /// <summary>
        /// 初始化OData服务模型
        /// </summary>
        /// <param name="builder"></param>
        public abstract void InitEdmModel(ODataConventionModelBuilder builder);
        /// <summary>
        /// 项目配置
        /// </summary>
        public ProjectConfigure Configure
        {
            get
            {
                if (_Configure == null)
                {
                    try
                    {
                        string path = HostingEnvironment.MapPath($"~/bin/Projects/{this.GetType().Assembly.GetName().Name}.configure");
                        if (System.IO.File.Exists(path))
                        {
                            _Configure = Utility.Deserialize(System.IO.File.ReadAllText(path)) as ProjectConfigure;
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"读取项目配置文件,程序集[{this.GetType().Assembly.FullName}]异常：");
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                    }
                }
                return _Configure;
            }
        }
        private ProjectConfigure _Configure;
    }
}

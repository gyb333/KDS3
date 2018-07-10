using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wilmar.Foundation.Projects;

namespace Wilmar.Service.Common.Generate
{
    /// <summary>
    /// 生成器基类
    /// </summary>
    public abstract class BuildBase
    {
        /// <summary>
        /// 生成器ID
        /// </summary>
        public abstract int Id { get; }
        /// <summary>
        /// 生成器标题
        /// </summary>
        public abstract string Title { get; }
        /// <summary>
        /// 生成器类型ID
        /// </summary>
        public abstract int BuildTypeId { get; }
        /// <summary>
        /// 当前生成器是否参与整个项目编译过程
        /// </summary>
        public virtual bool ProjectCompile
        {
            get { return true; }
        }
        /// <summary>
        /// 生成单个项目文档，并返回生成结果，默认返回空字符串
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public virtual string Build(object body) => string.Empty;
        /// <summary>
        /// 执行生成
        /// </summary>
        /// <param name="compile">编译器></param>
        /// <param name="doc">指定的文档对象，如果是全局生成器则为空</param>
        public virtual void Build(CompileBase compile, ProjectDocument doc) { }
    }
}
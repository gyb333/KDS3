using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wilmar.Foundation.Common;
using Wilmar.Service.Common.Generate;

namespace Wilmar.Build.Core.Ionic.Default
{
    /// <summary>
    /// 主页生成器
    /// </summary>
    public class IndexBuild : BuildBase
    {
        /// <summary>
        /// 生成
        /// </summary>
        /// <param name="compile">DOJO编译器</param>
        /// <param name="doc">文档对象模型</param>
        public override void Build(CompileBase compile, Foundation.Projects.ProjectDocument doc)
        {
        }

        /// <summary>
        /// 生成类型
        /// </summary>
        public override int BuildTypeId
        {
            get { return GlobalIds.BuildType.IonicIndex; }
        }

        /// <summary>
        /// 唯一标识
        /// </summary>
        public override int Id
        {
            get { return GlobalIds.BuildType.IonicIndex + 1; }
        }

        /// <summary>
        /// 标题
        /// </summary>
        public override string Title
        {
            get { return "Ionic项目主页生成器"; }
        }
    }
}

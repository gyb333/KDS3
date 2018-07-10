using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wilmar.Foundation.Common;

namespace Wilmar.Compile.Core.Kendo
{
    /// <summary>
    /// Kendo编译器
    /// </summary>
    public class KendoCompile : TerminalCompileBase
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public override int Id
        {
            get { return GlobalIds.Compile.KendoPC; }
        }

        /// <summary>
        /// 编译器器开始变以前处理逻辑
        /// </summary>
        public override void BeginCompile()
        {

        }

        /// <summary>
        /// 编译器编译完成后处理逻辑
        /// </summary>
        public override void EndCompile()
        {

        }

        /// <summary>
        /// 标题
        /// </summary>
        public override string Title
        {
            get { return "Kendo框架编译器"; }
        }

        /// <summary>
        /// 生成类型
        /// </summary>
        public override int[] BuildTypes
        {
            get { return _BuildTypes; }
        }

        private static int[] _BuildTypes = new int[] { 
            GlobalIds.BuildType.KendoIndex, 
            GlobalIds.BuildType.KendoPreviewScreen,
            GlobalIds.BuildType.KendoScreen
        };
    }
}

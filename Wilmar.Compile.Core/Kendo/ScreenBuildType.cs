using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wilmar.Foundation.Common;
using Wilmar.Service.Common.Generate;

namespace Wilmar.Compile.Core.Kendo
{
    /// <summary>
    /// 屏幕生成类型
    /// </summary>
    public class ScreenBuildType : BuildTypeBase
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public override int Id
        {
            get { return GlobalIds.BuildType.KendoScreen; }
        }

        /// <summary>
        /// 标题
        /// </summary>
        public override string Title
        {
            get { return "屏幕生成"; }
        }

        /// <summary>
        /// 文档类型
        /// </summary>
        public override int DocumentType
        {
            get { return GlobalIds.DocumentType.Screen; }
        }
    }
}

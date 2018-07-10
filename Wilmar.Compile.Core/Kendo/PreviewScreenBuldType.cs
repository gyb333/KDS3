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
    /// 屏幕预览生成类型
    /// </summary>
    public class PreviewScreenBuldType : BuildTypeBase
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public override int Id
        {
            get { return GlobalIds.BuildType.KendoPreviewScreen; }
        }

        /// <summary>
        /// 标题
        /// </summary>
        public override string Title
        {
            get { return "预览屏幕生成"; }
        }
    }
}

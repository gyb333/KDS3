using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using Wilmar.Foundation.Common;
using Wilmar.Model.Core.Definitions;
using Wilmar.Model.Core.Definitions.Screens;
using Wilmar.Service.Common.Generate;
using Wilmar.Build.Core.Ionic.Default.Builders;

namespace Wilmar.Build.Core.Ionic.Default
{
    /// <summary>
    /// 屏幕预览生成器
    /// </summary>
    public class PreviewScreenBuild : BuildBase
    {
        /// <summary>
        /// 生成逻辑
        /// </summary>
        /// <param name="body">屏幕定义对象</param>
        public override string Build(object body)
        {
            try
            {
                var screenDefinition = body as ScreenDefinition;
                if (screenDefinition == null) return string.Empty;

                ControlHost controlHost = screenDefinition.Root;
                using (var writer = new StringWriter())
                {
                    //根据屏幕生成HTML
                    var xmlWriter = new HtmlTextWriter(writer);

                    var builder = controlHost.GetBuilder(true, screenDefinition, null, null, null, xmlWriter);
                    builder.Build();

                    return writer.ToString().Replace(System.Environment.NewLine, "");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 生成类型
        /// </summary>
        public override int BuildTypeId
        {
            get { return GlobalIds.BuildType.IonicPreviewScreen; }
        }

        /// <summary>
        /// 唯一标识
        /// </summary>
        public override int Id
        {
            get { return GlobalIds.BuildType.IonicPreviewScreen + 1; }
        }

        /// <summary>
        /// 标题
        /// </summary>
        public override string Title
        {
            get { return "Ionic屏幕预览生成器"; }
        }
    }
}

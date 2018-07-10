using System;
using System.IO;
using System.Web.UI;
using Wilmar.Build.Core.Dojo.Default.Builders;
using Wilmar.Foundation.Common;
using Wilmar.Foundation.Projects;
using Wilmar.Model.Core.Definitions;
using Wilmar.Model.Core.Definitions.Screens;
using Wilmar.Service.Common.Generate;

namespace Wilmar.Build.Core.Dojo.Default
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
                    xmlWriter.AddAttribute("dojoType", "dojox/mvc/Group");
                    xmlWriter.AddAttribute("style", "width:100%;height:100%;");
                    xmlWriter.AddAttribute("class", "groupDiv");
                    xmlWriter.RenderBeginTag("div");

                    var builder = controlHost.GetBuilder(true, screenDefinition, null, null, null, xmlWriter);
                    builder.Build();

                    xmlWriter.RenderEndTag();

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
            get { return GlobalIds.BuildType.DojoPreviewScreen; }
        }

        /// <summary>
        /// 唯一标识
        /// </summary>
        public override int Id
        {
            get { return GlobalIds.BuildType.DojoPreviewScreen + 1; }
        }

        /// <summary>
        /// 标题
        /// </summary>
        public override string Title
        {
            get { return "Dojo屏幕预览生成器"; }
        }
    }
}

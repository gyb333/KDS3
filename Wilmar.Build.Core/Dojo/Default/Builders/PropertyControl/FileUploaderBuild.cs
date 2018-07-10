using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using Wilmar.Foundation.Projects;
using Wilmar.Model.Core.Definitions;
using Wilmar.Model.Core.Definitions.Screens;
using Wilmar.Model.Core.Definitions.Screens.Controls;
using Wilmar.Service.Common.Generate;

namespace Wilmar.Build.Core.Dojo.Default.Builders
{
    /// <summary>
    /// 文件上传生成器
    /// </summary>
    internal class FileUploaderBuild : ControlBuildBase
    {
        public FileUploaderBuild(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
            : base(isPreview, controlHost, screenDef, compile, doc, permissionData, htmlWriter)
        {

        }

        /// <summary>
        /// 设置属性
        /// </summary>
        protected override void SetAttributes()
        {

            base.SetAttributes();
        }

        /// <summary>
        /// 设置子元素
        /// </summary>
        protected override void SetChildElements()
        {
            FileUploader control = this.ControlHost.Content as FileUploader;
            this.HtmlWriter.AddAttribute("dojoType", "Controls/Uploader");
            if (!IsPreview && !string.IsNullOrEmpty(this.ControlHost.Name))
            {
                this.HtmlWriter.AddAttribute("id", this.ControlHost.Name);
                this.HtmlWriter.AddAttribute("name", this.ControlHost.Name);
            }

            StringBuilder sbProps = new StringBuilder();
            StringBuilder returnContent = new StringBuilder();
            string props = control.BuildControlProps(this.ScreenDefinition, this.IsPreview, this.PermissionData, returnContent);
            if (!string.IsNullOrEmpty(props)) sbProps.AppendFormat("{0},", props);
            sbProps.AppendFormat("multiple:{0},", control.Multiple.ToString().ToLower());
            if (!string.IsNullOrEmpty(control.ProgressWidgetId))
            {
                sbProps.AppendFormat("progressWidgetId:'{0}',", this.ProjectDocument.Name + "_" + control.ProgressWidgetId);
            }
            sbProps.AppendFormat("uploadUrl:'{0}',", control.UploadUrl);
            sbProps.AppendFormat("showInput:'before',");

            if (sbProps.ToString().Length > 0)
            {
                this.HtmlWriter.AddAttribute("data-dojo-props", sbProps.ToString().Substring(0, sbProps.ToString().Length - 1), false);
            }

            this.HtmlWriter.RenderBeginTag("div");
            this.HtmlWriter.WriteEncodedText("选择...");

            this.HtmlWriter.RenderEndTag();
        }
    }
}

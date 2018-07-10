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
    /// 富文本框生成器
    /// </summary>
    internal class RichTextBoxBuild : ControlBuildBase
    {
        public RichTextBoxBuild(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
            : base(isPreview, controlHost, screenDef, compile, doc, permissionData, htmlWriter)
        {

        }

        /// <summary>
        /// 设置属性
        /// </summary>
        protected override void SetAttributes()
        {
            RichTextBox control = this.ControlHost.Content as RichTextBox;
            this.HtmlWriter.AddAttribute("dojoType", "Controls/Panel");

            base.SetAttributes();
        }
        /// <summary>
        /// 设置子元素
        /// </summary>
        protected override void SetChildElements()
        {
            RichTextBox control = this.ControlHost.Content as RichTextBox;
            this.HtmlWriter.AddAttribute("dojoType", "Controls/ContentPane");
            this.HtmlWriter.RenderBeginTag(this.TagName);

            this.HtmlWriter.AddAttribute("dojoType", "Controls/Editor");
            if (!IsPreview && !string.IsNullOrEmpty(this.ControlHost.Name))
            {
                this.HtmlWriter.AddAttribute("id", this.ControlHost.Name);
                this.HtmlWriter.AddAttribute("name", this.ControlHost.Name);
            }
            this.HtmlWriter.AddAttribute("class", control.GetType().Name.ToLower());
            if (control.ExistProperty("IsReadOnly") && control.IsReadOnly) this.HtmlWriter.AddAttribute("readonly", "readonly");
            if (control.ExistProperty("IsEnable") && !control.IsEnable) this.HtmlWriter.AddAttribute("disabled", "disabled");

            StringBuilder sbProps = new StringBuilder();
            StringBuilder returnContent = new StringBuilder();
            string props = control.BuildControlProps(this.ScreenDefinition, this.IsPreview, this.PermissionData, returnContent);
            if (!string.IsNullOrEmpty(props)) sbProps.AppendFormat("{0},", props);
            if (!control.HasToolBar) sbProps.AppendFormat("{0},", "toolBar:false");
            sbProps.AppendFormat("{0},", "height:'100%',style:'box-sizing:border-box;border:0px'");
            if (sbProps.ToString().Length > 0)
            {
                this.HtmlWriter.AddAttribute("data-dojo-props", sbProps.ToString().Substring(0, sbProps.ToString().Length - 1), false);
            }

            this.HtmlWriter.RenderBeginTag(this.TagName);
            this.HtmlWriter.RenderEndTag();
            this.HtmlWriter.RenderEndTag();
        }
        /// <summary>
        /// 设置Style样式
        /// </summary>
        /// <returns></returns>
        protected override string SetControlStyle()
        {
            string baseStyle = base.SetControlStyle();
            string currentStyle = "border: 1px solid #b5bcc7;";

            return baseStyle + currentStyle;
        }
    }
}

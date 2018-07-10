using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Xml;
using Wilmar.Foundation.Projects;
using Wilmar.Model.Core.Definitions;
using Wilmar.Model.Core.Definitions.Screens;
using Wilmar.Model.Core.Definitions.Screens.Controls;
using Wilmar.Service.Common.Generate;

namespace Wilmar.Build.Core.Dojo.Default.Builders
{
    /// <summary>
    /// 复选框生成器
    /// </summary>
    internal class CheckBoxBuild : ControlBuildBase
    {
        public CheckBoxBuild(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
            : base(isPreview, controlHost, screenDef, compile, doc, permissionData, htmlWriter)
        {

        }

        /// <summary>
        /// 设置标签
        /// </summary>
        protected override string TagName
        {
            get
            {
                return "label";
            }
        }
        /// <summary>
        /// 设置属性
        /// </summary>
        protected override void SetAttributes()
        {
            CheckBox control = this.ControlHost.Content as CheckBox;
            if (!string.IsNullOrEmpty(control.ToolTip) && this.ProjectDocument != null)
            {
                this.HtmlWriter.AddAttribute("tooltip-name", this.ProjectDocument.Name + "_" + control.ToolTip);
            }

            base.SetAttributes();
        }
        /// <summary>
        /// 设置子元素
        /// </summary>
        protected override void SetChildElements()
        {
            CheckBox control = this.ControlHost.Content as CheckBox;
            this.HtmlWriter.AddAttribute("dojoType", "Controls/CheckBox");
            if (!IsPreview && !string.IsNullOrEmpty(this.ControlHost.Name))
            {
                this.HtmlWriter.AddAttribute("id", this.ControlHost.Name);
                this.HtmlWriter.AddAttribute("name", this.ControlHost.Name);
            }

            StringBuilder sbProps = new StringBuilder();
            StringBuilder returnContent = new StringBuilder();
            string props = control.BuildControlProps(this.ScreenDefinition, this.IsPreview, this.PermissionData, returnContent);
            if (!string.IsNullOrEmpty(props)) sbProps.AppendFormat("{0},", props);
            if (sbProps.ToString().Length > 0)
            {
                this.HtmlWriter.AddAttribute("data-dojo-props", sbProps.ToString().Substring(0, sbProps.ToString().Length - 1), false);
            }

            if (control.ExistProperty("IsReadOnly") && control.IsReadOnly) this.HtmlWriter.AddAttribute("readonly", "readonly");
            if (control.ExistProperty("IsEnable") && !control.IsEnable) this.HtmlWriter.AddAttribute("disabled", "disabled");
            this.HtmlWriter.AddAttribute("style", "width:15px !important;height:16px !important;margin-bottom:4px;");
            this.HtmlWriter.RenderBeginTag("input");
            this.HtmlWriter.RenderEndTag();
            this.HtmlWriter.RenderBeginTag("span");
            this.HtmlWriter.WriteEncodedText(control.Text == null ? "" : control.Text);
            this.HtmlWriter.RenderEndTag();
        }
    }
}

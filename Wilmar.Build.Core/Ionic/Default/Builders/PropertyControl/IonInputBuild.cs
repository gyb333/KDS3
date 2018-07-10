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

namespace Wilmar.Build.Core.Ionic.Default.Builders
{
    internal class IonInputBuild : ControlBuildBase
    {
        public IonInputBuild(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
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
                return "wm-input";
            }
        }
        /// <summary>
        /// 设置属性
        /// </summary>
        protected override void SetAttributes()
        {
            IonInput control = this.ControlHost.Content as IonInput;
            if (control.IsPassword) this.HtmlWriter.AddAttribute("type", "password");
            else this.HtmlWriter.AddAttribute("type", "text");
            if (!string.IsNullOrEmpty(control.Watermark))
            {
                this.HtmlWriter.AddAttribute("placeholder", control.Watermark);
            }

            //控件属性绑定
            string bindPropertyStr = BuildCommon.BuildControlBindProperty(control, this.ScreenDefinition, this.IsPreview);
            if (!string.IsNullOrEmpty(bindPropertyStr)) this.HtmlWriter.AddAttribute(bindPropertyStr, null);

            base.SetAttributes();
        }
    }
}

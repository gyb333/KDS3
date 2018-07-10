
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
    internal class IonRefresherBuild : ContainerBuildBase
    {
        public IonRefresherBuild(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
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
                return "wm-refresher";
            }
        }
        /// <summary>
        /// 设置属性
        /// </summary>
        protected override void SetAttributes()
        {
            IonRefresher control = this.ControlHost.Content as IonRefresher;
            //控件属性绑定
            string bindPropertyStr = BuildCommon.BuildControlBindProperty(control, this.ScreenDefinition, this.IsPreview);
            if (!string.IsNullOrEmpty(bindPropertyStr)) this.HtmlWriter.AddAttribute(bindPropertyStr, null);

            base.SetAttributes();
        }
        /// <summary>
        /// 设置子元素
        /// </summary>
        protected override void SetChildElements()
        {
            IonRefresher control = this.ControlHost.Content as IonRefresher;
            this.HtmlWriter.AddAttribute("pullingIcon", "arrow-dropdown");
            this.HtmlWriter.AddAttribute("pullingText", "下拉刷新");
            this.HtmlWriter.AddAttribute("refreshingSpinner", "circles");
            this.HtmlWriter.AddAttribute("refreshingText", "正在刷新...");
            this.HtmlWriter.RenderBeginTag("wm-refresher-content");
            this.HtmlWriter.RenderEndTag();
        }
    }
}

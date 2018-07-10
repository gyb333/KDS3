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
    internal class IonSlidesBuild : ContainerBuildBase
    {
        public IonSlidesBuild(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
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
                return "wm-slides";
            }
        }
        /// <summary>
        /// 设置属性
        /// </summary>
        protected override void SetAttributes()
        {
            IonSlides control = this.ControlHost.Content as IonSlides;
            if (control.Pager) this.HtmlWriter.AddAttribute("pager", "");
            if (control.Autoplay != null && control.Autoplay > 0) this.HtmlWriter.AddAttribute("autoplay", control.Autoplay.ToString());
            if (control.Loop) this.HtmlWriter.AddAttribute("loop", "true");
            this.HtmlWriter.AddAttribute("zoom", "true");

            base.SetAttributes();
        }
        /// <summary>
        /// 设置子元素
        /// </summary>
        protected override void SetChildElements()
        {
            IonSlides control = this.ControlHost.Content as IonSlides;
            foreach (var c in this.ControlHost.Children)
            {
                this.HtmlWriter.RenderBeginTag("wm-slide");
                var builder = c.GetBuilder(this.IsPreview, this.ScreenDefinition, this.Compile, this.ProjectDocument, this.PermissionData, this.HtmlWriter);
                builder.Parent = this;
                builder.Build();
                this.HtmlWriter.RenderEndTag();
            }
        }
    }
}

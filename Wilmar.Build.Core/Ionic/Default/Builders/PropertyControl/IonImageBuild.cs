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
    internal class IonImageBuild : ControlBuildBase
    {
        public IonImageBuild(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
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
                IonImage control = this.ControlHost.Content as IonImage;
                if (control.ShowStyle == EIonImageStyle.Avatar) return "ion-avatar";
                else if (control.ShowStyle == EIonImageStyle.Thumbnail) return "ion-thumbnail";
                else return "div";
            }
        }
        /// <summary>
        /// 设置属性
        /// </summary>
        protected override void SetAttributes()
        {
            IonImage control = this.ControlHost.Content as IonImage;
            IonItemAttach attachObject = this.ControlHost.AttachObject as IonItemAttach;
            if (attachObject != null)
            {
                if (attachObject.ItemDock == EIonItemDock.Start) this.HtmlWriter.AddAttribute("item-start", null);
                else if (attachObject.ItemDock == EIonItemDock.End) this.HtmlWriter.AddAttribute("item-end", null);
            }

            base.SetAttributes();
        }
        /// <summary>
        /// 设置子元素
        /// </summary>
        protected override void SetChildElements()
        {
            IonImage control = this.ControlHost.Content as IonImage;
            if (!string.IsNullOrEmpty(control.Url)) this.HtmlWriter.AddAttribute("src", control.Url);
            this.HtmlWriter.RenderBeginTag("img");
            this.HtmlWriter.RenderEndTag();
        }
    }
}

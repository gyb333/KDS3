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
    internal class IonListBuild : ContainerBuildBase
    {
        public IonListBuild(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
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
                return "wm-list";
            }
        }
        /// <summary>
        /// 设置属性
        /// </summary>
        protected override void SetAttributes()
        {
            IonList control = this.ControlHost.Content as IonList;
            if (control.ShowStyle == EIonListStyle.Inset) this.HtmlWriter.AddAttribute("inset", null);
            else if (control.ShowStyle == EIonListStyle.NoBorder) this.HtmlWriter.AddAttribute("no-border", null);
            else if (control.ShowStyle == EIonListStyle.NoLines) this.HtmlWriter.AddAttribute("no-lines", null);
            if (!this.IsPreview)
            {
                if (this.ControlHost.AttachObject != null)
                {
                    IonSegmentContentAttach attachObject = this.ControlHost.AttachObject as IonSegmentContentAttach;
                    if (!string.IsNullOrEmpty(attachObject.SegmentName))
                    {
                        this.HtmlWriter.AddAttribute("*ngSwitchCase", "'" + attachObject.SegmentName + "'", false);
                    }
                }
            }

            base.SetAttributes();
        }
        /// <summary>
        /// 设置子元素
        /// </summary>
        protected override void SetChildElements()
        {
            IonList control = this.ControlHost.Content as IonList;
            if (control.IsHeader)
            {
                this.HtmlWriter.RenderBeginTag("wm-list-header");
                this.HtmlWriter.WriteEncodedText(this.ControlHost.Title);
                this.HtmlWriter.RenderEndTag();
            }
            foreach (var c in this.ControlHost.Children)
            {
                var builder = c.GetBuilder(this.IsPreview, this.ScreenDefinition, this.Compile, this.ProjectDocument, this.PermissionData, this.HtmlWriter);
                builder.Parent = this;
                builder.Build();
            }
        }
    }
}

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
    internal class IonButtonBuild : ControlBuildBase
    {
        public IonButtonBuild(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
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
                return "button";
            }
        }
        /// <summary>
        /// 设置属性
        /// </summary>
        protected override void SetAttributes()
        {
            IonButton control = this.ControlHost.Content as IonButton;
            if (control.IonItem) this.HtmlWriter.AddAttribute("wm-item", null);
            else this.HtmlWriter.AddAttribute("wm-button", null);
            if (!control.IsEnable) this.HtmlWriter.AddAttribute("disabled", "true");
            if (control.Outline) this.HtmlWriter.AddAttribute("outline", null);
            if (control.Clear) this.HtmlWriter.AddAttribute("clear", null);
            if (control.Round) this.HtmlWriter.AddAttribute("round", null);
            if (control.Block) this.HtmlWriter.AddAttribute("block", null);
            if (control.Full) this.HtmlWriter.AddAttribute("full", null);
            if (control.IconOnly) this.HtmlWriter.AddAttribute("icon-only", null);
            if (control.IconDock == EIonButtonIconDock.Start) this.HtmlWriter.AddAttribute("icon-start", null);
            else if (control.IconDock == EIonButtonIconDock.End) this.HtmlWriter.AddAttribute("icon-end", null);
            //this.HtmlWriter.AddAttribute("[style.color]", "'" + control.ForeColor.ToString().ToLower() + "'", false);
            //this.HtmlWriter.AddAttribute("[style.background-color]", "'" + control.BackColor.ToString().ToLower() + "'", false);
            //附加属性
            IonItemAttach attachObject = this.ControlHost.AttachObject as IonItemAttach;
            if (attachObject != null)
            {
                if (attachObject.ItemDock == EIonItemDock.Start) this.HtmlWriter.AddAttribute("item-start", null);
                else if (attachObject.ItemDock == EIonItemDock.End) this.HtmlWriter.AddAttribute("item-end", null);
            }
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
            IonButton control = this.ControlHost.Content as IonButton;
            if (control.IconOnly)
            {
                if (!string.IsNullOrEmpty(control.IconName))
                {
                    this.HtmlWriter.AddAttribute("name", control.IconName);
                    this.HtmlWriter.RenderBeginTag("wm-icon");
                    this.HtmlWriter.RenderEndTag();
                }
            }
            else
            {
                if (control.IconDock == EIonButtonIconDock.Start)
                {
                    if (!string.IsNullOrEmpty(control.IconName))
                    {
                        this.HtmlWriter.AddAttribute("name", control.IconName);
                        this.HtmlWriter.RenderBeginTag("wm-icon");
                        this.HtmlWriter.RenderEndTag();
                    }
                    this.HtmlWriter.WriteEncodedText(control.Value == null ? "" : control.Value);
                }
                else
                {
                    this.HtmlWriter.WriteEncodedText(control.Value == null ? "" : control.Value);
                    if (!string.IsNullOrEmpty(control.IconName))
                    {
                        this.HtmlWriter.AddAttribute("name", control.IconName);
                        this.HtmlWriter.RenderBeginTag("wm-icon");
                        this.HtmlWriter.RenderEndTag();
                    }
                }
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

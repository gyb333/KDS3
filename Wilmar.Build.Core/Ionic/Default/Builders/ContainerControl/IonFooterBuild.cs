﻿using System;
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
    internal class IonFooterBuild : ContainerBuildBase
    {
        public IonFooterBuild(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
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
                return "ion-footer";
            }
        }
        /// <summary>
        /// 设置属性
        /// </summary>
        protected override void SetAttributes()
        {
            IonFooter control = this.ControlHost.Content as IonFooter;

            base.SetAttributes();
        }
        /// <summary>
        /// 设置子元素
        /// </summary>
        protected override void SetChildElements()
        {
            IonFooter control = this.ControlHost.Content as IonFooter;
            this.HtmlWriter.RenderBeginTag("ion-toolbar");
            this.HtmlWriter.RenderBeginTag("ion-title");
            this.HtmlWriter.WriteEncodedText(this.ControlHost.Title);
            this.HtmlWriter.RenderEndTag();
            this.HtmlWriter.RenderEndTag();
        }
    }
}

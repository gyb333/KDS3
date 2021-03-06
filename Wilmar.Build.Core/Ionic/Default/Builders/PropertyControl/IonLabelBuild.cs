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
    internal class IonLabelBuild : ControlBuildBase
    {
        public IonLabelBuild(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
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
                return "wm-label";
            }
        }
        /// <summary>
        /// 设置属性
        /// </summary>
        protected override void SetAttributes()
        {
            IonLabel control = this.ControlHost.Content as IonLabel;
            if (control.ShowStyle == EIonLabelStyle.Fixed) this.HtmlWriter.AddAttribute("fixed", null);
            else if (control.ShowStyle == EIonLabelStyle.Floating) this.HtmlWriter.AddAttribute("floating", null);
            else if (control.ShowStyle == EIonLabelStyle.Stacked) this.HtmlWriter.AddAttribute("stacked", null);
            //this.HtmlWriter.AddAttribute("[style.color]", "'" + control.ForeColor.ToString().ToLower() + "'", false);

            base.SetAttributes();
        }
        /// <summary>
        /// 设置子元素
        /// </summary>
        protected override void SetChildElements()
        {
            IonLabel control = this.ControlHost.Content as IonLabel;

            this.HtmlWriter.WriteEncodedText(this.ControlHost.Title);
        }
    }
}

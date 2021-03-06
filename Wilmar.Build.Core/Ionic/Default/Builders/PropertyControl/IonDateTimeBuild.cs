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
    internal class IonDateTimeBuild : ControlBuildBase
    {
        public IonDateTimeBuild(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
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
                return "wm-datetime";
            }
        }
        /// <summary>
        /// 设置属性
        /// </summary>
        protected override void SetAttributes()
        {
            IonDateTime control = this.ControlHost.Content as IonDateTime;
            if (control.ShowStyle == EIonDateTimeStyle.YYYYMMDD) this.HtmlWriter.AddAttribute("displayFormat", "YYYY-MM-DD");
            else if (control.ShowStyle == EIonDateTimeStyle.MMMDDYYYY) this.HtmlWriter.AddAttribute("displayFormat", "MMM DD YYYY");
            else if (control.ShowStyle == EIonDateTimeStyle.hmmA) this.HtmlWriter.AddAttribute("displayFormat", "h:mm A");

            //控件属性绑定
            string bindPropertyStr = BuildCommon.BuildControlBindProperty(control, this.ScreenDefinition, this.IsPreview);
            if (!string.IsNullOrEmpty(bindPropertyStr)) this.HtmlWriter.AddAttribute(bindPropertyStr, null);

            base.SetAttributes();
        }
    }
}

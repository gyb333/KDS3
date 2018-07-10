using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Web.UI;
using Wilmar.Foundation.Projects;
using Wilmar.Model.Core.Definitions;
using Wilmar.Model.Core.Definitions.Configure;
using Wilmar.Model.Core.Definitions.Screens;
using Wilmar.Model.Core.Definitions.Screens.Controls;
using Wilmar.Service.Common.Generate;
using System;

namespace Wilmar.Build.Core.Ionic.Default.Builders
{
    internal class IonPanelBuild : ContainerBuildBase
    {
        public IonPanelBuild(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
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
                return "div";
            }
        }
        /// <summary>
        /// 设置属性
        /// </summary>
        protected override void SetAttributes()
        {
            IonPanel control = this.ControlHost.Content as IonPanel;
            if (control.ShowStyle == EIonPanelStyle.Padding)
            {
                this.HtmlWriter.AddAttribute("padding", null);
            }

            base.SetAttributes();
        }
    }
}

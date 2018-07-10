
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
    internal class IonMenuBuild : ContainerBuildBase
    {
        public IonMenuBuild(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
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
                return "wm-menu";
            }
        }
        /// <summary>
        /// 设置属性
        /// </summary>
        protected override void SetAttributes()
        {
            IonMenu control = this.ControlHost.Content as IonMenu;
            if (!this.IsPreview)
            {
                if (!string.IsNullOrEmpty(this.ControlHost.Name))
                {
                    string name = this.ControlHost.Name.Replace(this.ProjectDocument.Name + "_", "");
                    this.HtmlWriter.AddAttribute("[content]", name);
                }
            }

            base.SetAttributes();
        }
    }
}

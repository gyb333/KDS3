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
    internal class IonTitleBuild : ControlBuildBase
    {
        public IonTitleBuild(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
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
                return "wm-title";
            }
        }
        /// <summary>
        /// 设置属性
        /// </summary>
        protected override void SetAttributes()
        {
            IonTitle control = this.ControlHost.Content as IonTitle;
            
            base.SetAttributes();
        }
        /// <summary>
        /// 设置子元素
        /// </summary>
        protected override void SetChildElements()
        {
            IonTitle control = this.ControlHost.Content as IonTitle;
            if (!string.IsNullOrEmpty(control.Value)) this.HtmlWriter.WriteEncodedText(control.Value);
            else
            {
                //控件值绑定
                string bindPropertyStr = BuildCommon.BuildControlBindTextProp(control, this.ScreenDefinition, this.IsPreview);
                if (!string.IsNullOrEmpty(bindPropertyStr)) this.HtmlWriter.WriteEncodedText(bindPropertyStr);
            }
        }
    }
}

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

namespace Wilmar.Build.Core.Dojo.Default.Builders
{
    /// <summary>
    /// 多维报表控件生成器
    /// </summary>
    internal class MeasurePanelBuild : ContainerBuildBase
    {
        public MeasurePanelBuild(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
            : base(isPreview, controlHost, screenDef, compile, doc, permissionData, htmlWriter)
        {

        }

        /// <summary>
        /// 设置属性
        /// </summary>
        protected override void SetAttributes()
        {
            
        }

        /// <summary>
        /// 设置子元素
        /// </summary>
        protected override void SetChildElements()
        {
            
        }
    }
}

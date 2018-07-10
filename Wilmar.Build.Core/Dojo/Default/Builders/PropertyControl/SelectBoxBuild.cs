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

namespace Wilmar.Build.Core.Dojo.Default.Builders
{
    /// <summary>
    /// 选择框生成器
    /// </summary>
    internal class SelectBoxBuild : ControlBuildBase
    {
        public SelectBoxBuild(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
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
                return "input";
            }
        }
        /// <summary>
        /// 设置属性
        /// </summary>
        protected override void SetAttributes()
        {
            SelectBox control = this.ControlHost.Content as SelectBox;
            this.HtmlWriter.AddAttribute("dojoType", "Controls/SelectBox");
            if (!IsPreview && !string.IsNullOrEmpty(this.ControlHost.Name))
            {
                this.HtmlWriter.AddAttribute("id", this.ControlHost.Name);
                this.HtmlWriter.AddAttribute("name", this.ControlHost.Name);
            }
            if (!string.IsNullOrEmpty(control.ToolTip) && this.ProjectDocument != null)
            {
                this.HtmlWriter.AddAttribute("tooltip-name", this.ProjectDocument.Name + "_" + control.ToolTip);
            }
            this.HtmlWriter.AddAttribute("readOnlyItem", "true");


            StringBuilder sbProps = new StringBuilder();
            StringBuilder returnContent = new StringBuilder();
            string props = control.BuildControlProps(this.ScreenDefinition, this.IsPreview, this.PermissionData, returnContent);
            if (!string.IsNullOrEmpty(props)) sbProps.AppendFormat("{0},", props);
            sbProps.AppendFormat("isMultiSelect:{0},", control.IsMultiSelect.ToString().ToLower());
            sbProps.AppendFormat("clearTip:{0},", control.ClearTip.ToString().ToLower());
            if (control.ScreenTitle != null) sbProps.AppendFormat("screenTitle:'{0}',", control.ScreenTitle);
            if (control.ScreenName != null) sbProps.AppendFormat("screenName:'{0}',", control.ScreenName);
            if (control.ScreenInParameter != null) sbProps.AppendFormat("inputParameter:'{0}',", control.ScreenInParameter);
            if (control.ScreenOutParameter != null) sbProps.AppendFormat("outputParameter:'{0}',", control.ScreenOutParameter);
            if (control.ScreenOtherParameter != null) sbProps.AppendFormat("otherParameter:'{0}',", control.ScreenOtherParameter);
            if (control.ScreenWidth != 0) sbProps.AppendFormat("screenWidth:{0},", control.ScreenWidth);
            if (control.ScreenHeight != 0) sbProps.AppendFormat("screenHeight:{0},", control.ScreenHeight);
            string showModel = control.ShowModel ? "dialog" : "tooltip";
            sbProps.AppendFormat("showModel:'{0}',", showModel);

            if (!IsPreview) sbProps.AppendFormat("{0},", "context:at('rel:','VM')");
            if (sbProps.ToString().Length > 0)
            {
                this.HtmlWriter.AddAttribute("data-dojo-props", sbProps.ToString().Substring(0, sbProps.ToString().Length - 1), false);
            }

            base.SetAttributes();
        }
        /// <summary>
        /// 设置Class样式
        /// </summary>
        protected override string SetControlClass()
        {
            string baseClass = base.SetControlClass();
            string isEnable = "";
            SelectBox control = this.ControlHost.Content as SelectBox;
            if (!control.IsEnable) isEnable = "disabled ";
            return isEnable + baseClass;
        }
        /// <summary>
        /// 设置Style样式
        /// </summary>
        /// <returns></returns>
        protected override string SetControlStyle()
        {
            SelectBox control = this.ControlHost.Content as SelectBox;
            string baseStyle = base.SetControlStyle();
            string backColor = control.BackColor.R.ToString() + "," + control.BackColor.G.ToString() + "," + control.BackColor.B.ToString() + "," + control.BackColor.A.ToString();
            string currentStyle = string.Format("background-color:rgba({0});", backColor);

            return baseStyle + currentStyle;
        }
    }
}

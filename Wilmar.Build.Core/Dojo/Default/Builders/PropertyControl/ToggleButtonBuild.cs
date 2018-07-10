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
    /// 切换按钮生成器
    /// </summary>
    internal class ToggleButtonBuild : ControlBuildBase
    {
        public ToggleButtonBuild(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
            : base(isPreview, controlHost, screenDef, compile, doc, permissionData, htmlWriter)
        {

        }

        /// <summary>
        /// 设置属性
        /// </summary>
        protected override void SetAttributes()
        {
            ToggleButton control = this.ControlHost.Content as ToggleButton;
            this.HtmlWriter.AddAttribute("dojoType", "Controls/ToggleButton");
            if (!IsPreview && !string.IsNullOrEmpty(this.ControlHost.Name))
            {
                this.HtmlWriter.AddAttribute("id", this.ControlHost.Name);
                this.HtmlWriter.AddAttribute("name", this.ControlHost.Name);
            }


            StringBuilder sbProps = new StringBuilder();
            StringBuilder returnContent = new StringBuilder();
            string props = control.BuildControlProps(this.ScreenDefinition, this.IsPreview, this.PermissionData, returnContent);
            if (!string.IsNullOrEmpty(props)) sbProps.AppendFormat("{0},", props);
            if (sbProps.ToString().Length > 0)
            {
                this.HtmlWriter.AddAttribute("data-dojo-props", sbProps.ToString().Substring(0, sbProps.ToString().Length - 1), false);
            }

            base.SetAttributes();
        }
        /// <summary>
        /// 设置子元素
        /// </summary>
        protected override void SetChildElements()
        {
            ToggleButton control = this.ControlHost.Content as ToggleButton;
            //设置文本值
            StringBuilder sbAttribute = new StringBuilder();
            if (!string.IsNullOrEmpty(this.ControlHost.Title))
            {
                if (control.Width > 0) sbAttribute.AppendFormat("width:{0}px;", control.Width.ToString());
                if (control.Height > 0) sbAttribute.AppendFormat("height:{0}px;", control.Height.ToString());

                if (control.MinWidth > 0)
                {
                    if (control.Width == 0 || control.Width == null) sbAttribute.AppendFormat("width:{0}px;", control.MinWidth.ToString());
                    sbAttribute.AppendFormat("min-width:{0}px;", control.MinWidth.ToString());
                }

                if (control.MinHeight > 0)
                {
                    if (control.Height == 0 || control.Height == null) sbAttribute.AppendFormat("height:{0}px;", control.MinHeight.ToString());
                    sbAttribute.AppendFormat("min-height:{0}px;", control.MinHeight.ToString());
                }

                int pWidth = 0,
                    width = control.Width == null ? 0 : (int)control.Width,
                    minwidth = control.MinWidth,
                    maxwidth = control.MaxWidth == null ? 0 : (int)control.MaxWidth;
                if (width < minwidth) width = minwidth;
                if (minwidth > maxwidth) maxwidth = minwidth;
                if (width < maxwidth) width = maxwidth;
                pWidth = width;

                int pHeight = 0,
                    height = control.Height == null ? 0 : (int)control.Height,
                    minheight = control.MinHeight,
                    maxheight = control.MaxHeight == null ? 0 : (int)control.MaxHeight;
                if (height < minheight) height = minheight;
                if (minheight > maxheight) maxheight = minheight;
                if (height < maxheight) height = maxheight;
                pHeight = height;

                if (pHeight > 0) sbAttribute.AppendFormat("line-height:{0}px;", pHeight.ToString());
                if (sbAttribute.ToString().Length > 0) this.HtmlWriter.AddAttribute("style", sbAttribute.ToString());

                this.HtmlWriter.RenderBeginTag("div");
                this.HtmlWriter.WriteEncodedText(this.ControlHost.Title);

                this.HtmlWriter.RenderEndTag();
            }
        }
    }
}

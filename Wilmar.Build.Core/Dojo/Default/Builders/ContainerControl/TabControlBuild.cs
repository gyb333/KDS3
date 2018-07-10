﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using Wilmar.Foundation.Projects;
using Wilmar.Model.Core.Definitions;
using Wilmar.Model.Core.Definitions.Screens;
using Wilmar.Model.Core.Definitions.Screens.Controls;
using Wilmar.Service.Common.Generate;

namespace Wilmar.Build.Core.Dojo.Default.Builders
{
    /// <summary>
    /// Tab控件生成器
    /// </summary>
    internal class TabControlBuild : ContainerBuildBase
    {
        public TabControlBuild(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
            : base(isPreview, controlHost, screenDef, compile, doc, permissionData, htmlWriter)
        {

        }

        /// <summary>
        /// 设置属性
        /// </summary>
        protected override void SetAttributes()
        {
            TabControl control = this.ControlHost.Content as TabControl;
            this.HtmlWriter.AddAttribute("dojoType", "Controls/TabContainer");
            if (!IsPreview && !string.IsNullOrEmpty(this.ControlHost.Name))
            {
                this.HtmlWriter.AddAttribute("id", this.ControlHost.Name);
                this.HtmlWriter.AddAttribute("name", this.ControlHost.Name);
            }
            if (control.ExistProperty("Position"))
            {
                this.HtmlWriter.AddAttribute("tabPosition", control.Position.ToString().ToLower());
            }
            if (control.ExistProperty("IsNested") && control.IsNested)
            {
                this.HtmlWriter.AddAttribute("nested", "true");
            }
            this.HtmlWriter.AddAttribute("tabStrip", "true");

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
            foreach (var child in this.ControlHost.Children)
            {
                TabAttach tabAttach = child.AttachObject as TabAttach;
                this.HtmlWriter.AddAttribute("dojoType", "Controls/ContentPane");
                this.HtmlWriter.AddAttribute("title", child.Title);
                if (tabAttach != null)
                {
                    if (tabAttach.Closable) this.HtmlWriter.AddAttribute("closable", tabAttach.Closable.ToString().ToLower());
                }
                this.HtmlWriter.RenderBeginTag("div");

                var builder = child.GetBuilder(this.IsPreview, this.ScreenDefinition, this.Compile, this.ProjectDocument, this.PermissionData, this.HtmlWriter);
                builder.Build();

                this.HtmlWriter.RenderEndTag();
            }
        }
    }
}

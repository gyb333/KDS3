using System;
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
    /// 停靠布局生成器
    /// </summary>
    internal class DockPanelBuild : ContainerBuildBase
    {
        public DockPanelBuild(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
            : base(isPreview, controlHost, screenDef, compile, doc, permissionData, htmlWriter)
        {

        }

        /// <summary>
        /// 设置属性
        /// </summary>
        protected override void SetAttributes()
        {
            DockPanel control = (DockPanel)this.ControlHost.Content;
            this.HtmlWriter.AddAttribute("dojoType", "Controls/BorderContainer");
            if (!IsPreview && !string.IsNullOrEmpty(this.ControlHost.Name))
            {
                this.HtmlWriter.AddAttribute("id", this.ControlHost.Name);
                this.HtmlWriter.AddAttribute("name", this.ControlHost.Name);
            }
            this.HtmlWriter.AddAttribute("liveSplitters", control.LiveSplitter.ToString().ToLower());
            this.HtmlWriter.AddAttribute("design", control.Design.ToString().ToLower());

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
                DockAttach dockAttach = child.AttachObject as DockAttach;
                string containerName = child.Content.GetType().Name;

                string styleStr = string.Empty;
                if (this.GetContainerForDockPanel(containerName))
                {
                    styleStr = "border:none !important;";
                }
                if(dockAttach.Dock == EDock.Center) this.HtmlWriter.AddAttribute("dojoType", "Controls/ContentPane");
                else this.HtmlWriter.AddAttribute("dojoType", "Controls/ExpandoPane");
                this.HtmlWriter.AddAttribute("region", dockAttach.Dock.ToString().ToLower());
                this.HtmlWriter.AddAttribute("splitter", dockAttach.AllowResize.ToString().ToLower());
                if (!string.IsNullOrEmpty(child.Title)) this.HtmlWriter.AddAttribute("title", child.Title);
                if (!IsPreview && !string.IsNullOrEmpty(child.Name))
                {
                    string ctlId = this.ProjectDocument.Name + "_" + child.Name + "s";
                    this.HtmlWriter.AddAttribute("id", ctlId);
                    this.HtmlWriter.AddAttribute("name", ctlId);
                }

                dynamic panel = child.Content;
                int panelWidth = panel.Width == null ? -1 : panel.Width;
                int panelHeight = panel.Height == null ? -1 : panel.Height;
                int panelMinWidth = panel.MinWidth == null ? -1 : panel.MinWidth;
                int panelMaxWidth = panel.MaxWidth == null ? -1 : panel.MaxWidth;
                int panelMinHeight = panel.MinHeight == null ? -1 : panel.MinHeight;
                int panelMaxHeight = panel.MaxHeight == null ? -1 : panel.MaxHeight;

                if ((dockAttach.Dock == EDock.Left || dockAttach.Dock == EDock.Right) && panelMinWidth > 0) this.HtmlWriter.AddAttribute("minSize", panelMinWidth.ToString());
                if ((dockAttach.Dock == EDock.Left || dockAttach.Dock == EDock.Right) && panelMaxWidth > 0) this.HtmlWriter.AddAttribute("maxSize", panelMaxWidth.ToString());
                if ((dockAttach.Dock == EDock.Top || dockAttach.Dock == EDock.Bottom) && panelMinHeight > 0) this.HtmlWriter.AddAttribute("minSize", panelMinHeight.ToString());
                if ((dockAttach.Dock == EDock.Top || dockAttach.Dock == EDock.Bottom) && panelMaxHeight > 0) this.HtmlWriter.AddAttribute("maxSize", panelMaxHeight.ToString());

                if (panelWidth > -1) styleStr += string.Format("width:{0}px;", panelWidth);
                if (panelHeight > -1) styleStr += string.Format("height:{0}px;", panelHeight);
                if (styleStr.Length > 0) this.HtmlWriter.AddAttribute("style", styleStr);

                this.HtmlWriter.RenderBeginTag("div");

                if (dockAttach.Dock == EDock.Left || dockAttach.Dock == EDock.Right)
                {
                    panel.Width = 0;
                    panel.MinWidth = 0;
                    panel.MaxWidth = 0;
                }
                if (dockAttach.Dock == EDock.Left || dockAttach.Dock == EDock.Right)
                {
                    panel.Height = 0;
                    panel.MinHeight = 0;
                    panel.MaxHeight = 0;
                }
                var builder = child.GetBuilder(this.IsPreview, this.ScreenDefinition, this.Compile, this.ProjectDocument, this.PermissionData, this.HtmlWriter);
                builder.Build();

                this.HtmlWriter.RenderEndTag();
            }
        }

        /// <summary>
        /// 获取可用于停靠面板控件下的容器
        /// </summary>
        /// <param name="control">控件</param>
        /// <returns></returns>
        private bool GetContainerForDockPanel(string controlName)
        {
            Dictionary<string, string> dicts = new Dictionary<string, string>();
            string[] keys = new string[]
            {
                "Accordion", "DockPanel", "TabControl", "TitlePane", "ToolBar", "Menu"
            };
            foreach (var c in keys)  dicts.Add(c, c);

            if (dicts.ContainsKey(controlName)) return true;
            else return false;
        }
    }
}

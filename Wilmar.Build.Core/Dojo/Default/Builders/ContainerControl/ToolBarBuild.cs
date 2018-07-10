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
    /// 工具栏生成器
    /// </summary>
    internal class ToolBarBuild : ContainerBuildBase
    {
        public ToolBarBuild(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
            : base(isPreview, controlHost, screenDef, compile, doc, permissionData, htmlWriter)
        {

        }

        /// <summary>
        /// 设置属性
        /// </summary>
        protected override void SetAttributes()
        {
            ToolBar control = this.ControlHost.Content as ToolBar;
            this.HtmlWriter.AddAttribute("dojoType", "Controls/Toolbar");
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
            this.HtmlWriter.AddAttribute("dojoType", "Controls/Declaration");
            this.HtmlWriter.AddAttribute("data-dojo-props", "widgetClass:'ToolbarSectionStart', defaults:{ label: 'Label'}", false);
            this.HtmlWriter.RenderBeginTag("span");
            this.HtmlWriter.AddAttribute("dojoType", "Controls/ToolbarSeparator");
            this.HtmlWriter.RenderBeginTag("span");
            this.HtmlWriter.RenderEndTag();
            this.HtmlWriter.RenderEndTag();
            this.HtmlWriter.AddAttribute("dojoType", "ToolbarSectionStart");
            this.HtmlWriter.AddAttribute("data-dojo-props", "label:'Buttons'", false);
            this.HtmlWriter.RenderBeginTag("div");
            this.HtmlWriter.RenderEndTag();

            foreach (var child in this.ControlHost.Children)
            {
                string controlName = child.Content.GetType().Name;
                if (this.GetContainerForToolBar(controlName))
                {
                    this.HtmlWriter.AddAttribute("dojoType", "Controls/DropDownButton");
                    this.HtmlWriter.RenderBeginTag("div");

                    this.HtmlWriter.RenderBeginTag("span");
                    this.HtmlWriter.WriteEncodedText(child.Title);
                    this.HtmlWriter.RenderEndTag();

                    this.HtmlWriter.AddAttribute("dojoType", "Controls/TooltipDialog");
                    this.HtmlWriter.RenderBeginTag("div");

                    var builder = child.GetBuilder(this.IsPreview, this.ScreenDefinition, this.Compile, this.ProjectDocument, this.PermissionData, this.HtmlWriter);
                    builder.Build();

                    this.HtmlWriter.RenderEndTag();
                    this.HtmlWriter.RenderEndTag();
                }
                else
                {
                    var builder = child.GetBuilder(this.IsPreview, this.ScreenDefinition, this.Compile, this.ProjectDocument, this.PermissionData, this.HtmlWriter);
                    builder.Build();
                }
            }
        }

        /// <summary>
        /// 获取容器用于工具栏
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        private bool GetContainerForToolBar(string controlName)
        {
            Dictionary<string, string> dicts = new Dictionary<string, string>();
            string[] keys = new string[]
            {
                "Accordion", "DataGrid", "DockPanel", "FormPanel", "GridPanel", "ListBox",
                "Menu", "Panel", "StackPanel", "TabControl", "ToolBar", "TitlePane", "TreeView"
            };
            foreach (var c in keys) dicts.Add(c, c);
            
            if (dicts.ContainsKey(controlName)) return true;
            else return false;
        }
    }
}

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
    /// 菜单项生成器
    /// </summary>
    internal class MenuItemBuild : ContainerBuildBase
    {
        public MenuItemBuild(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
            : base(isPreview, controlHost, screenDef, compile, doc, permissionData, htmlWriter)
        {

        }

        /// <summary>
        /// 生成
        /// </summary>
        public override void Build()
        {
            MenuItem control = (MenuItem)this.ControlHost.Content;
            Menu parentMenu = this.GetMemu();
            bool isFirst = (this.Parent.ControlHost.Content == this.GetMemu());
            bool isLast = (this.ControlHost.Children.Count == 0);
            if (isLast && isFirst)
            {
                this.BuildFirstAndLastMenuItem(parentMenu, this.HtmlWriter);
            }
            else if (isFirst)
            {
                this.BuildFirstMenuItem(parentMenu, this.HtmlWriter);
            }
            else if (isLast)
            {
                this.BuildLastMenuItem(parentMenu, this.HtmlWriter);
            }
            else
            {
                this.BuildCenterMenuItem(parentMenu, this.HtmlWriter);
            }
        }
        /// <summary>
        /// 设置属性
        /// </summary>
        protected override void SetAttributes()
        {
            var c = this.ControlHost.Content;
            dynamic control = c;

            if (c.ExistProperty("IsReadOnly") && control.IsReadOnly) this.HtmlWriter.AddAttribute("readonly", "readonly");
            if (c.ExistProperty("IsChecked") && control.IsChecked) this.HtmlWriter.AddAttribute("checked", "checked");
            if (c.ExistProperty("IsEnable") && !control.IsEnable) this.HtmlWriter.AddAttribute("disabled", "disabled");

            //获取Style样式
            string classStr = base.SetControlClass();
            if (!string.IsNullOrEmpty(classStr)) this.HtmlWriter.AddAttribute("class", classStr);

            //获取Style样式
            string styleStr = base.SetControlStyle();
            if (!string.IsNullOrEmpty(styleStr)) this.HtmlWriter.AddAttribute("style", styleStr);
        }
        /// <summary>
        /// 设置子元素
        /// </summary>
        protected override void SetChildElements()
        {
            
        }

        /// <summary>
        /// 获取父Menu控件
        /// </summary>
        /// <returns></returns>
        private Menu GetMemu()
        {
            var parent = this.Parent;
            while (parent.ControlHost.Content.GetType() != typeof(Menu))
            {
                parent = parent.Parent;
            }

            return (Menu)parent.ControlHost.Content;
        }
        /// <summary>
        /// 生成第一级并且是最后一级
        /// </summary>
        /// <param name="parentMenu"></param>
        /// <param name="htmlWriter"></param>
        private void BuildFirstAndLastMenuItem(Menu parentMenu, HtmlTextWriter htmlWriter)
        {
            MenuItem control = this.ControlHost.Content as MenuItem;
            if (parentMenu.Orientation == EOrientation.Horizontal) htmlWriter.AddAttribute("dojoType", "Controls/MenuBarItem");
            else  htmlWriter.AddAttribute("dojoType", "Controls/MenuItem");
            if (!IsPreview && !string.IsNullOrEmpty(this.ControlHost.Name))
            {
                htmlWriter.AddAttribute("id", this.ControlHost.Name);
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

            this.SetAttributes();
            htmlWriter.RenderBeginTag(this.TagName);

            //设置标题
            htmlWriter.WriteEncodedText(this.ControlHost.Title);


            htmlWriter.RenderEndTag();
        }
        /// <summary>
        /// 生成第一级
        /// </summary>
        /// <param name="parentMenu"></param>
        /// <param name="htmlWriter"></param>
        private void BuildFirstMenuItem(Menu parentMenu, HtmlTextWriter htmlWriter)
        {
            MenuItem control = this.ControlHost.Content as MenuItem;
            if (parentMenu.Orientation == EOrientation.Horizontal)  htmlWriter.AddAttribute("dojoType", "Controls/PopupMenuBarItem");
            else  htmlWriter.AddAttribute("dojoType", "Controls/PopupMenuItem");
            if (!IsPreview && !string.IsNullOrEmpty(this.ControlHost.Name))
            {
                htmlWriter.AddAttribute("id", this.ControlHost.Name);
            }

            StringBuilder sbProps = new StringBuilder();
            StringBuilder returnContent = new StringBuilder();
            string props = control.BuildControlProps(this.ScreenDefinition, this.IsPreview, this.PermissionData, returnContent);
            if (!string.IsNullOrEmpty(props)) sbProps.AppendFormat("{0},", props);
            if (sbProps.ToString().Length > 0)
            {
                this.HtmlWriter.AddAttribute("data-dojo-props", sbProps.ToString().Substring(0, sbProps.ToString().Length - 1), false);
            }

            this.SetAttributes();
            htmlWriter.RenderBeginTag(this.TagName);

            //设置标题
            htmlWriter.RenderBeginTag(this.TagName);
            htmlWriter.WriteEncodedText(this.ControlHost.Title);
            htmlWriter.RenderEndTag();


            //设置子元素
            if (this.ControlHost.Children.Count > 0)
            {
                htmlWriter.AddAttribute("dojoType", "Controls/Menu");
                htmlWriter.RenderBeginTag(this.TagName);
                foreach (var child in this.ControlHost.Children)
                {
                    var builder = child.GetBuilder(this.IsPreview, this.ScreenDefinition, this.Compile, this.ProjectDocument, this.PermissionData, this.HtmlWriter);
                    builder.Parent = this;
                    builder.Build();
                }
                htmlWriter.RenderEndTag();
            }

            htmlWriter.RenderEndTag();
        }
        /// <summary>
        /// 生成最后一级
        /// </summary>
        /// <param name="parentMenu"></param>
        /// <param name="htmlWriter"></param>
        private void BuildLastMenuItem(Menu parentMenu, HtmlTextWriter htmlWriter)
        {
            MenuItem control = this.ControlHost.Content as MenuItem;
            htmlWriter.AddAttribute("dojoType", "Controls/MenuItem");
            if (!IsPreview && !string.IsNullOrEmpty(this.ControlHost.Name))
            {
                htmlWriter.AddAttribute("id", this.ControlHost.Name);
            }

            StringBuilder sbProps = new StringBuilder();
            StringBuilder returnContent = new StringBuilder();
            string props = control.BuildControlProps(this.ScreenDefinition, this.IsPreview, this.PermissionData, returnContent);
            if (!string.IsNullOrEmpty(props)) sbProps.AppendFormat("{0},", props);
            if (sbProps.ToString().Length > 0)
            {
                this.HtmlWriter.AddAttribute("data-dojo-props", sbProps.ToString().Substring(0, sbProps.ToString().Length - 1), false);
            }

            this.SetAttributes();
            htmlWriter.RenderBeginTag(this.TagName);
            //设置标题
            htmlWriter.WriteEncodedText(this.ControlHost.Title);
            htmlWriter.RenderEndTag();

            //分割条
            MenuItem menuItem = this.ControlHost.Content as MenuItem;
            if (menuItem.Separated)
            {
                htmlWriter.AddAttribute("dojoType", "Controls/MenuSeparator");
                htmlWriter.RenderBeginTag(this.TagName);
                htmlWriter.RenderEndTag();
            }
        }
        /// <summary>
        /// 生成中间级
        /// </summary>
        /// <param name="parentMenu"></param>
        /// <param name="htmlWriter"></param>
        private void BuildCenterMenuItem(Menu parentMenu, HtmlTextWriter htmlWriter)
        {
            MenuItem control = this.ControlHost.Content as MenuItem;
            htmlWriter.AddAttribute("dojoType", "Controls/PopupMenuItem");
            if (!IsPreview && !string.IsNullOrEmpty(this.ControlHost.Name))
            {
                htmlWriter.AddAttribute("id", this.ControlHost.Name);
            }

            StringBuilder sbProps = new StringBuilder();
            StringBuilder returnContent = new StringBuilder();
            string props = control.BuildControlProps(this.ScreenDefinition, this.IsPreview, this.PermissionData, returnContent);
            if (!string.IsNullOrEmpty(props)) sbProps.AppendFormat("{0},", props);
            if (sbProps.ToString().Length > 0)
            {
                this.HtmlWriter.AddAttribute("data-dojo-props", sbProps.ToString().Substring(0, sbProps.ToString().Length - 1), false);
            }

            this.SetAttributes();
            htmlWriter.RenderBeginTag(this.TagName);

            //设置标题
            htmlWriter.RenderBeginTag(this.TagName);
            htmlWriter.WriteEncodedText(this.ControlHost.Title);
            htmlWriter.RenderEndTag();

            //设置子元素
            if (this.ControlHost.Children.Count > 0)
            {
                htmlWriter.AddAttribute("dojoType", "Controls/Menu");
                htmlWriter.RenderBeginTag(this.TagName);
                foreach (var child in this.ControlHost.Children)
                {
                    var builder = child.GetBuilder(this.IsPreview, this.ScreenDefinition, this.Compile, this.ProjectDocument, this.PermissionData, this.HtmlWriter);
                    builder.Parent = this;
                    builder.Build();
                }
                htmlWriter.RenderEndTag();
            }

            htmlWriter.RenderEndTag();

            //分割条
            MenuItem menuItem = this.ControlHost.Content as MenuItem;
            if (menuItem.Separated)
            {
                htmlWriter.AddAttribute("dojoType", "Controls/MenuSeparator");
                htmlWriter.RenderBeginTag(this.TagName);
                htmlWriter.RenderEndTag();
            }
        }
    }
}

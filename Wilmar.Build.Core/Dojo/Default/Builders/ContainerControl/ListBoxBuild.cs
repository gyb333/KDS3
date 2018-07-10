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
    /// ListBox控件生成器
    /// </summary>
    internal class ListBoxBuild : ContainerBuildBase
    {
        public ListBoxBuild(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
            : base(isPreview, controlHost, screenDef, compile, doc, permissionData, htmlWriter)
        {

        }

        /// <summary>
        /// 设置属性
        /// </summary>
        protected override void SetAttributes()
        {
            ListBox control = this.ControlHost.Content as ListBox;
            this.HtmlWriter.AddAttribute("dojoType", "Controls/ListBox");
            if (!IsPreview && !string.IsNullOrEmpty(this.ControlHost.Name))
            {
                this.HtmlWriter.AddAttribute("id", this.ControlHost.Name);
                this.HtmlWriter.AddAttribute("name", this.ControlHost.Name);
            }

            StringBuilder sbProps = new StringBuilder();
            StringBuilder returnContent = new StringBuilder();
            string props = control.BuildControlProps(this.ScreenDefinition, this.IsPreview, this.PermissionData, returnContent);
            if (!string.IsNullOrEmpty(props)) sbProps.AppendFormat("{0},", props);
            string stores = GetStore();
            string dojoProps = stores + returnContent;
            if (!string.IsNullOrEmpty(dojoProps)) sbProps.AppendFormat("{0},", dojoProps);

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
            if (this.ControlHost.Children.Count > 0)
            {
                this.HtmlWriter.RenderBeginTag("li");

                foreach (var child in this.ControlHost.Children)
                {
                    var builder = child.GetBuilder(this.IsPreview, this.ScreenDefinition, this.Compile, this.ProjectDocument, this.PermissionData, this.HtmlWriter);
                    builder.Parent = this;
                    builder.Build();
                }

                this.HtmlWriter.RenderEndTag();
            }
        }

        /// <summary>
        /// 设置Store
        /// </summary>
        /// <returns></returns>
        private string GetStore()
        {
            ListBox control = this.ControlHost.Content as ListBox;

            StringBuilder result = new StringBuilder();
            bool bindingDataSource = false;
            Dictionary<string, string> dictProperty = control.GetPropertyBindValue();
            if (!IsPreview && control.Bindings.Count > 0)
            {
                foreach (var item in control.Bindings)
                {
                    string bindPath = item.Path == null ? "" : item.Path;
                    string bindProperty = item.Property == null ? "" : item.Property;
                    if (bindProperty.ToLower() == "datasource")
                    {
                        string property = string.Empty;
                        if (dictProperty.ContainsKey(bindProperty))
                        {
                            if (dictProperty.TryGetValue(bindProperty, out property)) bindProperty = property;
                        }
                        if (!string.IsNullOrEmpty(bindPath) && !string.IsNullOrEmpty(bindProperty))
                        {
                            result.AppendFormat("store:at('rel:{0}', '{1}').direction(1),", "", bindPath);
                            if (bindProperty.ToLower() == "datasource") bindingDataSource = true;
                        }
                    }
                }
            }
            if (!IsPreview && control.ExistProperty("DataSource") && !bindingDataSource)
            {
                string bindPath = control.DataSource;
                if (!string.IsNullOrEmpty(bindPath))
                {
                    result.AppendFormat("store:at('rel:{0}', '{1}').direction(1),", "", bindPath);
                }
            }

            return result.ToString();
        }
    }
}

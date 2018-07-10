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
    /// 菜单生成器
    /// </summary>
    internal class MenuBuild : ContainerBuildBase
    {
        public MenuBuild(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
            : base(isPreview, controlHost, screenDef, compile, doc, permissionData, htmlWriter)
        {

        }

        /// <summary>
        /// 设置属性
        /// </summary>
        protected override void SetAttributes()
        {
            Menu control = (Menu)this.ControlHost.Content;
            if (control.Orientation == EOrientation.Horizontal) this.HtmlWriter.AddAttribute("dojoType", "Controls/MenuBar");
            else this.HtmlWriter.AddAttribute("dojoType", "Controls/Menu");
            if (!IsPreview && !string.IsNullOrEmpty(this.ControlHost.Name))
            {
                this.HtmlWriter.AddAttribute("id", this.ControlHost.Name);
                this.HtmlWriter.AddAttribute("name", this.ControlHost.Name);
            }
            string stores = GetStore();
            if (!IsPreview && !string.IsNullOrEmpty(stores))
            {
                this.HtmlWriter.AddAttribute("store", stores, false);
            }

            string props = GetDataGridProsps(this.ControlHost);
            if (!string.IsNullOrEmpty(props))
            {
                this.HtmlWriter.AddAttribute("data-dojo-props", props, false);
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
                var builder = child.GetBuilder(this.IsPreview, this.ScreenDefinition, this.Compile, this.ProjectDocument, this.PermissionData, this.HtmlWriter);
                builder.Parent = this;
                builder.Build();
            }
        }

        /// <summary>
        /// 设置Prosps
        /// </summary>
        /// <returns></returns>
        private string GetDataGridProsps(ControlHost controlHost)
        {
            string result = string.Empty, controlName = controlHost.Content.GetType().Name;
            if (controlName == "Menu")
            {
                Menu control = controlHost.Content as Menu;
                StringBuilder returnContent = new StringBuilder();
                result = control.BuildControlProps(this.ScreenDefinition, this.IsPreview, this.PermissionData, returnContent);
            }
            else if (controlName == "MenuItem")
            {
                MenuItem control = controlHost.Content as MenuItem;
                StringBuilder returnContent = new StringBuilder();
                result = control.BuildControlProps(this.ScreenDefinition, this.IsPreview, this.PermissionData, returnContent);
            }
            return result;
        }
        /// <summary>
        /// 设置Store
        /// </summary>
        /// <returns></returns>
        private string GetStore()
        {
            Menu control = (Menu)this.ControlHost.Content;
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
                            result.AppendFormat("at('rel:{0}', '{1}').direction(1),", "", bindPath);
                            if (bindProperty.ToLower() == "datasource") bindingDataSource = true;
                        }
                    }
                }
            }
            if (!IsPreview && control.ExistProperty("DataSource") && !bindingDataSource)
            {
                string bindPath = control.DataSource;
                string bindProperty = "DataSource";
                if (!string.IsNullOrEmpty(bindPath))
                {
                    string property = string.Empty;
                    if (dictProperty.ContainsKey(bindProperty))
                    {
                        if (dictProperty.TryGetValue(bindProperty, out property)) bindProperty = property;
                    }
                    result.AppendFormat("at('rel:{0}', '{1}').direction(1),", "", bindPath);
                }
            }

            return result.ToString().Length == 0 ? "" : result.ToString().Substring(0, result.Length - 1);
        }
    }
}

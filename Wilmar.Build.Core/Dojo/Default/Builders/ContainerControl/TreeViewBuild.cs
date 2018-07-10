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
    /// 树控件生成器
    /// </summary>
    internal class TreeViewBuild : ContainerBuildBase
    {
        public TreeViewBuild(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
            : base(isPreview, controlHost, screenDef, compile, doc, permissionData, htmlWriter)
        {

        }

        /// <summary>
        /// 设置属性
        /// </summary>
        protected override void SetAttributes()
        {
            TreeView control = (TreeView)this.ControlHost.Content;
            this.HtmlWriter.AddAttribute("dojoType", "Controls/Tree");
            if (!IsPreview && !string.IsNullOrEmpty(this.ControlHost.Name))
            {
                this.HtmlWriter.AddAttribute("id", this.ControlHost.Name);
                this.HtmlWriter.AddAttribute("name", this.ControlHost.Name);
            }
            this.HtmlWriter.AddAttribute("autoExpand", control.AutoExpand.ToString().ToLower());
            if (!string.IsNullOrEmpty(control.NodeTitle))
            {
                this.HtmlWriter.AddAttribute("label", control.NodeTitle);
            }
            if (control.ClickExpand)
            {
                this.HtmlWriter.AddAttribute("openOnClick", "true");
                this.HtmlWriter.AddAttribute("openOnDblClick", "false");
            }
            else
            {
                this.HtmlWriter.AddAttribute("openOnClick", "false");
                this.HtmlWriter.AddAttribute("openOnDblClick", "true");
            }
           
            string stores = GetStore();
            if (!IsPreview && !string.IsNullOrEmpty(stores))
            {
                this.HtmlWriter.AddAttribute("store", stores, false);
            }

            StringBuilder sbProps = new StringBuilder();
            StringBuilder returnContent = new StringBuilder();
            string props = control.BuildControlProps(this.ScreenDefinition, this.IsPreview, this.PermissionData, returnContent);
            if (!string.IsNullOrEmpty(props)) sbProps.AppendFormat("{0},", props);
            if (control.CheckBoxes) sbProps.AppendFormat("checkBoxes:{0},", control.CheckBoxes.ToString().ToLower());
            if (!IsPreview)
            {
                sbProps.AppendFormat("autoExpandChild:{0},", control.AutoExpandChild.ToString().ToLower());
                sbProps.AppendFormat("{0},", "showRoot:false");
                sbProps.AppendFormat("{0},", "context:at('rel:','VM')");
            }
            if (sbProps.ToString().Length > 0)
            {
                this.HtmlWriter.AddAttribute("data-dojo-props", sbProps.ToString().Substring(0, sbProps.ToString().Length - 1), false);
            }

            base.SetAttributes();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string GetStore()
        {
            TreeView control = (TreeView)this.ControlHost.Content;
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
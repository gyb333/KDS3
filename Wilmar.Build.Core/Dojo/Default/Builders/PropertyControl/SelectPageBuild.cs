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
    /// 下拉分页选择框生成器
    /// </summary>
    internal class SelectPageBuild : ControlBuildBase
    {
        public SelectPageBuild(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
            : base(isPreview, controlHost, screenDef, compile, doc, permissionData, htmlWriter)
        {

        }

        /// <summary>
        /// 设置属性
        /// </summary>
        protected override void SetAttributes()
        {
            SelectPage control = this.ControlHost.Content as SelectPage;
            this.HtmlWriter.AddAttribute("dojoType", "Controls/SelectPage");
            if (!IsPreview && !string.IsNullOrEmpty(this.ControlHost.Name))
            {
                this.HtmlWriter.AddAttribute("id", this.ControlHost.Name);
                this.HtmlWriter.AddAttribute("name", this.ControlHost.Name);
            }
            if (!IsPreview) this.HtmlWriter.AddAttribute("forScreen", this.ProjectDocument.Name);

            string stores = GetStore();
            if (!string.IsNullOrEmpty(stores))
            {
                this.HtmlWriter.AddAttribute("store", stores, false);
            }
            StringBuilder sbProps = new StringBuilder();
            StringBuilder returnContent = new StringBuilder();
            string props = control.BuildControlProps(this.ScreenDefinition, this.IsPreview, this.PermissionData, returnContent);
            if (!string.IsNullOrEmpty(props)) sbProps.AppendFormat("{0},", props);
            string searchAttr = GetSearchAttr();
            if (!string.IsNullOrEmpty(searchAttr)) sbProps.AppendFormat("{0},", searchAttr);
            sbProps.AppendFormat("{0},", "multiple:" + control.Multiple.ToString().ToLower());
            sbProps.AppendFormat("{0},", "pagination:" + control.Pagination.ToString().ToLower());
            sbProps.AppendFormat("{0},", "dropButton:" + control.DropButton.ToString().ToLower());
            sbProps.AppendFormat("{0},", "selectToCloseList:" + control.SelectToCloseList.ToString().ToLower());
            sbProps.AppendFormat("{0},", "autoSelectFirst:" + control.AutoSelectFirst.ToString().ToLower());
            sbProps.AppendFormat("{0},", "maxSelectLimit:" + control.MaxSelectLimit.ToString());
            sbProps.AppendFormat("{0},", "inputDelay:" + control.InputDelay.ToString());
            if(control.ShowSearch) sbProps.AppendFormat("{0},", "showSearch:false");
            else sbProps.AppendFormat("{0},", "showSearch:true");
            if (!string.IsNullOrEmpty(control.SelectIndex)) sbProps.AppendFormat("{0},", "selectIndex:'" + control.SelectIndex.ToString() + "'");
            string dropDownButtonName = string.Empty;
            if (!IsPreview && this.Parent != null) dropDownButtonName = BuildCommonMethod.GetIsDropDownButton(this.Parent);
            sbProps.AppendFormat("{0},", "dropDownButtonName:'" + dropDownButtonName + "'");

            if (sbProps.ToString().Length > 0)
            {
                this.HtmlWriter.AddAttribute("data-dojo-props", sbProps.ToString().Substring(0, sbProps.ToString().Length - 1), false);
            }

            base.SetAttributes();
        }

        /// <summary>
        /// 设置Store
        /// </summary>
        /// <returns></returns>
        private string GetStore()
        {
            StringBuilder sb = new StringBuilder();
            bool bindingDataSource = false;
            SelectPage control = this.ControlHost.Content as SelectPage;
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
                            string path = bindPath;
                            sb.AppendFormat("at('rel:{0}', '{1}').direction(1),", "", path);
                            bindingDataSource = true;
                        }
                    }
                }
            }
            if (!IsPreview && control.ExistProperty("DataSource") && !bindingDataSource)
            {
                string bindPath = control.DataSource;
                if (!string.IsNullOrEmpty(bindPath))
                {
                    sb.AppendFormat("at('rel:{0}', '{1}').direction(1),", "", bindPath);
                }
            }
            string result = sb.ToString().Length == 0 ? "" : sb.ToString().Substring(0, sb.Length - 1);

            return result;
        }
        /// <summary>
        /// 设置搜索成员
        /// </summary>
        /// <returns></returns>
        private string GetSearchAttr()
        {
            StringBuilder result = new StringBuilder();
            SelectPage control = this.ControlHost.Content as SelectPage;
            if (!IsPreview && control.Bindings.Count > 0)
            {
                foreach (var item in control.Bindings)
                {
                    string bindPath = item.Path == null ? "" : item.Path;
                    string bindProperty = item.Property == null ? "" : item.Property;
                    string field = bindPath.Split('.')[bindPath.Split('.').Length - 1];
                    if (bindProperty.ToLower() == "searchvalue" && !string.IsNullOrEmpty(bindPath) && !string.IsNullOrEmpty(bindProperty))
                    {
                        string path = string.Empty;
                        if (bindPath.Split('.').Length > 1) path = bindPath.Split('.')[0];
                        result.AppendFormat("{0}: at('rel:{1}','{2}')", "searchField", path, field);
                    }
                }
            }
            if (result.ToString().Length == 0)
            {
                if (!string.IsNullOrEmpty(control.SearchValue)) result.AppendFormat("{0}:'@{1}',", "searchField", control.SearchValue);
                else if (!string.IsNullOrEmpty(control.DisplayMember)) result.AppendFormat("{0}:'@{1}'", "searchField", control.DisplayMember);
            }
            return result.ToString();
        }
    }
}

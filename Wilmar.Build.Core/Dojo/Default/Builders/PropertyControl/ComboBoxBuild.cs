using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Xml;
using Wilmar.Foundation.Projects;
using Wilmar.Model.Core.Definitions;
using Wilmar.Model.Core.Definitions.Screens;
using Wilmar.Model.Core.Definitions.Screens.Controls;
using Wilmar.Model.Core.Definitions.Screens.Members;
using Wilmar.Service.Common.Generate;

namespace Wilmar.Build.Core.Dojo.Default.Builders
{
    /// <summary>
    /// 下拉框生成器
    /// </summary>
    internal class ComboBoxBuild : ControlBuildBase
    {
        public ComboBoxBuild(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
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
                return "select";
            }
        }
        /// <summary>
        /// 设置属性
        /// </summary>
        protected override void SetAttributes()
        {
            ComboBox control = this.ControlHost.Content as ComboBox;
            this.HtmlWriter.AddAttribute("dojoType", "Controls/FilteringSelect");
            if (!IsPreview && !string.IsNullOrEmpty(this.ControlHost.Name))
            {
                this.HtmlWriter.AddAttribute("id", this.ControlHost.Name);
                this.HtmlWriter.AddAttribute("name", this.ControlHost.Name);
            }
            if (!string.IsNullOrEmpty(control.ToolTip) && this.ProjectDocument != null)
            {
                this.HtmlWriter.AddAttribute("tooltip-name", this.ProjectDocument.Name + "_" + control.ToolTip);
            }
            this.HtmlWriter.AddAttribute("required", "false");

            string stores = GetStore();
            if (!string.IsNullOrEmpty(stores))
            {
                this.HtmlWriter.AddAttribute("store", stores, false);
            }

            StringBuilder sbProps = new StringBuilder();
            StringBuilder returnContent = new StringBuilder();
            string props = control.BuildControlProps(this.ScreenDefinition, this.IsPreview, this.PermissionData, returnContent);
            if (!string.IsNullOrEmpty(props)) sbProps.AppendFormat("{0},", props);
            if (control.AutoLoadData) sbProps.AppendFormat("{0},", "autoLoadData:false");

            //if (control.SelectedIndex != null) sbProps.AppendFormat("selectedIndex:{0},", control.SelectedIndex.ToString());
            sbProps.AppendFormat("{0},", "maxHeight:200");
            #region pageSize
            var pageSize = 0;
            var path = string.Empty; //绑定数据源名称
            //获取pageSize
            if (!string.IsNullOrEmpty(control.DataSource))
            {
                path = control.DataSource;
            }
            else
            {
                if (control.Bindings.Count > 0)
                {
                    var dsData = (from t in control.Bindings where t.Property != null && t.Property.ToLower() == "datasource" select t).FirstOrDefault();
                    if (dsData != null)
                    {
                        if (dsData.Path != null) path = dsData.Path;
                    }
                }
            }
            if (!string.IsNullOrEmpty(path))
            {
                var screenMember = (from t in this.ScreenDefinition.Children where t.Name == path select t).FirstOrDefault();
                if (screenMember != null)
                {
                    if (screenMember.MemberType == EMemberType.DataSet)
                    {
                        pageSize = (screenMember as DataSet).PageSize;
                    }
                }
            }
            if (pageSize > 0)
            {
                sbProps.AppendFormat("{0},", "pageSize:" + pageSize + "");
            }
            #endregion

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
            ComboBox control = this.ControlHost.Content as ComboBox;
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
    }
}

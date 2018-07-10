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
using Wilmar.Service.Common.Generate;

namespace Wilmar.Build.Core.Dojo.Default.Builders
{
    /// <summary>
    /// 单选组合框生成器
    /// </summary>
    internal class RadioButtonListBuild : ControlBuildBase
    {
        public RadioButtonListBuild(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
            : base(isPreview, controlHost, screenDef, compile, doc, permissionData, htmlWriter)
        {

        }

        /// <summary>
        /// 设置属性
        /// </summary>
        protected override void SetAttributes()
        {
            if (!this.IsPreview)
            {
                RadioButtonList control = this.ControlHost.Content as RadioButtonList;
                this.HtmlWriter.AddAttribute("dojoType", "Controls/RadioButtonList");
                if (!IsPreview && !string.IsNullOrEmpty(this.ControlHost.Name))
                {
                    this.HtmlWriter.AddAttribute("id", this.ControlHost.Name);
                    this.HtmlWriter.AddAttribute("name", this.ControlHost.Name);
                }
                if (!string.IsNullOrEmpty(control.ToolTip) && this.ProjectDocument != null)
                {
                    this.HtmlWriter.AddAttribute("tooltip-name", this.ProjectDocument.Name + "_" + control.ToolTip);
                }


                string stores = GetStore();
                if (!string.IsNullOrEmpty(stores))
                {
                    this.HtmlWriter.AddAttribute("store", stores, false);
                }

                StringBuilder sbProps = new StringBuilder();
                StringBuilder returnContent = new StringBuilder();
                string props = control.BuildControlProps(this.ScreenDefinition, this.IsPreview, this.PermissionData, returnContent);
                if (!string.IsNullOrEmpty(props)) sbProps.AppendFormat("{0},", props);
                sbProps.AppendFormat("orientation:'{0}',", control.Orientation.ToString().ToLower());
                if (sbProps.ToString().Length > 0)
                {
                    this.HtmlWriter.AddAttribute("data-dojo-props", sbProps.ToString().Substring(0, sbProps.ToString().Length - 1), false);
                }
            }

            base.SetAttributes();
        }
        /// <summary>
        /// 设置子元素
        /// </summary>
        protected override void SetChildElements()
        {
            //预览自定义
            if (this.IsPreview)
            {
                RadioButtonList control = this.ControlHost.Content as RadioButtonList;
                //第一项
                if (control.Orientation == EOrientation.Horizontal)
                {
                    this.HtmlWriter.AddAttribute("style", "display:inline;");
                }
                this.HtmlWriter.RenderBeginTag("div");
                this.HtmlWriter.AddAttribute("dojoType", "Controls/RadioButton");
                this.HtmlWriter.AddAttribute("style", "width:16px !important;height:16px !important;");
                this.HtmlWriter.RenderBeginTag("input");
                this.HtmlWriter.RenderEndTag();
                this.HtmlWriter.AddAttribute("for", "radio1");
                this.HtmlWriter.RenderBeginTag("label");
                this.HtmlWriter.WriteEncodedText("Radio1");
                this.HtmlWriter.RenderEndTag();
                this.HtmlWriter.RenderEndTag();

                //第二项
                if (control.Orientation == EOrientation.Horizontal)
                {
                    this.HtmlWriter.AddAttribute("style", "display:inline;");
                }
                this.HtmlWriter.RenderBeginTag("div");
                this.HtmlWriter.AddAttribute("dojoType", "Controls/RadioButton");
                this.HtmlWriter.AddAttribute("style", "width:16px !important;height:16px !important;");
                this.HtmlWriter.RenderBeginTag("input");
                this.HtmlWriter.RenderEndTag();
                this.HtmlWriter.AddAttribute("for", "radio2");
                this.HtmlWriter.RenderBeginTag("label");
                this.HtmlWriter.WriteEncodedText("Radio2");
                this.HtmlWriter.RenderEndTag();
                this.HtmlWriter.RenderEndTag();
            }
        }

        /// <summary>
        /// 设置Store
        /// </summary>
        /// <returns></returns>
        private string GetStore()
        {
            StringBuilder sb = new StringBuilder();
            bool bindingDataSource = false;
            RadioButtonList control = this.ControlHost.Content as RadioButtonList;
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
                string bindProperty = "DataSource";
                if (!string.IsNullOrEmpty(bindPath))
                {
                    string path = bindPath;
                    sb.AppendFormat("at('rel:{0}', '{1}').direction(1),", "", path);
                }
            }
            string result = sb.ToString().Length == 0 ? "" : sb.ToString().Substring(0, sb.Length - 1);

            return result;
        }
    }
}

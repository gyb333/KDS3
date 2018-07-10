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

namespace Wilmar.Build.Core.Ionic.Default.Builders
{
    internal class IonItemSlidingBuild : ContainerBuildBase
    {
        public IonItemSlidingBuild(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
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
                return "wm-item-sliding";
            }
        }
        /// <summary>
        /// 设置属性
        /// </summary>
        protected override void SetAttributes()
        {
            IonItemSliding control = this.ControlHost.Content as IonItemSliding;
            //Store绑定
            string store = GetStore();
            if (!string.IsNullOrEmpty(store)) this.HtmlWriter.AddAttribute(store, null);
            //控件属性绑定
            string bindPropertyStr = BuildCommon.BuildControlBindProperty(control, this.ScreenDefinition, this.IsPreview);
            if (!string.IsNullOrEmpty(bindPropertyStr)) this.HtmlWriter.AddAttribute(bindPropertyStr, null);

            base.SetAttributes();
        }

        private string GetStore()
        {
            IonItemSliding control = this.ControlHost.Content as IonItemSliding;
            bool bindingDataSource = false;
            StringBuilder result = new StringBuilder();
            if (!this.IsPreview && control.Bindings.Count > 0)
            {
                foreach (var item in control.Bindings)
                {
                    string bindPath = item.Path == null ? "" : item.Path;
                    string bindProperty = item.Property == null ? "" : item.Property;
                    if (bindProperty.ToLower() == "datasource")
                    {
                        if (!string.IsNullOrEmpty(bindPath) && !string.IsNullOrEmpty(bindProperty))
                        {
                            result.AppendFormat("*ngFor=\"let item of {0}\"", bindPath);
                            bindingDataSource = true;
                        }
                        break;
                    }
                }
            }
            if (!this.IsPreview && control.ExistProperty("DataSource") && !bindingDataSource)
            {
                string bindPath = control.DataSource;
                if (!string.IsNullOrEmpty(bindPath))
                {
                    result.AppendFormat("*ngFor=\"let item of {0}\"", bindPath);
                }
            }
            return result.ToString();
        }
    }
}

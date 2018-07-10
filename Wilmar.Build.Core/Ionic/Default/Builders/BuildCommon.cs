using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wilmar.Model.Core.Definitions;
using Wilmar.Model.Core.Definitions.Screens;

namespace Wilmar.Build.Core.Ionic.Default.Builders
{
    public static class BuildCommon
    {
        /// <summary>
        /// 判断控件中是否存属性
        /// </summary>
        /// <param  name="control">控件</param>
        /// <param name="propertyName">属性名称</param>
        /// <returns></returns>
        public static bool ExistProperty(this ControlBase controlBase, string propertyName)
        {
            var property = controlBase.GetType().GetProperty(propertyName);
            return property != null;
        }

        public static string BuildControlBindProperty(this ControlBase controlBase, ScreenDefinition screenDef, bool isPreview)
        {
            StringBuilder result = new StringBuilder();
            string controlName = controlBase.GetType().Name;
            if (!isPreview && controlBase.Bindings.Count > 0)
            {
                foreach (var item in controlBase.Bindings)
                {
                    string bindPath = item.Path == null ? "" : item.Path.Replace("CurrentItem", "SelectedItem");
                    string bindProperty = item.Property == null ? "" : item.Property;
                    #region DataSource
                    if (bindProperty.ToLower() == "datasource")
                    {
                        continue;
                    }
                    #endregion
                    #region Value
                    if (bindProperty.ToLower() == "value")
                    {
                        string bindReslt = $"[(ngModel)]=\"{bindPath}\"";
                        if (controlName.ToLower() == "ionsegmentcontent") bindReslt = $"[ngSwitch]=\"{bindPath}\"";
                        result.Append(bindReslt);
                    }
                    #endregion
                    #region OnClick
                    else if (bindProperty.ToLower() == "onclick")
                    {
                        string bindResult = $"(click)=\"{bindPath}()\"";
                        if (controlName.ToLower() == "ionitemsliding") bindResult = $"(click)=\"{bindPath}(item)\"";
                        if (controlName.ToLower() == "ionrefresher") bindResult = $"(ionRefresh)=\"{bindPath}($event)\"";
                        if (controlName.ToLower() == "ioninfinitescroll") bindResult = $"(ionInfinite)=\"{bindPath}($event)\"";
                        result.Append(bindResult);
                    }
                    #endregion
                    #region Url
                    if (bindProperty.ToLower() == "url")
                    {
                        result.AppendFormat("[root]=\"{0}\"", bindPath);
                    }
                    #endregion
                }
            }
            return result.ToString();
        }
        public static string BuildControlBindTextProp(this ControlBase controlBase, ScreenDefinition screenDef, bool isPreview)
        {
            StringBuilder result = new StringBuilder();
            string controlName = controlBase.GetType().Name;
            if (!isPreview && controlBase.Bindings.Count > 0)
            {
                foreach (var item in controlBase.Bindings)
                {
                    string bindPath = item.Path == null ? "" : item.Path.Replace("CurrentItem", "SelectedItem");
                    string bindProperty = item.Property == null ? "" : item.Property;
                    #region Value
                    if (bindProperty.ToLower() == "value")
                    {
                        result.Append("{{");
                        if (bindPath.Split('.').Length == 1) result.AppendFormat("{0}", bindPath);
                        else
                        {
                            string field = bindPath.Split('.')[bindPath.Split('.').Length - 1];
                            result.AppendFormat("item.{0}", field);
                        }
                        result.Append("}}");
                        break;
                    }
                    #endregion
                }
            }
            return result.ToString();
        }
    }
}

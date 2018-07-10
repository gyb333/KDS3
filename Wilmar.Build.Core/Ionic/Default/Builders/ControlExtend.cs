using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using Wilmar.Foundation.Projects;
using Wilmar.Model.Core.Definitions;
using Wilmar.Model.Core.Definitions.Screens;
using Wilmar.Service.Common.Generate;

namespace Wilmar.Build.Core.Ionic.Default.Builders
{
    /// <summary>
    /// 控件基类扩展
    /// </summary>
    public static class ControlExtend
    {
        /// <summary>
        /// 获取生成器
        /// </summary>
        /// <param name="control">控件对象</param>
        /// <returns>返回控件对应的生成器</returns>
        public static ControlBuildBase GetBuilder(this ControlHost controlHost, bool isPreview, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
        {
            if (!isPreview && doc != null && !string.IsNullOrEmpty(controlHost.Name))
            {
                controlHost.Name = doc.Name + "_" + controlHost.Name;
            }
            if (controlHost.Content != null)
            {
                var assembly = Assembly.GetAssembly(typeof(ControlExtend));
                string typeName = string.Format("Wilmar.Build.Core.Ionic.Default.Builders.{0}Build", controlHost.Content.GetType().Name);
                if (assembly.GetType(typeName) == null)
                {
                    throw new Exception(string.Format("类型【{0}】没对应的生成器。", typeName));
                }
                ControlBuildBase builder = Activator.CreateInstance(assembly.GetType(typeName), isPreview, controlHost, screenDef, compile, doc, permissionData, htmlWriter) as ControlBuildBase;
                return builder;
            }
            else return null;
        }
    }
}

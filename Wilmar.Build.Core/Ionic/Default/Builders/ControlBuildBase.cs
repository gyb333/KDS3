using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using Wilmar.Foundation.Projects;
using Wilmar.Model.Core.Definitions;
using Wilmar.Model.Core.Definitions.Screens;
using Wilmar.Service.Common.Generate;

namespace Wilmar.Build.Core.Ionic.Default.Builders
{
    public abstract class ControlBuildBase
    {
        #region 
        protected ScreenDefinition ScreenDefinition
        {
            get;
            set;
        }
        protected CompileBase Compile
        {
            get;
            set;
        }
        protected ProjectDocument ProjectDocument
        {
            get;
            set;
        }
        protected HtmlTextWriter HtmlWriter
        {
            get;
            set;
        }
        protected bool IsPreview
        {
            get;
            set;
        }
        public ControlHost ControlHost
        {
            get;
            set;
        }
        public ControlBuildBase Parent
        {
            get;
            set;
        }
        public Dictionary<int, Tuple<int, string>> PermissionData
        {
            get;
            set;
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="isPreview">是否预览</param>
        /// <param name="controlHost">控件</param>
        /// <param name="compile">编译器对象</param>
        /// <param name="htmlWriter">htmlWriter</param>
        public ControlBuildBase(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
        {
            this.IsPreview = isPreview;
            this.ControlHost = controlHost;
            this.ScreenDefinition = screenDef;
            this.Compile = compile;
            this.ProjectDocument = doc;
            this.PermissionData = permissionData;
            this.HtmlWriter = htmlWriter;
        }
        #endregion

        /// <summary>
        /// 生成HTML
        /// </summary>
        public virtual void Build()
        {
            //设置属性
            this.SetAttributes();

            //创建开始标签
            this.HtmlWriter.RenderBeginTag(this.TagName);

            //设置子元素
            this.SetChildElements();

            //创建结束标签
            this.HtmlWriter.RenderEndTag();
        }
        /// <summary>
        /// 输出元素标签名
        /// </summary>
        protected virtual string TagName
        {
            get
            {
                return "div";
            }
        }
        /// <summary>
        /// 设置子元素
        /// </summary>
        protected virtual void SetChildElements()
        {

        }

        /// <summary>
        /// 设置属性
        /// </summary>
        protected virtual void SetAttributes()
        {
            var c = this.ControlHost.Content;
            dynamic control = c;

            //获取Class样式名
            string classStr = SetControlClass();
            if (!string.IsNullOrEmpty(classStr)) this.HtmlWriter.AddAttribute("class", classStr);
        }

        /// <summary>
        /// 获取Class样式
        /// </summary>
        /// <returns></returns>
        protected virtual string SetControlClass()
        {
            var c = this.ControlHost.Content;
            dynamic control = c;
            StringBuilder sbAttribute = new StringBuilder();
            if (c.ExistProperty("StyleName"))
            {
                if (!string.IsNullOrEmpty(control.StyleName)) sbAttribute.Append(control.StyleName);
            }
            return sbAttribute.ToString();
        }
    }
}

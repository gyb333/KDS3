﻿using System;
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
    /// 年月选择框生成器
    /// </summary>
    internal class MonthYearTextBoxBuild : ControlBuildBase
    {
        public MonthYearTextBoxBuild(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
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
                return "input";
            }
        }
        /// <summary>
        /// 设置属性
        /// </summary>
        protected override void SetAttributes()
        {
            MonthYearTextBox control = this.ControlHost.Content as MonthYearTextBox;
            if (control.MonthYearType == EMonthYearType.MonthAndYear)
            {
                this.HtmlWriter.AddAttribute("dojoType", "Controls/MonthYearTextBox");
            }
            else if (control.MonthYearType == EMonthYearType.Year)
            {
                this.HtmlWriter.AddAttribute("dojoType", "Controls/YearTextBox");
            }

            if (!IsPreview && !string.IsNullOrEmpty(this.ControlHost.Name))
            {
                this.HtmlWriter.AddAttribute("id", this.ControlHost.Name);
                this.HtmlWriter.AddAttribute("name", this.ControlHost.Name);
            }

            StringBuilder sbProps = new StringBuilder();
            StringBuilder sbConstraints = new StringBuilder();
            StringBuilder returnContent = new StringBuilder();
            string props = control.BuildControlProps(this.ScreenDefinition, this.IsPreview, this.PermissionData, returnContent, sbConstraints);
            if (!string.IsNullOrEmpty(props)) sbProps.AppendFormat("{0},", props);
            if (sbConstraints.ToString().Length > 0)
            {
                string constrains = sbConstraints.ToString().Substring(0, sbConstraints.ToString().Length - 1);
                sbProps.Append("constraints:{" + constrains + "},");
            }
            if (sbProps.ToString().Length > 0)
            {
                this.HtmlWriter.AddAttribute("data-dojo-props", sbProps.ToString().Substring(0, sbProps.ToString().Length - 1), false);
            }

            base.SetAttributes();
        }
    }
}

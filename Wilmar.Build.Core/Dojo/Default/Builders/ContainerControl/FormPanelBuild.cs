using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Linq;
using Wilmar.Foundation.Projects;
using Wilmar.Model.Core.Definitions;
using Wilmar.Model.Core.Definitions.Screens;
using Wilmar.Model.Core.Definitions.Screens.Members;
using Wilmar.Model.Core.Definitions.Screens.Controls;
using Wilmar.Model.Core.Definitions.Entities.DataTypes;
using Wilmar.Service.Common.Generate;
using System;
using Wilmar.Foundation;

namespace Wilmar.Build.Core.Dojo.Default.Builders
{
    /// <summary>
    /// 表单控件生成器
    /// </summary>
    internal class FormPanelBuild : ContainerBuildBase
    {
        public FormPanelBuild(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
            : base(isPreview, controlHost, screenDef, compile, doc, permissionData, htmlWriter)
        {

        }

        /// <summary>
        /// 设置属性
        /// </summary>
        protected override void SetAttributes()
        {
            FormPanel control = (FormPanel)this.ControlHost.Content;
            this.HtmlWriter.AddAttribute("dojoType", "Controls/TableContainer");
            this.HtmlWriter.AddAttribute("customClass", "greyBlueLNF");
            if (!IsPreview && !string.IsNullOrEmpty(this.ControlHost.Name))
            {
                this.HtmlWriter.AddAttribute("id", this.ControlHost.Name);
                this.HtmlWriter.AddAttribute("name", this.ControlHost.Name);
            }
            this.HtmlWriter.AddAttribute("cols", control.ColumnCount.ToString());
            string labelWidth = "-1";
            if (control.LabelWidth > 0) labelWidth = control.LabelWidth.ToString();
            this.HtmlWriter.AddAttribute("labelWidth", labelWidth);
            if (control.Orientation == EOrientation.Vertical) this.HtmlWriter.AddAttribute("orientation", "vert");
            if (!control.IsVisibleLabel) this.HtmlWriter.AddAttribute("showLabels", "false");

            StringBuilder sbProps = new StringBuilder();
            StringBuilder returnContent = new StringBuilder();
            string props = control.BuildControlProps(this.ScreenDefinition, this.IsPreview, this.PermissionData, returnContent);
            if (!string.IsNullOrEmpty(props)) sbProps.AppendFormat("{0},", props);
            if (sbProps.ToString().Length > 0)
            {
                this.HtmlWriter.AddAttribute("data-dojo-props", sbProps.ToString().Substring(0, sbProps.ToString().Length - 1), false);
            }

            base.SetAttributes();
        }
        /// <summary>
        /// 设置Style
        /// </summary>
        /// <returns></returns>
        protected override string SetControlStyle()
        {
            string overflow = "overflow-y:auto;";
            return base.SetControlStyle() + overflow;
        }
        /// <summary>
        /// 设置Class
        /// </summary>
        /// <returns></returns>
        protected override string SetControlClass()
        {
            FormPanel control = (FormPanel)this.ControlHost.Content;
            string[] alignHorizontals = new string[] { "label_align_left", "label_align_center", "label_align_right", "label_align_right" };
            string[] alignVerticals = new string[] { "label_align_top", "label_align_middle", "label_align_bottom", "label_algin_middle" };
            string labelHorizontal = alignHorizontals[(int)control.LabelHorizontalAlignment];
            string labelVerticals = alignVerticals[(int)control.LabelVerticalAlignment];

            return base.SetControlClass() + " " + labelHorizontal + " " + labelVerticals;
        }
        /// <summary>
        /// 设置子元素
        /// </summary>
        protected override void SetChildElements()
        {
            FormPanel control = (FormPanel)this.ControlHost.Content;
            foreach (var child in this.ControlHost.Children)
            {
                var c = child.Content;
                dynamic dc = c;
                string classStr = string.Empty;
                #region IsEnable
                if (c.ExistProperty("IsEnable"))
                {
                    if (!dc.IsEnable) classStr += "disabled ";
                }
                #endregion
                #region ValidatorMember
                bool isValidatorRequired = false;
                string controlName = child.Content.GetType().Name;

                string validatorFullName = string.Empty;
                if (child.Content.Bindings.Count > 0)
                {
                    var bindValue = (from t in child.Content.Bindings where t.Path != null && t.Property != null && t.Property.ToLower() == "value" select t).FirstOrDefault();
                    if (bindValue != null)
                    {
                        validatorFullName = bindValue.Path;
                        bool isBindNavigatorMember = control.bindNavigatorMember(this.ScreenDefinition, validatorFullName);
                        if (isBindNavigatorMember)
                        {
                            var validatorMember = (from t in child.Content.Bindings where t.Path != null && t.Property != null && t.Property.ToLower() == "validatormember" select t).FirstOrDefault();
                            if (validatorMember != null)
                            {
                                validatorFullName = validatorMember.Path;
                            }
                        }
                    }
                }

                List<Model.Core.Definitions.Entities.ValidatorBase> listValidators = new List<Model.Core.Definitions.Entities.ValidatorBase>();
                EDataBaseType baseType = EDataBaseType.String;
                CommonDataType dataTypeContent = null;
                control.GetValidators(this.ScreenDefinition, validatorFullName, ref listValidators, ref baseType, ref dataTypeContent);
                foreach (var validator in listValidators)
                {
                    if (validator.ValidatorType == Model.Core.Definitions.Entities.EValidatorType.Required) isValidatorRequired = true;
                    break;
                }
                if (isValidatorRequired) classStr += "required ";
                #endregion

                this.HtmlWriter.AddAttribute("dojoType", "Controls/ContentPane");
                this.HtmlWriter.AddAttribute("label", child.Title);
                if (!string.IsNullOrEmpty(classStr)) this.HtmlWriter.AddAttribute("class", classStr);

                FormAttach formAttach = (FormAttach)child.AttachObject;
                if (formAttach.ColumnSpan > 1) this.HtmlWriter.AddAttribute("colspan", formAttach.ColumnSpan.ToString().ToLower());
                if (formAttach.CollapsedTitle) this.HtmlWriter.AddAttribute("spanLabel", "true");
                this.HtmlWriter.RenderBeginTag("div");

                var builder = child.GetBuilder(this.IsPreview, this.ScreenDefinition, this.Compile, this.ProjectDocument, this.PermissionData, this.HtmlWriter);
                builder.Parent = this;
                builder.Build();

                this.HtmlWriter.RenderEndTag();
            }
        }
    }
}

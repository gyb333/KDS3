using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using Wilmar.Foundation.Projects;
using Wilmar.Model.Core.Definitions;
using Wilmar.Model.Core.Definitions.Screens;
using Wilmar.Model.Core.Definitions.Screens.Controls;
using Wilmar.Service.Common.Generate;

namespace Wilmar.Build.Core.Dojo.Default.Builders
{
    /// <summary>
    /// 控件生成器
    /// </summary>
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
            var controlName = c.GetType().Name;
            dynamic control = c;

            if (c.ExistProperty("IsReadOnly") && control.IsReadOnly) this.HtmlWriter.AddAttribute("readonly", "readonly");
            if (c.ExistProperty("IsChecked") && control.IsChecked) this.HtmlWriter.AddAttribute("checked", "checked");
            if (c.ExistProperty("IsEnable") && !control.IsEnable) this.HtmlWriter.AddAttribute("disabled", "disabled");

            //获取Style样式
            string classStr = SetControlClass();
            if (!string.IsNullOrEmpty(classStr))
            {
                if (controlName == "FluidLayout") this.HtmlWriter.AddAttribute("customClass", classStr);
                else this.HtmlWriter.AddAttribute("class", classStr);
            }

            //获取Style样式
            string styleStr = SetControlStyle();
            if (!string.IsNullOrEmpty(styleStr)) this.HtmlWriter.AddAttribute("style", styleStr);
        }
        /// <summary>
        /// 获取Class样式
        /// </summary>
        /// <returns></returns>
        protected virtual string SetControlClass()
        {
            //布局样式
            string[] alignHorizontals = new string[] { "align_left", "align_center", "align_right", "align_stretch_horizontal" };
            string[] alignVerticals = new string[] { "align_top", "align_middle", "align_bottom", "align_stretch_vertical" };

            var c = this.ControlHost.Content;
            var controlName = c.GetType().Name;
            dynamic control = c;

            //报表中的属性控件
            if (controlName == "ChartAction" || controlName == "ChartSeries" || controlName == "ChartAxisX" || controlName == "ChartAxisY")
            {
                return string.Empty;
            }

            string classStr = controlName.ToLower();
            if (controlName == "StackPanel" || controlName == "Menu")
            {
                classStr += " " + control.Orientation.ToString().ToLower();
            }

            if (c.ExistProperty("HorizontalAlignment"))
            {
                classStr += " " + alignHorizontals[(int)control.HorizontalAlignment];
            }
            if (c.ExistProperty("VerticalAlignment"))
            {
                classStr += " " + alignVerticals[(int)control.VerticalAlignment];
            }

            return classStr;
        }
        /// <summary>
        /// 获取Style样式
        /// </summary>
        /// <returns></returns>
        protected virtual string SetControlStyle()
        {
            var c = this.ControlHost.Content;
            var controlName = c.GetType().Name;
            dynamic control = c;
            StringBuilder sbAttribute = new StringBuilder();

            //宽度
            #region Width
            if (c.ExistProperty("Width"))
            {
                if (control.Width != null && control.Width > 0)
                {
                    if (controlName == "Button") sbAttribute.AppendFormat("width:{0}px !important;", control.Width + 17);
                    else sbAttribute.AppendFormat("width:{0}px !important;", control.Width.ToString());
                }
                else if (controlName == "ComboBox" || controlName == "CheckedMultiSelect" || controlName == "SearchMultiSelect") sbAttribute.AppendFormat("width:{0}px;", "180");
            }
            #endregion
            //高度
            #region Height
            if (c.ExistProperty("Height"))
            {
                string defaultHeight = string.Empty;
                if (this.GetContainerFixedHeight(controlName))
                {
                    if (controlName == "Menu")
                    {
                        if (control.Orientation == EOrientation.Vertical) { defaultHeight = string.Empty; }
                        else
                        {
                            if (control.Height != null && control.Height > 0)
                            {
                                defaultHeight = control.Height.ToString() + "px";
                            }
                            else
                            {
                                if (this.IsPreview) defaultHeight = "28px";
                                else defaultHeight = "29px";
                            }
                        }
                    }
                    else if (controlName == "ToolBar")
                    {
                        if (control.Height != null && control.Height > 0)
                        {
                            defaultHeight = control.Height.ToString() + "px";
                        }
                        else
                        {
                            if (this.IsPreview) defaultHeight = "28px";
                            else defaultHeight = "29px";
                        }
                    }
                    else if (controlName == "Calendar")
                    {
                        if (control.Height != null && control.Height > 0)
                        {
                            defaultHeight = control.Height.ToString() + "px";
                        }
                        else
                        {
                            if (this.IsPreview) defaultHeight = "210px";
                            else defaultHeight = "213px";
                        }
                    }
                    else if (controlName == "ComboBox" || controlName == "Numeric" || controlName == "TextBox" || controlName == "DatePicker" || controlName == "TimePicker" || controlName == "MonthYearTextBox" || controlName == "DateTimeTextBox")
                    {
                        if (this.IsPreview) defaultHeight = "19px";
                        else defaultHeight = "20px";
                    }
                    else if (controlName == "CheckedMultiSelect" || controlName == "SearchMultiSelect")
                    {
                        if (this.IsPreview) defaultHeight = "20px";
                        else defaultHeight = "20px";
                    }
                    else if (controlName == "SelectPage")
                    {
                        defaultHeight = "";
                    }
                    else if (controlName == "ProgressBar")
                    {
                        if (this.IsPreview) defaultHeight = "15px";
                        else defaultHeight = "17px";
                    }
                    else if (controlName == "SelectBox" || controlName == "FileUploader")
                    {
                        if (this.IsPreview) defaultHeight = "23px";
                        else defaultHeight = "24px";
                    }
                    else if (controlName == "RichTextBox")
                    {
                        if (control.Height != null && control.Height > 0)
                        {
                            defaultHeight = control.Height.ToString() + "px";
                        }
                        else
                        {
                            if (this.IsPreview) defaultHeight = "120px";
                            else defaultHeight = "120px";
                        }
                    }
                    if (!string.IsNullOrEmpty(defaultHeight))
                    {
                        sbAttribute.AppendFormat("height:{0} !important;", defaultHeight);
                    }
                }
                else
                {
                    if (control.Height != null && control.Height > 0)
                    {
                        int _height = control.Height;
                        if (controlName == "Button" || controlName == "ComboButton" || controlName == "DropDownButton" || controlName == "ToggleButton")
                        {
                            _height = _height + 8;
                        }
                        sbAttribute.AppendFormat("height:{0}px !important;", _height);
                    }
                }
            }
            #endregion
            //最小宽度
            #region MinWidth
            if (c.ExistProperty("MinWidth"))
            {
                if (control.MinWidth > 0)
                {
                    if (control.Width == null) sbAttribute.AppendFormat("width:{0}px;", control.MinWidth.ToString());
                    sbAttribute.AppendFormat("min-width:{0}px;", control.MinWidth.ToString());
                }
            }
            #endregion
            //最大宽度
            #region MaxWidth
            if (c.ExistProperty("MaxWidth"))
            {
                if (control.MaxWidth > 0)
                {
                    if (control.Width == null) sbAttribute.AppendFormat("width:{0}px;", control.MaxWidth.ToString());
                    sbAttribute.AppendFormat("max-width:{0}px;", control.MaxWidth.ToString());
                }
            }
            #endregion
            //最小高度
            #region MinHeight
            if (c.ExistProperty("MinHeight"))
            {
                if (control.MinHeight > 0)
                {
                    if (control.Height == null) sbAttribute.AppendFormat("height:{0}px;", control.MinHeight.ToString());
                    sbAttribute.AppendFormat("min-height:{0}px;", control.MinHeight.ToString());
                }
            }
            #endregion
            //最大高度
            #region MaxHeight
            if (c.ExistProperty("MaxHeight"))
            {
                if (control.MaxHeight > 0)
                {
                    if (control.Height == null) sbAttribute.AppendFormat("height:{0}px;", control.MaxHeight.ToString());
                    sbAttribute.AppendFormat("max-height:{0}px;", control.MaxHeight.ToString());
                }
            }
            #endregion
            //外边距设置
            #region Margin
            if (c.ExistProperty("Margin"))
            {
                int marginWidth = control.Margin.Right + control.Margin.Left;
                int marginHeight = control.Margin.Top + control.Margin.Botton;
                if (control.Margin.Top > 0) sbAttribute.AppendFormat("margin-top:{0}px;", control.Margin.Top.ToString());
                if (control.Margin.Right > 0) sbAttribute.AppendFormat("margin-right:{0}px;", control.Margin.Right.ToString());
                if (control.Margin.Botton > 0) sbAttribute.AppendFormat("margin-bottom:{0}px;", control.Margin.Botton.ToString());
                if (control.Margin.Left > 0) sbAttribute.AppendFormat("margin-left:{0}px;", control.Margin.Left.ToString());
                if ((int)control.HorizontalAlignment == 3 && marginWidth > 0)
                {
                    sbAttribute.AppendFormat("width:calc(100% - {0}px) !important;", marginWidth + 2);
                }
            }
            #endregion
            //边框颜色
            #region BorderThickness
            if (c.ExistProperty("BorderThickness"))
            {
                string borderColor = control.BorderColor.R.ToString() + "," + control.BorderColor.G.ToString() + "," + control.BorderColor.B.ToString() + "," + control.BorderColor.A.ToString();
                if (control.BorderThickness.Top > 0) sbAttribute.AppendFormat("border-top:solid {0}px rgba({1});", control.BorderThickness.Top.ToString(), borderColor);
                if (control.BorderThickness.Right > 0) sbAttribute.AppendFormat("border-right:solid {0}px rgba({1});", control.BorderThickness.Right.ToString(), borderColor);
                if (control.BorderThickness.Botton > 0) sbAttribute.AppendFormat("border-bottom:solid {0}px rgba({1});", control.BorderThickness.Botton.ToString(), borderColor);
                if (control.BorderThickness.Left > 0) sbAttribute.AppendFormat("border-left:solid {0}px rgba({1});", control.BorderThickness.Left.ToString(), borderColor);
            }
            #endregion
            //透明度
            #region Opacity
            if (c.ExistProperty("Opacity"))
            {
                if (control.Opacity != 100) sbAttribute.AppendFormat("opacity:{0};", (Convert.ToDouble(control.Opacity) / 100).ToString());
            }
            #endregion
            //水平滚动条
            #region HorizontalScrollBar
            if (c.ExistProperty("HorizontalScrollBar"))
            {
                string[] scrollBars = new string[] { "auto", "disabled", "hidden", "visible" };
                sbAttribute.AppendFormat("overflow-x:{0};", scrollBars[(int)control.HorizontalScrollBar]);
            }
            #endregion
            //垂直滚动条
            #region VerticalScrollBar
            if (c.ExistProperty("VerticalScrollBar"))
            {
                string[] scrollBars = new string[] { "auto", "disabled", "hidden", "visible" };
                sbAttribute.AppendFormat("overflow-y:{0};", scrollBars[(int)control.VerticalScrollBar]);
            }
            #endregion
            //可见性
            #region Visibility
            if (c.ExistProperty("Visibility"))
            {
                string[] visibilitys = new string[] { "collapse", "hidden", "visible" };
                if (visibilitys[(int)control.Visibility] == "collapse")
                {
                    sbAttribute.AppendFormat("display:{0};", "none");
                }
                else if (visibilitys[(int)control.Visibility] != "visible")
                {
                    sbAttribute.AppendFormat("visibility:{0};", visibilitys[(int)control.Visibility]);
                }
            }
            #endregion
            //前景色
            #region ForeColor
            if (c.ExistProperty("ForeColor"))
            {
                string foreColor = control.ForeColor.R.ToString() + "," + control.ForeColor.G.ToString() + "," + control.ForeColor.B.ToString() + "," + control.ForeColor.A.ToString();
                if (control.ForeColor.ToString() != "#FF000000")
                {
                    sbAttribute.AppendFormat("color:rgba({0});", foreColor);
                }
            }
            #endregion
            //背景色
            #region BackColor
            if (c.ExistProperty("BackColor"))
            {
                string backColor = control.BackColor.R.ToString() + "," + control.BackColor.G.ToString() + "," + control.BackColor.B.ToString() + "," + control.BackColor.A.ToString();
                if (control.BackColor.ToString() != "#FFFFFFFF")
                {
                    sbAttribute.AppendFormat("background-color:rgba({0});", backColor);
                }
            }
            #endregion
            //字体
            #region 
            if (controlName != "ChartAxisX" && controlName != "ChartAxisY")
            {
                if (c.ExistProperty("FontFamily"))
                {
                    if (!string.IsNullOrEmpty(control.FontFamily)) sbAttribute.AppendFormat("font-family:{0};", control.FontFamily.ToString().ToLower());
                }
                if (c.ExistProperty("FontSize"))
                {
                    if (control.FontSize > 0) sbAttribute.AppendFormat("font-size:{0};", control.FontSize.ToString());
                }
                if (c.ExistProperty("FontWeight"))
                {
                    if (control.FontWeight.ToString() != "Normal")
                    {
                        sbAttribute.AppendFormat("font-weight:{0};", ((int)control.FontWeight).ToString());
                    }
                }
                if (c.ExistProperty("FontStyle"))
                {
                    string[] fontStyles = new string[] { "normal", "bold", "italic", "underline", "strikeout" };
                    if (fontStyles[(int)control.FontStyle] != "normal")
                    {
                        sbAttribute.AppendFormat("font-style:{0};", fontStyles[(int)control.FontStyle]);
                    }
                }
            }
            #endregion

            return sbAttribute.ToString();
        }

        /// <summary>
        ///是否固定高度容器
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        private bool GetContainerFixedHeight(string controlName)
        {
            Dictionary<string, string> dicts = new Dictionary<string, string>();
            string[] keys = new string[]
            {
                "ComboBox", "DatePicker", "TimePicker", "MonthYearTextBox", "DateTimeTextBox",
                "Numeric", "TextBox", "ProgressBar", "Menu", "ToolBar", "SelectBox",
                "CheckedMultiSelect", "SearchMultiSelect", "RichTextBox","FileUploader",
                 "SelectPage",
            };
            foreach (var c in keys) dicts.Add(c, c);

            if (dicts.ContainsKey(controlName)) return true;
            else return false;
        }
    }
}

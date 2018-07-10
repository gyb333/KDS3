using System.IO;
using System.Linq;
using System.Web.UI;
using Wilmar.Build.Core.Dojo.Default.Builders;
using Wilmar.Model.Core.Definitions;
using Wilmar.Model.Core.Definitions.Screens;
using Wilmar.Model.Core.Definitions.Screens.Members;

namespace Wilmar.Build.Core.Dojo.Default
{
    /// <summary>
    /// 屏幕生成器
    /// </summary>
    public class ScreenBuilder
    {
        /// <summary>
        /// 屏幕HTML生成器
        /// </summary>
        class HtmlBuilder
        {
            public void Build(string project,ScreenDefinition sd, HtmlTextWriter htmlWriter,bool isPreview)
            {
                htmlWriter.AddAttribute("dojoType", "dojox/mvc/Group");
                htmlWriter.AddAttribute("style", "width:100%;height:100%;");
                htmlWriter.AddAttribute("class", "groupDiv");
                htmlWriter.RenderBeginTag("div");

                var builder = sd.Root.GetBuilder(isPreview, sd, null, htmlWriter);
                builder.Build();

                htmlWriter.RenderEndTag();
            }
        }

        /// <summary>
        /// 屏幕JS生成器
        /// </summary>
        class JsBuilder
        {
            public void Build(string project,string screen, ScreenDefinition sd, HtmlTextWriter htmlWriter)
            {
                htmlWriter.WriteLine();
                htmlWriter.WriteLine("<script type=\"text/javascript\">");
                htmlWriter.WriteLine("function " + project + "_" + screen + "_init(screen) {");

                if (sd.Children.Count > 0)
                {
                    this.RegisterToMetaData(sd.Root, sd, htmlWriter);
                    this.RegisterToViewModel(sd.Root, sd, htmlWriter);
                }

                htmlWriter.WriteLine("screen.Initialize(0);");
                htmlWriter.WriteLine("}");
                htmlWriter.WriteLine("</script>");
            }

            private void RegisterToViewModel(ControlHost controlHost, ScreenDefinition screenDef, HtmlTextWriter htmlWriter)
            {
                this.RegGridEvent(controlHost, htmlWriter);

                htmlWriter.WriteLine("screen.RegToViewModel([");

                this.RegisterMethod(controlHost, screenDef, htmlWriter);
                this.RegisterDataSet(controlHost, screenDef, htmlWriter);
                this.RegisterProperty(controlHost, screenDef, htmlWriter);

                htmlWriter.WriteLine("]);");
            }
            private void RegisterToMetaData(ControlHost controlHost, ScreenDefinition screenDef, HtmlTextWriter htmlWriter)
            {
                htmlWriter.WriteLine("screen.RegToMetaData([");
                this.RegisterPropertyMetaData(controlHost, screenDef, htmlWriter);
                htmlWriter.WriteLine("]);");
            }

            /// <summary>
            /// 生成DataGrid方法注册
            /// </summary>
            /// <param name="controlHost">控制基类</param>
            /// <param name="htmlWriter">输出</param>
            private void RegGridEvent(ControlHost controlHost, HtmlTextWriter htmlWriter)
            {
                string typeName = controlHost.Content.GetType().Name;
                if (typeName == "DataGrid")
                {
                    htmlWriter.WriteLine("screen.RegGridEvent('" + controlHost.Name + "_event', " + controlHost.Name + ");");
                }

                foreach (var item in controlHost.Children)
                {
                    RegGridEvent(item, htmlWriter);
                }
            }
            /// <summary>
            /// 生成DataGrid方法
            /// </summary>
            /// <param name="controlHost">控制基类</param>
            /// <param name="htmlWriter">输出</param>
            private void GetDataSetDefaultMethod(ControlHost controlHost, HtmlTextWriter htmlWriter)
            {
                string typeName = controlHost.Content.GetType().Name;
                if (typeName == "DataGrid")
                {
                    htmlWriter.WriteLine("{");
                    htmlWriter.WriteLine("RegType:\"Method\",");
                    htmlWriter.WriteLine("RegValue:{");
                    htmlWriter.WriteLine("" + controlHost.Name + "_OnSelected: function (index, current) {");
                    htmlWriter.WriteLine("if(current === undefined){ current = this; }");
                    htmlWriter.WriteLine("var row = current.getItem(index);");
                    htmlWriter.WriteLine("if(row){ current.store.SetSelectItem(row); }");
                    htmlWriter.WriteLine("}");
                    htmlWriter.WriteLine("}");
                    htmlWriter.WriteLine("},");

                    htmlWriter.WriteLine("{");
                    htmlWriter.WriteLine("RegType:\"Method\",");
                    htmlWriter.WriteLine("RegValue:{");
                    htmlWriter.WriteLine("" + controlHost.Name + "_OnApplyCellEdit: function (inValue, inRowIndex, inFieldIndex) {");
                    htmlWriter.WriteLine("screen.ViewModel." + controlHost.Name + "_OnSelected(inRowIndex, this);");
                    htmlWriter.WriteLine("}");
                    htmlWriter.WriteLine("}");
                    htmlWriter.WriteLine("},");
                }

                foreach (var item in controlHost.Children)
                {
                    GetDataSetDefaultMethod(item, htmlWriter);
                }
            }
            /// <summary>
            /// 生成方法
            /// </summary>
            /// <param name="controlHost">控制基类</param>
            /// <param name="compile">编译器</param>
            /// <param name="screenDef">屏幕定义器</param>
            /// <param name="htmlWriter">输出</param>
            private void RegisterMethod(ControlHost controlHost, ScreenDefinition screenDef, HtmlTextWriter htmlWriter)
            {
                this.GetDataSetDefaultMethod(controlHost, htmlWriter);

                var dataMethod = from t in screenDef.Children
                                 where t.MemberType == EMemberType.Method
                                 select t;
                if (dataMethod != null && dataMethod.ToList().Count > 0)
                {
                    int index = 0;
                    foreach (var item in dataMethod.ToList())
                    {
                        if (index == 0) htmlWriter.WriteLine("{");
                        else htmlWriter.WriteLine(",{");
                        index++;

                        htmlWriter.WriteLine("RegType:\"Method\",");
                        htmlWriter.WriteLine("RegValue:{");
                        htmlWriter.WriteLine("" + item.Name + ":function(e){");
                        htmlWriter.WriteLine("}");
                        htmlWriter.WriteLine("}");

                        if (index == dataMethod.ToList().Count()) htmlWriter.WriteLine("},");
                        else htmlWriter.WriteLine("}");
                    }
                }
            }
            /// <summary>
            /// 生成集合
            /// </summary>
            /// <param name="controlHost">控制基类</param>
            /// <param name="compile">编译器</param>
            /// <param name="screenDef">屏幕定义器</param>
            /// <param name="htmlWriter">输出</param>
            private void RegisterDataSet(ControlHost controlHost,  ScreenDefinition screenDef, HtmlTextWriter htmlWriter)
            {
                //var dataDataSet = from t in screenDef.Children
                //                  where t.MemberType == EMemberType.DataSet
                //                  select t;
                //if (dataDataSet != null && dataDataSet.ToList().Count() > 0)
                //{
                //    int index = 0;
                //    foreach (var item in dataDataSet.ToList())
                //    {
                //        if (index == 0) htmlWriter.WriteLine("{");
                //        else htmlWriter.WriteLine(",{");
                //        index++;

                //        htmlWriter.WriteLine("RegType:\"Collection\",");
                //        htmlWriter.WriteLine("RegValue:{");
                //        DataSet ds = (DataSet)item;
                //        var entityName = (from t in compile.ProjectItems
                //                          where t.Value.DocumentType == 1 && t.Key == ds.EntityId
                //                          select t.Value.Name).FirstOrDefault();
                //        if (entityName != null)
                //        {
                //            htmlWriter.WriteLine("Name:\"" + item.Name + "\", Entity:\"" + entityName + "\"");
                //        }
                //        htmlWriter.WriteLine("}");

                //        if (index == dataDataSet.ToList().Count()) htmlWriter.WriteLine("},");
                //        else htmlWriter.WriteLine("}");
                //    }
                //}
            }
            /// <summary>
            /// 生成属性
            /// </summary>
            /// <param name="controlHost">控制基类</param>
            /// <param name="compile">编译器</param>
            /// <param name="screenDef">屏幕定义器</param>
            /// <param name="htmlWriter">输出</param>
            private void RegisterProperty(ControlHost controlHost, ScreenDefinition screenDef, HtmlTextWriter htmlWriter)
            {
                var dataProperty = from t in screenDef.Children
                                   where t.MemberType == EMemberType.Property
                                   select t;
                if (dataProperty != null && dataProperty.ToList().Count > 0)
                {
                    int index = 0;
                    foreach (var item in dataProperty.ToList())
                    {
                        if (index == 0) htmlWriter.WriteLine("{");
                        else htmlWriter.WriteLine(",{");

                        htmlWriter.WriteLine("RegType:\"Property\",");
                        htmlWriter.WriteLine("RegValue:{");
                        htmlWriter.WriteLine("Name:\"" + item.Name + "\"");
                        htmlWriter.WriteLine("}");

                        htmlWriter.WriteLine("}");
                        index++;
                    }
                }
            }

            /// <summary>
            /// 生成属性数据
            /// </summary>
            /// <param name="controlHost">控制基类</param>
            /// <param name="compile">编译器</param>
            /// <param name="screenDef">屏幕定义器</param>
            /// <param name="htmlWriter">输出</param>
            private void RegisterPropertyMetaData(ControlHost controlHost,  ScreenDefinition screenDef, HtmlTextWriter htmlWriter)
            {

                var dataProperty = from t in screenDef.Children
                                   where t.MemberType == EMemberType.Property
                                   select t;
                if (dataProperty != null && dataProperty.ToList().Count > 0)
                {
                    int index = 0;
                    foreach (var item in dataProperty.ToList())
                    {
                        var property = item as Property;
                        var content = property.Content;
                        dynamic type = content;

                        if (index == 0) htmlWriter.WriteLine("{");
                        else htmlWriter.WriteLine(",{");

                        htmlWriter.WriteLine("RegType:\"Property\",");
                        htmlWriter.WriteLine("RegValue:{");
                        htmlWriter.WriteLine("Name:\"" + item.Name + "\",");
                        if (content.GetType().GetProperty("Title") != null)
                        {
                            htmlWriter.WriteLine("Title:\"" + type.Title + "\",");
                        }
                        if (content.GetType().GetProperty("IsRequired") != null)
                        {
                            htmlWriter.WriteLine("IsRequired:\"" + property.IsRequired.ToString().ToLower() + "\",");
                        }
                        if (content.GetType().GetProperty("IsCollection") != null)
                        {
                            htmlWriter.WriteLine("IsCollection:\"" + property.IsCollection.ToString().ToLower() + "\",");
                        }
                        if (content.GetType().GetProperty("DefaultValue") != null)
                        {
                            htmlWriter.WriteLine("DefaultValue:\"" + type.DefaultValue + "\",");
                        }
                        if (content.GetType().GetProperty("MaxLength") != null)
                        {
                            htmlWriter.WriteLine("MaxLength:\"" + type.MaxLength + "\",");
                        }
                        if (content.GetType().GetProperty("MinLength") != null)
                        {
                            htmlWriter.WriteLine("MinLength:\"" + type.MinLength + "\"");
                        }
                        htmlWriter.WriteLine("}");

                        htmlWriter.WriteLine("}");
                        index++;
                    }
                }
            }
        }



        /// <summary>
        /// 生成逻辑
        /// </summary>
        /// <param name="compile">DOJO编译器</param>
        /// <param name="doc">文档对象模型</param>
        public  string Build(string project,string screen, ScreenDefinition sd,bool isPreview)
        {
            using (var writer = new StringWriter())
            {
                //根据屏幕生成HTML
                var htmlWriter = new HtmlTextWriter(writer);

                HtmlBuilder htmlBuilder = new HtmlBuilder();
                htmlBuilder.Build(project, sd, htmlWriter, isPreview);

                if (!isPreview)
                {
                    JsBuilder jsBuilder = new JsBuilder();
                    jsBuilder.Build(project, screen, sd, htmlWriter);
                }

                return writer.ToString();
            }
        }
    }
}

using System.IO;
using Wilmar.Compile.Core.Dojo;
using Wilmar.Foundation.Common;
using Wilmar.Model.Core.Definitions;
using Wilmar.Model.Core.Definitions.Screens;
using Wilmar.Service.Common.Generate;
using Wilmar.Build.Core.Dojo.Default.Builders;
using Wilmar.Foundation.Projects;
using System.Web.UI;
using System.Collections.Generic;
using Wilmar.Model.Core.Definitions.Screens.Permissions;
using System;

namespace Wilmar.Build.Core.Dojo.Default
{
    /// <summary>
    /// 屏幕生成器
    /// </summary>
    public class ScreenBuild : BuildBase
    {
        #region 
        /// <summary>
        /// 生成类型
        /// </summary>
        public override int BuildTypeId
        {
            get { return GlobalIds.BuildType.DojoScreen; }
        }

        /// <summary>
        /// 唯一标识
        /// </summary>
        public override int Id
        {
            get { return GlobalIds.BuildType.DojoScreen + 1; }
        }

        /// <summary>
        /// 标题
        /// </summary>
        public override string Title
        {
            get { return "Dojo屏幕生成器"; }
        }
        #endregion

        /// <summary>
        /// 生成逻辑
        /// </summary>
        /// <param name="compile">DOJO编译器</param>
        /// <param name="doc">文档对象模型</param>
        public override void Build(CompileBase compile, ProjectDocument doc)
        {
            var docBody = compile.GetDocumentBody(doc);
            var screenDefinition = docBody as ScreenDefinition;
            if (screenDefinition != null)
            {
                ControlHost controlHost = screenDefinition.Root;
                BuildHtml(doc.Name, controlHost, compile, screenDefinition, doc);
            }
        }

        /// <summary>
        /// 生成HTML
        /// </summary>
        /// <param name="name">屏幕名称</param>
        /// <param name="controlHost">控件基类</param>
        /// <param name="compile">编译基类</param>
        /// <param name="screenDef">屏幕定义</param>
        /// <param name="doc">项目文档</param>
        private void BuildHtml(string name, ControlHost controlHost, CompileBase compile, ScreenDefinition screenDef, ProjectDocument doc)
        {
            using (var writer = new StringWriter())
            {
                var dojoCompile = compile as DojoCompile;

                #region 写入权限操作
                //序号，权限ID，权限作用
                Dictionary<int, Tuple<int, string>> itemPermissionData = new Dictionary<int, Tuple<int, string>>();
                var permissionConfigure = screenDef.PermissionConfigure;
                if (permissionConfigure != null && permissionConfigure.EnableAuth)
                {
                    int permissionItemId = 1;
                    string identity = dojoCompile.Project.Identity;
                    var permissionItems = screenDef.PermissionConfigure.PermissionItems;
                    foreach (var item in permissionItems)
                    {
                        short purpose = DojoCompile.PermissionScreen;
                        switch (item.Purpose)
                        {
                            case EPermissionPurpose.Operate:
                                purpose = DojoCompile.PermissionOperate;
                                break;
                            case EPermissionPurpose.Visible:
                                purpose = DojoCompile.PermissionVisible;
                                break;
                            case EPermissionPurpose.Custom:
                                purpose = DojoCompile.PermissionCustom;
                                break;
                        }
                        permissionItemId = int.Parse(item.Id);
                        string title = item.Title;
                        string desc = doc.Title + " " + item.Description;
                        int permissionId = dojoCompile.Permission.Write(doc.Id, purpose, item.Id, title, desc);
                        itemPermissionData.Add(permissionItemId, new Tuple<int, string>(permissionId, item.Purpose.ToString().ToLower()));
                    }
                }
                #endregion

                //根据屏幕生成HTML
                var xmlWriter = new HtmlTextWriter(writer);
                xmlWriter.AddAttribute("dojoType", "dojox/mvc/Group");
                xmlWriter.AddAttribute("style", "width:100%;height:100%;position:relative;");
                xmlWriter.AddAttribute("class", "groupDiv");
                xmlWriter.RenderBeginTag("div");

                var builder = controlHost.GetBuilder(false, screenDef, compile, doc, itemPermissionData, xmlWriter);
                builder.Build();

                xmlWriter.RenderEndTag();

                //生成屏幕JS
                BuildScreenJs.BuildJs(controlHost, compile, screenDef, xmlWriter, doc);
                //生成文件全路径
                var file = Path.Combine(((DojoCompile)compile).OutputPath, name + ".html");
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
                //创建HTML文件
                File.WriteAllText(file, writer.ToString(), System.Text.UTF8Encoding.UTF8);

                ControlExtend.ctlNumber = 1;
            }
        }
    }
}

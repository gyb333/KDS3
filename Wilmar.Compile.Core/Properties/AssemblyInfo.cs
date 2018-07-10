using System.Reflection;
using System.Runtime.InteropServices;
using Wilmar.Foundation;
using Wilmar.Foundation.Attributes;
using Wilmar.Foundation.Common;
using Wilmar.Service.Common.Attributes;
using Service = Wilmar.Compile.Core.Service;
using Dojo = Wilmar.Compile.Core.Dojo;

// 有关程序集的一般信息由以下
// 控制。更改这些特性值可修改
// 与程序集关联的信息。
[assembly: AssemblyTitle("Wilmar.Compile.Core")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Microsoft")]
[assembly: AssemblyProduct("Wilmar.Compile.Core")]
[assembly: AssemblyCopyright("Copyright © Microsoft 2016")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

//将 ComVisible 设置为 false 将使此程序集中的类型
//对 COM 组件不可见。  如果需要从 COM 访问此程序集中的类型，
//请将此类型的 ComVisible 特性设置为 true。
[assembly: ComVisible(false)]

// 如果此项目向 COM 公开，则下列 GUID 用于类型库的 ID
[assembly: Guid("2db83fdb-ad76-47a2-8b8c-d267a9bfa2c0")]

// 程序集的版本信息由下列四个值组成: 
//
//      主版本
//      次版本
//      生成号
//      修订号
//
//可以指定所有这些值，也可以使用“生成号”和“修订号”的默认值，
// 方法是按如下所示使用“*”: :
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
//当前扩展程序集声明
[assembly: AssemblyExtension(EExtensionType.Compile)]
//后台权限定义
[assembly: PermissionPurpose(GlobalIds.Compile.Service, Service.ServiceCompile.PermissionAction, "HTTP访问权限", Description = "用户是否可以访问指定的HTTP资源")]
[assembly: PermissionPurpose(GlobalIds.Compile.Service, Service.ServiceCompile.PermissionModify, "修改属性权限", Description = "用户修改数据时控制指定属性的修改权限")]
[assembly: PermissionPurpose(GlobalIds.Compile.Service, Service.ServiceCompile.PermissionSelect, "查询属性权限", Description = "用户查询数据时控制是否可以查询指定的属性字段")]
//前端权限定义
[assembly: PermissionPurpose(GlobalIds.Compile.DojoPC, Dojo.DojoCompile.PermissionScreen, "屏幕权限", Description = "用户打开指定屏幕的权限", IsCache = false)]
[assembly: PermissionPurpose(GlobalIds.Compile.DojoPC, Dojo.DojoCompile.PermissionVisible, "可见权限", Description = "用户是否可以看见指定控件的权限", IsCache = false)]
[assembly: PermissionPurpose(GlobalIds.Compile.DojoPC, Dojo.DojoCompile.PermissionOperate, "操作权限", Description = "用户是否可以修改指定控件值的权限", IsCache = false)]
[assembly: PermissionPurpose(GlobalIds.Compile.DojoPC, Dojo.DojoCompile.PermissionCustom, "自定义权限", Description = "用户在屏幕内的自定义的权限", IsCache = false)]
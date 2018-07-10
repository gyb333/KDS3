using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wilmar.Model.Core.Definitions.Entities;
using Wilmar.Model.Core.Definitions.Entities.Configure;
using Wilmar.Model.Core.Definitions.Entities.Members;
using Wilmar.Service.Common.Generate;

namespace Wilmar.Compile.Core.Service.Models
{
    public class ControllerMetadata : AnnotationMetadataBase
    {

        public List<EntityMetadata> InheritEntitys { get; private set; } = new List<EntityMetadata>();

        public EntityMetadata Entity { get; private set; }

        public string ClassName
        {
            get { return Entity.ClassName + "sController"; }
        }

        public IPermissionContext Permission { get; private set; }

        public List<ControllerMethodMetadata> Methods { get; private set; } = new List<ControllerMethodMetadata>();

        public Dictionary<int, string> UpdateEntityCommonPropertyPerssions { get; private set; } = new Dictionary<int, string>();

        public Dictionary<string, int> UpdateEntityNavigatePropertyPermissions { get; private set; } = new Dictionary<string, int>();

        public string EntityKeyParameter { get; private set; }

        public string EntityLinqWhere { get; private set; }

        public ControllerMetadata(EntityMetadata data)
        {
            this.Entity = data;
        }

        private void InitializeInheritEntity(ProjectMetadata project, EntityMetadata parent)
        {
            int id = parent.Definition.Configure.InheritEntityId;
            EntityMetadata entity;
            if (id > 0 && project.Entitys.TryGetValue(id, out entity))
            {
                InheritEntitys.Insert(0, entity);
                InitializeInheritEntity(project, entity);
            }
        }

        private void Test()
        {
            //Data


        }

        public override void Initialize(ProjectMetadata project)
        {
            base.Initialize(project);

            InheritEntitys.Insert(0, Entity);
            InitializeInheritEntity(project, Entity);

            var controller = Entity.Definition.Controller;

            if (controller.AuthorizeMode == EAuthorizeMode.Authorize)
                Permission = project.Compiler.Permission;
            if (controller.AuthorizeMode != EAuthorizeMode.Anonymous)
                Write("Wilmar.Service.Common.Attributes.Authorize");
            if (Entity.PrimaryPropertys.Count > 0)
            {
                project.Service.Entitys.Add(Entity);

                if (Entity.PrimaryPropertys.Count == 1)
                {
                    var member = Entity.PrimaryPropertys.Values.First();
                    EntityKeyParameter = $"[FromODataUri] {member.PropertyType.TypeName} key";
                    EntityLinqWhere = $"item => item.{member.Name} == key";
                }
                else
                {
                    EntityKeyParameter = string.Join(" ,", Entity.PrimaryPropertys.Values.Select(a => $"[FromODataUri] {a.PropertyType.TypeName} key{a.Name}").ToArray());
                    EntityLinqWhere = $"item => " + string.Join(" && ", Entity.PrimaryPropertys.Values.Select(a => $"item.{a.Name} == key{a.Name}").ToArray());
                }

                if (controller.CanGet)
                    GenerateGetAction(controller);
                if (!this.Entity.IsDimFast)
                {
                    if (controller.CanPost)
                        GeneratePostAction(controller);
                    if (controller.CanPut)
                        GeneratePutAction(controller);
                    if (controller.CanDelete)
                        GenerateDeleteAction(controller);

                    GenerateCustomAction(controller);
                }
            }
        }


        private void GenerateGetAction(ControllerConfigure controller)
        {
            int permissionid = PermissionAction("RETRIEVE", "查询权限", "查询数据[" + Entity.ProjectItem.Title + "]的权限");

            var getcol = new ControllerMethodMetadata(EControllerMethodType.GetQueryable);
            if (controller.AuthGet)
                getcol.PermissionAction(permissionid);

            var getobj = new ControllerMethodMetadata(EControllerMethodType.GetSingleResult);
            if (controller.AuthGet)
                getobj.PermissionAction(permissionid);

            string idstr = string.Empty, namestr = string.Empty;
            Dictionary<string, int> permissionNavis = PermissionPropertys(controller, ref idstr, ref namestr);
            if (!string.IsNullOrEmpty(idstr) && controller.AuthGet)
            {
                if (controller.DisablePage)
                    getcol.Write("Wilmar.Service.Common.Attributes.ODataQuery", idstr, namestr, $"MaxNodeCount={int.MaxValue}", "MaxExpansionDepth = 4");
                else
                    getcol.Write("Wilmar.Service.Common.Attributes.ODataQuery", idstr, namestr, $"MaxNodeCount={int.MaxValue}", "MaxExpansionDepth = 4", $"PageSize={controller.PageSize}");
                getobj.Write("Wilmar.Service.Common.Attributes.ODataQuery", idstr, namestr);
            }
            else
            {
                if (controller.DisablePage)
                    getcol.Write("Wilmar.Service.Common.Attributes.CustomEnableQuery", $"MaxNodeCount={int.MaxValue}", "MaxExpansionDepth = 4");
                else
                    getcol.Write("Wilmar.Service.Common.Attributes.CustomEnableQuery", $"MaxNodeCount={int.MaxValue}", "MaxExpansionDepth = 4", $"PageSize={controller.PageSize}");
                getobj.Write("Wilmar.Service.Common.Attributes.CustomEnableQuery");
            }
            Methods.Add(getcol);
            Methods.Add(getobj);

            GenerateGetNavigationAction(permissionid, permissionNavis);
        }

        private void GenerateGetNavigationAction(int permissionid, Dictionary<string, int> permissionNavis)
        {
            foreach (var item in Entity.NavigationPropertys.Values)
            {
                ControllerMethodMetadata method;
                var controller = Entity.Definition.Controller;
                if (item.PropertyType.IsCollection)
                {
                    method = new ControllerMethodMetadata(EControllerMethodType.GetNavigateQueryable).PermissionAction(permissionid);
                    if (controller.DisablePage)
                        method.Write("System.Web.OData.EnableQuery");
                    else
                        method.Write("System.Web.OData.EnableQuery", $"PageSize={controller.PageSize}");
                }
                else
                {
                    method = new ControllerMethodMetadata(EControllerMethodType.GetNavigateSingleResult).PermissionAction(permissionid);
                    method.Write("System.Web.OData.EnableQuery");
                }
                method.Navigation = item;
                if (permissionNavis.ContainsKey(item.Name))
                    method.PermissionAction(permissionNavis[item.Name]);
                Methods.Add(method);
            }
        }

        private void GeneratePostAction(ControllerConfigure controller)
        {
            int permissionid = PermissionAction("CREATE", "创建权限", "创建数据[" + Entity.ProjectItem.Title + "]的权限");
            var post = new ControllerMethodMetadata(EControllerMethodType.Post);
            if (controller.AuthPost)
                post.PermissionAction(permissionid);
            Methods.Add(post);

            var create = new ControllerMethodMetadata(EControllerMethodType.OnCreateEntity);

            int count = InheritEntitys.SelectMany(a => a.Definition.Members).OfType<PrimaryMember>().Where(a => a.GenerateMode == EPrimaryGenerateMode.Code).Count();
            count += InheritEntitys.SelectMany(a => a.Definition.Members).OfType<ColumnMember>().Where(a => a.GenerateMode == EColumnGenerateMode.CodeCreate || a.GenerateMode == EColumnGenerateMode.CodeCreateUpdate).Count();

            if (InheritEntitys.Select(a => a.Definition.Controller).Where(a => !string.IsNullOrEmpty(a.PostInterceptor)).Count() > 0 || count > 0)
                Methods.Add(create);
        }

        private void GenerateDeleteAction(ControllerConfigure controller)
        {
            int permissionid = PermissionAction("DELETE", "删除权限", "删除数据[" + Entity.ProjectItem.Title + "]的权限");

            var delete = new ControllerMethodMetadata(EControllerMethodType.Delete);
            if (controller.AuthDelete)
                delete.PermissionAction(permissionid);
            Methods.Add(delete);

            if (InheritEntitys.Select(a => a.Definition.Controller).Where(a => !string.IsNullOrEmpty(a.DeleteInterceptor)).Count() > 0)
                Methods.Add(new ControllerMethodMetadata(EControllerMethodType.OnDeleteEntity));
        }

        private void GeneratePutAction(ControllerConfigure controller)
        {
            int permissionid = PermissionAction("UPDATE", "修改权限", "修改数据[" + Entity.ProjectItem.Title + "]的权限");

            var put = new ControllerMethodMetadata(EControllerMethodType.Put);
            if (controller.AuthPut)
                put.PermissionAction(permissionid);
            Methods.Add(put);

            var patch = new ControllerMethodMetadata(EControllerMethodType.Patch);
            if (controller.AuthPut)
                patch.PermissionAction(permissionid);
            Methods.Add(patch);


            var update = new ControllerMethodMetadata(EControllerMethodType.OnUpdateEntity);
            int count = InheritEntitys.SelectMany(a => a.Definition.Members).OfType<ColumnMember>().Where(a => a.GenerateMode == EColumnGenerateMode.CodeCreate).Count();
            count += InheritEntitys.SelectMany(a => a.Definition.Members).OfType<ColumnMember>().Where(a => a.GenerateMode == EColumnGenerateMode.CodeUpdate || a.GenerateMode == EColumnGenerateMode.CodeCreateUpdate).Count();

            if (InheritEntitys.Select(a => a.Definition.Controller).Where(a => !string.IsNullOrEmpty(a.PutInterceptor)).Count() > 0 || count > 0)
                Methods.Add(update);

            if (Permission != null && controller.AuthPut)
            {
                foreach (var a in Entity.ColumnPropertys.Values.Where(a => a.Member.EnableAuth && a.Member.GenerateMode == EColumnGenerateMode.None))
                    UpdateEntityCommonPropertyPerssions.Add(Permission.Write(Entity.ProjectItem.Id, ServiceCompile.PermissionModify, a.Name, a.Member.Title + "修改", $"修改属性[{Entity.ProjectItem.Title}]的权限"), a.Name);
                foreach (var a in Entity.NavigationPropertys.Values.Where(a => a.Member.EnableAuth))
                    UpdateEntityNavigatePropertyPermissions.Add(a.Name, Permission.Write(Entity.ProjectItem.Id, ServiceCompile.PermissionModify, a.Name, a.Member.Title + "修改", $"修改属性[{Entity.ProjectItem.Title}]的权限"));
            }

            if (InheritEntitys.SelectMany(a => a.NavigationPropertys).Count() > 0)
            {
                Methods.Add(new ControllerMethodMetadata(EControllerMethodType.CreateRef));
                Methods.Add(new ControllerMethodMetadata(EControllerMethodType.CreateRefOverride));
                Methods.Add(new ControllerMethodMetadata(EControllerMethodType.DeleteRef1));
                Methods.Add(new ControllerMethodMetadata(EControllerMethodType.DeleteRef2));
                Methods.Add(new ControllerMethodMetadata(EControllerMethodType.DeleteRefOverride));
            }
        }

        private void GenerateCustomAction(ControllerConfigure controller)
        {
            foreach (var action in controller.Actions)
            {
                var actionData = new ControllerMethodMetadata(EControllerMethodType.CustomAction)
                {
                    Action = new ControllerActionMetadata(action, Project, Entity)
                };
                int permissionid = PermissionAction(action.Name, action.Title + "执行", $"调用操作[{Entity.ProjectItem.Title}]->[{action.Title}]的权限");
                if (action.EnableAuth)
                    actionData.PermissionAction(permissionid);
                Methods.Add(actionData);

                Project.Service.Actions.Add(actionData.Action);
            }
        }

        public Dictionary<string, int> PermissionPropertys(ControllerConfigure controller, ref string idstr, ref string namestr)
        {
            Dictionary<string, int> permissionNavis = new Dictionary<string, int>();
            if (Permission != null && controller.AuthGet)
            {
                List<string> ids = new List<string>();
                List<string> names = new List<string>();
                foreach (var item in Entity.Definition.Members)
                {
                    if (item.MemberType != EMemberType.Primary)
                    {
                        if (item.MemberType == EMemberType.Column && !((ColumnMember)item).EnableAuth)
                            continue;
                        if (item.MemberType == EMemberType.Navigation && !((NavigationMember)item).EnableAuth)
                            continue;
                        int id = Permission.Write(Entity.ProjectItem.Id, ServiceCompile.PermissionSelect, item.Name, item.Title + "查询", $"查询[{Entity.ProjectItem.Title}]->[{item.Title}]的权限");
                        ids.Add(id.ToString());
                        names.Add(item.Name);
                        if (item.MemberType == EMemberType.Navigation)
                            permissionNavis.Add(item.Name, id);
                    }
                }
                if (ids.Count > 0)
                {
                    idstr = $"\"{string.Join(",", ids.ToArray())}\"";
                    namestr = $"\"{string.Join(",", names.ToArray())}\"";
                }
            }
            return permissionNavis;
        }

        public int PermissionAction(string identity, string title, string desc)
        {
            if (this.Permission != null)
                return Permission.Write(Entity.ProjectItem.Id, ServiceCompile.PermissionAction, identity, title, desc);
            return 0;
        }
    }

    public class ControllerMethodMetadata : AnnotationMetadataBase
    {
        public ControllerMethodMetadata(EControllerMethodType methodtype)
        {
            MethodType = methodtype;
        }

        public ControllerMethodMetadata PermissionAction(int id)
        {
            if (id > 0)
                this.Write($"Wilmar.Service.Common.Attributes.Authorize", id.ToString());
            return this;
        }

        public EControllerMethodType MethodType { get; private set; }

        public PropertyNavigationData Navigation { get; set; }

        public ControllerActionMetadata Action { get; set; }
    }

    public class ControllerActionMetadata
    {
        public ControllerActionMetadata(ActionConfigure action, ProjectMetadata project, EntityMetadata entity)
        {
            this.Entity = entity;
            Source = action;
            foreach (var para in action.Parameters)
            {
                Parameters.Add(para.Name, new DataTypeMetadata(para.DataType, project, true));
            }
            if (action.ReturnType != null)
            {
                this.ReturnType = new DataTypeMetadata(action.ReturnType, project, true);
            }
        }

        public string Name
        {
            get { return Source.Name; }
        }

        public EntityMetadata Entity { get; private set; }

        public ActionConfigure Source { get; private set; }

        public Dictionary<string, DataTypeMetadata> Parameters { get; private set; } = new Dictionary<string, DataTypeMetadata>();

        public DataTypeMetadata ReturnType { get; private set; }
    }

    public enum EControllerMethodType
    {
        GetSingleResult,
        GetQueryable,
        GetNavigateSingleResult,
        GetNavigateQueryable,
        OnCreateEntity,
        OnUpdateEntity,
        OnDeleteEntity,
        Post,
        Delete,
        Put,
        Patch,
        CreateRef,
        DeleteRef1,
        DeleteRef2,
        CreateRefOverride,
        DeleteRefOverride,
        CustomAction,
    }
}

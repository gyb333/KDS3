using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wilmar.Compile.Core.Service;
using Wilmar.Compile.Core.Service.Models;
using Wilmar.Foundation.Common;
using Wilmar.Foundation.Projects;
using Wilmar.Model.Core.Definitions;
using Wilmar.Model.Core.Definitions.Entities;
using Wilmar.Model.Core.Definitions.Entities.Configure;
using Wilmar.Model.Core.Definitions.Entities.DataTypes;
using Wilmar.Service.Common.Generate;

namespace Wilmar.Build.Core.Service.Default.ServiceTemplate
{
    public partial class ProjectServiceTemplate
    {
        /// <summary>
        /// 编译器对象
        /// </summary>
        public ServiceCompile Compiler { get; private set; }
        /// <summary>
        /// 当前实体集合
        /// </summary>
        public Dictionary<ProjectDocument, ControllerConfigure> Entitys { get; private set; }

        public ServiceMetadata Data { get; private set; }

        private string GenerateParameterPrefix(DataTypeMetadata data)
        {
            if (data.IsEntityType)
            {
                if (data.IsCollection)
                    return "CollectionEntity";
                else
                    return "Entity";
            }
            if (data.IsCollection)
                return "Collection";
            return "";
        }

        private string GenerateReturnsPrefix(DataTypeMetadata data)
        {
            if (data.IsEntityType && data.EntityType.DataContext != null)
            {
                if (data.IsCollection)
                    return "CollectionFromEntitySet";
                else
                    return "FromEntitySet";
            }
            if (data.IsCollection)
                return "Collection";
            return "";
        }

        private string GenerateElementType(DataTypeMetadata data)
        {
            if (!data.IsEntityType && data.IsCollection)
            {
                var type = data.ClrType;
                if (type.IsArray)
                    return type.GetElementType().FullName;
                if (type.IsGenericType)
                {
                    var types = type.GetGenericArguments();
                    if (types.Length == 0)
                        return types[0].FullName;
                }
            }
            return data.TypeName;
        }

        public string GenerateReturnsEntityName(DataTypeMetadata data)
        {
            if (data.IsEntityType && data.EntityType.DataContext != null)
                return "\"" + data.TypeName + "s\"";
            return "";
        }

        public ProjectServiceTemplate(CompileBase compile, ServiceMetadata data)
        {
            this.Data = data;

            this.Compiler = compile as ServiceCompile;
            this.Entitys = new Dictionary<ProjectDocument, ControllerConfigure>();
            foreach (var entity in Compiler.ProjectItems.Values.Where(a => a.DocumentType == GlobalIds.DocumentType.Entity))
            {
                var def = Compiler.GetDocumentBody(entity) as EntityDefinition;
                if (def.Controller != null)
                {
                    Entitys.Add(entity, def.Controller);
                }
            }
        }
    }
}

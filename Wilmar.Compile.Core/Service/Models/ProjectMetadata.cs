using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wilmar.Foundation.Common;
using Wilmar.Foundation.Projects;
using Wilmar.Foundation.Projects.Configures;
using Wilmar.Model.Core.Definitions;

namespace Wilmar.Compile.Core.Service.Models
{
    public class ProjectMetadata
    {
        public ProjectMetadata(ServiceCompile com)
        {
            this.Compiler = com;
            this.Intialize(com);
            this.Identity = com.Identity;
        }

        public string Identity { get; private set; }

        public ServiceCompile Compiler { get; private set; }

        public ServiceMetadata Service { get; private set; }

        public Dictionary<int, EntityMetadata> Entitys { get; private set; }

        public Dictionary<int, ControllerMetadata> Controllers { get; private set; }

        public Dictionary<int, DataContextMetadata> DataContexts { get; private set; }

        private void Intialize(ServiceCompile compiler)
        {
            DataContexts = new Dictionary<int, DataContextMetadata>();
            Controllers = new Dictionary<int, ControllerMetadata>();
            Entitys = new Dictionary<int, EntityMetadata>();
            Service = new ServiceMetadata();
            DatabaseConfigure temp = compiler.Project.Configures.OfType<DatabaseConfigure>().FirstOrDefault();
            if (temp != null)
            {
                foreach (var each in temp.Children)
                    DataContexts.Add(each.Id, new DataContextMetadata(each));
            }

            Service.Initialize(this);

            foreach (var item in compiler.ProjectItems.Values.Where(a => a.DocumentType == GlobalIds.DocumentType.Entity))
                ProecessEntity(item);

            foreach (var entity in Entitys.Values)
                entity.Initialize(this);

            foreach (var context in DataContexts.Values)
                context.Initialize(this);

            foreach (var control in Controllers.Values)
                control.Initialize(this);
        }

        private void ProecessEntity(ProjectDocument item)
        {
            EntityDefinition entity = Compiler.GetDocumentBody(item) as EntityDefinition;
            object sourceid;
            if (item.Propertys.TryGetValue("DataSourceId", out sourceid))
            {
                if (sourceid is int)
                {
                    DataContextMetadata context;
                    if (DataContexts.TryGetValue((int)sourceid, out context))
                    {
                        var data = new EntityMetadata(item, context, entity);
                        this.Entitys.Add(item.Id, data);
                        context.Entitys.Add(item.Id, data);
                        if (entity.Controller != null)
                        {
                            Controllers.Add(item.Id, new ControllerMetadata(data));
                        }
                    }
                }
            }
            else
            {
                this.Entitys.Add(item.Id, new EntityMetadata(item, null, entity));
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wilmar.Compile.Core.Service;
using Wilmar.Foundation.Projects;
using Wilmar.Foundation;
using Wilmar.Model.Core.Definitions;
using Wilmar.Model.Core.Definitions.Entities;
using Wilmar.Model.Core.Definitions.Entities.Configure;
using Wilmar.Model.Core.Definitions.Entities.DataTypes;
using Wilmar.Model.Core.Definitions.Entities.Members;
using Wilmar.Model.Core.Definitions.Entities.Validators;
using Wilmar.Service.Common.Generate;
using Wilmar.Compile.Core.Service.Models;

namespace Wilmar.Build.Core.Service.Default.ServiceTemplate
{
    public partial class EntityTemplate
    {
        /// <summary>
        /// 数据对象
        /// </summary>
        public EntityMetadata Data { get; private set; }

        public bool IsFast { get; }
        public EntityTemplate(EntityMetadata data)
        {
            Data = data;
            IsFast = data.Definition.CubeType == Model.Definitions.Entities.EEntityCubeType.Fact;
        }

        private string WriteDefaultValue(string value)
        {
            if (!string.IsNullOrEmpty(value))
                return "= " + value;
            return "";
        }
    }




}

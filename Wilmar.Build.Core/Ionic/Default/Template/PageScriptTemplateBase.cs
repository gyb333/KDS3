using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wilmar.Foundation.Projects;
using Wilmar.Model.Core.Definitions;
using System.IO;
using System.Text.RegularExpressions;
using Wilmar.Compile.Core.Ionic;
using System.Collections;
using Wilmar.Model.Core.Definitions.Screens.Members;
using Wilmar.Model.Core.Definitions.Screens;
using Wilmar.Model.Core.Definitions.Entities.DataTypes;
using Wilmar.Build.Core.Dojo.Default.Builders;

namespace Wilmar.Build.Core.Ionic.Default.Template
{
    /// <summary>
    /// 控制器模版
    /// </summary>
    public partial class PageScriptTemplate
    {
        /// <summary>
        /// 编译对象
        /// </summary>
        public IonicCompile Compile
        {
            get; private set;
        }
        /// <summary>
        /// 屏幕对象
        /// </summary>
        public ScreenDefinition ScreenDef
        {
            get; private set;
        }
        /// <summary>
        /// 文档对象
        /// </summary>
        public ProjectDocument Documnet
        {
            get; private set;
        }

        public Dictionary<string, MetaDataProperty> PropertyData = new Dictionary<string, MetaDataProperty>();
        public Dictionary<string, MetaDataMethod> MethodData = new Dictionary<string, MetaDataMethod>();
        public Dictionary<string, MetaDataMethod> EventData = new Dictionary<string, MetaDataMethod>();
        public Dictionary<string, MetaDataScreenParam> ScreenParamData = new Dictionary<string, MetaDataScreenParam>();
        public Dictionary<string, MetaDataDataSet> DataSetData = new Dictionary<string, MetaDataDataSet>();
        public Dictionary<string, string> ImportEntity = new Dictionary<string, string>();

        public PageScriptTemplate(IonicCompile ionicCompile, ScreenDefinition screenDef, ProjectDocument doc)
        {
            this.Compile = ionicCompile;
            this.ScreenDef = screenDef;
            this.Documnet = doc;
            Initial();
        }
        /// <summary>
        /// 初始化数据
        /// </summary>
        private void Initial()
        {
            PropertyData = BuildComm.RegisterPropertys(this.Compile, this.ScreenDef, this.Documnet);
            MethodData = BuildComm.RegisterMethods(this.Compile, this.ScreenDef, this.Documnet);
            EventData = BuildComm.RegisterEvents(this.Compile, this.ScreenDef, this.Documnet);
            ScreenParamData = BuildComm.RegisterScreenParams(this.Compile, this.ScreenDef, this.Documnet);
            DataSetData = BuildComm.RegisterDataSets(this.Compile, this.ScreenDef, this.Documnet);
            foreach (var dataSet in DataSetData.Values)
            {
                if (!string.IsNullOrEmpty(dataSet.EntityName) && !ImportEntity.ContainsKey(dataSet.EntityName))
                {
                    ImportEntity.Add(dataSet.EntityName, dataSet.EntityName);
                }
            }
        }
    }
}

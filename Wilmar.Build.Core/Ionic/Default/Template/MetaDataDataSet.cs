using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wilmar.Model.Core.Definitions.Screens;

namespace Wilmar.Build.Core.Ionic.Default.Template
{
    public class MetaDataDataSet : MemberBase
    {
        public override EMemberType MemberType
        {
            get { return EMemberType.DataSet; }
        }

        public string Type
        {
            get; set;
        }

        public string EntityName
        {
            get; set;
        }

        public string Options
        {
            get; set;
        }

        private List<MetaDataProperty> _DataPropertys = new List<MetaDataProperty>();
        public List<MetaDataProperty> DataPropertys
        {
            get { return this._DataPropertys; }
            set { this._DataPropertys = value; }
        }

        private List<MetaDataMethod> _DataEvents = new List<MetaDataMethod>();
        public List<MetaDataMethod> DataEvents
        {
            get { return this._DataEvents; }
            set { this._DataEvents = value; }
        }

        private List<MetaDataMethod> _DataMethods = new List<MetaDataMethod>();
        public List<MetaDataMethod> DataMethods
        {
            get { return this._DataMethods; }
            set { this._DataMethods = value; }
        }

        private List<MetaDataProperty> _SelectedItemMembers = new List<MetaDataProperty>();
        public List<MetaDataProperty> SelectedItemMembers
        {
            get { return this._SelectedItemMembers; }
            set { this._SelectedItemMembers = value; }
        }

        private List<MetaDataMethod> _SelectedItemMethods = new List<MetaDataMethod>();
        public List<MetaDataMethod> SelectedItemMethods
        {
            get { return this._SelectedItemMethods; }
            set { this._SelectedItemMethods = value; }
        }
    }
}

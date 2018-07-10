using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wilmar.Model.Core.Definitions.Screens;

namespace Wilmar.Build.Core.Ionic.Default.Template
{
    public class MetaDataProperty : MemberBase
    {
        public override EMemberType MemberType
        {
            get { return EMemberType.Property; }
        }

        public string DataType
        {
            get; set;
        }
        public bool IsRequired
        {
            get; set;
        }
        public bool IsCollection
        {
            get; set;
        }
        public string DefaultValue
        {
            get; set;
        }
        public string MaxValue
        {
            get; set;
        }
        public string MinValue
        {
            get; set;
        }
        public string MaxLength
        {
            get; set;
        }
        public string MinLength
        {
            get; set;
        }
    }
}

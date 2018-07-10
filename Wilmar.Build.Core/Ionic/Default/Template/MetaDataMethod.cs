using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wilmar.Model.Core.Definitions.Screens;

namespace Wilmar.Build.Core.Ionic.Default.Template
{
    public class MetaDataMethod : MemberBase
    {
        public override EMemberType MemberType
        {
            get { return EMemberType.Method; }
        }

        public string Body
        {
            get; set;
        }

        public string Params
        {
            get; set;
        }

        public string ParamsBind
        {
            get; set;
        }
    }
}

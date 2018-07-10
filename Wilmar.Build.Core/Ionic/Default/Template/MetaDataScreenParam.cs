using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wilmar.Model.Core.Definitions.Screens;

namespace Wilmar.Build.Core.Ionic.Default.Template
{
    public class MetaDataScreenParam : MemberBase
    {
        public override EMemberType MemberType
        {
            get { return EMemberType.ScreenParameters; }
        }

        public string Content
        {
            get; set;
        }
    }
}

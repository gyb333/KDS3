using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wilmar.Compile.Core.Service.Models
{
    public abstract class AnnotationMetadataBase : MetadataBase
    {
        public string[] Annotations
        {
            get { return _Annotations.ToArray(); }
        }
        private List<string> _Annotations = new List<string>();

        public void Write(string name, params string[] para)
        {
            if (!Annotations.Any(a => a.StartsWith(name)))
            {
                if (para.Length == 0)
                    _Annotations.Add(name);
                else
                {
                    StringBuilder builder = new StringBuilder();
                    builder.Append(name + "(");
                    bool isfirst = true;
                    foreach (var item in para)
                    {
                        if (isfirst)
                            isfirst = false;
                        else
                            builder.Append(",");
                        builder.Append(item);
                    }
                    builder.Append(')');
                    _Annotations.Add(builder.ToString());
                }
            }
        }

        public void Clear()
        {
            _Annotations.Clear();
        }

        public string GenerateCode()
        {
            if (_Annotations.Count > 0)
            {
                return "[" + string.Join(",", _Annotations.ToArray()) + "]";
            }
            return "";
        }
    }
}

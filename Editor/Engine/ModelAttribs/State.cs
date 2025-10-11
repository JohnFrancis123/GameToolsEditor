using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Engine.ModelAttribs
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class State
    {
        public bool Selected { get; set; }

        public override string ToString()
        {
            return "";
        }
    }
}

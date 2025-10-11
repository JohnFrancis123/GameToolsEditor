using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections; // 🛑 NEW: Required for StandardValuesCollection

namespace Editor.Engine.ModelAttribs
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Appearance
    {
        //applying the nested TypeConverter to enable the dropdown behavior
        [TypeConverter(typeof(TextureNameConverter))]
        public string DiffuseTexture { get; set; } = "Grass"; // Initialize with a default

        public override string ToString()
        {
            return "";
        }

        //adding the nested class to supply the list of texture names
        public class TextureNameConverter : StringConverter
        {
            //telling the PropertyGrid that this property supports a standard list of values.
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true;
            }

            //telling the PropertyGrid to use ONLY the values provided (a dropdown, not an editable combo-box).
            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
            {
                return true;
            }

            //providing the hardcoded list of standard values.
            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                return new StandardValuesCollection(new string[] {
                    "Grass",
                    "HeightMap",
                    "Metal"
                });
            }
        }
    }
}
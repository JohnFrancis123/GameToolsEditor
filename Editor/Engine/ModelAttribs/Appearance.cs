using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Numerics;

namespace Editor.Engine.ModelAttribs
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Appearance : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string _propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(_propertyName));
        }

        //applying the nested TypeConverter to enable the dropdown behavior
        [TypeConverter(typeof(TextureNameConverter))]
        public string DiffuseTexture { get => m_diffuseTexture; 
            set 
            {
                if (m_diffuseTexture != value) {
                    m_diffuseTexture = value;
                    OnPropertyChanged("DiffuseTexture");
                }
            }
        }

        private string m_diffuseTexture;


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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Editor.Engine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.ComponentModel;

namespace Editor.Engine.ModelAttribs
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Transformation : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string _propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(_propertyName));
        }

        private Vector3 m_position = Vector3.Zero;
        private Vector3 m_rotation = Vector3.Zero;
        private float m_scale = 1.0f;
        
        public Vector3 Position { get => m_position; set 
            {
                if (m_position != value) 
                {
                    m_position = value;
                    OnPropertyChanged("Position");
                }
            }
        }

        public Vector3 Rotation { get => m_rotation; set 
            {
                if (m_rotation != value) 
                {
                    m_rotation = value;
                    OnPropertyChanged("Rotation");
                }
            } 
        }

        public float Scale { get => m_scale; set
            {
                if (m_scale != value)
                {
                    m_scale = value;
                    OnPropertyChanged("Scale");
                }
            }
        }
        public override string ToString()
        {
            return "";
        }
    }
}

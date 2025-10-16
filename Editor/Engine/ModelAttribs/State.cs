using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Engine.ModelAttribs
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class State : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string _propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(_propertyName));
        }

        private bool m_selected;
        public bool Selected { get => m_selected;
            set
            {
                if (m_selected != value) 
                {
                    m_selected = value;
                    OnPropertyChanged("Selected");
                }
            }
        }

        public override string ToString()
        {
            return "";
        }
    }
}

using System;
using System.ComponentModel;
using System.Reflection;

namespace RepzScreenshot.Model
{
    abstract class ModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        protected void NotifyPropertyChanged(string propertyName)
        {

            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected bool UpdateProperty<T>(string propertyName, T val)
        {
            PropertyInfo property = this.GetType().GetProperty(propertyName);
            
            if(!((T)property.GetValue(this)).Equals(val))
            {
                property.SetValue(this, val);
                return true;
            }
            else
            {
                return false;
            }
            
        }

        public abstract bool Update(ModelBase m);
        
    }
}

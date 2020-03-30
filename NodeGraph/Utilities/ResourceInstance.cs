using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace NodeGraph.Utilities
{
    public class ResourceInstance<T> where T : DispatcherObject
    {
        public T Get(string resourceName)
        {
            if(_Resource == null)
            {
                _Resource = Application.Current.TryFindResource(resourceName) as T;
            }

            return _Resource;
        }

        T _Resource = null;
    }
}

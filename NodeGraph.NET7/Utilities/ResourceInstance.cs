using System.Windows;
using System.Windows.Threading;

namespace NodeGraph.NET7.Utilities
{
    public class ResourceInstance<T> where T : DispatcherObject
    {
        public T Get(string resourceName)
        {
            if (_Resource == null)
            {
                _Resource = (Application.Current.TryFindResource(resourceName) as T)!;
            }

            return _Resource!;
        }

        T _Resource = null!;
    }
}

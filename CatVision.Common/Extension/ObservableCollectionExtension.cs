using System;
using System.Collections.ObjectModel;

namespace CatVision.Common.Extension
{
    public static class ObservableCollectionExtension
    {
        public static int FindIndex<T>(this ObservableCollection<T> collection, Predicate<T> match)
        {
            int _size = collection.Count;
            if (_size <= 0)
            {
                return -1;
            }
            int num = _size;
            for (int i = 0; i < num; i++)
            {
                if (match(collection[i]))
                {
                    return i;
                }
            }
            return -1;

        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CollectionsExtensions
{
    public static class ListExtentions
    {
        public static bool TryGetElement<T>(this List<T> list, int index, out T element)
        {
            if (index >= 0 && index < list.Count)
            {
                element = list[index];
                return true;
            }
            element = default;
            return false;
        }
    } 
}

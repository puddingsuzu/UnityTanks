using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tanks
{
    public static class ExtensionMethods
    {
        public static T FindAnyChild<T>(this Transform trans, string name) where T : Component
        {
            for (var n = 0; n < trans.childCount; n++)
            {
                var child = trans.GetChild(n);
                if (child.name == name)
                {
                    return child.GetComponent<T>();
                }
                if (child.childCount > 0)
                {
                    var sub_child = child.FindAnyChild<Transform>(name);
                    if (sub_child != null)
                        return sub_child.GetComponent<T>();
                }
            }
            return default;
        }
    }
}
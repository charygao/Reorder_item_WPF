using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GongSolutions.Wpf.DragDrop.Utilities
{
    public static class TypeUtilities
    {
        #region  Methods

        public static IEnumerable CreateDynamicallyTypedList(IEnumerable source)
        {
            var sourceArray = source as object[] ?? source.Cast<object>().ToArray();
            var type = GetCommonBaseClass(sourceArray);
            var listType = typeof(List<>).MakeGenericType(type);
            var addMethod = listType.GetMethod("Add");
            var list = listType.GetConstructor(Type.EmptyTypes)?.Invoke(null);

            foreach (var o in sourceArray)
                if (addMethod != null)
                    addMethod.Invoke(list, new[] {o});

            return (IEnumerable) list;
        }

        public static Type GetCommonBaseClass(IEnumerable e)
        {
            var types = e.Cast<object>().Select(o => o.GetType()).ToArray();
            return GetCommonBaseClass(types);
        }

        private static Type GetCommonBaseClass(Type[] types)
        {
            if (types.Length == 0)
                return typeof(object);

            var ret = types[0];

            for (var i = 1; i < types.Length; ++i)
                if (types[i].IsAssignableFrom(ret))
                    ret = types[i];
                else
                    while (ret != null && !ret.IsAssignableFrom(types[i]))
                        ret = ret.BaseType;

            return ret;
        }

        #endregion
    }
}
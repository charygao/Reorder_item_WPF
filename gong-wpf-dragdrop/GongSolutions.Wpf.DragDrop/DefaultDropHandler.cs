using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GongSolutions.Wpf.DragDrop.Utilities;

namespace GongSolutions.Wpf.DragDrop
{
    internal sealed class DefaultDropHandler : IDropTarget
    {
        public void DragOver(DropInfo dropInfo)
        {
            if (CanAcceptData(dropInfo))
            {
                dropInfo.Effects = DragDropEffects.Copy;
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            }
        }

        public void Drop(DropInfo dropInfo)
        {
            var insertIndex = dropInfo.InsertIndex;
            var destinationList = GetList(dropInfo.TargetCollection);
            var data = ExtractData(dropInfo.Data);

            var dataArray = data as object[] ?? data.Cast<object>().ToArray();
            if (Equals(dropInfo.DragInfo.VisualSource, dropInfo.VisualTarget))
            {
                var sourceList = GetList(dropInfo.DragInfo.SourceCollection);

                foreach (var o in dataArray)
                {
                    var index = sourceList.IndexOf(o);

                    if (index != -1)
                    {
                        sourceList.RemoveAt(index);

                        if (Equals(sourceList, destinationList) && index < insertIndex) --insertIndex;
                    }
                }
            }

            foreach (var o in dataArray) destinationList.Insert(insertIndex++, o);
        }

        #region  Methods

        private static bool CanAcceptData(DropInfo dropInfo)
        {
            if (Equals(dropInfo.DragInfo.SourceCollection, dropInfo.TargetCollection))
                return GetList(dropInfo.TargetCollection) != null;

            if (dropInfo.DragInfo.SourceCollection is ItemCollection) return false;

            if (TestCompatibleTypes(dropInfo.TargetCollection, dropInfo.Data))
                return !IsChildOf(dropInfo.VisualTargetItem, dropInfo.DragInfo.VisualSourceItem);
            return false;
        }

        private static IEnumerable ExtractData(object data)
        {
            if (data is IEnumerable enumerable && !(enumerable is string))
                return enumerable;
            return Enumerable.Repeat(data, 1);
        }

        private static IList GetList(IEnumerable enumerable)
        {
            if (enumerable is ICollectionView view)
                return view.SourceCollection as IList;
            return enumerable as IList;
        }

        private static bool IsChildOf(UIElement targetItem, UIElement sourceItem)
        {
            var parent = ItemsControl.ItemsControlFromItemContainer(targetItem);

            while (parent != null)
            {
                if (Equals(parent, sourceItem)) return true;

                parent = ItemsControl.ItemsControlFromItemContainer(parent);
            }

            return false;
        }

        private static bool TestCompatibleTypes(IEnumerable target, object data)
        {
            bool TypeFilter(Type t, object o)
            {
                return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>);
            }

            var enumerableInterfaces = target.GetType().FindInterfaces(TypeFilter, null);
            var enumerableTypes = from i in enumerableInterfaces select i.GetGenericArguments().Single();

            var enumerableTypesArray = enumerableTypes as Type[] ?? enumerableTypes.ToArray();
            if (enumerableTypesArray.Any())
            {
                var dataType = TypeUtilities.GetCommonBaseClass(ExtractData(data));
                return enumerableTypesArray.Any(t => t.IsAssignableFrom(dataType));
            }

            return target is IList;
        }

        #endregion
    }
}
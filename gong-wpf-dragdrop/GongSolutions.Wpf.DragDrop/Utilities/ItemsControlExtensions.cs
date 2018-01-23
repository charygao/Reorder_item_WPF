using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace GongSolutions.Wpf.DragDrop.Utilities
{
    public static class ItemsControlExtensions
    {
        #region  Methods

        public static bool CanSelectMultipleItems(this ItemsControl itemsControl)
        {
            if (itemsControl is MultiSelector)
            {
                var obj = (bool?)itemsControl.GetType()
                    .GetProperty("CanSelectMultipleItems", BindingFlags.Instance | BindingFlags.NonPublic)
                    ?.GetValue(itemsControl, null);

                return obj != null && (bool)obj;

            }
            if (itemsControl is ListBox box)
                return box.SelectionMode != SelectionMode.Single;
            return false;
        }

        public static UIElement GetItemContainer(this ItemsControl itemsControl, UIElement child)
        {
            var itemType = GetItemContainerType(itemsControl);

            if (itemType != null) return (UIElement)child.GetVisualAncestor(itemType);

            return null;
        }

        public static UIElement GetItemContainerAt(this ItemsControl itemsControl, Point position)
        {
            var inputElement = itemsControl.InputHitTest(position);

            if (inputElement is UIElement uiElement) return GetItemContainer(itemsControl, uiElement);

            return null;
        }

        private static Type GetItemContainerType(this ItemsControl itemsControl)
        {
            // There is no safe way to get the item container type for an ItemsControl. The
            // best we can do is to look for the control's ItemsPresenter, get it's child 
            // panel and the first child of that *should* be an item container.
            //
            // If the control currently has no items, we're out of luck.
            if (itemsControl.Items.Count > 0)
            {
                var itemsPresenters = itemsControl.GetVisualDescendents<ItemsPresenter>();

                foreach (var itemsPresenter in itemsPresenters)
                {
                    var panel = VisualTreeHelper.GetChild(itemsPresenter, 0);
                    var itemContainer = VisualTreeHelper.GetChild(panel, 0);

                    // Ensure that this actually *is* an item container by checking it with
                    // ItemContainerGenerator.
                    if (itemsControl.ItemContainerGenerator.IndexFromContainer(itemContainer) != -1)
                        return itemContainer.GetType();
                }
            }

            return null;
        }

        public static Orientation GetItemsPanelOrientation(this ItemsControl itemsControl)
        {
            var itemsPresenter = itemsControl.GetVisualDescendent<ItemsPresenter>();
            var itemsPanel = VisualTreeHelper.GetChild(itemsPresenter, 0);
            var orientationProperty = itemsPanel.GetType().GetProperty("Orientation", typeof(Orientation));

            if (orientationProperty != null)
                return (Orientation)orientationProperty.GetValue(itemsPanel, null);
            return Orientation.Vertical;
        }

        public static IEnumerable GetSelectedItems(this ItemsControl itemsControl)
        {
            if (itemsControl is MultiSelector selector) return selector.SelectedItems;

            if (itemsControl is ListBox listBox)
            {
                if (listBox.SelectionMode == SelectionMode.Single)
                    return Enumerable.Repeat(listBox.SelectedItem, 1);
                return listBox.SelectedItems;
            }

            if (itemsControl is TreeView view)
                return Enumerable.Repeat(view.SelectedItem, 1);
            if (itemsControl is Selector selector1)
                return Enumerable.Repeat(selector1.SelectedItem, 1);
            return Enumerable.Empty<object>();
        }

        #endregion
    }
}
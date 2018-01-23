using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GongSolutions.Wpf.DragDrop.Utilities;

namespace GongSolutions.Wpf.DragDrop
{
    public class DragInfo
    {
        #region Fields and Properties

        public object Data { get; set; }
        public Point DragStartPosition { get; }
        public DragDropEffects Effects { get; set; }
        //public MouseButton MouseButton { get; }
        public IEnumerable SourceCollection { get; }
        public object SourceItem { get; }
        public IEnumerable SourceItems { get; }
        public UIElement VisualSource { get; }
        public UIElement VisualSourceItem { get; }

        #endregion

        #region  Constructors

        public DragInfo(object sender, MouseButtonEventArgs e)
        {
            DragStartPosition = e.GetPosition(null);
            Effects = DragDropEffects.None;
            //MouseButton = e.ChangedButton;
            VisualSource = sender as UIElement;

            if (sender is ItemsControl itemsControl)
            {
                var item = itemsControl.GetItemContainer((UIElement) e.OriginalSource);

                if (item != null)
                {
                    var itemParent = ItemsControl.ItemsControlFromItemContainer(item);

                    SourceCollection = itemParent.ItemsSource ?? itemParent.Items;
                    SourceItem = itemParent.ItemContainerGenerator.ItemFromContainer(item);
                    SourceItems = itemsControl.GetSelectedItems();

                    // Some controls (I'm looking at you TreeView!) haven't updated their
                    // SelectedItem by this point. Check to see if there 1 or less item in 
                    // the SourceItems collection, and if so, override the SelectedItems
                    // with the clicked item.
                    if (SourceItems.Cast<object>().Count() <= 1) SourceItems = Enumerable.Repeat(SourceItem, 1);

                    VisualSourceItem = item;
                }
                else
                {
                    SourceCollection = itemsControl.ItemsSource ?? itemsControl.Items;
                }
            }

            if (SourceItems == null) SourceItems = Enumerable.Empty<object>();
        }

        #endregion
    }
}
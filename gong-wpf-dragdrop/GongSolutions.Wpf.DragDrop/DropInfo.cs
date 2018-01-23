using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using GongSolutions.Wpf.DragDrop.Utilities;

namespace GongSolutions.Wpf.DragDrop
{
    public class DropInfo
    {
        #region Fields and Properties

        public object Data { get; }
        public DragInfo DragInfo { get; }
        public Type DropTargetAdorner { get; set; }
        public DragDropEffects Effects { get; set; }
        public int InsertIndex { get; }
        public IEnumerable TargetCollection { get; }
        public object TargetItem { get; }
        public UIElement VisualTarget { get; }
        public UIElement VisualTargetItem { get; }
        public Orientation VisualTargetOrientation { get; }

        #endregion

        #region  Constructors

        public DropInfo(object sender, DragEventArgs e, DragInfo dragInfo, string dataFormat)
        {
            Data = e.Data.GetDataPresent(dataFormat) ? e.Data.GetData(dataFormat) : e.Data;
            DragInfo = dragInfo;

            VisualTarget = sender as UIElement;

            if (sender is ItemsControl)
            {
                var itemsControl = (ItemsControl) sender;
                var item = itemsControl.GetItemContainerAt(e.GetPosition(itemsControl));

                VisualTargetOrientation = itemsControl.GetItemsPanelOrientation();

                if (item != null)
                {
                    var itemParent = ItemsControl.ItemsControlFromItemContainer(item);

                    InsertIndex = itemParent.ItemContainerGenerator.IndexFromContainer(item);
                    TargetCollection = itemParent.ItemsSource ?? itemParent.Items;
                    TargetItem = itemParent.ItemContainerGenerator.ItemFromContainer(item);
                    VisualTargetItem = item;

                    if (VisualTargetOrientation == Orientation.Vertical)
                    {
                        if (e.GetPosition(item).Y > item.RenderSize.Height / 2) InsertIndex++;
                    }
                    else
                    {
                        if (e.GetPosition(item).X > item.RenderSize.Width / 2) InsertIndex++;
                    }
                }
                else
                {
                    TargetCollection = itemsControl.ItemsSource ?? itemsControl.Items;
                    InsertIndex = itemsControl.Items.Count;
                }
            }
        }

        #endregion
    }
}
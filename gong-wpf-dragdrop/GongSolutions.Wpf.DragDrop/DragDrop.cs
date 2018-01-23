using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using GongSolutions.Wpf.DragDrop.Utilities;

namespace GongSolutions.Wpf.DragDrop
{
    public static class DragDrop
    {
        #region Fields and Properties

        public static readonly DependencyProperty DragAdornerTemplateProperty =
            DependencyProperty.RegisterAttached("DragAdornerTemplate", typeof(DataTemplate), typeof(DragDrop));

        public static readonly DependencyProperty DragHandlerProperty =
            DependencyProperty.RegisterAttached("DragHandler", typeof(IDragSource), typeof(DragDrop));

        public static readonly DependencyProperty DropHandlerProperty =
            DependencyProperty.RegisterAttached("DropHandler", typeof(IDropTarget), typeof(DragDrop));

        public static readonly DependencyProperty IsDragSourceProperty =
            DependencyProperty.RegisterAttached("IsDragSource", typeof(bool), typeof(DragDrop),
                new UIPropertyMetadata(false, IsDragSourceChanged));

        public static readonly DependencyProperty IsDropTargetProperty =
            DependencyProperty.RegisterAttached("IsDropTarget", typeof(bool), typeof(DragDrop),
                new UIPropertyMetadata(false, IsDropTargetChanged));

        private static IDragSource _mDefaultDragHandler;
        private static IDropTarget _mDefaultDropHandler;
        private static DragAdorner _mDragAdorner;
        private static DragInfo _mDragInfo;
        private static DropTargetAdorner _mDropTargetAdorner;
        private static readonly DataFormat MFormat = DataFormats.GetDataFormat("GongSolutions.Wpf.DragDrop");

        private static IDragSource DefaultDragHandler => _mDefaultDragHandler ?? (_mDefaultDragHandler = new DefaultDragHandler());

        private static IDropTarget DefaultDropHandler => _mDefaultDropHandler ?? (_mDefaultDropHandler = new DefaultDropHandler());

        private static DragAdorner DragAdorner
        {
            get => _mDragAdorner;
            set
            {
                _mDragAdorner?.Detatch();

                _mDragAdorner = value;
            }
        }

        private static DropTargetAdorner DropTargetAdorner
        {
            get => _mDropTargetAdorner;
            set
            {
                _mDropTargetAdorner?.Detatch();

                _mDropTargetAdorner = value;
            }
        }

        #endregion

        #region  Methods

        private static DataTemplate GetDragAdornerTemplate(UIElement target) => (DataTemplate) target.GetValue(DragAdornerTemplateProperty);

        private static IDragSource GetDragHandler(UIElement target) => (IDragSource) target.GetValue(DragHandlerProperty);

        public static IDropTarget GetDropHandler(UIElement target) => (IDropTarget) target.GetValue(DropHandlerProperty);

        public static bool GetIsDragSource(UIElement target) => (bool) target.GetValue(IsDragSourceProperty);

        public static bool GetIsDropTarget(UIElement target) => (bool) target.GetValue(IsDropTargetProperty);

        public static void SetDragAdornerTemplate(UIElement target, DataTemplate value) => target.SetValue(DragAdornerTemplateProperty, value);

        public static void SetDragHandler(UIElement target, IDragSource value) => target.SetValue(DragHandlerProperty, value);

        public static void SetDropHandler(UIElement target, IDropTarget value)
        {
            target.SetValue(DropHandlerProperty, value);
        }

        public static void SetIsDragSource(UIElement target, bool value)
        {
            target.SetValue(IsDragSourceProperty, value);
        }

        public static void SetIsDropTarget(UIElement target, bool value)
        {
            target.SetValue(IsDropTargetProperty, value);
        }

        private static void CreateDragAdorner()
        {
            var template = GetDragAdornerTemplate(_mDragInfo.VisualSource);

            if (template != null)
            {
                var rootElement = (UIElement) Application.Current.MainWindow?.Content;
                UIElement adornment = null;

                if (_mDragInfo.Data is IEnumerable enumerable && !(enumerable is string))
                {
                    var itemsControlItemsSource = enumerable as object[] ?? enumerable.Cast<object>().ToArray();
                    if (itemsControlItemsSource.Length <= 10)
                    {
                        var itemsControl = new ItemsControl
                        {
                            ItemsSource = itemsControlItemsSource,
                            ItemTemplate = template
                        };

                        // The ItemsControl doesn't display unless we create a border to contain it.
                        // Not quite sure why this is...
                        var border = new Border {Child = itemsControl};
                        adornment = border;
                    }
                }
                else
                {
                    var contentPresenter = new ContentPresenter
                    {
                        Content = _mDragInfo.Data,
                        ContentTemplate = template
                    };
                    adornment = contentPresenter;
                }

                if (adornment != null)
                {
                    adornment.Opacity = 0.5;
                    DragAdorner = new DragAdorner(rootElement, adornment);
                }
            }
        }

        private static void DragSource_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Ignore the click if the user has clicked on a scrollbar.
            if (HitTestScrollBar(sender, e))
            {
                _mDragInfo = null;
                return;
            }

            _mDragInfo = new DragInfo(sender, e);

            // If the sender is a list box that allows multiple selections, ensure that clicking on an 
            // already selected item does not change the selection, otherwise dragging multiple items 
            // is made impossible.

            if (_mDragInfo.VisualSourceItem != null && sender is ItemsControl itemsControl && itemsControl.CanSelectMultipleItems())
            {
                var selectedItems = itemsControl.GetSelectedItems();

                if (selectedItems.Cast<object>().Contains(_mDragInfo.SourceItem)) e.Handled = true;
            }
        }

        private static void DragSource_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _mDragInfo = null;
        }

        private static void DragSource_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_mDragInfo != null)
            {
                var dragStart = _mDragInfo.DragStartPosition;
                var position = e.GetPosition(null);

                if (Math.Abs(position.X - dragStart.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(position.Y - dragStart.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    var dragHandler = GetDragHandler(_mDragInfo.VisualSource);

                    if (dragHandler != null)
                        dragHandler.StartDrag(_mDragInfo);
                    else
                        DefaultDragHandler.StartDrag(_mDragInfo);

                    if (_mDragInfo.Effects != DragDropEffects.None && _mDragInfo.Data != null)
                    {
                        var data = new DataObject(MFormat.Name, _mDragInfo.Data);
                        System.Windows.DragDrop.DoDragDrop(_mDragInfo.VisualSource, data, _mDragInfo.Effects);
                        _mDragInfo = null;
                    }
                }
            }
        }

        private static void DropTarget_PreviewDragEnter(object sender, DragEventArgs e)
        {
            DropTarget_PreviewDragOver(sender, e);
        }

        private static void DropTarget_PreviewDragLeave(object sender, DragEventArgs e)
        {
            DragAdorner = null;
            DropTargetAdorner = null;
        }

        private static void DropTarget_PreviewDragOver(object sender, DragEventArgs e)
        {
            var dropInfo = new DropInfo(sender, e, _mDragInfo, MFormat.Name);
            var dropHandler = GetDropHandler((UIElement) sender);

            if (dropHandler != null)
                dropHandler.DragOver(dropInfo);
            else
                DefaultDropHandler.DragOver(dropInfo);

            // Update the drag adorner.
            if (dropInfo.Effects != DragDropEffects.None)
            {
                if (DragAdorner == null && _mDragInfo != null) CreateDragAdorner();

                if (DragAdorner != null)
                {
                    DragAdorner.MousePosition = e.GetPosition(DragAdorner.AdornedElement);
                    DragAdorner.InvalidateVisual();
                }
            }
            else
            {
                DragAdorner = null;
            }

            // If the target is an ItemsControl then update the drop target adorner.
            if (sender is ItemsControl control)
            {
                UIElement adornedElement = control.GetVisualDescendent<ItemsPresenter>();

                if (dropInfo.DropTargetAdorner == null)
                    DropTargetAdorner = null;
                else if (!dropInfo.DropTargetAdorner.IsInstanceOfType(DropTargetAdorner))
                    DropTargetAdorner = DropTargetAdorner.Create(dropInfo.DropTargetAdorner, adornedElement);

                if (DropTargetAdorner != null)
                {
                    DropTargetAdorner.DropInfo = dropInfo;
                    DropTargetAdorner.InvalidateVisual();
                }
            }

            e.Effects = dropInfo.Effects;
            e.Handled = true;

            Scroll((DependencyObject) sender, e);
        }

        private static void DropTarget_PreviewDrop(object sender, DragEventArgs e)
        {
            var dropInfo = new DropInfo(sender, e, _mDragInfo, MFormat.Name);
            var dropHandler = GetDropHandler((UIElement) sender);

            DragAdorner = null;
            DropTargetAdorner = null;

            if (dropHandler != null)
                dropHandler.Drop(dropInfo);
            else
                DefaultDropHandler.Drop(dropInfo);

            e.Handled = true;
        }

        private static bool HitTestScrollBar(object sender, MouseButtonEventArgs e)
        {
            var hit = VisualTreeHelper.HitTest((Visual) sender, e.GetPosition((IInputElement) sender));
            return hit.VisualHit.GetVisualAncestor<ScrollBar>() != null;
        }

        private static void IsDragSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uiElement = (UIElement) d;

            if ((bool) e.NewValue)
            {
                uiElement.PreviewMouseLeftButtonDown += DragSource_PreviewMouseLeftButtonDown;
                uiElement.PreviewMouseLeftButtonUp += DragSource_PreviewMouseLeftButtonUp;
                uiElement.PreviewMouseMove += DragSource_PreviewMouseMove;
            }
            else
            {
                uiElement.PreviewMouseLeftButtonDown -= DragSource_PreviewMouseLeftButtonDown;
                uiElement.PreviewMouseLeftButtonUp -= DragSource_PreviewMouseLeftButtonUp;
                uiElement.PreviewMouseMove -= DragSource_PreviewMouseMove;
            }
        }

        private static void IsDropTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uiElement = (UIElement) d;

            if ((bool) e.NewValue)
            {
                uiElement.AllowDrop = true;
                uiElement.PreviewDragEnter += DropTarget_PreviewDragEnter;
                uiElement.PreviewDragLeave += DropTarget_PreviewDragLeave;
                uiElement.PreviewDragOver += DropTarget_PreviewDragOver;
                uiElement.PreviewDrop += DropTarget_PreviewDrop;
            }
            else
            {
                uiElement.AllowDrop = false;
                uiElement.PreviewDragEnter -= DropTarget_PreviewDragEnter;
                uiElement.PreviewDragLeave -= DropTarget_PreviewDragLeave;
                uiElement.PreviewDragOver -= DropTarget_PreviewDragOver;
                uiElement.PreviewDrop -= DropTarget_PreviewDrop;
            }
        }

        private static void Scroll(DependencyObject o, DragEventArgs e)
        {
            var scrollViewer = o.GetVisualDescendent<ScrollViewer>();

            if (scrollViewer != null)
            {
                var position = e.GetPosition(scrollViewer);
                var scrollMargin = Math.Min(scrollViewer.FontSize * 2, scrollViewer.ActualHeight / 2);

                if (position.X >= scrollViewer.ActualWidth - scrollMargin &&
                    scrollViewer.HorizontalOffset < scrollViewer.ExtentWidth - scrollViewer.ViewportWidth)
                    scrollViewer.LineRight();
                else if (position.X < scrollMargin && scrollViewer.HorizontalOffset > 0)
                    scrollViewer.LineLeft();
                else if (position.Y >= scrollViewer.ActualHeight - scrollMargin &&
                         scrollViewer.VerticalOffset < scrollViewer.ExtentHeight - scrollViewer.ViewportHeight)
                    scrollViewer.LineDown();
                else if (position.Y < scrollMargin && scrollViewer.VerticalOffset > 0)
                    scrollViewer.LineUp();
            }
        }

        #endregion
    }
}
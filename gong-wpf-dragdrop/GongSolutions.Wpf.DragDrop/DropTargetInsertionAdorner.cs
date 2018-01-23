using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GongSolutions.Wpf.DragDrop
{
    public class DropTargetInsertionAdorner : DropTargetAdorner
    {
        #region Fields and Properties

        private static readonly Pen MPen;
        private static readonly PathGeometry MTriangle;

        #endregion

        #region  Constructors

        static DropTargetInsertionAdorner()
        {
            // Create the pen and triangle in a static constructor and freeze them to improve performance.
            const int triangleSize = 3;

            MPen = new Pen(Brushes.Gray, 2);
            MPen.Freeze();

            var firstLine = new LineSegment(new Point(0, -triangleSize), false);
            firstLine.Freeze();
            var secondLine = new LineSegment(new Point(0, triangleSize), false);
            secondLine.Freeze();

            var figure = new PathFigure {StartPoint = new Point(triangleSize, 0)};
            figure.Segments.Add(firstLine);
            figure.Segments.Add(secondLine);
            figure.Freeze();

            MTriangle = new PathGeometry();
            MTriangle.Figures.Add(figure);
            MTriangle.Freeze();
        }

        public DropTargetInsertionAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
        }

        #endregion

        #region  Methods

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (DropInfo.VisualTarget is ItemsControl itemsControl)
            {
                // Get the position of the item at the insertion index. If the insertion point is
                // to be after the last item, then get the position of the last item and add an 
                // offset later to draw it at the end of the list.

                var itemParent = DropInfo.VisualTargetItem != null ? ItemsControl.ItemsControlFromItemContainer(DropInfo.VisualTargetItem) : itemsControl;

                var index = Math.Min(DropInfo.InsertIndex, itemParent.Items.Count - 1);
                var itemContainer = (UIElement) itemParent.ItemContainerGenerator.ContainerFromIndex(index);

                if (itemContainer != null)
                {
                    var itemRect = new Rect(itemContainer.TranslatePoint(new Point(), AdornedElement),
                        itemContainer.RenderSize);
                    Point point1, point2;
                    double rotation = 0;

                    if (DropInfo.VisualTargetOrientation == Orientation.Vertical)
                    {
                        if (DropInfo.InsertIndex == itemParent.Items.Count)
                            itemRect.Y += itemContainer.RenderSize.Height;

                        point1 = new Point(itemRect.X, itemRect.Y);
                        point2 = new Point(itemRect.Right, itemRect.Y);
                    }
                    else
                    {
                        if (DropInfo.InsertIndex == itemParent.Items.Count)
                            itemRect.X += itemContainer.RenderSize.Width;

                        point1 = new Point(itemRect.X, itemRect.Y);
                        point2 = new Point(itemRect.X, itemRect.Bottom);
                        rotation = 90;
                    }

                    drawingContext.DrawLine(MPen, point1, point2);
                    DrawTriangle(drawingContext, point1, rotation);
                    DrawTriangle(drawingContext, point2, 180 + rotation);
                }
            }
        }

        private void DrawTriangle(DrawingContext drawingContext, Point origin, double rotation)
        {
            drawingContext.PushTransform(new TranslateTransform(origin.X, origin.Y));
            drawingContext.PushTransform(new RotateTransform(rotation));

            drawingContext.DrawGeometry(MPen.Brush, null, MTriangle);

            drawingContext.Pop();
            drawingContext.Pop();
        }

        #endregion
    }
}
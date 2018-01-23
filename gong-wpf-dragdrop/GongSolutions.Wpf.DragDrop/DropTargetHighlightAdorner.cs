using System.Windows;
using System.Windows.Media;

namespace GongSolutions.Wpf.DragDrop
{
    public class DropTargetHighlightAdorner : DropTargetAdorner
    {
        #region  Constructors

        public DropTargetHighlightAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
        }

        #endregion

        #region  Methods

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (DropInfo.VisualTargetItem != null)
            {
                var rect = new Rect(
                    DropInfo.VisualTargetItem.TranslatePoint(new Point(), AdornedElement),
                    VisualTreeHelper.GetDescendantBounds(DropInfo.VisualTargetItem).Size);
                drawingContext.DrawRoundedRectangle(null, new Pen(Brushes.Gray, 2), rect, 2, 2);
            }
        }

        #endregion
    }
}
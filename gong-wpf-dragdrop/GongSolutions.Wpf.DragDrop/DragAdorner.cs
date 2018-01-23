using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace GongSolutions.Wpf.DragDrop
{
    internal class DragAdorner : Adorner
    {
        #region Fields and Properties

        private readonly AdornerLayer _mAdornerLayer;
        private readonly UIElement _mAdornment;
        private Point _mMousePosition;

        public Point MousePosition
        {
            private get => _mMousePosition;
            set
            {
                if (_mMousePosition != value)
                {
                    _mMousePosition = value;
                    _mAdornerLayer.Update(AdornedElement);
                }
            }
        }

        protected override int VisualChildrenCount => 1;

        #endregion

        #region  Constructors

        public DragAdorner(UIElement adornedElement, UIElement adornment)
            : base(adornedElement)
        {
            _mAdornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
            _mAdornerLayer.Add(this);
            _mAdornment = adornment;
            IsHitTestVisible = false;
        }

        #endregion

        #region  Methods

        public void Detatch()
        {
            _mAdornerLayer.Remove(this);
        }

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            var result = new GeneralTransformGroup();
            result.Children.Add(base.GetDesiredTransform(transform));
            result.Children.Add(new TranslateTransform(MousePosition.X - 4, MousePosition.Y - 4));

            return result;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _mAdornment.Arrange(new Rect(finalSize));
            return finalSize;
        }

        protected override Visual GetVisualChild(int index)
        {
            return _mAdornment;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            _mAdornment.Measure(constraint);
            return _mAdornment.DesiredSize;
        }

        #endregion
    }
}
using System;
using System.Windows;
using System.Windows.Documents;

namespace GongSolutions.Wpf.DragDrop
{
    public abstract class DropTargetAdorner : Adorner
    {
        #region Fields and Properties

        private readonly AdornerLayer _mAdornerLayer;

        public DropInfo DropInfo { protected get; set; }

        #endregion

        #region  Constructors

        protected DropTargetAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            _mAdornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
            _mAdornerLayer.Add(this);
            IsHitTestVisible = false;
        }

        #endregion

        #region  Methods

        public void Detatch()
        {
            _mAdornerLayer.Remove(this);
        }

        internal static DropTargetAdorner Create(Type type, UIElement adornedElement)
        {
            if (!typeof(DropTargetAdorner).IsAssignableFrom(type))
                throw new InvalidOperationException(
                    "The requested adorner class does not derive from DropTargetAdorner.");

            return (DropTargetAdorner) type.GetConstructor(new[] {typeof(UIElement)})
                ?.Invoke(new object[] {adornedElement});
        }

        #endregion
    }
}
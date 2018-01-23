using System;

namespace GongSolutions.Wpf.DragDrop
{
    public static class DropTargetAdorners
    {
        #region Fields and Properties

        public static Type Highlight => typeof(DropTargetHighlightAdorner);

        public static Type Insert => typeof(DropTargetInsertionAdorner);

        #endregion
    }
}
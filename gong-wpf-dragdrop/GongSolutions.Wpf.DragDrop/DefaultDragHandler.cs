using System.Linq;
using System.Windows;
using GongSolutions.Wpf.DragDrop.Utilities;

namespace GongSolutions.Wpf.DragDrop
{
    public sealed class DefaultDragHandler : IDragSource
    {
        public void StartDrag(DragInfo dragInfo)
        {
            var itemCount = dragInfo.SourceItems.Cast<object>().Count();

            if (itemCount == 1)
                dragInfo.Data = dragInfo.SourceItems.Cast<object>().First();
            else if (itemCount > 1)
                dragInfo.Data = TypeUtilities.CreateDynamicallyTypedList(dragInfo.SourceItems);

            dragInfo.Effects = dragInfo.Data != null
                ? DragDropEffects.Copy | DragDropEffects.Move
                : DragDropEffects.None;
        }
    }
}
namespace GongSolutions.Wpf.DragDrop
{
    public interface IDropTarget
    {
        #region  Methods

        void DragOver(DropInfo dropInfo);
        void Drop(DropInfo dropInfo);

        #endregion
    }
}
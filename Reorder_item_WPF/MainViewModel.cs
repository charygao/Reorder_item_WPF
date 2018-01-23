using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using GongSolutions.Wpf.DragDrop;

namespace Reorder_item_WPF
{
    internal class MainViewModel : IDropTarget
    {
        #region Fields and Properties

        public ObservableCollection<Msp> MspCollection { get; }
        public ObservableCollection<Msp> NewMspCollection { get; }

        #endregion

        #region  Constructors

        public MainViewModel()
        {
            MspCollection = new ObservableCollection<Msp>
            {
                new Msp
                {
                    Id = 1,
                    Name = "Anis Derbel"
                },
                new Msp
                {
                    Id = 2,
                    Name = "Firas Mdimagh"
                },
                new Msp
                {
                    Id = 3,
                    Name = "Khaled Jemni"
                },
                new Msp
                {
                    Id = 4,
                    Name = "Sahbouch"
                }
            };
            NewMspCollection = new ObservableCollection<Msp>
            {
                new Msp
                {
                    Id = 1,
                    Name = "DropTargetTest"
                },
            };
        }

        #endregion


        void IDropTarget.DragOver(DropInfo dropInfo)
        {
            if (dropInfo.Data is Msp)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Move;
            }
        }

        void IDropTarget.Drop(DropInfo dropInfo)
        {
            var msp = (Msp) dropInfo.Data;
            ((IList) dropInfo.DragInfo.SourceCollection).Remove(msp);
        }
    }
}
using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Reorder_item_WPF
{
    class MainViewModel : IDropTarget
    {
        public ObservableCollection<MSP> MSPCollection { get; set; }

        public MainViewModel()
        {


            MSPCollection = new ObservableCollection<MSP>();

            MSPCollection.Add(new MSP() { 
                Id = 1,
                Name = "Anis Derbel"
            });

            MSPCollection.Add(new MSP()
            {
                Id = 2,
                Name = "Firas Mdimagh"
            });

            MSPCollection.Add(new MSP()
            {
                Id = 3,
                Name = "Khaled Jemni"
            });

            MSPCollection.Add(new MSP()
            {
                Id = 4,
                Name = "Sahbouch"
            });

        
        }


        void IDropTarget.DragOver(DropInfo dropInfo)
        {
            if (dropInfo.Data is MSP)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Move;
            }
        }

        void IDropTarget.Drop(DropInfo dropInfo)
        {
           
            MSP msp = (MSP)dropInfo.Data;
            ((IList)dropInfo.DragInfo.SourceCollection).Remove(msp);
        }
    }
}

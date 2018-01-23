using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using GongSolutions.Wpf.DragDrop;

namespace SchoolsExample
{
    internal class MainViewModel : IDropTarget
    {
        #region Fields and Properties

        public ICollectionView Pupils { get; set; }

        public ICollectionView Schools { get; }

        #endregion

        #region  Constructors

        public MainViewModel()
        {
            var schools = new ObservableCollection<SchoolViewModel>
            {
                new SchoolViewModel
                {
                    Name = "Bloomfield School",
                    Pupils = new ObservableCollection<PupilViewModel>
                    {
                        new PupilViewModel {FullName = "Adam James"},
                        new PupilViewModel {FullName = "Sophie Johnston"},
                        new PupilViewModel {FullName = "Kevin Sandler"},
                        new PupilViewModel {FullName = "Oscar Peterson"}
                    }
                },
                new SchoolViewModel
                {
                    Name = "Redacre School",
                    Pupils = new ObservableCollection<PupilViewModel>
                    {
                        new PupilViewModel {FullName = "Tom Jefferson"},
                        new PupilViewModel {FullName = "Tony Potts"}
                    }
                },
                new SchoolViewModel
                {
                    Name = "Top Valley School",
                    Pupils = new ObservableCollection<PupilViewModel>
                    {
                        new PupilViewModel {FullName = "Alex Thompson"},
                        new PupilViewModel {FullName = "Tabitha Smith"},
                        new PupilViewModel {FullName = "Carl Pederson"},
                        new PupilViewModel {FullName = "Sarah Jones"},
                        new PupilViewModel {FullName = "Paul Lowcroft"}
                    }
                }
            };


            Schools = CollectionViewSource.GetDefaultView(schools);

            var pupils = new ObservableCollection<PupilViewModel>
            {
                new PupilViewModel {FullName = "TestPupil1"},
                new PupilViewModel {FullName = "TestPupil2"},
                new PupilViewModel {FullName = "TestPupil3"},
                new PupilViewModel {FullName = "TestPupil4"},
                new PupilViewModel {FullName = "TestPupil5"}
            };

            Pupils = CollectionViewSource.GetDefaultView(pupils);

        }

        #endregion

        void IDropTarget.DragOver(DropInfo dropInfo)
        {
            if (dropInfo.Data is PupilViewModel && dropInfo.TargetItem is SchoolViewModel)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Move;
            }
        }

        void IDropTarget.Drop(DropInfo dropInfo)
        {
            var school = (SchoolViewModel) dropInfo.TargetItem;
            var pupil = (PupilViewModel) dropInfo.Data;
            school.Pupils.Add(pupil);
            ((IList) dropInfo.DragInfo.SourceCollection).Remove(pupil);
        }
    }
}
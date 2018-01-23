using System.Collections.ObjectModel;

namespace SchoolsExample
{
    internal class SchoolViewModel
    {
        #region Fields and Properties

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Name { get; set; }
        // ReSharper disable once CollectionNeverQueried.Global
        public ObservableCollection<PupilViewModel> Pupils { get; set; }

        #endregion

        #region  Constructors

        public SchoolViewModel()
        {
            Pupils = new ObservableCollection<PupilViewModel>();
        }

        #endregion
    }
}
using MediaFilesManager.Abstractions;
using Sample.ViewModels;
using Xamarin.Forms;

namespace Sample
{
    public partial class Photos : ContentPage
    {
        public Photos(IMediaAlbum album)
        {
            InitializeComponent();

            BindingContext = new PhotosViewModel(album);
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var asset = (e.SelectedItem as PhotosViewModel.PhotoItem)?.Asset as IMediaFileImage;
            if (asset != null)
            {
                this.Navigation.PushAsync(new Photo(asset));
            }
        }
    }
}

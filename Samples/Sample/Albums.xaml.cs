using Sample.ViewModels;
using Xamarin.Forms;

namespace Sample
{
    public partial class Albums : ContentPage
    {
        public Albums()
        {
            InitializeComponent();

            BindingContext = new AlbumsViewModel();
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var albumItem = e.SelectedItem as AlbumsViewModel.AlbumItem;
            if(albumItem != null)
            {
                this.Navigation.PushAsync(new Photos(albumItem.AssetsAlbum));
            }
        }
    }
}

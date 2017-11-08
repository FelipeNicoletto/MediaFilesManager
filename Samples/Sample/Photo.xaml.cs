using System;
using Sample.ViewModels;
using Xamarin.Forms;
using MediaFilesManager.Abstractions;

namespace Sample
{
    public partial class Photo : ContentPage
    {
        public Photo(IMediaFileImage image)
        {
            InitializeComponent();

            imgOrientationPicker.Items.Add("Up");
            imgOrientationPicker.Items.Add("UpMirrored");
            imgOrientationPicker.Items.Add("Down");
            imgOrientationPicker.Items.Add("DownMirrored");
            imgOrientationPicker.Items.Add("LeftMirrored");
            imgOrientationPicker.Items.Add("Left");
            imgOrientationPicker.Items.Add("RightMirrored");
            imgOrientationPicker.Items.Add("Right");

            imgResizeModePicker.Items.Add("Fill");
            imgResizeModePicker.Items.Add("AspectFill");
            imgResizeModePicker.Items.Add("AspectFit");

            BindingContext = new PhotoViewModel(image);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            ((PhotoViewModel)BindingContext).Apply();
        }
    }
}

using Acr.UserDialogs;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System.Threading.Tasks;
using TravelMonkey.Services;
using Xamarin.Forms;

namespace TravelMonkey.ViewModels
{
    public class AddNewTranslationPageViewModel : BaseViewModel
    {
        private readonly ComputerVisionService _computerVisionService = new ComputerVisionService();

        public bool ShowImagePlaceholder => !ShowPhoto;
        public bool ShowPhoto => _photoSource != null;

        MediaFile _photo;
        StreamImageSource _photoSource;
        public StreamImageSource PhotoSource
        {
            get => _photoSource;
            set
            {
                if (Set(ref _photoSource, value))
                {
                    RaisePropertyChanged(nameof(ShowPhoto));
                    RaisePropertyChanged(nameof(ShowImagePlaceholder));
                }
            }
        }

        private bool _isPosting;
        public bool IsPosting
        {
            get => _isPosting;
            set => Set(ref _isPosting, value);
        }

        private Color _backgroundColor = Color.SteelBlue;
        public Color BackgroundColor
        {
            get => _backgroundColor;
            set => Set(ref _backgroundColor, value);
        }

        private string _displayText;
        public string DisplayText
        {
            get => _displayText;
            set => Set(ref _displayText, value);
        }

        public Command TakePhotoCommand { get; }
        public Command TranslateTextCommand { get; }

        public AddNewTranslationPageViewModel()
        {
            TakePhotoCommand = new Command(async () => await TakePhoto());
            TranslateTextCommand = new Command(async () => await TranslateText());
        }

        private async Task TranslateText()
        {
            IsPosting = true;

            try
            {
                if (string.IsNullOrWhiteSpace(_displayText))
                {
                    await UserDialogs.Instance.AlertAsync("Please choose an image/phote with text", "No text recognized");
                    return;
                }

                MessagingCenter.Send(this, Constants.TranslationSuccessMessage);
            }
            finally
            {
                IsPosting = false;
            }
        }

        private async Task TakePhoto()
        {
            var result = await UserDialogs.Instance.ActionSheetAsync("What do you want to do?",
                "Cancel", null, null, "Take photo", "Choose photo");

            if (result.Equals("Take photo"))
            {
                _photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions { PhotoSize = PhotoSize.Small });

                PhotoSource = (StreamImageSource)ImageSource.FromStream(() => _photo.GetStream());
            }
            else if (result.Equals("Choose photo"))
            {
                _photo = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions { PhotoSize = PhotoSize.Small });

                PhotoSource = (StreamImageSource)ImageSource.FromStream(() => _photo.GetStream());
            }
            else
            {
                return;
            }

            if (_photo != null)
                await PostImage();
        }

        private async Task PostImage()
        {
            if (_photo == null)
            {
                await UserDialogs.Instance.AlertAsync("Please select an image first", "No image selected");
                return;
            }

            IsPosting = true;

            try
            {
                var pictureStream = _photo.GetStreamWithImageRotatedForExternalStorage();
                var result = await _computerVisionService.ExtractTextFromPicture(pictureStream);

                if (!result.Succeeded)
                {
                    MessagingCenter.Send(this, Constants.PictureFailedMessage);
                    return;
                }

                DisplayText = result.Succeeded ? result.ExtractedText : result.ErrorMessage;                
            }
            finally
            {
                IsPosting = false;
            }
        }
    }
}
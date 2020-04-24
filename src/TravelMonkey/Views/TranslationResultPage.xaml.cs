using System;
using TravelMonkey.Services;
using TravelMonkey.ViewModels;
using Xamarin.Forms;

namespace TravelMonkey.Views
{
    public partial class TranslationResultPage : ContentPage
    {
        private readonly TranslateResultPageViewModel _translateResultPageViewModel =
            new TranslateResultPageViewModel();

        private readonly SpeechService _speechService = new SpeechService();

        public TranslationResultPage(string inputText)
        {
            InitializeComponent();

            MessagingCenter.Subscribe<TranslateResultPageViewModel>(this,
                Constants.TranslationFailedMessage,
                async (s) =>
                {
                    await DisplayAlert("Whoops!", "We lost our dictionary, something went wrong while translating", "OK");
                });

            _translateResultPageViewModel.InputText = inputText;

            BindingContext = _translateResultPageViewModel;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            await _speechService.TextToSpeechAsync(_translateResultPageViewModel.InputText, _translateResultPageViewModel.InputLanguage);
        }

        private async void SpeechButton_Clicked(object sender, EventArgs e)
        {
            string key = ((ImageButton)sender).BindingContext as string;

            if(_translateResultPageViewModel.Translations.TryGetValue(key, out string text))
            {
                await _speechService.TextToSpeechAsync(text, key);
            }
        }
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using TravelMonkey.Services;
using Xamarin.Forms;

namespace TravelMonkey.ViewModels
{
    public class TranslateResultPageViewModel : BaseViewModel
    {
        private readonly TranslationService _translationService =
            new TranslationService();

        private string _inputText;
        private Dictionary<string, string> _translations;
        private string _inputLanguage;

        public string InputText
        {
            get => _inputText;
            set
            {
                if (_inputText == value)
                    return;

                Set(ref _inputText, value);

                TranslateText();
            }
        }
              
        public string InputLanguage
        {
            get => _inputLanguage;
            set => Set(ref _inputLanguage, value);
        }

        public Dictionary<string, string> Translations
        {
            get => _translations;
            set
            {
                Set(ref _translations, value);
            }
        }
        public Command<string> TranslateTextCommand => new Command<string>((inputText) =>
        {
            InputText = inputText;
        });

        private async void TranslateText()
        {
            var result = await _translationService.TranslateText(_inputText);

            if (!result.Succeeded)
                MessagingCenter.Send(this, Constants.TranslationFailedMessage);

            Translations = result.Translations;
            InputLanguage = result.InputLanguage;
        }
    }
}
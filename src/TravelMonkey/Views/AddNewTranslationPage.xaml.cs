using System;
using TravelMonkey.ViewModels;
using Xamarin.Forms;

namespace TravelMonkey.Views
{
    public partial class AddNewTranslationPage : ContentPage
    {
        public AddNewTranslationPage()
        {
            InitializeComponent();

            BindingContext = new AddNewTranslationPageViewModel();

            MessagingCenter.Subscribe<AddNewTranslationPageViewModel>(this, Constants.TranslationPictureAddedMessage, 
                async (vm) => await Navigation.PopModalAsync(true));

            MessagingCenter.Subscribe<AddNewTranslationPageViewModel>(this, Constants.TranslationPictureFailedMessage, 
                async (vm) => await DisplayAlert("Uh-oh!", "Can you hand me my glasses? Something went wrong while analyzing this image", "OK"));

            MessagingCenter.Subscribe<AddNewTranslationPageViewModel>(this, Constants.TranslationSuccessMessage,
                async (vm) => await Navigation.PushModalAsync(new TranslationResultPage(vm.DisplayText))); 
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}
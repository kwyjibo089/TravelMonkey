using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace TravelMonkey.Views.SpeechComponents
{
    public class SpeechDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate LeftText { get; set; }
        public DataTemplate RightText { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (((Models.SpeechChatText)item).IsTranlation)
                return RightText;
            return LeftText;
        }
    }
}
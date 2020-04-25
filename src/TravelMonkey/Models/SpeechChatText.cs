using System;
using System.Collections.Generic;
using System.Text;

namespace TravelMonkey.Models
{
    public class SpeechChatText
    {
        public bool IsTranlation { get; set; }
        public string Text { get; set; }
        public string LanguageCode { get; set; }
        public string DateString { get; set; }
    }
}

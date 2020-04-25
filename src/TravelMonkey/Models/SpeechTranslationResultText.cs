using System;
using System.Collections.Generic;
using System.Text;

namespace TravelMonkey.Models
{
    public class SpeechTranslationResultText
    {
        public string SourceLanguageCode { get; set; }
        public string TargetLanguageCode { get; set; }
        public string OriginalText { get; set; }
        public string TranslatedText { get; set; }
    }
}

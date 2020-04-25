using System;
using System.Collections.Generic;
using System.Text;
using TravelMonkey.Models;

namespace TravelMonkey.Helpers
{
    public static class LanguagesHelper
    {
        public static List<Language> GetSupportedLanguages()
        {
            var languages = new List<Language>
            {
                new Language() { Name = "Arabic (Saudi Arabia)", Code = "ar-SA", Voice = "ar-SA-Naayf" },
                new Language() { Name = "Catalan", Code = "ca-ES", Voice = "ca-ES-HerenaRUS" },
                new Language() { Name = "Danish (Denmark)", Code = "da-DK", Voice = "da-DK-HelleRUS" },
                new Language() { Name = "German (Germany)", Code = "de-DE", Voice = "de-DE-Hedda" },
                new Language() { Name = "English (Australia)", Code = "en-AU", Voice = "en-AU-Catherine" },
                new Language() { Name = "English (Canada)", Code = "en-CA", Voice = "en-CA-Linda" },
                new Language() { Name = "English (United Kingdom)", Code = "en-GB", Voice = "en-GB-Susan-Apollo" },
                new Language() { Name = "English (India)", Code = "en-IN", Voice = "en-IN-Heera-Apollo" },
                new Language() { Name = "English (United States)", Code = "en-US", Voice = "en-US-ZiraRUS" },
                new Language() { Name = "Spanish (Spain)", Code = "es-ES", Voice = "es-ES-Laura-Apollo" },
                new Language() { Name = "Spanish (Mexico)", Code = "es-MX", Voice = "es-MX-HildaRUS" },
                new Language() { Name = "Finnish (Finland)", Code = "fi-FI", Voice = "fi-FI-HeidiRUS" },
                new Language() { Name = "French (Canada)", Code = "fr-CA", Voice = "fr-CA-Caroline" },
                new Language() { Name = "French (France)", Code = "fr-FR", Voice = "fr-FR-Julie-Apollo" },
                new Language() { Name = "Hindi (India)", Code = "hi-IN", Voice = "hi-IN-Kalpana-Apollo" },
                new Language() { Name = "Italian (Italy)", Code = "it-IT", Voice = "it-IT-Cosimo-Apollo" },
                new Language() { Name = "Japanese (Japan)", Code = "ja-JP", Voice = "ja-JP-Ayumi-Apollo" },
                new Language() { Name = "Korean (Korea)", Code = "ko-KR", Voice = "ko-KR-HeamiRUS" },
                new Language() { Name = "Norwegian (Bokmål) (Norway)", Code = "nb-NO", Voice = "nb-NO-HuldaRUS" },
                new Language() { Name = "Dutch (Netherlands)", Code = "nl-NL", Voice = "nl-NL-HannaRUS" },
                new Language() { Name = "Polish (Poland)", Code = "pl-PL", Voice = "pl-PL-PaulinaRUS" },
                new Language() { Name = "Portuguese (Brazil)", Code = "pt-BR", Voice = "pt-BR-HeloisaRUS" },
                new Language() { Name = "Portuguese (Portugal)", Code = "pt-PT", Voice = "pt-PT-HeliaRUS" },
                new Language() { Name = "Russian (Russia)", Code = "ru-RU", Voice = "ru-RU-Irina-Apollo" },
                new Language() { Name = "Swedish (Sweden)", Code = "sv-SE", Voice = "sv-SE-HedvigRUS" },
                new Language() { Name = "Tamil (India)", Code = "ta-IN", Voice = "ta-IN-Valluvar" },
                new Language() { Name = "Telugu (India)", Code = "te-IN", Voice = "te-IN-Chitra" },
                new Language() { Name = "Chinese (Mandarin, simplified)", Code = "zh-CN", Voice = "zh-CN-HuihuiRUS" },
                new Language() { Name = "Chinese (Cantonese, Traditional)", Code = "zh-HK", Voice = "zh-HK-Tracy-Apollo" },
                new Language() { Name = "Chinese (Taiwanese Mandarin)", Code = "zh-TW", Voice = "zh-TW-Yating-Apollo" },
                new Language() { Name = "Thai (Thailand)", Code = "th-TH", Voice = "th-TH-Pattara" },
                new Language() { Name = "Turkey", Code = "tr-TR", Voice = "tr-TR-SedaRUS" }
            };

            return languages;
        }
    }
}

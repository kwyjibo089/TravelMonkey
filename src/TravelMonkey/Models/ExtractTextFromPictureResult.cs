using System;
using System.Collections.Generic;
using System.Text;

namespace TravelMonkey.Models
{
    public class ExtractTextFromPictureResult
    {
        public string ExtractedText { get; }

        public string[] TextLines { get; }

        public bool Succeeded => !string.IsNullOrEmpty(ExtractedText);

        public string ErrorMessage { get; set; }

        public ExtractTextFromPictureResult() { }

        public ExtractTextFromPictureResult(string[] textLines)
        {
            TextLines = textLines;
            ExtractedText = $"I see {string.Join(" ", textLines)}";
        }
    }
}

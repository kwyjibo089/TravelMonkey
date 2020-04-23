using System.Globalization;

namespace TravelMonkey.Models
{
    public class ExtractTextFromPictureResult
    {
        public string ExtractedText
        { 
            get
            {
                return (TextLines == null || TextLines.Length==0) ?
                    string.Empty :
                    CultureInfo.InvariantCulture.TextInfo.ToTitleCase(string.Join(" ", TextLines).ToLower());
            }
        }

        public string DisplayText { get { return $"I see {ExtractedText}"; } }

        public string[] TextLines { get; }

        public bool Succeeded => !string.IsNullOrEmpty(ExtractedText);

        public string ErrorMessage { get; set; }

        public ExtractTextFromPictureResult() { }

        public ExtractTextFromPictureResult(string[] textLines)
        {
            TextLines = textLines;
        }
    }
}

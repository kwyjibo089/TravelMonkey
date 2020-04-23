using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using TravelMonkey.Models;
using Xamarin.Forms;

namespace TravelMonkey.Services
{
    public class ComputerVisionService
    {
        private const int numberOfCharsInOperationId = 36;
        private const int maxNumberOfRetries = 10;

        private readonly ComputerVisionClient _computerVisionClient = new ComputerVisionClient(new ApiKeyServiceClientCredentials(ApiKeys.ComputerVisionApiKey))
        {
            Endpoint = ApiKeys.ComputerVisionEndpoint
        };

        public async Task<AddPictureResult> AddPicture(Stream pictureStream)
        {
            try
            {
                var result = await _computerVisionClient.AnalyzeImageInStreamAsync(pictureStream, details: new[] { Details.Landmarks }, visualFeatures: new[] { VisualFeatureTypes.Color, VisualFeatureTypes.Description });

                // Get most likely description
                var description = result.Description.Captions.OrderByDescending(d => d.Confidence).FirstOrDefault()?.Text ?? "nothing! No description found";

                // Get accent color
                var accentColor = Color.FromHex($"#{result.Color.AccentColor}");

                // Determine if there are any landmarks to be seen
                var landmark = result.Categories.FirstOrDefault(c => c.Detail != null && c.Detail.Landmarks.Any());

                var landmarkDescription = "";

                landmarkDescription = landmark != null ? landmark.Detail.Landmarks.OrderByDescending(l => l.Confidence).First().Name : "";

                // Wrap in our result object and send along
                return new AddPictureResult(description, accentColor, landmarkDescription);
            }
            catch
            {
                return new AddPictureResult();
            }
        }

        public async Task<ExtractTextFromPictureResult> ExtractTextFromPicture(Stream pictureStream)
        {
            try
            {
                BatchReadFileInStreamHeaders textHeaders = await _computerVisionClient.BatchReadFileInStreamAsync(pictureStream);

                string operationLocation = textHeaders.OperationLocation;

                // Retrieve the URI where the recognized text will be stored from the Operation-Location header.
                // We only need the ID and not the full URL                
                string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

                // Extract the text
                // Delay is between iterations and tries a maximum of 10 times.
                int numberOfRetries = 0;
                ReadOperationResult results;
                
                do
                {
                    results = await _computerVisionClient.GetReadOperationResultAsync(operationId);
                    await Task.Delay(1000);
                    if (numberOfRetries == 9)
                    {
                        return new ExtractTextFromPictureResult { ErrorMessage = "Server timed out." };
                    }
                }
                while ((results.Status == TextOperationStatusCodes.Running ||
                    results.Status == TextOperationStatusCodes.NotStarted) && numberOfRetries++ < maxNumberOfRetries);

                var textRecognitionLocalFileResults = results.RecognitionResults;
                IList<string> textLines = new List<string>();
                foreach (TextRecognitionResult recResult in textRecognitionLocalFileResults)
                {
                    foreach (Line line in recResult.Lines)
                    {
                        textLines.Add(line.Text);
                    }
                }

                return new ExtractTextFromPictureResult(textLines.ToArray());
            } 
            catch (Exception e)
            {
                return new ExtractTextFromPictureResult { ErrorMessage = e.Message };
            }
        }
    }
}
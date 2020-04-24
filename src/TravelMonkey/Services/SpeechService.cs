using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace TravelMonkey.Services
{
    public class SpeechService
    {
        public async Task TextToSpeechAsync(string text, string language)
        {
            using (var client = new HttpClient())
            {
                string token = await GetTokenAsync(client);

                var ssml = "<speak version='1.0' xmlns=\"http://www.w3.org/2001/10/synthesis\" xml:lang='en-US'>" +
                           "<voice  name='Microsoft Server Speech Text to Speech Voice (en-US, Jessa24kRUS)'>" +
                                text + "</voice> </speak>";

                HttpRequestMessage audioRequest = new HttpRequestMessage(HttpMethod.Post, ApiKeys.SpeechEndpoint);

                audioRequest.Content = new StringContent(ssml);
                audioRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/ssml+xml");
                audioRequest.Headers.Authorization = AuthenticationHeaderValue.Parse("Bearer " + token);
                audioRequest.Headers.UserAgent.Add(new ProductInfoHeaderValue("TravelMonkey", "1.0"));
                audioRequest.Headers.Add("X-Microsoft-OutputFormat", "audio-24khz-48kbitrate-mono-mp3");

                var audioResult = client.SendAsync(audioRequest).Result;

                var audio = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
                audio.Load(audioResult.Content.ReadAsStreamAsync().Result);
                audio.Play();
            }
        }

        private async Task<string> GetTokenAsync(HttpClient client)
        {
            HttpRequestMessage authRequest = new HttpRequestMessage(HttpMethod.Post, ApiKeys.SpeechAuthEndpoint);

            authRequest.Headers.Add("Ocp-Apim-Subscription-Key", $"{ApiKeys.SpeechApiKey}");

            var authResult = await client.SendAsync(authRequest);

            return await authResult.Content.ReadAsStringAsync();
        }
    }
}

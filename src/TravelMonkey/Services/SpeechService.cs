using Newtonsoft.Json;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TravelMonkey.Models.CognitiveServices;

namespace TravelMonkey.Services
{
    public class SpeechService
    {
        private const string SSML = 
            "<speak version='1.0' xmlns=\"http://www.w3.org/2001/10/synthesis\" xmlns:mstts=\"https://www.w3.org/2001/mstts\" xml:lang='{0}'>" +
                "<voice name = '{1}' >" +
                    "{2}" +
                "</voice>" +
            "</speak>";

        public async Task TextToSpeechAsync(string text, string language)
        {
            using (var client = new HttpClient())
            {
                string token = await GetTokenAsync(client);

                if (VoiceResult.Voices == null)
                {
                    HttpRequestMessage voicesRequest = new HttpRequestMessage(HttpMethod.Get, ApiKeys.VoicesEndpoint);
                    voicesRequest.Headers.Authorization = AuthenticationHeaderValue.Parse("Bearer " + token);
                    voicesRequest.Headers.UserAgent.Add(new ProductInfoHeaderValue("TravelMonkey", "1.0"));
                    var voicesResponse = await client.SendAsync(voicesRequest);
                    var jsonString = await voicesResponse.Content.ReadAsStringAsync();

                    VoiceResult.Voices = JsonConvert.DeserializeObject<Voice[]>(jsonString);
                }

                // try to get a neural voice
                var voice = VoiceResult.Voices.FirstOrDefault(v => v.VoiceType == VoiceType.Neural && v.Locale.StartsWith(language));
                if(voice == null)
                {
                    // fallback is normal voice
                    voice = VoiceResult.Voices.FirstOrDefault(v => v.Locale.StartsWith(language));
                }
                
                var ssml = string.Format(SSML, language, voice.Name, text);

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

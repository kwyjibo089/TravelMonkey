using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Translation;
using Plugin.AudioRecorder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TravelMonkey.Helpers;
using TravelMonkey.Models;
using Xamarin.Cognitive.Speech;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TravelMonkey.ViewModels
{
    public class SpeechTranslationViewModel : BaseViewModel
    {
        #region Properties

        public enum TranslationState
        {
            Idle,
            Listening,
            Processing,
            Translating
        }

        TranslationState _state;
        public TranslationState State
        {
            get { return _state; }
            set
            {
                _state = value;
                RaisePropertyChanged();
                HandleTranslationStateChanged(State);
            }
        }

        List<Language> _sourceLanguages;
        public List<Language> SourceLanguages
        {
            get { return _sourceLanguages; }
            set
            {
                _sourceLanguages = value;
                RaisePropertyChanged();
            }
        }

        Language _selectedSourceLanguage;
        public Language SelectedSourceLanguage
        {
            get { return _selectedSourceLanguage; }
            set
            {
                _selectedSourceLanguage = value;
                RaisePropertyChanged();
                Speaking = $"Speak in {SelectedSourceLanguage.Name}";
            }
        }

        List<Language> _targetLanguages;
        public List<Language> TargetLanguages
        {
            get { return _targetLanguages; }
            set
            {
                _targetLanguages = value;
                RaisePropertyChanged();
            }
        }

        Language _selectedTargetLanguage;
        public Language SelectedTargetLanguage
        {
            get { return _selectedTargetLanguage; }
            set
            {
                _selectedTargetLanguage = value;
                RaisePropertyChanged();
            }
        }

        public SpeechTranslationResultText TranslationResult { get; set; }

        List<Language> _languages;
        public List<Language> Languages
        {
            get { return _languages; }
            set
            {
                _languages = value;
                RaisePropertyChanged();
            }
        }

        bool _isInputEnabled;
        public bool IsInputEnabled
        {
            get { return _isInputEnabled; }
            set
            {
                _isInputEnabled = value;
                RaisePropertyChanged();
            }
        }

        Color _stateColor;
        public Color StateColor
        {
            get { return _stateColor; }
            set
            {
                _stateColor = value;
                RaisePropertyChanged();
            }
        }

        string _speaking;
        public string Speaking
        {
            get { return _speaking; }
            set
            {
                _speaking = value;
                RaisePropertyChanged();
            }
        }

        string _stateText;
        public string StateText
        {
            get { return _stateText; }
            set
            {
                _stateText = value;
                RaisePropertyChanged();
            }
        }

        ObservableCollection<SpeechChatText> _chatList;
        public ObservableCollection<SpeechChatText> ChatList
        {
            get { return _chatList; }
            set
            {
                _chatList = value;
                RaisePropertyChanged();
            }
        }

        AudioRecorderService Recorder;
        AudioPlayer Player;
        SpeechTranslationConfig DeviceSpeechTranslationConfig;
        AudioConfig DeviceAudioConfig;
        TranslationRecognizer DeviceTranslationRecognizer;
        string WAVFilePath;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public SpeechTranslationViewModel()
        {
            _ = CheckAndRequestMicrophonePermission();

            ChatList = new ObservableCollection<SpeechChatText>();

            State = TranslationState.Idle;

            Languages = LanguagesHelper.GetSupportedLanguages();
            SourceLanguages = Languages;
            TargetLanguages = Languages;

            SelectedTargetLanguage = TargetLanguages.Where(s => s.Code.Equals("fr-FR")).FirstOrDefault();
            SelectedSourceLanguage = SourceLanguages.Where(s => s.Code.Equals("en-GB")).FirstOrDefault();
           

            Recorder = new AudioRecorderService
            {
                StopRecordingOnSilence = true, 
                StopRecordingAfterTimeout = true,
                AudioSilenceTimeout = TimeSpan.FromSeconds(3), // will stop recording after 3 seconds of silence
                TotalAudioTimeout = TimeSpan.FromSeconds(15) // audio will stop recording after 15 seconds
            };

            Player = new AudioPlayer();

            Player.FinishedPlaying += Player_FinishedPlaying;
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Request for Microphone Permission
        /// </summary>
        public async Task CheckAndRequestMicrophonePermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Microphone>();

            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Microphone>();
            }

            if(status != PermissionStatus.Granted)
            {
                await Application.Current.MainPage.DisplayAlert("Microphone Permission Denied", "Please allow access to Microphone in Settings", "OK");
            }
        }

        /// <summary>
        /// Event Handler when Player finishes playing audio
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Player_FinishedPlaying(object sender, EventArgs e)
        {
            State = TranslationState.Idle;
        }

        /// <summary>
        /// Function to change the list of target languages after a source language has been selected
        /// Tries to avoid having duplicate selected languages
        /// </summary>
        /// <param name="languageCode"></param>
        void HandleSourceLanguageChanged(Language language)
        {
            Speaking = $"Speaking in {language.Name}";
            TargetLanguages = ReturnAvailableLanguages(language.Code);
        }

        /// <summary>
        /// Function to change the list of source languages after a target language has been selected
        /// Tries to avoid having duplicate selected languages
        /// </summary>
        /// <param name="languageCode"></param>
        void HandleTargetLanguageChanged(Language language)
        {
            SourceLanguages = ReturnAvailableLanguages(language.Code);
        }

        /// <summary>
        /// Function to get remaining languages after one has been selected
        /// Basically returns all Languages other than the selected one
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns>List of Languages</returns>
        List<Language> ReturnAvailableLanguages(string languageCode)
        {
            return Languages.Where(s => s.Code.ToLower() != languageCode.ToLower()).ToList();
        }

        /// <summary>
        /// Function to initiate listening of audio
        /// </summary>
        async Task ListenForAudio()
        {
            if (!Recorder.IsRecording)
            {
                WAVFilePath = null;

                State = TranslationState.Listening;

                var filePath = await Recorder.StartRecording().Result;

                if (!string.IsNullOrEmpty(filePath))
                {
                    State = TranslationState.Processing;

                    WAVFilePath = filePath;

                    await StartRecognizingSpeech();
                }
                else
                {
                    State = TranslationState.Idle;
                }
            }
        }

        /// <summary>
        /// Function to initiate translation
        /// </summary>
        public async Task StartRecognizingSpeech()
        {
            try
            {
                InitializeTranslator();

                await DeviceTranslationRecognizer.RecognizeOnceAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Function to initialize translator
        /// </summary>
        void InitializeTranslator()
        {
            DeviceSpeechTranslationConfig = SpeechTranslationConfig.FromSubscription(ApiKeys.SpeechAPIKey, ApiKeys.AzureRegion);
            DeviceSpeechTranslationConfig.SpeechRecognitionLanguage = SelectedSourceLanguage.Code;
            DeviceSpeechTranslationConfig.VoiceName = SelectedTargetLanguage.Voice;
            DeviceSpeechTranslationConfig.AddTargetLanguage(SelectedTargetLanguage.Code);

            Player.Play(WAVFilePath);

            DeviceAudioConfig = AudioConfig.FromWavFileInput(WAVFilePath);

            DeviceSpeechTranslationConfig.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Riff16Khz16BitMonoPcm);

            DeviceTranslationRecognizer = new TranslationRecognizer(DeviceSpeechTranslationConfig, DeviceAudioConfig);

            DeviceTranslationRecognizer.Synthesizing += OnSynthesizingEventHandler;
            DeviceTranslationRecognizer.Recognized += OnRecognizedEventHandler;
        }

        /// <summary>
        /// TranslationRecognition Event Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRecognizedEventHandler(object sender, TranslationRecognitionEventArgs e)
        {
            try
            {
                if (e.Result.Text.Length == 0)
                {
                    DialogsHelper.HandleMessageDialog(DialogsHelper.MessageType.Defined, $"No text, reason: {e.Result.Reason}");
                    State = TranslationState.Idle;
                }
                else
                {
                    SpeechTranslationResultText translationResult = new SpeechTranslationResultText
                    {
                        OriginalText = e.Result.Text,
                        SourceLanguageCode = SelectedSourceLanguage.Code.Substring(0, 2),
                    };

                    foreach (var t in e.Result.Translations)
                    {
                        translationResult.TargetLanguageCode = t.Key;
                        translationResult.TranslatedText = t.Value;
                    }

                    TranslationResult = translationResult;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                IsInputEnabled = true;
            }
        }

        void HandleChatMessage(SpeechTranslationResultText translationResult)
        {
            ChatList.Add(new SpeechChatText
            {
                IsTranlation = false,
                LanguageCode = translationResult.SourceLanguageCode,
                Text = translationResult.OriginalText,
                DateString = DateTime.Now.ToString("h:mm tt")
            });

            ChatList.Add(new SpeechChatText
            {
                IsTranlation = true,
                LanguageCode = translationResult.TargetLanguageCode,
                Text = translationResult.TranslatedText,
                DateString = DateTime.Now.ToString("h:mm tt")
            });
        }

        /// <summary>
        /// TranslationSynthesis Event Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="translationSynthesisEventArgs"></param>
        private void OnSynthesizingEventHandler(object sender, TranslationSynthesisEventArgs translationSynthesisEventArgs)
        {
            try
            {
                if (
                    translationSynthesisEventArgs.Result.Reason != ResultReason.SynthesizingAudioCompleted 
                    && translationSynthesisEventArgs.Result.GetAudio().Length > 0
                    )
                {
                    State = TranslationState.Translating;

                    byte[] audioResponse = translationSynthesisEventArgs.Result.GetAudio();

                    string filePath = FileUtility.Instance.TemporarilySaveFile("audio_response.wav", audioResponse);

                    HandleChatMessage(TranslationResult);

                    Player.Play(filePath);
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                IsInputEnabled = true;
            }
        }

        /// <summary>
        /// Function to handle TranslationState changed
        /// </summary>
        /// <param name="state">Takes in the current state</param>
        void HandleTranslationStateChanged(TranslationState state)
        {
            StateText = ConvertEnumToString(State);

            switch (state)
            {
                case TranslationState.Idle:
                    HandleIdleState();
                    break;
                case TranslationState.Listening:
                    HandleListeningState();
                    break;
                case TranslationState.Processing:
                    HandleProcessingState();
                    break;
                case TranslationState.Translating:
                    HandleTranslatingState();
                    break;
            };
        }

        void HandleIdleState()
        {
            StateText = "Start";
            IsInputEnabled = true;
            StateColor = Color.FromHex("#9e9e9e");
        }

        void HandleListeningState()
        {
            IsInputEnabled = false;
            StateColor = Color.FromHex("#49c94b");
        }

        void HandleProcessingState()
        {
            IsInputEnabled = false;
            StateColor = Color.FromHex("#ff9800");
        }

        void HandleTranslatingState()
        {
            IsInputEnabled = false;
            StateColor = Color.FromHex("#2196F3");
        }

        void SwitchSpeaker()
        {
            Language tempSourceLanguage = SelectedSourceLanguage;
            Language tempTargetLanguage = SelectedTargetLanguage;

            SelectedSourceLanguage = tempTargetLanguage;
            SelectedTargetLanguage = tempSourceLanguage;
        }

        /// <summary>
        /// Helper for converting enum to string
        /// </summary>
        string ConvertEnumToString(Enum eff)
        {
            return Enum.GetName(eff.GetType(), eff);
        }

        /// <summary>
        /// Command to initiate translation
        /// </summary>
        ICommand _startListeningCommand = null;
        public ICommand StartListeningCommand
        {
            get
            {
                return _startListeningCommand ?? (_startListeningCommand = new Command(async (object obj) => await ListenForAudio()));
            }
        }

        /// <summary>
        /// Command for switching speaker
        /// </summary>
        ICommand _switchSpeakerCommand = null;
        public ICommand SwitchSpeakerCommand
        {
            get
            {
                return _switchSpeakerCommand ?? (_switchSpeakerCommand = new Command((object obj) => SwitchSpeaker()));
            }
        }

        /// <summary>
        /// Command for clearing chat
        /// </summary>
        ICommand _clearChatCommand = null;
        public ICommand ClearChatCommand
        {
            get
            {
                return _clearChatCommand ?? (_clearChatCommand = new Command((object obj) => ChatList.Clear()));
            }
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechSynthesis;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace XamlBrewer.Universal.Speech
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        private async void VoiceButton_Click(object sender, RoutedEventArgs e)
        {
            var currentText = this.SpeechInputBox.Text;

            if (this.SpeechInputBox.VoiceGender == VoiceGender.Female)
            {
                this.SpeechInputBox.Text = "Switched to my male voice.";
                this.SpeechInputBox.VoiceGender = VoiceGender.Male;
            }
            else
            {
                this.SpeechInputBox.Text = "Switched to my female voice.";
                this.SpeechInputBox.VoiceGender = VoiceGender.Female;
            }

            await this.SpeechInputBox.Speak();
            this.SpeechInputBox.Text = currentText;
        }

        private async void ConstraintsButton_Click(object sender, RoutedEventArgs e)
        {
            this.SpeechInputBox.Question = "What's your favorite color?";
            this.SpeechInputBox.Text = "What is your favorite color?";
            await this.SpeechInputBox.Speak();

            var storageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets//ColorRecognizer.xml"));
            var grammarFileConstraint = new Windows.Media.SpeechRecognition.SpeechRecognitionGrammarFileConstraint(storageFile, "colors");
            this.SpeechInputBox.Constraints.Clear();
            this.SpeechInputBox.Constraints.Add(grammarFileConstraint);          
            this.SpeechInputBox.StartListening();
        }

        private async void SpeakButton_Click(object sender, RoutedEventArgs e)
        {
            await this.SpeechInputBox.Speak();
        }

        private void ListenButton_Click(object sender, RoutedEventArgs e)
        {
            this.SpeechInputBox.StartListening();
        }
    }
}

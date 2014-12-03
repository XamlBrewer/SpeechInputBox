using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechRecognition;
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
            var currentText = this.SpeechDialogBox.Text;

            if (this.SpeechDialogBox.VoiceGender == VoiceGender.Female)
            {
                this.SpeechDialogBox.Text = "Switched to my male voice.";
                this.SpeechDialogBox.VoiceGender = VoiceGender.Male;
            }
            else
            {
                this.SpeechDialogBox.Text = "Switched to my female voice.";
                this.SpeechDialogBox.VoiceGender = VoiceGender.Female;
            }

            await this.SpeechDialogBox.Speak();
            this.SpeechDialogBox.Text = currentText;
        }

        private async void ConversationButton_Click(object sender, RoutedEventArgs e)
        {
            // Set the question.
            this.SpeechDialogBox.Question = "What's your favorite color?";

            // Let the control ask the question out loud.
            await this.SpeechDialogBox.Speak("What is your favorite color?");

            // Reset the control when it answered (optional).
            this.SpeechDialogBox.TextChanged += this.SpeechInputBox_TextChanged;

            // Teach the control to recognize the colors of the rainbow in a random text.
            var storageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets//ColorRecognizer.xml"));
            var grammarFileConstraint = new SpeechRecognitionGrammarFileConstraint(storageFile, "colors");
            this.SpeechDialogBox.Constraints.Clear();
            this.SpeechDialogBox.Constraints.Add(grammarFileConstraint);

            // Format the spoken response.
            this.SpeechDialogBox.ResponsePattern = "What a coincidence. {0} is my favorite color too.";

            // Start listening
            this.SpeechDialogBox.StartListening();
        }

        private async void SpeechInputBox_TextChanged(object sender, EventArgs e)
        {
            this.SpeechDialogBox.TextChanged -= this.SpeechInputBox_TextChanged;
            await this.SpeechDialogBox.Reset();
        }

        private async void SpeakButton_Click(object sender, RoutedEventArgs e)
        {
            await this.SpeechDialogBox.Speak();
        }

        private void ListenButton_Click(object sender, RoutedEventArgs e)
        {
            this.SpeechDialogBox.StartListening();
        }

        private async void SpeakButton2_Click(object sender, RoutedEventArgs e)
        {
            await this.SpeechDialogBox.Speak("Hello there.");

            await Task.Delay(1000); // Alternative: queue speaking terms.

            var folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            folder = await folder.GetFolderAsync("Assets");
            var file = await folder.GetFileAsync("SSML_Sample.xml");
            var ssml = await Windows.Storage.FileIO.ReadTextAsync(file);
            await this.SpeechDialogBox.SpeakSsml(ssml);

            await Task.Delay(20000); // Alternative: queue speaking terms.
        }
    }
}

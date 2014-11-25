namespace XamlBrewer.Universal.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Windows.ApplicationModel.Resources;
    using Windows.Foundation;
    using Windows.Media.SpeechRecognition;
    using Windows.Media.SpeechSynthesis;
    using Windows.System;
    using Windows.UI;
    using Windows.UI.Core;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Media;

    // Resource mgt: http://msdn.microsoft.com/en-us/library/windows/apps/xaml/hh965326.aspx

    [TemplatePart(Name = MediaElementPartName, Type = typeof(MediaElement))]
    [TemplatePart(Name = DefaultStatePartName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = TextBlockPartName, Type = typeof(TextBlock))]
    [TemplatePart(Name = TextStatePartName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = TextBoxPartName, Type = typeof(TextBox))]
    [TemplatePart(Name = MicrophoneButtonPartName, Type = typeof(Button))]
    [TemplatePart(Name = ListeningStatePartName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = ListeningButtonPartName, Type = typeof(Button))]
    [TemplatePart(Name = ThinkingStatePartName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = ThinkingButtonPartName, Type = typeof(Button))]
    public sealed class SpeechInputBox : Control
    {
        #region Constants

        private const string MediaElementPartName = "PART_MediaElement";
        private const string DefaultStatePartName = "PART_DefaultState";
        private const string TextBlockPartName = "PART_TextBlock";
        private const string TextStatePartName = "PART_TextState";
        private const string TextBoxPartName = "PART_TextBox";
        private const string MicrophoneButtonPartName = "PART_MicrophoneButton";
        private const string ListeningStatePartName = "PART_ListeningState";
        private const string ListeningButtonPartName = "PART_ListeningButton";
        private const string ThinkingStatePartName = "PART_ThinkingState";
        private const string ThinkingButtonPartName = "PART_ThinkingButton";

        #endregion

        #region Fields

        private MediaElement mediaElement;
        private FrameworkElement defaultState;
        private TextBlock textBlock;
        private FrameworkElement textState;
        private TextBox textBox;
        private Button microphoneButton;
        private FrameworkElement listeningState;
        private Button listeningButton;
        private FrameworkElement thinkingState;
        private Button thinkingButton;

        private SpeechInputBoxState state = SpeechInputBoxState.Default;
        private List<ISpeechRecognitionConstraint> constraints = new List<ISpeechRecognitionConstraint>() { new SpeechRecognitionTopicConstraint(SpeechRecognitionScenario.Dictation, "Development") };

        #endregion

        #region Constructors
        public SpeechInputBox()
        {
            this.DefaultStyleKey = typeof(SpeechInputBox);
        }

        #endregion

        #region Dependency Property Registrations

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("TextProperty", typeof(string), typeof(SpeechInputBox), new PropertyMetadata(string.Empty, OnTextChanged));

        public static readonly DependencyProperty QuestionProperty =
            DependencyProperty.Register("Question", typeof(string), typeof(SpeechInputBox), new PropertyMetadata("Ask me anything...", OnQuestionChanged));

        public static readonly DependencyProperty ButtonBackgroundProperty =
            DependencyProperty.Register("ButtonBackground", typeof(Brush), typeof(SpeechInputBox), new PropertyMetadata(new SolidColorBrush(Colors.DimGray)));

        public static readonly DependencyProperty HighlightProperty =
            DependencyProperty.Register("Highlight", typeof(Brush), typeof(SpeechInputBox), new PropertyMetadata(new SolidColorBrush(Colors.OrangeRed)));

        public static readonly DependencyProperty VoiceGenderProperty =
            DependencyProperty.Register("VoiceGender", typeof(VoiceGender), typeof(SpeechInputBox), new PropertyMetadata(VoiceGender.Female));

        #endregion

        #region Events

        public event EventHandler TextChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the text.
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            private set { SetValue(TextProperty, value); }
        }

        public List<ISpeechRecognitionConstraint> Constraints
        {
            get { return constraints; }
            set { constraints = value; }
        }

        public string Question
        {
            get { return (string)GetValue(QuestionProperty); }
            set { SetValue(QuestionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the button background brush.
        /// </summary>
        public Brush ButtonBackground
        {
            get { return (Brush)GetValue(ButtonBackgroundProperty); }
            set { SetValue(ButtonBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the highlight brush.
        /// </summary>
        public Brush Highlight
        {
            get { return (Brush)GetValue(HighlightProperty); }
            set { SetValue(HighlightProperty, value); }
        }

        /// <summary>
        /// Gets or sets the voice gender.
        /// </summary>
        public VoiceGender VoiceGender
        {
            get { return (VoiceGender)GetValue(VoiceGenderProperty); }
            set { SetValue(VoiceGenderProperty, value); }
        }

        private MediaElement MediaElement
        {
            get
            {
                if (this.mediaElement == null)
                {
                    this.mediaElement = this.GetTemplateChild(MediaElementPartName) as MediaElement;
                }

                return this.mediaElement;
            }
        }

        private FrameworkElement DefaultState
        {
            get
            {
                if (this.defaultState == null)
                {
                    this.defaultState = this.GetTemplateChild(DefaultStatePartName) as FrameworkElement;
                }

                return this.defaultState;
            }
        }

        private FrameworkElement TextState
        {
            get
            {
                if (this.textState == null)
                {
                    this.textState = this.GetTemplateChild(TextStatePartName) as FrameworkElement;
                }

                return this.textState;
            }
        }

        private FrameworkElement ListeningState
        {
            get
            {
                if (this.listeningState == null)
                {
                    this.listeningState = this.GetTemplateChild(ListeningStatePartName) as FrameworkElement;
                }

                return this.listeningState;
            }
        }

        private FrameworkElement ThinkingState
        {
            get
            {
                if (this.thinkingState == null)
                {
                    this.thinkingState = this.GetTemplateChild(ThinkingStatePartName) as FrameworkElement;
                }

                return this.thinkingState;
            }
        }

        private TextBlock TextBlock
        {
            get
            {
                if (this.textBlock == null)
                {
                    this.textBlock = this.GetTemplateChild(TextBlockPartName) as TextBlock;
                }

                return this.textBlock;
            }
        }

        private TextBox TextBox
        {
            get
            {
                if (this.textBox == null)
                {
                    this.textBox = this.GetTemplateChild(TextBoxPartName) as TextBox;
                }

                return this.textBox;
            }
        }

        private Button MicrophoneButton
        {
            get
            {
                if (this.microphoneButton == null)
                {
                    this.microphoneButton = this.GetTemplateChild(MicrophoneButtonPartName) as Button;
                }

                return this.microphoneButton;
            }
        }

        private Button ListeningButton
        {
            get
            {
                if (this.listeningButton == null)
                {
                    this.listeningButton = this.GetTemplateChild(ListeningButtonPartName) as Button;
                }

                return this.listeningButton;
            }
        }

        private Button ThinkingButton
        {
            get
            {
                if (this.thinkingButton == null)
                {
                    this.thinkingButton = this.GetTemplateChild(ThinkingButtonPartName) as Button;
                }
                return this.thinkingButton;
            }
        }

        #endregion

        protected override async void OnApplyTemplate()
        {
            this.Highlight = new SolidColorBrush(Colors.Red);

            this.TextBlock.Tapped += this.TextBlock_Tapped;
            this.TextBlock.KeyUp += this.TextBlock_KeyUp;
            this.MicrophoneButton.Click += this.Microphone_Tapped;
            this.ListeningButton.Click += this.Listening_Tapped;
            this.ThinkingButton.Click += this.Thinking_Tapped;

            await this.SetState(SpeechInputBoxState.Default);

            base.OnApplyTemplate();
        }

        /// <summary>
        /// Starts ... listening.
        /// </summary>
        public void StartListening()
        {
            Dispatcher.RunAsync(
                Windows.UI.Core.CoreDispatcherPriority.Normal,
                new DispatchedHandler(async () => await this.SetState(SpeechInputBoxState.Listening)));
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == TextProperty)
            {
                var that = d as SpeechInputBox;
                that.TextBlock.Text = e.NewValue.ToString();
                that.TextBox.Text = e.NewValue.ToString();
                if (that.TextChanged != null)
                {
                    that.TextChanged(that, EventArgs.Empty);
                }
            }
        }

        private static void OnQuestionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == QuestionProperty)
            {
                var that = d as SpeechInputBox;
                that.TextBlock.Text = e.NewValue.ToString();
            }
        }

        /// <summary>
        /// Start text input.
        /// </summary>
        private async void TextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await this.SetState(SpeechInputBoxState.Text);
        }

        /// <summary>
        /// Stop input when user hits enter key.
        /// </summary>
        private async void TextBlock_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Accept || e.Key == VirtualKey.Enter)
            {
                this.Text = this.TextBox.Text;
                await this.SetState(SpeechInputBoxState.Default);
            }
        }

        /// <summary>
        /// Start listening.
        /// </summary>
        private async void Microphone_Tapped(object sender, RoutedEventArgs e)
        {
            await this.SetState(SpeechInputBoxState.Listening);
        }

        /// <summary>
        /// Start thinking.
        /// </summary>
        private async void Listening_Tapped(object sender, RoutedEventArgs e)
        {
            await this.SetState(SpeechInputBoxState.Thinking);
        }

        /// <summary>
        /// Cancel thinking.
        /// </summary>
        private async void Thinking_Tapped(object sender, RoutedEventArgs e)
        {
            this.MediaElement.Source = new Uri("ms-appx:///Assets//Cancelled.wav");
            var loader = new ResourceLoader();
            this.Text = loader.GetString("Cancelled");
            await this.SetState(SpeechInputBoxState.Default);
        }

        /// <summary>
        /// Move to a new state.
        /// </summary>
        private async Task SetState(SpeechInputBoxState state)
        {
            this.state = state;
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new DispatchedHandler(
               async () =>
               {
                   // Hide all.
                   this.DefaultState.Visibility = Visibility.Collapsed;
                   this.TextState.Visibility = Visibility.Collapsed;
                   this.ListeningState.Visibility = Visibility.Collapsed;
                   this.ThinkingState.Visibility = Visibility.Collapsed;

                   switch (this.state)
                   {
                       case SpeechInputBoxState.Default:
                           this.DefaultState.Visibility = Visibility.Visible;
                           break;
                       case SpeechInputBoxState.Text:
                           this.TextState.Visibility = Visibility.Visible;
                           break;
                       case SpeechInputBoxState.Listening:
                           this.ListeningState.Visibility = Visibility.Visible;
                           this.MediaElement.Source = new Uri("ms-appx:///Assets//Listening.wav");
                           SpeechRecognizer recognizer = new SpeechRecognizer();

                           foreach (var constraint in this.Constraints)
                           {
                               recognizer.Constraints.Add(constraint);
                           }

                           await recognizer.CompileConstraintsAsync();

                           var reco = recognizer.RecognizeAsync();
                           reco.Completed += this.SpeechRecognition_Completed;
                           break;
                       case SpeechInputBoxState.Thinking:
                           this.ThinkingState.Visibility = Visibility.Visible;
                           break;
                       default:
                           break;
                   }
               }));
        }

        /// <summary>
        /// Speech recognition completed.
        /// </summary>
        private async void SpeechRecognition_Completed(IAsyncOperation<SpeechRecognitionResult> asyncInfo, AsyncStatus asyncStatus)
        {
            await this.SetState(SpeechInputBoxState.Thinking);

            var hadException = false;

            try
            {
                var results = asyncInfo.GetResults();

                if (results.Confidence != SpeechRecognitionConfidence.Rejected)
                {
                    var synthesizer = new SpeechSynthesizer();
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new DispatchedHandler(
                      () => { synthesizer.Voice = this.FindVoice(); }));

                    var stream = synthesizer.SynthesizeTextToStreamAsync(results.Text);

                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new DispatchedHandler(
                      () => { this.Text = results.Text; }));

                    stream.Completed += SpeechSynthesis_Completed;

                    return;
                }
                else
                {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new DispatchedHandler(
                      () =>
                      {
                          this.MediaElement.Source = new Uri("ms-appx:///Assets//Cancelled.wav");
                          var loader = new ResourceLoader();
                          this.Text = loader.GetString("NotUnderstood");
                      }));
                    await this.SetState(SpeechInputBoxState.Default);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                hadException = true;
            }

            if (hadException)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new DispatchedHandler(
                   () =>
                   {
                       this.MediaElement.Source = new Uri("ms-appx:///Assets//Cancelled.wav");
                       var loader = new ResourceLoader();
                       this.Text = loader.GetString("NotUnderstood");
                   }));
                await this.SetState(SpeechInputBoxState.Default);
            }
        }

        private VoiceInformation FindVoice()
        {
            var voices = SpeechSynthesizer.AllVoices;
            foreach (var voice in voices)
            {
                // Temp
                Debug.WriteLine("{0}/{1}", voice.Language, CultureInfo.CurrentUICulture.Name);

                if (voice.Gender == this.VoiceGender)
                {
                    if (voice.Language == CultureInfo.CurrentUICulture.Name)
                    {
                        return voice;
                    }
                }
            }

            // Nothing appropriate found.
            return voices.First();
        }

        /// <summary>
        /// Speech synthesis was completed.
        /// </summary>
        private async void SpeechSynthesis_Completed(IAsyncOperation<SpeechSynthesisStream> asyncInfo, AsyncStatus asyncStatus)
        {
            await this.SetState(SpeechInputBoxState.Default);

            var results = asyncInfo.GetResults();

            // Play the result, on the UI thread.
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new DispatchedHandler(
                () => { this.MediaElement.SetSource(results, results.ContentType); }));
        }
    }
}

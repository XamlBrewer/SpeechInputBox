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
    public sealed class SpeechDialogBox : Control
    {
        #region Constants

        // Style Template Parts
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

        // Text related default values.
        private const string DefaultText = "";
        private const string DefaultQuestion = "Ask me anything ...";
        private const string DefaultResponsePattern = "{0}";

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

        private SpeechDialogBoxState state = SpeechDialogBoxState.Default;
        private List<ISpeechRecognitionConstraint> constraints = new List<ISpeechRecognitionConstraint>() { new SpeechRecognitionTopicConstraint(SpeechRecognitionScenario.Dictation, "Development") };

        #endregion

        #region Constructors
        public SpeechDialogBox()
        {
            this.DefaultStyleKey = typeof(SpeechDialogBox);
        }

        #endregion

        #region Dependency Property Registrations

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("TextProperty", typeof(string), typeof(SpeechDialogBox), new PropertyMetadata(DefaultText, OnTextChanged));

        public static readonly DependencyProperty QuestionProperty =
            DependencyProperty.Register("Question", typeof(string), typeof(SpeechDialogBox), new PropertyMetadata(DefaultQuestion, OnQuestionChanged));

        public static readonly DependencyProperty ResponsePatternProperty =
            DependencyProperty.Register("ResponsePattern", typeof(string), typeof(SpeechDialogBox), new PropertyMetadata("{0}"));

        public static readonly DependencyProperty ButtonBackgroundProperty =
            DependencyProperty.Register("ButtonBackground", typeof(Brush), typeof(SpeechDialogBox), new PropertyMetadata(new SolidColorBrush(Colors.DimGray)));

        public static readonly DependencyProperty HighlightProperty =
            DependencyProperty.Register("Highlight", typeof(Brush), typeof(SpeechDialogBox), new PropertyMetadata(new SolidColorBrush(Colors.OrangeRed)));

        public static readonly DependencyProperty VoiceGenderProperty =
            DependencyProperty.Register("VoiceGender", typeof(VoiceGender), typeof(SpeechDialogBox), new PropertyMetadata(VoiceGender.Female));

        #endregion

        #region Events

        public event EventHandler TextChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
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

        public string ResponsePattern
        {
            get { return (string)GetValue(ResponsePatternProperty); }
            set { SetValue(ResponsePatternProperty, value); }
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

        /// <summary>
        /// Resets this instance to the default text settings.
        /// </summary>
        public async Task Reset()
        {
            // Allow me to start speaking.
            await Task.Delay(200); 

            await this.SetState(SpeechDialogBoxState.Default);
            this.Text = DefaultText;
            var loader = new ResourceLoader();
            this.Question = loader.GetString("Question");
            this.ResponsePattern = DefaultResponsePattern;
            this.Constraints.Clear();
        }

        /// <summary>
        /// Speaks the current text in the current voice and with the current ResponseTemplate.
        /// </summary>
        public async Task Speak()
        {
            this.state = SpeechDialogBoxState.Speaking;

            string currentText = string.Empty;
            var synthesizer = new SpeechSynthesizer();
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new DispatchedHandler(
              () =>
              {
                  synthesizer.Voice = this.FindVoice();
                  currentText = string.Format(this.ResponsePattern, this.Text);
              }));

            var stream = synthesizer.SynthesizeTextToStreamAsync(currentText);
            stream.Completed += SpeechSynthesis_Completed;
        }

        /// <summary>
        /// Enters listening mode.
        /// </summary>
        public void StartListening()
        {
            Dispatcher.RunAsync(
                Windows.UI.Core.CoreDispatcherPriority.Normal,
                new DispatchedHandler(async () => await this.SetState(SpeechDialogBoxState.Listening)));
        }

        /// <summary>
        /// Called when the style template is applied.
        /// </summary>
        protected override async void OnApplyTemplate()
        {
            this.Highlight = new SolidColorBrush(Colors.Red);

            this.TextBlock.Tapped += this.TextBlock_Tapped;
            this.TextBox.KeyUp += this.TextBox_KeyUp;
            this.TextBox.LostFocus += this.TextBox_LostFocus;
            this.MicrophoneButton.Click += this.Microphone_Tapped;
            this.ListeningButton.Click += this.Listening_Tapped;
            this.ThinkingButton.Click += this.Thinking_Tapped;

            await this.SetState(SpeechDialogBoxState.Default);

            base.OnApplyTemplate();
        }

        /// <summary>
        /// Updates the Text property.
        /// </summary>
        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == TextProperty)
            {
                var that = d as SpeechDialogBox;
                that.TextBlock.Text = e.NewValue.ToString();
                that.TextBox.Text = e.NewValue.ToString();
                if (that.TextChanged != null)
                {
                    that.TextChanged(that, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Updates the visible text, without modifying the Text property.
        /// </summary>
        private static void OnQuestionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == QuestionProperty)
            {
                var that = d as SpeechDialogBox;
                that.TextBlock.Text = e.NewValue.ToString();
            }
        }

        /// <summary>
        /// Start text input.
        /// </summary>
        private async void TextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await this.SetState(SpeechDialogBoxState.Text);
        }

        /// <summary>
        /// Stop input when user hits enter key.
        /// </summary>
        private async void TextBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Accept || e.Key == VirtualKey.Enter)
            {
                this.Text = this.TextBox.Text;
                await this.SetState(SpeechDialogBoxState.Default);
            }
        }

        /// <summary>
        /// Handles the LostFocus event of the TextBox control.
        /// </summary>
        private async void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            this.Text = this.TextBox.Text;
            await this.SetState(SpeechDialogBoxState.Default);
        }

        /// <summary>
        /// Start listening.
        /// </summary>
        private async void Microphone_Tapped(object sender, RoutedEventArgs e)
        {
            await this.SetState(SpeechDialogBoxState.Listening);
        }

        /// <summary>
        /// Start thinking.
        /// </summary>
        private async void Listening_Tapped(object sender, RoutedEventArgs e)
        {
            await this.SetState(SpeechDialogBoxState.Thinking);
        }

        /// <summary>
        /// Cancel thinking.
        /// </summary>
        private async void Thinking_Tapped(object sender, RoutedEventArgs e)
        {
            this.MediaElement.Source = new Uri("ms-appx:///Assets//Cancelled.wav");
            var loader = new ResourceLoader();
            this.Text = loader.GetString("Cancelled");
            await this.SetState(SpeechDialogBoxState.Default);
        }

        /// <summary>
        /// Move to a new state.
        /// </summary>
        private async Task SetState(SpeechDialogBoxState state)
        {
            // Do not interrupt while speaking.
            while (this.state == SpeechDialogBoxState.Speaking)
            {
                await Task.Delay(200);
            }

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
                       case SpeechDialogBoxState.Default:
                           this.DefaultState.Visibility = Visibility.Visible;
                           break;
                       case SpeechDialogBoxState.Text:
                           this.TextState.Visibility = Visibility.Visible;
                           break;
                       case SpeechDialogBoxState.Listening:
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
                       case SpeechDialogBoxState.Thinking:
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
            await this.SetState(SpeechDialogBoxState.Thinking);

            var hadException = false;

            try
            {
                var results = asyncInfo.GetResults();

                if (results.Confidence != SpeechRecognitionConfidence.Rejected)
                {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new DispatchedHandler(
                        () => { this.Text = results.Text; }));

                    await this.Speak();

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
                    await this.SetState(SpeechDialogBoxState.Default);
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
                await this.SetState(SpeechDialogBoxState.Default);
            }
        }

        private VoiceInformation FindVoice()
        {
            var voices = SpeechSynthesizer.AllVoices;
            foreach (var voice in voices)
            {
                if (voice.Gender == this.VoiceGender)
                {
                    if (voice.Language == CultureInfo.CurrentUICulture.Name)
                    {
                        return voice;
                    }
                }
            }

            // No appropriate voice found.
            return voices.First();
        }

        /// <summary>
        /// Speech synthesis was completed.
        /// </summary>
        private async void SpeechSynthesis_Completed(IAsyncOperation<SpeechSynthesisStream> asyncInfo, AsyncStatus asyncStatus)
        {
            var results = asyncInfo.GetResults();

            // Play the result, on the UI thread.
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new DispatchedHandler(
                () =>
                {
                    this.MediaElement.SetSource(results, results.ContentType);
                    this.MediaElement.CurrentStateChanged += this.MediaElement_CurrentStateChanged;
                }));
        }

        private async void MediaElement_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            if (this.MediaElement.CurrentState == MediaElementState.Paused ||
                this.MediaElement.CurrentState == MediaElementState.Stopped ||
                this.MediaElement.CurrentState == MediaElementState.Closed)
            {
                this.state = SpeechDialogBoxState.Default;
                await this.SetState(SpeechDialogBoxState.Default);
                this.MediaElement.CurrentStateChanged -= this.MediaElement_CurrentStateChanged;
            }
        }
    }
}

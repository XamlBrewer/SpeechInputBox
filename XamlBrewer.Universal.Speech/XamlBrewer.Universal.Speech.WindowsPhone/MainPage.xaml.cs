﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechSynthesis;
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

        private void VoiceButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.SpeechInputBox.VoiceGender == VoiceGender.Female)
            {
                this.SpeechInputBox.VoiceGender = VoiceGender.Male;
            }
            else
            {
                this.SpeechInputBox.VoiceGender = VoiceGender.Female;
            }
        }

        private void ConstraintsButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

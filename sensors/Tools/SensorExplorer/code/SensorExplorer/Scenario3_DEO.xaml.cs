﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Windows.UI.Core;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;

namespace SensorExplorer
{
    public sealed partial class Scenario3DEO : Page
    {
        public static Scenario3DEO Scenario3;

        private DisplayEnhancementOverride deo;
        private IReadOnlyList<NitRange> supportedNitRange;

        public Scenario3DEO()
        {
            InitializeComponent();
            Scenario3 = this;

            deo = DisplayEnhancementOverride.GetForCurrentView();

            SetNoBrightnessSettings();
            SetNoColorScenario();

            deo.IsOverrideActiveChanged += Deo_IsOverrideActiveChanged;
            deo.CanOverrideChanged += Deo_CanOverrideChanged;
            deo.DisplayEnhancementOverrideCapabilitiesChanged += Deo_DisplayEnhancementOverrideCapabilitiesChanged;

            comboBoxPercentage.ItemsSource = new List<string>() { "Slider", "Specific value" };
            comboBoxPercentage.SelectionChanged += OnSelectionChangedPercentage;

            comboBoxNits.ItemsSource = new List<string>() { "Slider", "Specific value" };
            comboBoxNits.SelectionChanged += OnSelectionChangedNits;

            PeriodicTimer.CreateScenario3();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            deo.StopOverride();

            deo.IsOverrideActiveChanged -= Deo_IsOverrideActiveChanged;
            deo.CanOverrideChanged -= Deo_CanOverrideChanged;
            deo.DisplayEnhancementOverrideCapabilitiesChanged -= Deo_DisplayEnhancementOverrideCapabilitiesChanged;
        }

        private void OnSelectionChangedPercentage(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxPercentage.SelectedItem != null)
            {
                string selected = comboBoxPercentage.SelectedItem.ToString();
                textBlockPercentageInputError.Visibility = Visibility.Collapsed;

                if (selected == "Slider")
                {
                    percentageBrightnessSlider.Visibility = Visibility.Visible;
                    stackPanelPercentageBrightness.Visibility = Visibility.Collapsed;
                }
                else if (selected == "Specific value")
                {
                    percentageBrightnessSlider.Visibility = Visibility.Collapsed;
                    stackPanelPercentageBrightness.Visibility = Visibility.Visible;
                }
            }
        }

        private void OnSelectionChangedNits(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxNits.SelectedItem != null)
            {
                string selected = comboBoxNits.SelectedItem.ToString();
                textBlockNitsInputError.Visibility = Visibility.Collapsed;

                if (selected == "Slider")
                {
                    nitsBrightnessSlider.Visibility = Visibility.Visible;
                    stackPanelNitsBrightness.Visibility = Visibility.Collapsed;
                }
                else if (selected == "Specific value")
                {
                    nitsBrightnessSlider.Visibility = Visibility.Collapsed;
                    stackPanelNitsBrightness.Visibility = Visibility.Visible;
                }
            }
        }

        #region DEO Callbacks   

        private async void Deo_DisplayEnhancementOverrideCapabilitiesChanged(DisplayEnhancementOverride sender, DisplayEnhancementOverrideCapabilitiesChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                supportedNitRange = args.Capabilities.GetSupportedNitRanges();

                brightnessPercentageSupportedStateTextBlock.Text = args.Capabilities.IsBrightnessControlSupported ? "Yes" : "No";
                brightnessNitsSupportedStateTextBlock.Text = args.Capabilities.IsBrightnessNitsControlSupported ? "Yes" : "No";

                stackPanelComboBoxPercentage.Visibility = args.Capabilities.IsBrightnessControlSupported ? Visibility.Visible : Visibility.Collapsed;
                textBlockPercentageBrightnessSettings.Visibility = args.Capabilities.IsBrightnessControlSupported ? Visibility.Collapsed : Visibility.Visible;

                stackPanelComboBoxNits.Visibility = args.Capabilities.IsBrightnessNitsControlSupported ? Visibility.Visible : Visibility.Collapsed;
                textBlockNitsBrightnessSettings.Visibility = args.Capabilities.IsBrightnessNitsControlSupported ? Visibility.Collapsed: Visibility.Visible;

                if (supportedNitRange.Count >= 1)
                {
                    textBlockNitsInputError.Text = "Please enter a number between " + supportedNitRange[0].MinNits + " and " + supportedNitRange[supportedNitRange.Count - 1].MaxNits + ".";
                    nitsBrightnessSlider.Minimum = supportedNitRange[0].MinNits;
                    nitsBrightnessSlider.Maximum = supportedNitRange[supportedNitRange.Count - 1].MaxNits;
                }
                else
                {
                    textBlockNitsInputError.Text = "No nit ranges on nits system!";
                }
            });
        }

        private async void Deo_CanOverrideChanged(DisplayEnhancementOverride sender, object args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                canOverrideActiveStateTextBlock.Text = sender.CanOverride ? "Yes" : "No";
            });
        }

        private async void Deo_IsOverrideActiveChanged(DisplayEnhancementOverride sender, object args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                isOverrideActiveStateTextBlock.Text = sender.IsOverrideActive ? "Yes" : "No";
            });
        }

        #endregion // DEO Callbacks

        #region Brightness Settings

        private async void SetBrightnessPercentage(double level)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                brightnessSettingStateTextBlock.Text = level + "%";
            });

            deo.BrightnessOverrideSettings = BrightnessOverrideSettings.CreateFromLevel(level / 100);
            CheckOverrideToggleEnableState();
        }

        private async void SetBrightnessNits(float nits)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                brightnessSettingStateTextBlock.Text = nits + " nits";
            });

            deo.BrightnessOverrideSettings = BrightnessOverrideSettings.CreateFromNits(nits);
            CheckOverrideToggleEnableState();
        }

        private async void SetBrightnessScenario(DisplayBrightnessOverrideScenario scenario)
        {
            string scenarioText = "";
            switch (scenario)
            {
                case DisplayBrightnessOverrideScenario.FullBrightness:
                    scenarioText = "Full Brightness";
                    break;
                case DisplayBrightnessOverrideScenario.BarcodeReadingBrightness:
                    scenarioText = "Barcode Brightness";
                    break;
                case DisplayBrightnessOverrideScenario.IdleBrightness:
                    scenarioText = "Idle Brightness";
                    break;
            }

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                brightnessSettingStateTextBlock.Text = scenarioText;
            });

            deo.BrightnessOverrideSettings = BrightnessOverrideSettings.CreateFromDisplayBrightnessOverrideScenario(scenario);
            CheckOverrideToggleEnableState();
        }

        private async void SetNoBrightnessSettings()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                brightnessSettingStateTextBlock.Text = "None";
            });

            deo.BrightnessOverrideSettings = null;
            CheckOverrideToggleEnableState();
        }

        public void BrightnessLevelChanged()
        {
            try
            {
                BrightnessOverride bo = BrightnessOverride.GetForCurrentView();
                double value = bo.GetLevelForScenario(DisplayBrightnessScenario.DefaultBrightness);
                currentBrightnessPercentageValueTextBlock.Text = value * 100 + " %";
            }
            catch { }
        }

        #endregion // Brightness Settings

        #region Color Settings

        private async void SetColorScenario(DisplayColorOverrideScenario scenario)
        {
            string scenarioText = "";
            switch (scenario)
            {
                case DisplayColorOverrideScenario.Accurate:
                    scenarioText = "Accurate Colors";
                    break;
            }

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                colorSettingStateTextBlock.Text = scenarioText;
            });

            deo.ColorOverrideSettings = ColorOverrideSettings.CreateFromDisplayColorOverrideScenario(DisplayColorOverrideScenario.Accurate);
            CheckOverrideToggleEnableState();
        }

        private async void SetNoColorScenario()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                colorSettingStateTextBlock.Text = "None";
            });

            deo.ColorOverrideSettings = null;
            CheckOverrideToggleEnableState();
        }

        #endregion // Color Settings

        #region Brightness Controls

        private void PercentageBrightnessSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SetBrightnessPercentage(e.NewValue);
        }

        private void ButtonPercentageBrightness_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double percentage = Convert.ToDouble(textBoxPercentageBrightness.Text);
                
                if(percentage >= 0 && percentage <= 100)
                {
                    SetBrightnessPercentage(percentage);
                    textBlockPercentageInputError.Visibility = Visibility.Collapsed;
                }
                else
                {
                    textBlockPercentageInputError.Visibility = Visibility.Visible;
                }
            }
            catch
            {
                textBlockPercentageInputError.Visibility = Visibility.Visible;
            }
        }

        private void NitsBrightnessSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SetBrightnessNits((float) e.NewValue);
        }

        private void ButtonNitsBrightness_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                float nits = (float) Convert.ToDouble(textBoxNitsBrightness.Text);

                if (supportedNitRange.Count >= 1)
                {
                    if (nits >= supportedNitRange[0].MinNits && nits <= supportedNitRange[supportedNitRange.Count - 1].MaxNits)
                    {
                        SetBrightnessNits(nits);
                        textBlockNitsInputError.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        textBlockNitsInputError.Visibility = Visibility.Visible;
                        textBlockNitsInputError.Text = "Please enter a number between " + supportedNitRange[0].MinNits + " and " + supportedNitRange[supportedNitRange.Count - 1].MaxNits + ".";
                    }
                }
            }
            catch
            {
                textBlockNitsInputError.Visibility = Visibility.Visible;
            }
        }

        private void FullBrightnessScenarioButton_Click(object sender, RoutedEventArgs e)
        {
            SetBrightnessScenario(DisplayBrightnessOverrideScenario.FullBrightness);
        }

        private void BarcodeBrightnessScenarioButton_Click(object sender, RoutedEventArgs e)
        {
            SetBrightnessScenario(DisplayBrightnessOverrideScenario.BarcodeReadingBrightness);
        }

        private void IdleBrightnessScenarioButton_Click(object sender, RoutedEventArgs e)
        {
            SetBrightnessScenario(DisplayBrightnessOverrideScenario.IdleBrightness);
        }

        private void NoneBrightnessScenarioButton_Click(object sender, RoutedEventArgs e)
        {
            SetNoBrightnessSettings();
        }

        #endregion // Brightness Controls

        #region Color Controls

        private void AccurateColorScenarioButton_Click(object sender, RoutedEventArgs e)
        {
            SetColorScenario(DisplayColorOverrideScenario.Accurate);
        }

        private void NonColorScenarioButton_Click(object sender, RoutedEventArgs e)
        {
            SetNoColorScenario();
        }

        #endregion // Color Controls

        #region General Controls

        private async void CheckOverrideToggleEnableState()
        {
            string debuggerText = "";

            if ((deo.ColorOverrideSettings != null) || (deo.BrightnessOverrideSettings != null))
            {
                debuggerText = "";
                overrideToggle.IsEnabled = true;
            }
            else if (!overrideToggle.IsOn)
            {
                debuggerText = "Please select a brightness or color setting before requesting an override";
                overrideToggle.IsEnabled = false;
            }

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                debugTextBlock.Text = debuggerText;
            });
        }

        private void OverrideToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                if (toggleSwitch.IsOn)
                {
                    deo.RequestOverride();
                }
                else
                {
                    CheckOverrideToggleEnableState();
                    deo.StopOverride();
                }
            }
        }

        #endregion // General Controls
    }
}
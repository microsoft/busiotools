using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Windows.UI.Core;

namespace SensorExplorer
{
    using Windows.Graphics.Display;

    public sealed partial class Scenario3DEO : Page
    {
        private DisplayEnhancementOverride deo;
        public static Scenario3DEO Scenario3;

        public Scenario3DEO()
        {
            this.InitializeComponent();

            deo = DisplayEnhancementOverride.GetForCurrentView();

            SetNoBrightnessSettings();
            SetNoColorScenario();

            deo.IsOverrideActiveChanged += Deo_IsOverrideActiveChanged;
            deo.CanOverrideChanged += Deo_CanOverrideChanged;
            deo.DisplayEnhancementOverrideCapabilitiesChanged += Deo_DisplayEnhancementOverrideCapabilitiesChanged;
        }

        #region DEO Callbacks   

        private void Deo_DisplayEnhancementOverrideCapabilitiesChanged(DisplayEnhancementOverride sender, DisplayEnhancementOverrideCapabilitiesChangedEventArgs args)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                BrightnessPercentageSupportedStateTextBlock.Text = args.Capabilities.IsBrightnessControlSupported ? "Yes" : "No";
                BrightnessNitsSupportedStateTextBlock.Text = args.Capabilities.IsBrightnessNitsControlSupported ? "Yes" : "No";
            });
        }

        private void Deo_CanOverrideChanged(DisplayEnhancementOverride sender, object args)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                CanOverrideActiveStateTextBlock.Text = sender.CanOverride ? "Yes" : "No";
            });
        }

        private void Deo_IsOverrideActiveChanged(DisplayEnhancementOverride sender, object args)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                IsOverrideActiveStateTextBlock.Text = sender.IsOverrideActive ? "Yes" : "No";
            });
        }

        #endregion // DEO Callbacks

        #region Brightness Settings

        private void SetBrightnessPercentage(double level)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                BrightnessSettingStateTextBlock.Text = level + "%";
            });

            deo.BrightnessOverrideSettings = BrightnessOverrideSettings.CreateFromLevel(level / 100);
            CheckOverrideToggleEnableState();
        }

        private void SetBrightnessNits(float nits)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                BrightnessSettingStateTextBlock.Text = nits + " nits";
            });

            deo.BrightnessOverrideSettings = BrightnessOverrideSettings.CreateFromNits(nits);
            CheckOverrideToggleEnableState();
        }

        private void SetBrightnessScenario(DisplayBrightnessOverrideScenario scenario)
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

            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                BrightnessSettingStateTextBlock.Text = scenarioText;
            });

            deo.BrightnessOverrideSettings = BrightnessOverrideSettings.CreateFromDisplayBrightnessOverrideScenario(scenario);
            CheckOverrideToggleEnableState();
        }

        private void SetNoBrightnessSettings()
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                BrightnessSettingStateTextBlock.Text = "None";
            });

            deo.BrightnessOverrideSettings = null;
            CheckOverrideToggleEnableState();
        }

        #endregion // Brightness Settings

        #region Color Settings

        private void SetColorScenario(DisplayColorOverrideScenario scenario)
        {
            string scenarioText = "";
            switch (scenario)
            {
                case DisplayColorOverrideScenario.Accurate:
                    scenarioText = "Accurate Colors";
                    break;
            }

            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ColorSettingStateTextBlock.Text = scenarioText;
            });

            deo.ColorOverrideSettings = ColorOverrideSettings.CreateFromDisplayColorOverrideScenario(DisplayColorOverrideScenario.Accurate);
            CheckOverrideToggleEnableState();
        }

        private void SetNoColorScenario()
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ColorSettingStateTextBlock.Text = "None";
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

        private void CheckOverrideToggleEnableState()
        {
            string debuggerText = "";

            if (((deo.ColorOverrideSettings != null) || (deo.BrightnessOverrideSettings != null)))
            {
                debuggerText = "";
                OverrideToggle.IsEnabled = true;
            }
            else if (!OverrideToggle.IsOn)
            {
                debuggerText = "Please select a brightness or color setting before requesting an override";
                OverrideToggle.IsEnabled = false;
            }

            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                DebugTextBlock.Text = debuggerText;
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

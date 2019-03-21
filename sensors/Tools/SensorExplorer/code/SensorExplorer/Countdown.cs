// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Windows.UI.Xaml;

namespace SensorExplorer
{
    class Countdown
    {
        private DispatcherTimer countdownTimer;
        private int remainingTime;
        private string testType;

        public Countdown(int sec, string testType)
        {
            remainingTime = sec;
            this.testType = testType;

            countdownTimer = new DispatcherTimer();
            countdownTimer.Tick += Tick;
            countdownTimer.Interval = TimeSpan.FromMilliseconds(1000);
            countdownTimer.Start();
        }

        private void Tick(object sender, object e)
        {
            Scenario0Tests.Scenario0.DisplayCountdown(remainingTime);
            remainingTime--;

            if (remainingTime < 0)
            {
                countdownTimer.Stop();
                if (testType == "Orientation")
                {
                    Scenario0Tests.Scenario0.Failed();
                }
                else
                {
                    Scenario0Tests.Scenario0.TestEnd();
                }
            }
            else if (Scenario0Tests.Scenario0.IsSimpleOrientationSensor)    
            {
                // check the current simple orientation every second
                Scenario0Tests.Scenario0.CurrentSimpleOrientation();    
            }
        }

        public void Stop()
        {
            countdownTimer.Stop();
        }
    }
}
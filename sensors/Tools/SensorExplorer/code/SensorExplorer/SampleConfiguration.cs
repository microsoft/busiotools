// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace SensorExplorer
{
    public partial class MainPage : Page
    {
        private List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Tests", ClassType = typeof(Scenario0Tests) },
            new Scenario() { Title = "View", ClassType = typeof(Scenario1View) },
            new Scenario() { Title = "MALT", ClassType = typeof(Scenario2MALT) },
            new Scenario() { Title = "Display Enhancement Override", ClassType = typeof(Scenario3DEO) },
        };
    }

    public class Scenario
    {
        public string Title { get; set; }
        public Type ClassType { get; set; }
    }
}

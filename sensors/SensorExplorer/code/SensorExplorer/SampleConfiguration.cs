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
            new Scenario() { Title = "Source", ClassType = typeof(Scenario3Source) }
        };
    }

    public class Scenario
    {
        public string Title { get; set; }
        public Type ClassType { get; set; }
    }
}

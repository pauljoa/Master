using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Geared;
using LiveCharts.Wpf;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace Visualizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SeriesCollection = new SeriesCollection();
        }

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }

        private void SelectFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if((bool)dialog.ShowDialog())
            {
                SeriesCollection.Clear();
                var modelLog = File.ReadAllText(dialog.FileName);
                List<Snapshot> snapshots = JsonConvert.DeserializeObject<List<Snapshot>>(modelLog);
                // Dictionary<String, bool> properties = new Dictionary<string, bool>();
                Dictionary<String, List<double>> series = new Dictionary<string, List<double>>();
                //int count = snapshots.Count;
                //int sampleRate = count / 3000;
                foreach (var snap in snapshots)
                {
                    
                    foreach(var key in snap.Snap)
                    {
                        if(series.TryGetValue(key.Name,out List<double> val))
                        {
                            val.Add(key.Value.Value);
                        }
                        else
                        {
                            List<double> serie = new List<double>();
                            serie.Add(key.Value.Value);
                            series.Add(key.Name,serie);
                        }
                    }
                }

                foreach(var valuePair in series)
                {
                    if (valuePair.Key != "Capacity" && valuePair.Key != "MaxRate" && valuePair.Key != "Voltage")
                    {
                        //var test = valuePair.Value.Where((x, i) => i % sampleRate == 0).ToList();
                        SeriesCollection.Add(new GLineSeries()
                        {
                            Title = valuePair.Key,
                            Values = new ChartValues<double>(valuePair.Value)
                        });

                    }
                }
                YFormatter = value => value.ToString("0.##");
                List<String> labels = new List<String>();
                //foreach(var s in snapshots)
                //{
                //    labels.Add(s.Timestamp.ToString());
                //}
                for(var i = 0;i<3000;i++)
                {
                    labels.Add(i.ToString());
                }
                Labels = labels.ToArray();
            }
            DataContext = this;
        }
    }
    public class Snapshot
    {
        [JsonProperty("T")]
        public int Timestamp { get; set; }
        [JsonProperty("S")]
        public dynamic Snap { get; set; }
    }
}

using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;

using CloudRating.Beatmap;
using CloudRating.Processor;

namespace CloudRating
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow
    {
        // ReSharper disable once InconsistentNaming
        private bool isCalculating;

        public MainWindow()
        {
            InitializeComponent();
            isCalculating = false;
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Filter = ".osu files (*.osu)|*.osu",
                RestoreDirectory = true
            };

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                PathText.Text = ofd.FileName;
        }

        private void Calc_Click(object sender, RoutedEventArgs e)
        {
            if (isCalculating)
                return;

            StateBlock.Text = "Calculating...";
            var calcThread = new Thread(CalcRating);
            calcThread.Start();
        }

        private void CalcRating()
        {
            string text = null, output;
            isCalculating = true;

            try
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                {
                    text = PathText.Text;
                }));

                var map = new BeatmapInfo(text);
                var jack = new PatternAnalyzer(map.Notes, map.LNs, map.Data.Keys);
                
                output = map.Data.Artist + " - " + map.Data.Title + " [" + map.Data.Diff + "]\nMade by " + map.Data.Creator
                    + "\nBPM: " + (Math.Abs(map.Data.MaxBpm - map.Data.MinBpm) < 0.001
                                    ? Convert.ToString(map.Data.MaxBpm, CultureInfo.CurrentCulture) 
                                    : map.Data.MinBpm + " - " + map.Data.MaxBpm + "\t")
                    + "\tOD: " + map.Data.Od + "\tHP: " + map.Data.Hp + "\tKeys: " + map.Data.Keys
                    + "\nMax Density: " + map.MaxDen + "\tAverage Density: " + map.AvgDen + "\tJack Raio: " + jack.GetJackRatio() + "%"
#if DEBUG
                    + "\nCorrected Max Density: " + map.CorMaxDen + "\tCorrected Average Density: " + map.CorAvgDen
#endif
                    + "\nRating: " + RatingCalculator.CalcRating(map);
            }
            catch (Exception ex)
            {
                output = ex.Message;
            }

            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                StateBlock.Text = output;
            }));

            isCalculating = false;
        }
    }
}
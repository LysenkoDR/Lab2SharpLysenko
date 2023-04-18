using ClassLibrary;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace WpfAppLab1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ViewData viewData = new();

        public Plot? plot;
        public List<String> RawDataList { get; set; }
        public List<String> SplineDataList { get; set; }

        public static RoutedCommand ViewDataFromControlsCommand = new RoutedCommand("ViewDataFromControls", typeof(WpfAppLab1.MainWindow));
        public static RoutedCommand ViewDataFromFileCommand = new RoutedCommand("ViewDataFromFile", typeof(WpfAppLab1.MainWindow));



        public MainWindow()
        {
            InitializeComponent();

            DataContext = viewData;
            RawDataList = new List<string>();
            SplineDataList = new List<string>();
            cbFunction.ItemsSource = Enum.GetValues(typeof(FRawEnum));
            OxyPlot.DataContext = plot;
        }

        public void ShowToListBox()
        {
            try
            {

                lbRawData.Items.Clear();
                lbSplineData.Items.Clear();

                for (int i = 0; i < viewData.RawData!.NumberGridNodes; i++)
                {
                    lbRawData.Items.Add($"Nodes={viewData.RawData.GridNodes[i]:F3}; Value={viewData.RawData.GridValues[i]:F3}");
                }

                foreach (var item in viewData.SplineData!.ListSplineData)
                {
                    lbSplineData.Items.Add(item.ToString());
                }

                tbIntegral.Text = $"{viewData.SplineData.IntegralValue:F3}";

                //viewData.BindIntegral = $"{viewData.SplineData.IntegralValue:F3}";

                //viewData.BindIntegral = viewData.SplineData.IntegralValue.ToString();


                plot = new Plot(viewData);
                OxyPlot.DataContext = plot;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }


        }



        //private void OnSaveClicked(object sender, RoutedEventArgs e)
        //{
        //    Microsoft.Win32.SaveFileDialog dlg = new()
        //    {
        //        FileName = "RawData",
        //        DefaultExt = ".txt",
        //        Filter = "Text documents (.txt)|*.txt"
        //    };

        //    bool? result = dlg.ShowDialog();

        //    if (result == true)
        //    {
        //        try
        //        {
        //            string filename = dlg.FileName;
        //            //viewData.CalculateSplines();
        //            viewData.Save(filename);
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show(ex.ToString());
        //        }
        //    }
        //}

        private void CanSaveCommandHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            if ((viewData.RawData != null) && (viewData.ValidDataError() == true))
            {
                e.CanExecute = true;
            }
            else 
            { 
                e.CanExecute = false;
            } 
        }
        private void SaveCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new()
            {
                FileName = "RawData",
                DefaultExt = ".txt",
                Filter = "Text documents (.txt)|*.txt"
            };

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                try
                {
                    string filename = dlg.FileName;
                    viewData.Save(filename);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }

        }



        //private void OnViewDataFromFileClicked(object sender, RoutedEventArgs e)
        //{
        //    Microsoft.Win32.OpenFileDialog dlg = new()
        //    {
        //        FileName = "RawData",
        //        DefaultExt = ".txt",
        //        Filter = "Text documents (.txt)|*.txt"
        //    };

        //    bool? result = dlg.ShowDialog();

        //    if (result == true)
        //    {
        //        try
        //        {
        //            string filename = dlg.FileName;
        //            viewData.Load(filename);
        //            //viewData.RawData = new RawData(filename);
        //            ////viewData.CalculateSplines();

        //            viewData.SplineData = new SplineData(viewData.RawData, viewData.BindLeftDeriv, viewData.BindRightDeriv, viewData.BindNumberSplineGridNodes);
        //            viewData.SplineData.MakeMKLSpline();

        //            //viewData.BindLeftAndRightEndPoint[0] = viewData.RawData.LeftEndPoint;
        //            //viewData.BindLeftAndRightEndPoint[1] = viewData.RawData.RightEndPoint;
        //            //viewData.BindNumberGridNodes = viewData.RawData.NumberGridNodes;
        //            //viewData.BindIsUniform = viewData.RawData.IsUniform;

        //            ShowToListBox();

        //            lbRawData.Items.Refresh();
        //            lbSplineData.Items.Refresh();
        //        }
        //        catch(Exception ex)
        //        {
        //            MessageBox.Show(ex.ToString());
        //        }
        //    }
        //}

        private void CanViewDataFromFileCommandHandler(object sender, CanExecuteRoutedEventArgs e)
        {

            if (viewData.ValidDataError() == true)
            {
                e.CanExecute = true;
            }
            else 
            { 
                e.CanExecute = false; 
            }
        }

        private void ViewDataFromFileCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new()
            {
                FileName = "RawData",
                DefaultExt = ".txt",
                Filter = "Text documents (.txt)|*.txt"
            };

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                try
                {
                    string filename = dlg.FileName;
                    viewData.Load(filename);
                    viewData.SplineData = new SplineData(viewData.RawData, viewData.BindLeftDeriv, viewData.BindRightDeriv, viewData.BindNumberSplineGridNodes);
                    viewData.SplineData.MakeMKLSpline();
                    ShowToListBox();

                    lbRawData.Items.Refresh();
                    lbSplineData.Items.Refresh();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }

        }


        //private void OnViewDataFromControlsClicked(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        viewData.CalculateSplines();
        //        ShowToListBox();

        //        lbRawData.Items.Refresh();
        //        lbSplineData.Items.Refresh();
        //    }
        //    catch(Exception ex)
        //    {
        //        MessageBox.Show(ex.ToString());
        //    }

        //}

        private void CanViewDataFromControlsCommandHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            if (viewData.ValidDataError() == true)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }


        }

        private void ViewDataFromControlsCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                viewData.CalculateSplines();
                ShowToListBox();

                lbRawData.Items.Refresh();
                lbSplineData.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void LbSplineData_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            int Temp = lbSplineData.SelectedIndex;
            //viewData.BindSelectedNode = viewData.SplineData!.ListSplineData[Temp].ToString("F3");
            tbSelectedNode.Text = viewData.SplineData!.ListSplineData[Temp].ToString("F3");

        }

    }

    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return Binding.DoNothing;
        }
    }

    public class StringToValuesConverter : IValueConverter
    {
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double[] range)
            {
                return $"{range[0]};{range[1]}";
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string input)
            {
                string[] parts = input.Split(';');
                if (parts.Length == 2 && double.TryParse(parts[0], out double start) && double.TryParse(parts[1], out double end))
                {
                    return new double[] { start, end };
                }
            }
            return new double[] { 0, 0 };
        }
    }


}

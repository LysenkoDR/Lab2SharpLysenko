
using ClassLibrary;
using OxyPlot.Legends;
using OxyPlot.Series;
using OxyPlot;
using System.Collections.Generic;
using OxyPlot.Wpf;
using System.Windows.Documents;
using System.Windows;

namespace WpfAppLab1
{
    public class Plot
    {
        public PlotModel plotModel { get; private set; }

        public ViewData viewData;


        public Plot(ViewData viewData)
        {
            this.viewData = viewData;
            this.plotModel = new PlotModel { };

            plotModel.Series.Clear();

            if (viewData.RawData != null)
            {
                LineSeries rawSeries = new();
                LineSeries pointSeries = new();
                for (int i = 0; i < viewData.RawData.NumberGridNodes; i++)
                {
                    DataPoint dataPoint = new(viewData.RawData.GridNodes[i], viewData.RawData.GridValues[i]);

                    rawSeries.Points.Add(dataPoint);
                    pointSeries.Points.Add(dataPoint);
                }

                pointSeries.Title = "RawDataPoint";
                pointSeries.Color = OxyColors.MediumVioletRed;
                pointSeries.LineStyle = LineStyle.None;
                pointSeries.MarkerType = MarkerType.Diamond;
                pointSeries.MarkerSize = 5;
                pointSeries.MarkerStroke = OxyColors.MediumVioletRed;
                pointSeries.MarkerFill = OxyColors.MediumVioletRed; 

                Legend leg = new();
                this.plotModel.Legends.Add(leg);
                this.plotModel.Series.Add(pointSeries);

                rawSeries.Title = "RawData";
                rawSeries.Color = OxyColors.DarkGreen;
                rawSeries.MarkerSize = 4;

                this.plotModel.Series.Add(rawSeries);
            }
            else
            {
                MessageBox.Show("Ошибка! Нет RawData для графика");
            }

            if (viewData.SplineData != null)
            {
                LineSeries rawSeries = new();
                LineSeries pointSeries = new();
                List<SplineDataItem> listSplineDataDraw = viewData.SplineData.ListSplineData;

                for (int i = 0; i < listSplineDataDraw.Count; i++)
                {
                    SplineDataItem item = listSplineDataDraw[i];

                    DataPoint dataPoint = new(item.Coord, item.Spline);

                    rawSeries.Points.Add(dataPoint);
                    pointSeries.Points.Add(dataPoint);
                }

                pointSeries.Title = "SplineDataPoint";
                pointSeries.Color = OxyColors.DeepPink;
                pointSeries.LineStyle = LineStyle.None;
                pointSeries.MarkerType = MarkerType.Diamond;
                pointSeries.MarkerSize = 5;
                pointSeries.MarkerStroke = OxyColors.DeepPink;
                pointSeries.MarkerFill = OxyColors.DeepPink;

                Legend leg = new();
                this.plotModel.Legends.Add(leg);
                this.plotModel.Series.Add(pointSeries);

                rawSeries.Title = "Spline";
                rawSeries.Color = OxyColors.Aqua;
                rawSeries.MarkerSize = 4;

                this.plotModel.Series.Add(rawSeries);

            }
            else
            {
                MessageBox.Show("Ошибка! Нет SplineData для графика");
            }

        }
    }
}

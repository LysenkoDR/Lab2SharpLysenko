using ClassLibrary;
using OxyPlot.Legends;
using OxyPlot.Wpf;
using System;
using System.ComponentModel;
using System.Windows;

namespace WpfAppLab1
{
    public class ViewData: IDataErrorInfo
    {
        //параметры для объекта RawData
        public double[] BindLeftAndRightEndPoint { get; set; } = new double[2] { 5.0, 10.0 };
        
        public int BindNumberGridNodes { get; set; } = 6;
        public bool BindIsUniform { get; set; } = true;

        public FRawEnum BindFunctionFRawEnum { get; set; } = FRawEnum.Cubic;
        public FRaw F { get; set; }
        //параметры для объекта SplineData
        public int BindNumberSplineGridNodes { get; set; } = 5;
        public double BindLeftDeriv { get; set; } = 75.0;
        public double BindRightDeriv { get; set; } = 300.0;

        public PlotView plotView { get; set; }

        // Ссылки на объекты RawData и SplineData
        public RawData? RawData;
        public SplineData? SplineData { get; set; }

        public string BindSelectedNode { get; set; }

        public string BindIntegral { get; set; }

        public ViewData() { }

        //public ViewData(RawData raw)
        //{
        //    //LeftEndPoint= raw.LeftEndPoint;
        //    //RightEndPoint= raw.RightEndPoint;
        //    BindLeftAndRightEndPoint[0] = raw.LeftEndPoint;
        //    BindLeftAndRightEndPoint[1] = raw.RightEndPoint;


        //    BindNumberGridNodes = raw.NumberGridNodes;
        //    BindIsUniform = raw.IsUniform;
        //    F = raw.F;
        //    //bindFunctionFRawEnum = FRawEnum.Linear;
        //    //NumberSplineGridNodes = 20;
        //    //LeftDeriv = 0;
        //    //RightDeriv = 1;
        //}
        private bool hasNumberGridNodesError = false;
        private bool hasLeftAndRightEndPointError = false;
        private bool hasNumberSplineGridNodesError = false;
        public string this[string columnName]
        {
            get
            {
                string error = string.Empty;
                double LeftEndPoint = BindLeftAndRightEndPoint[0];
                double RightEndPoint = BindLeftAndRightEndPoint[1];

                switch (columnName)
                {
                    case nameof(BindNumberGridNodes):
                        if (BindNumberGridNodes < 2)
                        {
                            error = "Error Node number (value < 2)";
                            MessageBox.Show(error);
                            hasNumberGridNodesError = true;
                        }
                        else
                        {
                            hasNumberGridNodesError = false;
                        }
                        break;
                    case nameof(BindLeftAndRightEndPoint):
                        if ((LeftEndPoint >= RightEndPoint) | (RightEndPoint <= LeftEndPoint))
                        {
                            error = "Error Left and Right point";
                            MessageBox.Show(error);
                            hasLeftAndRightEndPointError = true;
                        }
                        else
                        {
                            hasLeftAndRightEndPointError = false;
                        }
                        break;
                    case nameof(BindNumberSplineGridNodes):
                        if (BindNumberSplineGridNodes <= 2)
                        {
                            error = "Error Spline nodes number (value <= 2)";
                            MessageBox.Show(error);
                            hasNumberSplineGridNodesError = true;
                        }
                        else
                        {
                            hasNumberSplineGridNodesError = false;
                        }
                        break;
                }
                return error;
            }
        }
        public string Error { get; }

        public bool ValidDataError()
        {
            return !(hasNumberGridNodesError || hasLeftAndRightEndPointError || hasNumberSplineGridNodesError);
        }


        public void CalculateSplines()
        {
            try
            {
                //// для способа 1
                switch (BindFunctionFRawEnum)
                {
                    case FRawEnum.Linear:
                        F = RawData.Linear;
                        break;
                    case FRawEnum.Cubic:
                        F = RawData.Cubic;
                        break;
                    case FRawEnum.Random:
                        F = RawData.Random;
                        break;
                }
                
                // Создаем объекты RawData и SplineData
                RawData = new RawData(BindLeftAndRightEndPoint[0], BindLeftAndRightEndPoint[1], BindNumberGridNodes, BindIsUniform, F);
                SplineData = new SplineData(RawData, BindLeftDeriv, BindRightDeriv, BindNumberSplineGridNodes);
                SplineData.MakeMKLSpline();
                //BindIntegral = $"{SplineData.IntegralValue:F3}";



            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось посчитать сплайн: " + ex.Message,
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }




        public void Save(string filename)
        {
            try
            {
                //CalculateSplines();
                if (RawData != null)
                {
                    RawData.Save(filename);
                }
                else
                {
                    MessageBox.Show("Ошибка! Нет RawData");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось сохранить данные в файле: " + ex.Message, "Ошибка");
            }

        }
        public void Load(string filename)
        {
            try
            {
                RawData.Load(filename, out RawData);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось загрузить данные из файла" + ex.Message, "Ошибка");
            }
        }
    }
}

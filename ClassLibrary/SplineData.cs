using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;

namespace ClassLibrary
{
    public class SplineData
    {
        public RawData RawData { get; set; }
        public int NumberSplineGridNodes { get; set; }
        public List<SplineDataItem> ListSplineData { get; set; }
        public double LeftDeriv { get; set; }
        public double RightDeriv { get; set; }
        public double IntegralValue { get; set; }

        public enum InterpolationStatus
        {
            SUCCESS = 0,
            TASK_CREATION_ERROR,
            SPLINE_SETUP_ERROR,
            SPLINE_CONSTRUCTION_ERROR,
            INTERPOLATION_ERROR,
            INTEGRATION_ERROR,
            TASK_DELETION_ERROR,
            UNKNOWN_ERROR
        }

        public SplineData(RawData rawData, double leftDeriv, double rightDeriv, int numberSplineGridNodes)
        {
            this.RawData = rawData;
            this.NumberSplineGridNodes = numberSplineGridNodes;
            this.ListSplineData = new List<SplineDataItem>();

            this.LeftDeriv = leftDeriv;
            this.RightDeriv = rightDeriv;

        }

        //метод, в котором вызываются функции из библиотеки Intel MKL для построения
        //сплайна, вычисления значений сплайна, его производных и интеграла по отрезку
        //[a, b]; в этом методе добавляются элементы в коллекцию List<SplineDataItem>;

        public void MakeMKLSpline()
        {
            double[] Spline = new double[NumberSplineGridNodes];
            double[] SplineDeriv = new double[NumberSplineGridNodes];
            double[] SplineDoubleDeriv = new double[NumberSplineGridNodes];

            int num_points_x = RawData.NumberGridNodes;
            int num_points_y = 1;

            double[] x_values = new double[num_points_x];
            for (int i = 0; i < num_points_x; i++)
            {
                x_values[i] = RawData.GridNodes[i];
            }

            double[] y_values = new double[num_points_x * num_points_y];
            for (int i = 0; i < num_points_x * num_points_y; i++)
            {
                y_values[i] = RawData.GridValues[i];
            }

            double[] boundary_conditions = new double[2] { LeftDeriv, RightDeriv };
            double[] spline_coefficients = new double[(num_points_x - 1) * 4 * num_points_y];

            int num_sites = NumberSplineGridNodes;

            double[] sites = new double[num_sites];
            sites[0] = RawData.LeftEndPoint;
            sites[num_sites - 1] = RawData.RightEndPoint;

            for (int i = 1; i < NumberSplineGridNodes - 1; i++)
            {
                sites[i] = sites[0] + i * (RawData.RightEndPoint - RawData.LeftEndPoint) / (NumberSplineGridNodes - 1);
            }


            int num_derivatives = 3;
            int[] derivative_orders = new int[3] { 1, 1, 1 };
            double[] interpolated_values = new double[num_sites * 3 * num_points_y];

            InterpolationStatus return_status = 0; // статус выполнения

            int nlim = 1;
            double[] left_limit = new double[1] { RawData.LeftEndPoint };
            double[] right_limit = new double[1] { RawData.RightEndPoint };

            double[] integral_result = new double[1];

            try
            {
                CubeInterpolate(num_points_x, num_points_y,
                                RawData.IsUniform ? new double[] {RawData.LeftEndPoint, RawData.RightEndPoint} : x_values, 
                                y_values, boundary_conditions, spline_coefficients,
                                num_sites, RawData.IsUniform ? new double[] { sites[0], sites[num_sites-1] } : sites, 
                                num_derivatives, derivative_orders, interpolated_values,
                                ref return_status, nlim, left_limit, right_limit,
                                integral_result, RawData.IsUniform);

                if (return_status == InterpolationStatus.SUCCESS)
                {
                    IntegralValue = integral_result[0];

                    for (int i = 0; i < interpolated_values.Length; i++)
                    {
                        int index = i / 3;

                        if (i % 3 == 0)
                        {
                            Spline[index] = interpolated_values[i];
                        }
                        else if (i % 3 == 1)
                        {
                            SplineDeriv[index] = interpolated_values[i];
                        }
                        else if (i % 3 == 2)
                        {
                            SplineDoubleDeriv[index] = interpolated_values[i];
                        }
                    }
                }
                else
                {
                    throw new Exception($"Ошибка сплайна:{Convert.ToString(return_status)}");

                }

                for (int i = 0; i < NumberSplineGridNodes; i++)
                {
                    SplineDataItem item = new(sites[i], Spline[i], SplineDeriv[i], SplineDoubleDeriv[i]);
                    ListSplineData.Add(item);

                }

            }
            catch (Exception ex)
            { 
                throw new Exception(ex.Message);

            }

        }

        [DllImport("..\\..\\..\\..\\x64\\DEBUG\\DllMKL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CubeInterpolate(
                int num_points_x, int num_points_y, double[] x_values, double[] y_values,
                double[] boundary_conditions, double[] spline_coefficients, int num_sites,
                double[] sites, int num_derivatives, int[] derivative_orders, double[] interpolated_values,
                ref InterpolationStatus return_status, int nlim, double[] left_limit, double[] right_limit,
                double[] integral_result, bool isUniform);
    }
}

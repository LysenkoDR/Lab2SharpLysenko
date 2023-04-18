using System;
using System.IO;

namespace ClassLibrary
{
    public delegate double FRaw(double x);
    public enum FRawEnum
    {
        Linear,
        Cubic,
        Random
    }
    public class RawData
    {
        public double LeftEndPoint { get; set; }
        public double RightEndPoint { get; set; }
        public int NumberGridNodes { get; set; }
        public bool IsUniform { get; set; }

        public FRaw F { get; set; }

        public double[] GridNodes { get; set; }
        public double[] GridValues { get; set; }

        public RawData(double leftEndPoint, double rightEndPoint, int numberGridNodes, bool isUniform, FRaw f)
        {
            this.LeftEndPoint = leftEndPoint;
            this.RightEndPoint = rightEndPoint;
            this.NumberGridNodes = numberGridNodes;
            this.IsUniform = isUniform;
            this.F = f;

            GridNodes = new double[NumberGridNodes];
            GridValues = new double[NumberGridNodes];

            GridNodes[0] = LeftEndPoint;
            GridNodes[NumberGridNodes - 1] = RightEndPoint;

            GridValues[0] = F(LeftEndPoint);
            GridValues[NumberGridNodes - 1] = F(RightEndPoint);

            double step = (RightEndPoint - LeftEndPoint) / (NumberGridNodes - 1);

            if (IsUniform == true)
            {
                for (int i = 1; i < NumberGridNodes - 1; i++)
                {
                    GridNodes[i] = LeftEndPoint + i * step;
                    GridValues[i] = F(GridNodes[i]);
                }
            }
            else
            {
                Random rnd = new();

                for (int i = 1; i < NumberGridNodes - 1; i++)
                {
                    GridNodes[i] = LeftEndPoint + i * step + rnd.NextDouble();
                    GridValues[i] = F(GridNodes[i]);
                }

            }

        }

        //public RawData(double[] gridNodes, double[] rightEndPoint)
        //{
        //    this.LeftEndPoint = leftEndPoint;
        //    this.RightEndPoint = rightEndPoint;
        //    this.NumberGridNodes = numberGridNodes;
        //    this.IsUniform = isUniform;
        //    this.F = f;

        //    GridNodes = new double[NumberGridNodes];
        //    GridValues = new double[NumberGridNodes];

        //    GridNodes[0] = LeftEndPoint;
        //    GridNodes[NumberGridNodes - 1] = RightEndPoint;

        //    GridValues[0] = F(LeftEndPoint);
        //    GridValues[NumberGridNodes - 1] = F(RightEndPoint);

        //    double step = (RightEndPoint - LeftEndPoint) / (NumberGridNodes - 1);

        //    if (IsUniform == true)
        //    {
        //        for (int i = 1; i < NumberGridNodes - 1; i++)
        //        {
        //            GridNodes[i] = LeftEndPoint + i * step;
        //            GridValues[i] = F(GridNodes[i]);
        //        }
        //    }
        //    else
        //    {
        //        Random rnd = new();

        //        for (int i = 1; i < NumberGridNodes - 1; i++)
        //        {
        //            GridNodes[i] = LeftEndPoint + i * step + rnd.NextDouble();
        //            GridValues[i] = F(GridNodes[i]);
        //        }

        //    }

        //}

        //конструктор c одним параметрами типа string для имени файла,
        //в котором хранятся данные для инициализации RawData;

        public RawData(string fileName)
        {
            Load(fileName, out RawData rawData);

            this.LeftEndPoint = rawData.LeftEndPoint;
            this.RightEndPoint = rawData.RightEndPoint;
            this.NumberGridNodes = rawData.NumberGridNodes;
            this.IsUniform = rawData.IsUniform;
            this.F = rawData.F;
            this.GridNodes = rawData.GridNodes;
            this.GridValues = rawData.GridValues;
        }

        public static double Linear(double x)
        { return x; }
        public static double Cubic(double x)
        { return Math.Pow(x, 3); }
        public static double Random(double x)
        {
            Random rnd = new();
            return x * rnd.NextDouble();
        }

        public void Save(string fileName)
        {
            StreamWriter? writer = null;
            try
            {
                writer = new StreamWriter(fileName);
                writer.WriteLine(LeftEndPoint);
                writer.WriteLine(RightEndPoint);
                writer.WriteLine(NumberGridNodes);
                writer.WriteLine(IsUniform);
                //writer.WriteLine(Enum.GetName(typeof(FRawEnum), F.Method.Name));
                writer.WriteLine(F.Method.Name);
                for (int i = 0; i < NumberGridNodes; i++)
                {
                    writer.WriteLine(GridNodes[i]);
                    writer.WriteLine(GridValues[i]);
                }
                //for (int i = 0; i < NumberGridNodes; i++)
                //{
                //    writer.WriteLine(GridValues[i]);
                //}           
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save data to file {fileName}", ex);
            }
            finally
            {
                writer?.Dispose();
                writer?.Close();
            }
        }

        public static void Load(string fileName, out RawData rawData)
        {
            StreamReader? reader = null;
            try
            {
                reader = new StreamReader(fileName);
                double LeftEndPoint = Convert.ToDouble(reader.ReadLine());
                double RightEndPoint = Convert.ToDouble(reader.ReadLine());
                int NumberGridNodes = Convert.ToInt32(reader.ReadLine());
                bool IsUniform = Convert.ToBoolean(reader.ReadLine());

                string? Fstring = Convert.ToString(reader.ReadLine());
                FRaw? F = Fstring switch
                {
                    "Linear" => Linear,
                    "Cubic" => Cubic,
                    "Random" => Random,
                    _ => throw new InvalidDataException(),
                };

                rawData = new RawData(LeftEndPoint, RightEndPoint, NumberGridNodes, IsUniform, F);


                //double[] GridNodes = new double[NumberGridNodes];
                //double[] GridValues = new double[NumberGridNodes];

                for (int i = 0; i < NumberGridNodes; i++)
                {
                    rawData.GridNodes[i] = Convert.ToDouble(reader.ReadLine());
                    rawData.GridValues[i] = Convert.ToDouble(reader.ReadLine());
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load data to file {fileName}", ex);
            }
            finally
            {
                reader?.Dispose();
                reader?.Close();
            }
        }

    }
}

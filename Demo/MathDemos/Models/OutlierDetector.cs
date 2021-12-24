using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathDemos.Models
{
    public class OutlierDetector
    {
        public double CurrentDeviationsFromMean { set; get; }
        public bool CurrentIsOutlier { set; get; }
        public double CurrentOutlierScore { set; get; }
        public double VarianceDecayTime { set; get; }
        public double MeanDecayTime { set; get; }

        public double VarianceDecayFactor { set; get; }
        public double Variance { set; get; }

        public double MeanDecayFactor { set; get; }
        public double OutlierCutoff { set; get; }
        public double Mean { set; get; }

        public double DeltaT { set; get; }
        public int CountSum { set; get; }
        public int CountOutlier { set; get; }

        public double VarN { set; get; }
        public double M2n { set; get; }
        public double M20 { set; get; } = 1e-5;

        public OutlierDetector(double varianceDecayTime, double meanDecayTime, double outlierCutoff, double deltaT)
        {
            this.VarianceDecayTime = varianceDecayTime;
            this.MeanDecayTime = meanDecayTime;
            this.OutlierCutoff = outlierCutoff;
            this.DeltaT = deltaT;
        }
        public void Reset()
        {
            this.Mean = 0;
            this.CountSum = 0;
            this.CountOutlier = 0;
            this.M2n = M20;
        }

        public void OutlierDetect(float currentValue)
        {
            this.CurrentDeviationsFromMean = Math.Abs(currentValue - Mean) / Math.Sqrt(M2n / VarN);
            VarianceDecayFactor = CountSum < 2 ? 1 : Math.Exp(-DeltaT / VarianceDecayTime * Math.Log(2.0));
            CountSum++;
            VarN = VarN * VarianceDecayFactor + 1;

            switch (CountSum)
            {
                case 1:
                    Mean = currentValue;
                    M2n = M20;
                    CurrentDeviationsFromMean = 0.0;
                    CurrentOutlierScore = 0;
                    CurrentIsOutlier = false;
                    break;

                case 2:
                    Mean = (Mean + currentValue) / 2;
                    M2n = Math.Pow((Mean - currentValue) * 10, 2); // TODO why 10;
                    CurrentDeviationsFromMean = 0.0;
                    CurrentOutlierScore = 0;
                    CurrentIsOutlier = false;
                    break;

                default:
                    MeanDecayFactor = Math.Exp(-DeltaT / MeanDecayTime * Math.Log(2.0));
                    if (M2n == 0 || CurrentDeviationsFromMean < OutlierCutoff)
                    {
                        var preMean = Mean;
                        Mean = Mean * MeanDecayFactor + currentValue * (1.0 - MeanDecayFactor);
                        System.Diagnostics.Debug.WriteLine($"Mean:  {Mean}");

                        M2n = M2n * VarianceDecayFactor + (currentValue - preMean) / Math.Sqrt(Variance);
                    }
                    CurrentDeviationsFromMean = Math.Abs(currentValue - Mean) / Math.Sqrt(Variance);
                    CurrentOutlierScore = CurrentDeviationsFromMean / OutlierCutoff;
                    CurrentIsOutlier = true;
                    break;
            }

            Variance = M2n / VarN;
            System.Diagnostics.Debug.WriteLine($"Score: {CurrentOutlierScore}");
        }
    }
}

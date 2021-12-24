using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathDemos.Models
{
    /* public class DataGenerator
     {

     }*/
    public class DataGenerator
    {
        public float[,] RawData { set; get; }
        public float SampleRate { set; get; }
        public float TimeSum { set; get; }
        public float Drift { set; get; }
        public float VarianceWaviness { set; get; }
        public float OutlierCutoff { set; get; }

        public DataGenerator(float sampleRate, float drift, float varianceWaviness, float timeSum, float outlierCutoff)
        {
            this.SampleRate = sampleRate;
            this.Drift = drift;
            this.VarianceWaviness = varianceWaviness;
            this.TimeSum = timeSum;
            this.OutlierCutoff = outlierCutoff;

            var coutSum = (int)Math.Floor(TimeSum * SampleRate);
            this.RawData = new float[coutSum, 2];
        }

        public void DataGeneration()
        {
            var rand = new Random();
            var coutSum = TimeSum * SampleRate;

            for (int i = 0; i < coutSum; i++)
            {
                var nextRand =(float)rand.NextDouble();
                Drift = (float)rand.NextDouble() * 2.0f - 1.0f;
                VarianceWaviness = (float)rand.NextDouble() * 30 + 5;

                var t = 1 / SampleRate * i;
                var variance = 0.2 * (1.0 + 0.6 * Math.Sin(i / coutSum * VarianceWaviness));
                var bias = 0.1 * Math.Sin(i / coutSum * 7) + Drift * i / coutSum;

                RawData[i, 0] = t;
                if((float)rand.NextDouble() < OutlierCutoff / 100)
                {
                    RawData[i, 1] = (float)(bias + (float)rand.NextDouble() * 2 - 1);
                }
                else
                {
                    RawData[i, 1] = (float)(variance * Math.Pow(x:(float)rand.NextDouble() * 2 - 1, y: 2) + bias);
                }
                
                /*if (rand.NextDouble() < OutlierCutoff / 100)
                {
                   
                }*/
            }
        }
    }
}

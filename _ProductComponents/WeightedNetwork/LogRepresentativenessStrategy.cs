using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeightedNetwork
{
    public class LogRepresentativenessStrategy : IRepresentativenessStrategy
    {

        public LogRepresentativenessStrategy()
        {

        }

        public double GetRepresentativenessBase(double Nweight, double NNweight)
        {
            if (NNweight == 0)
            {
                if (Nweight == 0)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                double b;
                b = Math.Pow(1 + Nweight, 1 / NNweight);

                return b;
            }
        }

        public double GetRepresentativenessBase(double Nweight, double NNweight, double maxWeight)
        {
            if (Nweight == 0)
            {
                return -1;
            }
            else
            {
                if (NNweight < Nweight && Nweight <= maxWeight)
                {
                    double delta = 1 - Nweight / maxWeight;
                    NNweight += delta;
                }

                double b = 0;
                if (NNweight > 0)
                {
                    b = Math.Pow(1 + Nweight, 1 / NNweight);
                }

                return b;
            }
        }

        public double GetBaseRepresentativeness(double rBase, double Nweight, double NNweight)
        {
            if ((NNweight == 0) || (rBase <= 1))
            {
                return 0;
            }
            else
            {
                return NNweight / Math.Log(1 + Nweight, rBase); 
            }
        }

        public double GetRepresentativeness(double Nweight, double NNweight)
        {
            double b = this.GetRepresentativenessBase(Nweight, NNweight);
            if (b > 0)
            {
                return 1 / b;
            }
            else
            {
                return 0;
            }
        }

        public double GetRepresentativeness(double Nweight, double NNweight, double maxWeight)
        {
            double b = this.GetRepresentativenessBase(Nweight, NNweight, maxWeight);
            if (b > 0)
            {
                return 1 / b;
                //double w = 1;
                //if (!LRNET_DEFAULT_SETTING)
                //{
                //    double num = 1;
                //    double denom = 1;
                //    if (NNweight > 1)
                //    {
                //        double x = Math.Log(Nweight, NNweight);
                //        num = 1 + 1 / x;
                //        denom = Math.Log(Nweight);
                //    }
                //    w = num / denom;
                //    //if (w > 1)
                //    //{
                //    //    w = 1;
                //    //}
                //}

                //double ret = w * (1 / b);
                //return ret;
            }
            else
            {
                return 0;
            }
        }

    }
}

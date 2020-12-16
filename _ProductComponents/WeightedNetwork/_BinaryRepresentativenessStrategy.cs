//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace WeightedNetwork
//{
//    public class BinaryRepresentativenessStrategy : IRepresentativenessStrategy
//    {

//        public BinaryRepresentativenessStrategy()
//        {

//        }
        
//        public double GetRepresentativenessBase(double Nweight, double NNweight)
//        {
//            if (NNweight == 0)
//            {
//                if (Nweight == 0)
//                {
//                    return -1;
//                }
//                else
//                {
//                    return 0;
//                }
//            }
//            else
//            {
//                return 1;
//            }
//        }

//        public double GetRepresentativeness(double rBase, double Nweight, double NNweight)
//        {
//            if (NNweight > 0)
//            {
//                return 1;
//            }
//            else
//            {
//                return 0;
//            }
//        }

//        public double GetRepresentativeness(double Nweight, double NNweight)
//        {
//            return 1;
//        }

//    }
//}

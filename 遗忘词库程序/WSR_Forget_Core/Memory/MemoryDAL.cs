using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
namespace WSR_Forget_Core.Memory
{
    public class MemoryDAL
    {
        public static double CalcNewtonCooling(double count, double lambda)
        {
            return Math.Pow(Math.E, -1 * lambda * count);
        }

        public static double CalcRemeberValue(double count, MemoryParameter parameter)
        {
            double lambda = -1 * Math.Log(parameter.Threshold) / parameter.ContainerSize;
            return CalcNewtonCooling(count, lambda);
        }

      
    }
}

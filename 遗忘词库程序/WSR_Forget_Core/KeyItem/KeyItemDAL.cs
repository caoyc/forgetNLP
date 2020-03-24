using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WSR_Forget_Core.Memory;
namespace WSR_Forget_Core.KeyItem
{
    public static class KeyItemDAL
    {
        public static KeyItemColl<T> ClearKeyItemColl<T>(KeyItemColl<T> objKeyItemColl)
        {
            double dTotalVaildCount = objKeyItemColl.Parameter.TotalValidCount + 1;// 1.0 / ( 1.0 - MemoryDAL.CalcRemeberValue(1,objKeyItemColl.Parameter) );
            
            KeyItemColl<T> objResultColl = new KeyItemColl<T>();
            objResultColl.Parameter = objKeyItemColl.Parameter;
            objResultColl.Parameter.Entropy = 0;

            double dClearValidCount = 0;
            foreach (KeyItemMDL<T> mdl in objKeyItemColl)
            {
                double dRemeberValue = mdl.ValidCount * MemoryDAL.CalcRemeberValue(objKeyItemColl.Parameter.TotalOffset - mdl.UpdateOffset, objKeyItemColl.Parameter);
                if (dRemeberValue > objKeyItemColl.Parameter.Threshold)
                {
                    mdl.ValidCount = dRemeberValue;
                    mdl.UpdateOffset = 0;
                 
                    
                    if (!objResultColl.Contains(mdl.Key))
                    {
                        dClearValidCount += mdl.ValidCount;

                        double dProb = mdl.ValidCount / dTotalVaildCount;
                        objKeyItemColl.Parameter.Entropy += -dProb * Math.Log(dProb);
                        objResultColl.Add(mdl);
                    }
                }
            }
            objResultColl.Parameter.TotalOffset = 0;
            objResultColl.Parameter.TotalValidCount = dClearValidCount;
            return objResultColl;
        }
        public static KeyItemMDL<T> UpdateKeyItemColl<T>(T key, KeyItemColl<T> objKeyItemColl,OffsetWeightMDL objWeightMDL)
        {
            if (!objKeyItemColl.Contains(key))
            {
                KeyItemMDL<T> item = new KeyItemMDL<T>();
                item.Key = key;
                item.UpdateOffset = objKeyItemColl.Parameter.TotalOffset;
                objKeyItemColl.Add(item);
            }


            KeyItemMDL<T> mdl = objKeyItemColl[key];

            mdl.ValidCount = objWeightMDL.Weight + mdl.ValidCount * MemoryDAL.CalcRemeberValue(objKeyItemColl.Parameter.TotalOffset - mdl.UpdateOffset, objKeyItemColl.Parameter);
            mdl.TotalCount = objWeightMDL.Weight + mdl.TotalCount * MemoryDAL.CalcRemeberValue(objWeightMDL.Weight, objKeyItemColl.Parameter);
            mdl.UpdateOffset = objKeyItemColl.Parameter.TotalOffset;

            objKeyItemColl.Parameter.TotalValidCount = objWeightMDL.Weight + objKeyItemColl.Parameter.TotalValidCount* MemoryDAL.CalcRemeberValue(objWeightMDL.Offset, objKeyItemColl.Parameter);
            objKeyItemColl.Parameter.TotalOffset += objWeightMDL.Offset;
            
            return mdl;
        }
    }
}

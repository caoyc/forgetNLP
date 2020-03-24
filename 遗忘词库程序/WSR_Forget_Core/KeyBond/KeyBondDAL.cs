using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WSR_Forget_Core.Memory;
using WSR_Forget_Core.KeyItem;
namespace WSR_Forget_Core.KeyBond
{
    public static class KeyBondDAL
    {

        public static KeyBondColl<T, L> ClearKeyBondColl<T, L>(KeyBondColl<T, L> objKeyBondColl,bool bIsParentTotalOffset=true)
        {
            double dTotalVaildCount = objKeyBondColl.Parameter.TotalValidCount + 1;// 1.0 / ( 1.0 - MemoryDAL.CalcRemeberValue(1,objKeyBondColl.Parameter) );

            KeyBondColl<T, L> objResultColl = new KeyBondColl<T, L>();
            objResultColl.Parameter = objKeyBondColl.Parameter;
            objResultColl.Parameter.Entropy = 0;

            double dClearValidCount = 0;
            foreach (KeyBondMDL<T, L> bond in objKeyBondColl)
            {
                KeyItemMDL<T> mdl = bond.KeyItem;
                double dRemeberValue = mdl.ValidCount * MemoryDAL.CalcRemeberValue(objKeyBondColl.Parameter.TotalOffset - mdl.UpdateOffset, objKeyBondColl.Parameter);
                if (dRemeberValue > objKeyBondColl.Parameter.Threshold)
                {
                    mdl.ValidCount = dRemeberValue;
                    mdl.UpdateOffset = 0;

                    if (!objResultColl.Contains(mdl.Key))
                    {
                        dClearValidCount += mdl.ValidCount;

                        double dProb = mdl.ValidCount / dTotalVaildCount;
                        objKeyBondColl.Parameter.Entropy += -dProb * Math.Log(dProb);

                       if(bIsParentTotalOffset) bond.LinkColl.Parameter.TotalOffset = objKeyBondColl.Parameter.TotalOffset;
                        bond.LinkColl = KeyItemDAL.ClearKeyItemColl<L>(bond.LinkColl);
                        objResultColl.Add(bond);
                    }
                }
            }
            objResultColl.Parameter.TotalOffset = 0;
            objResultColl.Parameter.TotalValidCount = dClearValidCount;
            return objResultColl;
        }

        public static KeyItemMDL<T> UpdateHeadBondColl<T, L>(T head, KeyBondColl<T, L> objKeyBondColl, OffsetWeightMDL objHeadWeightMDL)
        {
            if (!objKeyBondColl.Contains(head))
            {

                KeyBondMDL<T, L> bond = new KeyBondMDL<T, L>();
                bond.KeyItem.Key = head;
                bond.KeyItem.UpdateOffset = objKeyBondColl.Parameter.TotalOffset;
                bond.LinkColl.Parameter.ContainerSize = objKeyBondColl.Parameter.ContainerSize;
                bond.LinkColl.Parameter.Threshold = objKeyBondColl.Parameter.Threshold;
                objKeyBondColl.Add(bond);
            }

            KeyItemMDL<T> mdl = objKeyBondColl[head].KeyItem;
            mdl.ValidCount = objHeadWeightMDL.Weight + mdl.ValidCount * MemoryDAL.CalcRemeberValue(objKeyBondColl.Parameter.TotalOffset - mdl.UpdateOffset, objKeyBondColl.Parameter);
            mdl.TotalCount = objHeadWeightMDL.Weight + mdl.TotalCount * MemoryDAL.CalcRemeberValue(objHeadWeightMDL.Weight, objKeyBondColl.Parameter);
            mdl.UpdateOffset = objKeyBondColl.Parameter.TotalOffset;

            objKeyBondColl.Parameter.TotalValidCount=objHeadWeightMDL.Weight + objKeyBondColl.Parameter.TotalValidCount * MemoryDAL.CalcRemeberValue(objHeadWeightMDL.Offset, objKeyBondColl.Parameter);
            objKeyBondColl.Parameter.TotalOffset += objHeadWeightMDL.Offset;

            return mdl;
        }
        public static KeyItemMDL<L> UpdateTailBondColl<T, L>(T head, L tail, KeyBondColl<T, L> objKeyBondColl, OffsetWeightMDL objTailWeightMDL)
        {
            if (!objKeyBondColl.Contains(head)) return null;


            KeyItemColl<L> objLinkColl = objKeyBondColl[head].LinkColl;
            if (objTailWeightMDL.Offset < 0)
            {
                //继承主计数
                objLinkColl.Parameter.TotalOffset = objKeyBondColl.Parameter.TotalOffset;
                objTailWeightMDL.Offset = 0;
            }
           return KeyItemDAL.UpdateKeyItemColl(tail, objLinkColl, objTailWeightMDL);

           
        }
        public static KeyBondMDL<T, L>   UpdateKeyBondColl<T, L>(T head, L tail, KeyBondColl<T, L> objKeyBondColl, OffsetWeightMDL objHeadWeightMDL, OffsetWeightMDL objTailWeightMDL)
        {
            if (!objKeyBondColl.Contains(head))
            {

                KeyBondMDL<T, L> bond = new KeyBondMDL<T, L>();
                bond.KeyItem.Key = head;
                bond.KeyItem.UpdateOffset = objKeyBondColl.Parameter.TotalOffset;
                bond.LinkColl.Parameter.ContainerSize = objKeyBondColl.Parameter.ContainerSize;
                bond.LinkColl.Parameter.Threshold = objKeyBondColl.Parameter.Threshold;
                objKeyBondColl.Add(bond);
            }

            KeyItemMDL<T> mdl = objKeyBondColl[head].KeyItem;
            mdl.ValidCount =objHeadWeightMDL.Weight  + mdl.ValidCount * MemoryDAL.CalcRemeberValue(objKeyBondColl.Parameter.TotalOffset - mdl.UpdateOffset, objKeyBondColl.Parameter);
            mdl.TotalCount = objHeadWeightMDL.Weight + mdl.TotalCount * MemoryDAL.CalcRemeberValue(1, objKeyBondColl.Parameter);
            mdl.UpdateOffset = objKeyBondColl.Parameter.TotalOffset;
        

            KeyItemColl<L> objLinkColl = objKeyBondColl[head].LinkColl;
            if (objTailWeightMDL.Offset < 0)
            {
                //继承主计数
                objLinkColl.Parameter.TotalOffset = objKeyBondColl.Parameter.TotalOffset;
                objTailWeightMDL.Offset = 0;
            }
            KeyItemDAL.UpdateKeyItemColl(tail, objLinkColl, objTailWeightMDL);

            objKeyBondColl.Parameter.TotalValidCount = objHeadWeightMDL.Weight + objKeyBondColl.Parameter.TotalValidCount * MemoryDAL.CalcRemeberValue(objHeadWeightMDL.Offset, objKeyBondColl.Parameter);
            objKeyBondColl.Parameter.TotalOffset += objHeadWeightMDL.Offset;

            return objKeyBondColl[head];

        }
    }
}

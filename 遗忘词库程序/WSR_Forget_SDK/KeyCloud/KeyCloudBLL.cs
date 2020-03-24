using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WSR_Forget_Core.Memory;
using WSR_Forget_Core.KeyItem;
using WSR_Forget_Core.KeyBond;
namespace WSR_Forget_SDK.KeyCloud
{
    public class KeyCloudBLL
    {
        public static void UpdateKeyCloudColl<T>(List<T> objKeyList,KeyBondColl<T,T> objKeyCloudColl,int nRadiusSize = 7)
        {
            OffsetWeightMDL objHeadWeightMDL = new  OffsetWeightMDL(1,1);
            OffsetWeightMDL objTailWeightMDL = new  OffsetWeightMDL(1,1);

            UpdateKeyCloudColl<T>(objKeyList,objKeyCloudColl,objHeadWeightMDL,objTailWeightMDL,nRadiusSize);
        }
        public static void UpdateKeyCloudColl<T>(List<T> objKeyList,KeyBondColl<T,T> objKeyCloudColl,OffsetWeightMDL objHeadWeightMDL,OffsetWeightMDL objTailWeightMDL,int nRadiusSize = 7)
        {
            Dictionary<int,KeyValuePair<int,double>> objRelateDict = new Dictionary<int,KeyValuePair<int,double>>();
            for (int k = 0; k < objKeyList.Count; k++)
            {
                objRelateDict.Add(k,new KeyValuePair<int,double>(k,-1));
                T keyTail = objKeyList[k];
                for (int r = 1; r <= Math.Min(k,nRadiusSize); r++)
                {
                    int nPos = k - r;
                    T keyHead = objKeyList[nPos];
                    double dRelateValue = KeyBondHelper.CalcBondRelateValue<T>(keyHead,keyTail,objKeyCloudColl);
                    if (dRelateValue > objRelateDict[k].Value) objRelateDict[k] = new KeyValuePair<int,double>(nPos,dRelateValue);
                    if (dRelateValue > objRelateDict[nPos].Value) objRelateDict[nPos] = new KeyValuePair<int,double>(k,dRelateValue);
                }
                if (k > nRadiusSize)
                {
                    int nPos = k - nRadiusSize - 1;
                    int nCorePos = nPos;
                    int nLinkPos = objRelateDict[nPos].Key;


                    if (nCorePos > nLinkPos)
                    {
                        nCorePos = nLinkPos;
                        nLinkPos = nPos;
                    }
                    lock (objKeyCloudColl)
                    {
                        KeyBondDAL.UpdateKeyBondColl<T,T>(objKeyList[nCorePos],objKeyList[nLinkPos],objKeyCloudColl,objHeadWeightMDL,objTailWeightMDL);
                    }

                    objRelateDict.Remove(nPos);
                }
            }
            foreach (KeyValuePair<int,KeyValuePair<int,double>> pair in objRelateDict)
            {
                int nPos = pair.Key;
                int nCorePos = nPos;
                int nLinkPos = objRelateDict[nPos].Key;
                if (nCorePos > nLinkPos)
                {
                    nCorePos = nLinkPos;
                    nLinkPos = nPos;
                }
                lock (objKeyCloudColl)
                {
                    KeyBondDAL.UpdateKeyBondColl<T,T>(objKeyList[nCorePos],objKeyList[nLinkPos],objKeyCloudColl,new OffsetWeightMDL(1,1),new OffsetWeightMDL(1,1));
                }
            }
        }

    }
}

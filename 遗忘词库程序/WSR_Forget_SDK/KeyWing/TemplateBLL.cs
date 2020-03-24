using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSR_Forget_Core.Memory;
using WSR_Forget_Core.KeyItem;
using WSR_Forget_Core.KeyBond;
namespace WSR_Forget_SDK.KeyWing
{
    public class TemplateBLL
    {
        #region 全句模板
        //public static void UpdateTemplateColl(List<string> objKeyList,KeyBondColl<string,string> objKeyCloudColl,KeyItemColl<string> objTemplateColl,int nRadiusSize = 7,string splitChar = "\t")
        //{
        //    Dictionary<int,PosWingColl> dict =KeyWingBLL. GetPosWingDict(objKeyList,objKeyCloudColl,nRadiusSize);
        //    UpdateTemplateColl(objKeyList,dict,objTemplateColl,splitChar);
        //}
        //public static void UpdateTemplateColl(List<string> objKeyList,Dictionary<int,PosWingColl> objPosWingDict,KeyItemColl<string> objTemplateColl,string splitChar = "\t")
        //{
        //    HashSet<int> objPosSet = new HashSet<int>();
        //    for (int k = 0; k < objKeyList.Count; k++)
        //    {
        //        objPosSet.Add(k);
        //    }
        //    PosWingColl objPosWingColl = new KeyWing.PosWingColl();
        //    foreach (KeyValuePair<int,PosWingColl> pair in objPosWingDict)
        //    {
        //        foreach (PosWingMDL mdl in pair.Value)
        //        {
        //            if (objPosWingColl.Contains(mdl.KeyIndex)) continue;
        //            objPosWingColl.Add(mdl);
        //        }
        //    }
        //    PosWingColl objResultColl = new PosWingColl();

        //    var objOrderedColl = objPosWingColl.OrderByDescending(x => x.Weight);
        //    foreach (PosWingMDL mdl in objOrderedColl)
        //    {
        //        if (objPosSet.Count <= 0) break;
        //        if (mdl.PosSet.All(x => !objPosSet.Contains(x))) continue; //没有包含新词，则跳过
        //        if (!objResultColl.Contains(mdl.KeyIndex))
        //        {
        //            objResultColl.Add(mdl);
        //        }
        //        objPosSet.RemoveWhere(x => mdl.PosSet.Contains(x));

        //    }


        //    foreach (PosWingMDL mdl in objResultColl)
        //    {
        //        SortedSet<int> set = new SortedSet<int>(mdl.PosSet);
        //        StringBuilder sb = new StringBuilder();
        //        int prev = -1;
        //        foreach (int k in set)
        //        {
        //            if (k-prev > 1) sb.Append(splitChar);
        //            sb.Append(String.Format("{0}",objKeyList[k]));
        //            prev = k;
        //        }
        //        string keywing = sb.ToString();
        //        KeyItemDAL.UpdateKeyItemColl<string>(keywing,objTemplateColl,new OffsetWeightMDL(1,1));
        //    }
        //}

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSR_Forget_Core.Memory
{
    [Serializable]
    public class MemoryParameter
    {
        private double _TotalOffset = 0;/*计数器*/
        private double _Threshold = 0.254;/*阈值*/
        private double _ContainerSize = 7/*每秒阅读字数*/ * 60/*秒*/ * 60/*分*/ * 24/*小时*/ * 6/*天*/;
        private double _Entropy = 0;
        private double _TotalValidCount = 0;/*词库有效总词频*/
        public double TotalOffset
        {
            get
            {
                return _TotalOffset;
            }

            set
            {
                _TotalOffset = value;
            }
        }

        public double Threshold
        {
            get
            {
                return _Threshold;
            }

            set
            {
                _Threshold = value;
            }
        }

        public double ContainerSize
        {
            get
            {
                return _ContainerSize;
            }

            set
            {
                _ContainerSize = value;
            }
        }

        public double Entropy
        {
            get
            {
                return _Entropy;
            }

            set
            {
                _Entropy = value;
            }
        }

        public double TotalValidCount
        {
            get
            {
                return _TotalValidCount;
            }

            set
            {
                _TotalValidCount = value;
            }
        }
    }
    [Serializable]
    public class MemoryMDL
    {
        private double _ValidCount = 0;//遗忘词频
        private double _TotalCount = 0;//累计总数
        private double _UpdateOffset = 0;//更新时偏移量


        public double ValidCount
        {
            get
            {
                return _ValidCount;
            }

            set
            {
                _ValidCount = value;
            }
        }

        public double TotalCount
        {
            get
            {
                return _TotalCount;
            }

            set
            {
                _TotalCount = value;
            }
        }

        public double UpdateOffset
        {
            get
            {
                return _UpdateOffset;
            }

            set
            {
                _UpdateOffset = value;
            }
        }


    }



    [Serializable]
    public class OffsetWeightMDL
    {
        private double _Offset = 0;
        private double _Weight = 0;

        public OffsetWeightMDL(double offset, double weight)
        {
            this.Offset = offset;
            this.Weight = weight;
        }

        public double Offset
        {
            get
            {
                return _Offset;
            }

            set
            {
                _Offset = value;
            }
        }

        public double Weight
        {
            get
            {
                return _Weight;
            }

            set
            {
                _Weight = value;
            }
        }
    }
}

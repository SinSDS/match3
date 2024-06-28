namespace com.F4A.MobileThird
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class DMCCoreDataInfo //: DMCBaseDataInfo<DMCCoreDataInfo>
    {
        [SerializeField]
        private bool _isRemoveAds;
        public bool IsRemoveAds
        {
            get { return _isRemoveAds; }
            set
            {
                _isRemoveAds = value;
            }
        }

        [SerializeField]
        private bool _isEnableSound = true;
        public bool IsEnableSound
        {
            get { return _isEnableSound; }
            set
            {
                _isEnableSound = value;
            }
        }

        [SerializeField]
        private bool _isEnableMusic = true;
        public bool IsEnableMusic
        {
            get { return _isEnableMusic; }
            set
            {
                _isEnableMusic = value;
            }
        }

        [SerializeField]
        private bool _isEnableVibrate = true;
        public bool IsEnableVibrate
        {
            get { return _isEnableMusic; }
            set
            {
                _isEnableMusic = value;
            }
        }

        //public override object GetData()
        //{
        //    throw new System.NotImplementedException();
        //}
    }
}

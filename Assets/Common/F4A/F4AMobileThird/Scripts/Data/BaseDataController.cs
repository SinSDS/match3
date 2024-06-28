namespace com.F4A.MobileThird
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class BaseDataController : MonoBehaviour /*, IBaseDataInfo*/
    {
        [SerializeField]
        private DMCCoreDataInfo _coreDataInfo = new DMCCoreDataInfo();
        public DMCCoreDataInfo CoreDataInfo
        {
            get { return _coreDataInfo; }
        }

        public virtual void Init() { }

        public virtual object GetData()
        {
            return null;
        }

        public virtual void RegisterSaveData() { }

        public virtual void SetData(string contentData) { }

        protected virtual void Start()
        {
            
        }
    }
}
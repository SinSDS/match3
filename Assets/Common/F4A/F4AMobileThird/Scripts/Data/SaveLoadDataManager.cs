namespace com.F4A.MobileThird
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;

    public class SaveLoadDataManager : SingletonMono<SaveLoadDataManager>
    {
        [SerializeField]
        private BaseDataController _dataController;
        public BaseDataController DataController
        {
            get { return _dataController; }
            set { _dataController = value; }
        }

        private void Awake()
        {
            if (!_dataController) _dataController = FindObjectOfType<BaseDataController>();

            _dataController.Init();
        }

        //public void Save<T>(T saveData, string savePath)
        //{
        //    try
        //    {
        //        //lock (m_saveLock)
        //        //{
        //        //    using (FileStream fileStream = new FileStream(savePath, FileMode.OpenOrCreate))
        //        //    {
        //        //        m_binaryFormatter.Serialize(fileStream, saveData);
        //        //        fileStream.Close();
        //        //    }
        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.LogError(ex.Message);
        //    }
        //}

        public void SaveData()
        {
        }

        public void SaveData(string path)
        {
        }

        public void LoadData()
        {

        }
    }
}
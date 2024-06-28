namespace com.F4A.MobileThird
{
#if DEFINE_FIREBASE_ANALYTIC || DEFINE_FIREBASE_CRASHLYTIC || DEFINE_FIREBASE_MESSAGING
    using Firebase;
#endif
    using UnityEngine;

    [System.Serializable]
    public class DMCFirebaseInfo
    {
        [SerializeField]
        private bool _isEnableFirebase;
        public bool IsEnableFirebase
        {
            get { return _isEnableFirebase; }
            set { _isEnableFirebase = value; }
        }
    }

    public partial class FirebaseManager : SingletonMono<FirebaseManager>
    {
        //[SerializeField]
        private bool _firebaseInitialized = false;
        public bool FirebaseInitialized
        {
            get { return _firebaseInitialized; }
            set { _firebaseInitialized = value; }
        }

#if DEFINE_FIREBASE_ANALYTIC || DEFINE_FIREBASE_CRASHLYTIC || DEFINE_FIREBASE_MESSAGING
        private DependencyStatus _dependencyStatus;
        private FirebaseApp _firebaseApp;
#endif

        private void Start()
        {
#if !UNITY_EDITOR
            InitializeFirebase();
#endif
        }

        private void InitializeFirebase()
        {
#if !UNITY_EDITOR
#if DEFINE_FIREBASE_ANALYTIC || DEFINE_FIREBASE_CRASHLYTIC || DEFINE_FIREBASE_MESSAGING || DEFINE_FIREBASE_AUTH
            _firebaseApp = FirebaseApp.DefaultInstance;
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                _dependencyStatus = task.Result;
                if (_dependencyStatus == DependencyStatus.Available)
                {
                    Debug.Log("Enabling data collection.");
                    _firebaseApp = FirebaseApp.DefaultInstance;
                    InitializeAnalytics();
                    InitializeCrashlytics();
                    InitializeMessaging();
                    _firebaseInitialized = true;
                }
                else
                {
                    _firebaseInitialized = false;
                    _firebaseApp = null;
                    Debug.LogError("Could not resolve all Firebase dependencies: " + _dependencyStatus);
                }
            });
#endif
#endif
        }
    }
}
namespace com.F4A.MobileThird
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Analytics;

    public class AnalyticManager : SingletonMono<AnalyticManager>
    {
        public void LoginEvent()
        {
#if DEFINE_UNITY_ANALYTIC
            Analytics.CustomEvent("Login");
#endif
        }

        public void CustomeEvent(string customEventName, IDictionary<string, object> eventData = null)
        {
#if DEFINE_UNITY_ANALYTIC
            if (eventData != null) Analytics.CustomEvent(customEventName, eventData);
            else Analytics.CustomEvent(customEventName);
#endif
        }
    }
}
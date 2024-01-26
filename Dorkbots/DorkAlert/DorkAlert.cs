using UnityEngine;

namespace Dorkbots.DorkAlert
{
    public static class DorkAlert
    {
        public static void SendAlert(string message)
        {
            DorkAlertVisualizer dorkAlert = new GameObject("DorkAlert").AddComponent<DorkAlertVisualizer>();
            dorkAlert.SetMessage(message);
        }
    }
}
using UnityEngine;

namespace Dorkbots.Tools
{
    public class OpenURL : MonoBehaviour
    {
        [SerializeField] private string urlToOpen;

        public void OpenURLLink()
        {
            Application.OpenURL(urlToOpen);
        }
    }
}
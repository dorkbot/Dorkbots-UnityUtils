using UnityEngine;

namespace Dorkbots
{
    public class SetTargetFramerate : MonoBehaviour
    {
        [Tooltip("Sets the application's target frame rate.")]         [SerializeField]  private int m_TargetFrameRate = 60;          /// <summary>         /// Get or set the application's target frame rate.         /// </summary>         public int targetFrameRate         {             get { return m_TargetFrameRate; }             set             {                 m_TargetFrameRate = value;                 SetFrameRate();             }         }          void Start()         {             SetFrameRate();         }

        private void SetFrameRate()
        {
            Application.targetFrameRate = targetFrameRate;
        }
    }
}
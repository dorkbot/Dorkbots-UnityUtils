using UnityEngine;

namespace Dorkbots.DorkAlert
{
    public class DorkAlertVisualizer : MonoBehaviour
    {
        private Vector2 _scrollPosition;
        private string _message;
        
        void OnGUI()
        {
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = 100;
            GUIStyle textStyle = new GUIStyle(GUI.skin.textArea);
            textStyle.fontSize = 100;
            
            GUI.Box (new Rect (0, 0, Screen.width, Screen.height), "");
            
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.Width(Screen.width-10), GUILayout.Height(Screen.height));
            
            if (GUILayout.Button("CLOSE", buttonStyle))
                Destroy(gameObject);
            
            GUILayout.TextArea(_message, textStyle);
            
            GUILayout.EndScrollView();
        }

        public void SetMessage(string message)
        {
            _message = message;
        }
    }
}
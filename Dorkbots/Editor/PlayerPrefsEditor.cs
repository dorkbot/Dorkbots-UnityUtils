using UnityEngine;
using UnityEditor;

namespace Dorkbots.EditorTools
{
    public class PlayerPrefsEditor : EditorWindow
    {
        [MenuItem("Dorkbots/PlayerPrefs Editor")]
        public static void openWindow()
        {
            PlayerPrefsEditor window = (PlayerPrefsEditor) EditorWindow.GetWindow(typeof(PlayerPrefsEditor));
            window.titleContent = new GUIContent("PlayerPrefs Editor");
            window.Show();
        }

        public enum FieldType
        {
            String,
            Integer,
            Float
        }

        private FieldType fieldType = FieldType.String;
        private string setKey = "";
        private string setVal = "";
        private string error = null;

        void OnGUI()
        {
            EditorGUILayout.LabelField("PlayerPrefs Editor", EditorStyles.boldLabel);
            EditorGUILayout.Separator();

            fieldType = (FieldType) EditorGUILayout.EnumPopup("Key Type", fieldType);
            setKey = EditorGUILayout.TextField("Key to Set", setKey);
            setVal = EditorGUILayout.TextField("Value to Set", setVal);

            if (error != null)
            {
                EditorGUILayout.HelpBox(error, MessageType.Error);
            }

            if (GUILayout.Button("Set Key"))
            {
                if (fieldType == FieldType.Integer)
                {
                    int result;
                    if (!int.TryParse(setVal, out result))
                    {

                        error = "Invalid input \"" + setVal + "\"";
                        return;

                    }
                    PlayerPrefs.SetInt(setKey, result);
                }
                else if (fieldType == FieldType.Float)
                {
                    float result;
                    if (!float.TryParse(setVal, out result))
                    {
                        error = "Invalid input \"" + setVal + "\"";
                        return;
                    }
                    PlayerPrefs.SetFloat(setKey, result);
                }
                else
                {
                    PlayerPrefs.SetString(setKey, setVal);

                }
                PlayerPrefs.Save();
                error = null;
            }

            if (GUILayout.Button("Get Key"))
            {
                if (fieldType == FieldType.Integer)
                {
                    setVal = PlayerPrefs.GetInt(setKey).ToString();
                }
                else if (fieldType == FieldType.Float)
                {
                    setVal = PlayerPrefs.GetFloat(setKey).ToString();
                }
                else
                {
                    setVal = PlayerPrefs.GetString(setKey);
                }
            }

            if (GUILayout.Button("Delete Key"))
            {
                PlayerPrefs.DeleteKey(setKey);
                PlayerPrefs.Save();
            }

            if (GUILayout.Button("Delete All Keys"))
            {
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();
            }
        }
    }
}
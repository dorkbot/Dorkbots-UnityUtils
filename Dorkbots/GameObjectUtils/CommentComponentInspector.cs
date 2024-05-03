#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace Dorkbots.GameObjectTools
{
	[CustomEditor(typeof(CommentComponent))]
	public class CommentComponentInspector : Editor 
	{
		private static Color _textColor;
		private static bool _init = false;
		
		private GUIStyle _commentStyle = new GUIStyle();
		private GUIStyle _messageStyle = new GUIStyle();
		private bool _infoFolded = false;
		private bool _colorFolded = false;
		private const string _commentColorPref = "dorkbots comment color";

		public override void OnInspectorGUI()
	    {
		    if (!_init)
		    {
			    if (EditorPrefs.HasKey(_commentColorPref))
			    {
				    ColorUtility.TryParseHtmlString("#" + EditorPrefs.GetString(_commentColorPref), out _textColor);
			    }
			    else
			    {
				    _textColor = new Color(255, 150, 4);
			    }

			    _init = true;
		    }

		    CommentComponent script = target as CommentComponent;
		    
	        if (serializedObject == null) return;
				
	        _commentStyle.wordWrap = true;
	        _commentStyle.fontSize = 16;
	        _commentStyle.normal.textColor = _textColor;
	        
	        _messageStyle.wordWrap = true;
	        _messageStyle.normal.textColor = _textColor;

	        serializedObject.Update();

	        EditorGUILayout.Space();
				
	        string text = EditorGUILayout.TextArea(script.Comment, _commentStyle);
	        if (text != script.Comment) 
	        {
	            Undo.RecordObject(script, "Edit Comments");
	            script.Comment = text;
	        }
	        
	        GUILayout.BeginHorizontal ();
	        GUILayout.Space (100);
	        EditorGUILayout.LabelField("");
	        GUILayout.EndHorizontal ();
	        
	        GUILayout.BeginHorizontal ();
	        GUILayout.Space (10);
	        _infoFolded = EditorGUILayout.Foldout(_infoFolded, "info");
	        GUILayout.EndHorizontal ();
	       
	        if (_infoFolded)
	        {
		        GUILayout.BeginHorizontal ();
		        GUILayout.Space (20);
		        EditorGUILayout.LabelField("The above text is a comment or note left by someone who felt it was important enough to leave behind...", _messageStyle);
		        //EditorGUILayout.Space(50);
		        GUILayout.EndHorizontal ();
		        
		        GUILayout.BeginHorizontal ();
		        GUILayout.Space (50);
		        EditorGUILayout.LabelField("");
		        GUILayout.EndHorizontal ();
		        
		        GUILayout.BeginHorizontal ();
		        GUILayout.Space (20);
		        _colorFolded = EditorGUILayout.Foldout(_colorFolded, "color");
		        GUILayout.EndHorizontal ();

		        if (_colorFolded)
		        {
			        GUILayout.BeginHorizontal ();
			        GUILayout.Space (30);
			        EditorGUILayout.LabelField("CHANGES COLOR OF ALL COMMENTS!!!!!!", _messageStyle);
			        GUILayout.EndHorizontal ();
		        
			        GUILayout.BeginHorizontal ();
			        GUILayout.Space (30);
			        _textColor = EditorGUILayout.ColorField(_textColor);
			        EditorGUILayout.Space(50);
			        GUILayout.EndHorizontal ();
			        
			        EditorPrefs.SetString(_commentColorPref, ColorUtility.ToHtmlStringRGB(_textColor)); // without alpha
		        }
	        }

	        EditorGUILayout.Space();
	        
		    serializedObject.ApplyModifiedProperties();
	    }
	}
}

#endif
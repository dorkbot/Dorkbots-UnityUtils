using UnityEngine;

namespace Dorkbots.GameObjectTools
{
    /// <summary>
    /// Attach this to a Game Object and write a comment...
    /// </summary>
    public class CommentComponent : MonoBehaviour
    {
        
#if UNITY_EDITOR
        
        [Multiline]
        [SerializeField] private string comment = "Comment here...";
        public string Comment
        {
            get => comment;
            set => comment = value; 
        }
        
#endif
        
    }
}
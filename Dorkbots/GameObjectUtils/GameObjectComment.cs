using UnityEngine;

namespace Dorkbots.GameObjectUtils
{
    /// <summary>
    /// Attach this to a Game Object, allows notes and comments
    /// </summary>
    public class GameObjectComment : MonoBehaviour
    {
        [TextArea]
        public string comment = "Comment Here."; // Enter comment in the Inspector.
    }
}
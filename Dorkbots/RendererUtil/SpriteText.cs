using UnityEngine;

namespace Dorkbots.RendererUtils
{
    [RequireComponent(typeof(TextMesh))]
    public class SpriteText : MonoBehaviour
    {
        [SerializeField] private string sortingLayerName;
        [SerializeField] private int sortingOrder;

        void Awake()
        {
            Renderer textRenderer = GetComponent<Renderer>();
            textRenderer.sortingLayerName = sortingLayerName;
            textRenderer.sortingOrder = sortingOrder;
        }
    } 
}
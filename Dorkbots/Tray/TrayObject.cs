using Dorkbots.Fractions;
using UnityEngine;
using UnityEngine.Rendering;

namespace Dorkbots.Tray
{
    public class TrayObject : MonoBehaviour 
	{
		//[SerializeField] private SpriteRenderer[] _spriteRenderer;
        [SerializeField] private FractionValues fractionValues = new FractionValues(1, 1);
        [SerializeField] private FractionValues dimensionFractionValues = new FractionValues(1, 1);
        [SerializeField] private GameObject _goForNoSpaceEffect;
        [SerializeField] private SortingGroup _renderSortingGroup;

		//public SpriteRenderer[] spriteRenderer { get { return _spriteRenderer; } }
		public Fraction dimensionFraction { get; protected set; }
		public Fraction fraction { get; protected set; }
        public GameObject goForNoSpaceEffect{ get { return _goForNoSpaceEffect; } }
        public SortingGroup renderSortingGroup { get { return _renderSortingGroup; } }

		protected uint[] blockSizes;

		/// <summary>
		/// Initialize the object
		/// </summary>
		public void InitFraction()
		{
            dimensionFraction = FractionTools.CreateFraction(dimensionFractionValues);
            fraction = FractionTools.CreateFraction(fractionValues);
		}
	}
}
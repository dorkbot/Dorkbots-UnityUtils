using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dorkbots.RendererTools
{
    public class ToggleMaterial : MonoBehaviour
    {
        [SerializeField] private bool addMaterialOnStart = false;
        [SerializeField] private Material addMaterial;
        [SerializeField] private Renderer[] addToRenders;

        private Dictionary<Renderer, Material> _rendererMaterialInstances = new Dictionary<Renderer, Material>();

        protected void Start()
        {
            if (addMaterialOnStart) AddMaterial();
        }

        protected void OnDisable()
        {
            RemoveMaterial();
        }

        protected void OnDestroy()
        {
            RemoveMaterial();
            addToRenders = null;
        }

        public void SetMaterial(Material material)
        {
            addMaterial = material;
        }

        public void SetRenderers(Renderer[] renderers)
        {
            addToRenders = renderers;
        }
        
        public void AddMaterial()
        {
            if (_rendererMaterialInstances.Count > 0)
            {
                return;
            }

            Renderer renderer;
            Material[] materialsArray;
            for (int i = 0; i < addToRenders.Length; i++)
            {
                renderer = addToRenders[i];
                materialsArray = new Material[renderer.materials.Length + 1];
                renderer.materials.CopyTo(materialsArray,0);
                materialsArray[materialsArray.Length - 1] = addMaterial;;
                renderer.materials = materialsArray;
                _rendererMaterialInstances.Add(renderer, renderer.materials[renderer.materials.Length - 1]);
            }
        }
 
        public void RemoveMaterial()
        {
            if (_rendererMaterialInstances.Count == 0)
            {
                return;
            }
            
            Renderer renderer;
            Material[] materialsArray;
            Material materialInstance;
            foreach (KeyValuePair<Renderer, Material> entry in _rendererMaterialInstances)
            {
                renderer = entry.Key;
                materialInstance = entry.Value;
                if (renderer.materials != null)
                {
                    materialsArray = new Material[renderer.materials.Length];
                    renderer.materials.CopyTo(materialsArray, 0);
                    int pos = Array.IndexOf(materialsArray, materialInstance);
                    if (pos >= 0)
                    {
                        List<Material> materialList = new List<Material>(materialsArray);
                        materialList.Remove(materialInstance);
                        materialsArray = materialList.ToArray();
                        renderer.materials = materialsArray;
                    }
                }
            }

            _rendererMaterialInstances.Clear();
        }

        public Material[] GetAllMaterialIntances()
        {
            if (_rendererMaterialInstances.Count == 0)
            {
                return Array.Empty<Material>();
            }
            
            return _rendererMaterialInstances.Values.ToArray();
        }
    }
}
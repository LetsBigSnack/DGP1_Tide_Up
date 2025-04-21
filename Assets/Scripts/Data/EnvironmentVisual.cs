using UnityEngine;

namespace Data
{
    [System.Serializable]
    public class EnvironmentVisual
    {
        [SerializeField] private EnvironmentState state;
        [SerializeField] private Material material;
        [SerializeField] private Mesh mesh;
    
        public EnvironmentState State
        {
            get => state;
            set => state = value;
        }

        public Material Material
        {
            get => material;
            set => material = value;
        }

        public Mesh Mesh
        {
            get => mesh;
            set => mesh = value;
        }
    }
}
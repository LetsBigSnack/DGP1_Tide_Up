using UnityEngine;

namespace Data
{
    [System.Serializable]
    public class EnvironmentVisual
    {
        [SerializeField] private EnvironmentState state;
        [SerializeField] private GameObject environmentObj;
    
        public EnvironmentState State
        {
            get => state;
            set => state = value;
        }

        public GameObject EnvironmentObj
        {
            get => environmentObj;
            set => environmentObj = value;
        }
    }
}
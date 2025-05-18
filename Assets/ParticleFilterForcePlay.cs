using UnityEngine;

public class ParticleFilterForcePlay : MonoBehaviour
{
    [SerializeField] private ParticleSystem filterL;
    [SerializeField] private ParticleSystem filterR;

    public void PlayFilterL()
    {
        filterL.Play();
    }

    public void PlayFilterR()
    {
        filterR.Play();
    }
}

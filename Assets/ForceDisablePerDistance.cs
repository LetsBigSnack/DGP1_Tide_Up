using UnityEngine;

public class ForceDisablePerDistance : MonoBehaviour
{
    private ParticleSystem particle;

    private void Start()
    {
        particle = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        float distance = Vector3.Distance(this.transform.position, Player.Instance.gameObject.transform.position);

        if(distance < 50f)
        {
            particle.playOnAwake = true;
        }
        else
        {
            particle.playOnAwake = false;
        }
    }
}

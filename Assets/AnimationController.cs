using UnityEngine;

public enum Animations
{
    Idle,
    Move,
    Fish,
    Dig,
    Pick
}

public class AnimationController : MonoBehaviour
{
    private float _velocity;

    public float Velocity
    {
        get => _velocity;
        set => _velocity = value;
    }

    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void EnableAnimation(Animations animation)
    {
        switch (animation)
        {
            case Animations.Idle:
                anim.SetBool("idle", true);
                break;
            case Animations.Move:
                anim.SetBool("move", true);
                anim.SetFloat("velocity", _velocity);
                break;
            case Animations.Fish:
                anim.SetBool("fish", true);
                break;
            case Animations.Dig:
                anim.SetBool("dig", true);
                break;
            case Animations.Pick:
                anim.SetBool("pick", true);
                anim.Play("PickUp");
                break;
        }
    }

    public void DisableAnimation(Animations animation) 
    {
        switch (animation)
        {
            case Animations.Idle:
                anim.SetBool("idle", false);
                break;
            case Animations.Move:
                anim.SetBool("move", false);
                anim.SetFloat("velocity", _velocity);
                break;
            case Animations.Fish:
                anim.SetBool("fish", false);
                break;
            case Animations.Dig:
                anim.SetBool("dig", false);
                break;
            case Animations.Pick:
                anim.SetBool("pick", false);
                break;
        }
    }

    public void PlayOnShotEmotion(Emotion emotion)
    {
        switch (emotion)
        {
            case Emotion.Angry:
                anim.SetTrigger("angry");
                break;
            case Emotion.Happy:
                anim.SetTrigger("happy");
                break;
            case Emotion.Thinking:
                anim.SetTrigger("thinking");
                break;
            case Emotion.Surprised:
                anim.SetTrigger("surpised");
                break;
            case Emotion.Sad:
                anim.SetTrigger("sad");
                break;
        }
    }
}

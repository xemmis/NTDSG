using UnityEngine;

public class AnimatorHandler : MonoBehaviour
{
    public int AnimState;
    public Animator Animator;

    private void Awake()
    {
        Animator = GetComponent<Animator>();
    }

    public virtual void HandleRun() => Animator.SetInteger("AnimState", 2);
    public virtual void HandleWalk() => Animator.SetInteger("AnimState", 1);
    public virtual void SetInt(string name, int value) => Animator.SetInteger(name, value);
    public virtual void SetBool(string name, bool value) => Animator.SetBool(name, value);
    public virtual void SetTrigger(string name) => Animator.SetTrigger(name);
}

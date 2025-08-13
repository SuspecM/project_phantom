using UnityEngine;

public class SaveIconManager : MonoBehaviour
{
    public Animator animator;

    public void PlayAnim()
    {
        animator.Play("save");
    }
}

using UnityEngine;

public class DiedPieceController : MonoBehaviour
{
    Animator animator;
    string currentAnim;
    void Start()
    {
        animator = GetComponent<Animator>();
        currentAnim = Idle();
        animator.SetBool(currentAnim, true);
    }

    void SwitchAnimation(string nextAnim)
    {
        animator.SetBool(currentAnim, false);
        currentAnim = nextAnim;
        animator.SetBool(currentAnim, true);
    }

    string Idle()
    {
        return "Idle" + Random.Range(0, 4).ToString();
    }

    public void RandomIdle()
    {
        SwitchAnimation(Idle());
    }
}
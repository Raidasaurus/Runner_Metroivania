using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    public Animator ani;
    Vector2 moveDir = new Vector2();


    private void Update()
    {

        moveDir = new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));

        if (moveDir != Vector2.zero)
            ani.SetBool("isMoving", true);
        else
            ani.SetBool("isMoving", false);

        ani.SetFloat("Horizontal", Input.GetAxis("Horizontal"));
        ani.SetFloat("Vertical", Input.GetAxis("Vertical"));
    }

    public void Jump()
    {
        ani.CrossFade("Jump", 0.001f);
    }

    public void LandSoft()
    {
        ani.CrossFade("Soft Landing", 0.001f);
    }

}

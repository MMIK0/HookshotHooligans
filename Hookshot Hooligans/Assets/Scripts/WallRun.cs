using System.Collections;
using UnityEngine;


public class WallRun : MonoBehaviour
{

    public bool isWallR = false;
    public bool isWallL = false;
    private RaycastHit hitR;
    private RaycastHit hitL;
    private int jumpCount = 0;
    public Rigidbody rb;
    public CharacterController cc;
    public Transform cameraEffect;
    public Animator anim;
    public bool canJump;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (cc.isGrounded)
        {
            jumpCount = 0;
            isWallL = false;
            isWallR = false;
        }
        if (isWallR == true && isWallL == false)
        {

            anim.SetBool("Left", true);
        }
        if (isWallR == false)
        {

            anim.SetBool("Left", false);
        }
        if (isWallL == false)
        {

            anim.SetBool("Right", false);
        }
        if (isWallR == false && isWallL == true)
        {

            anim.SetBool("Right", true);
        }

        if (canJump == true && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * 100, ForceMode.Impulse);
            if (isWallL == true)
            {
                Vector3 force = this.transform.right * 200;
                rb.AddForceAtPosition(force, this.transform.position, ForceMode.Impulse);
            }
            if (isWallR == true)
            {
                Vector3 force = -this.transform.right * 200;
                rb.AddForceAtPosition(force, this.transform.position, ForceMode.Impulse);
            }
        }

        if (!cc.isGrounded)
        {
            if (Physics.Raycast(transform.position, transform.right, out hitR, 1))
            {
                if (hitR.transform.tag == "Wall")
                {
                    canJump = true;
                    isWallR = true;
                    isWallL = false;
                    jumpCount += 1;

                    rb.useGravity = false;
                }
            }
            if (!Physics.Raycast(transform.position, transform.right, out hitR, 1))
            {

                isWallR = false;
                jumpCount += 1;
                if (isWallL == false)
                {
                    canJump = false;
                    rb.useGravity = true;
                }
            }
            if (Physics.Raycast(transform.position, -transform.right, out hitL, 1))
            {
                if (hitL.transform.tag == "Wall")
                {
                    canJump = true;
                    isWallL = true;
                    jumpCount += 1;
                    rb.useGravity = false;
                }
            }
            if (!Physics.Raycast(transform.position, -transform.right, out hitL, 1))
            {

                isWallL = false;
                jumpCount += 1;
                if (isWallR == false)
                {
                    canJump = false;
                    rb.useGravity = true;
                }
            }
        }
    }
}

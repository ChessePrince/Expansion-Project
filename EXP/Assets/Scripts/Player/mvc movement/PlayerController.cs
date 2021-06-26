using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerModel model;
    private void Start()
    {
        model.rb = GetComponent<Rigidbody>();
        model.rb.freezeRotation = true;
    }
    void Update()
    {
        Inputs();
        ControlDrag();
        Land();
        Steps();
        if (model.isGrounded && Input.GetKeyDown(model.jumpKey))
            Jump();
    }
    private void FixedUpdate()
    {
        Movement();
        model.isGrounded = Physics.CheckSphere(model.goGroundCheck.position, model.groundDistance, model.groundMask);
    }

    void Inputs()
    {
        model.inputHorizontal = Input.GetAxisRaw("Horizontal");
        model.inputVertical = Input.GetAxisRaw("Vertical");

        model.moveDirection = model.orientation.forward * model.inputVertical + model.orientation.right * model.inputHorizontal;
    }
    void Movement()
    {
        if (model.isGrounded)
        {
            model.rb.AddForce(model.moveDirection.normalized * model.walkSpeed * model.movementMultiplier, ForceMode.Acceleration);
            
        }
        else if (!model.isGrounded)
        {
            model.rb.AddForce(model.moveDirection.normalized * model.walkSpeed * model.movementMultiplier * model.airMultiplier, ForceMode.Acceleration);

            //model.rb.velocity = new Vector3(model.rb.velocity.x, 0, model.rb.velocity.z);
            model.rb.AddForce(transform.up * -2.5f, ForceMode.Acceleration);
        }
    }
    void ControlDrag()
    {
        if (model.isGrounded)
        {
            model.rb.drag = model.groundDrag;
        }
        else
        {
            model.rb.drag = model.airDrag;
            //model.rb.angularDrag = model.airDrag;
        }
    }
    void Jump()
    {
        if (model.isGrounded)
        {
            model.rb.velocity = new Vector3(model.rb.velocity.x, 0, model.rb.velocity.z);
            model.rb.AddForce(transform.up * model.jumpForce, ForceMode.Impulse);
            model.playerSfx.PlayJump();
        }
    }
    void Land()
    {
        if (model.isGrounded)
        {
            if (model.hasFallen)
            {
                model.playerSfx.PlayFall();
                //Debug.Log("fall");
                model.hasFallen = false;
            }
        }
        if (!model.isGrounded)
        {
            model.hasFallen = true;
        }
    }
    void Steps()
    {
        if (model.isGrounded && !model.playerSfx.audioSource.isPlaying)
        {
            if ((model.inputHorizontal != 0) || (model.inputVertical != 0))
                model.playerSfx.PlaySteps();
        }
    }
}

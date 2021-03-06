﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Cinemachine;

public class PlayerMovement : NetworkBehaviour
{
    TeamManager manager;

    private CharacterController controller;
    private Vector3 playerVelocity;
    public bool groundedPlayer, cursorVisible;
    public float oGSpeed;
    public float playerSpeed = 2.0f;
    public float jumpHeight = 1.0f;
    public float gravityValue = -9.81f;
    public float turnSpeed;
    public float xInput;
    public float yInput;

    public AudioClip[] stepClips;
    public AudioSource stepsAudio;
    public float stepInterval;
    float stepTimer;

    public GameObject model;
    public CinemachineFreeLook cam;

    public Animator myAnim;

    [SyncVar]
    public bool isWalking;

    //Evt legge til on start local player


    private void Start()
    {

        stepTimer = 0;
        manager = FindObjectOfType<TeamManager>();
        cam = GetComponentInChildren<CinemachineFreeLook>();
        if (!isLocalPlayer)
        {
            cam.gameObject.SetActive(false);
            GetComponentInChildren<Canvas>().gameObject.SetActive(false);
        }
        controller = gameObject.GetComponent<CharacterController>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
    }

    public override void OnStopClient()
    {
        manager.playersConnected.Remove(this);
    }

    void Update()
    {
        myAnim.SetBool("IsWalking", isWalking);
        if (isLocalPlayer)
        {
            if (oGSpeed < 4)
            {
                oGSpeed = playerSpeed;
            }
            GetInputs();
            RotatePlayer();
            MovePlayer();
            PlayerSteps();
            //if (xInput != 0 && yInput != 0)
            //{
            //    float playerS = oGSpeed / 2;
            //    playerSpeed = playerS;
            //}
            //else
            //{
            //    playerSpeed = oGSpeed;
            //}
            if (FindObjectOfType<TeamManager>().gameStarted)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    cursorVisible = !cursorVisible;
                }
                if (cursorVisible)
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
                else
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }
        }
    }

    public void PlayerSteps()
    {
        if (xInput != 0 || yInput != 0)
        {
            if (!isWalking)
            {
                isWalking = true;
                CmdIsWalking(true);
            }
            if (stepTimer <= 0)
            {
                stepsAudio.clip = stepClips[Random.Range(0, stepClips.Length)];
                stepsAudio.pitch = Random.Range(0.8f, 1.3f);
                stepsAudio.Play();
                stepTimer = stepInterval;
            }
            else
            {
                stepTimer -= Time.deltaTime;
            }
        }
        else if (isWalking)
        {
            isWalking = false;
            CmdIsWalking(false);
        }
    }

    [Command]
    public void CmdIsWalking(bool t)
    {
        isWalking = t;
    }

    [Command]
    public void CmdDoJump()
    {
        myAnim.SetTrigger("DoJump");
    }
    [ClientRpc]
    public void RpcDoJump()
    {
        myAnim.SetTrigger("DoJump");
    }

    void GetInputs()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");
    }

    void MovePlayer()
    {
        if (controller.isGrounded)
        {
            Vector3 forward = Camera.main.transform.TransformDirection(Vector3.forward);
            forward.y = 0;
            forward = forward.normalized;
            Vector3 right = new Vector3(forward.z, 0, -forward.x);
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            playerVelocity = (h * right + v * forward);

            if (Input.GetButton("Jump"))
            {
                myAnim.SetTrigger("DoJump");
                CmdDoJump();
                RpcDoJump();
                playerVelocity.y = jumpHeight;
            }
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * playerSpeed * Time.deltaTime);
    }

    void RotatePlayer()
    {
        if (yInput == 1 && xInput == 1)
        {
            GetRotation(new Vector3(-1f, 0f, -1f));
        }
        else if (yInput == 1 && xInput == -1)
        {
            GetRotation(new Vector3(1f, 0f, -1f));
        }
        else if (yInput == -1 && xInput == 1)
        {
            GetRotation(new Vector3(-1f, 0f, 1f));
        }
        else if (yInput == -1 && xInput == -1)
        {
            GetRotation(new Vector3(1f, 0f, 1f));
        }
        else if (yInput == 1)
        {
            GetRotation(new Vector3(0f, 0f, -1f));
        }
        else if (yInput == -1)
        {
            GetRotation(new Vector3(0f, 0f, 1f));
        }
        else if (xInput == 1)
        {
            GetRotation(new Vector3(-1f, 0f, 0f));
        }
        else if (xInput == -1)
        {
            GetRotation(new Vector3(1f, 0f, 0f));
        }
    }

    void GetRotation(Vector3 toRotation)
    {
        Vector3 relativePos = Camera.main.transform.TransformDirection(toRotation);
        relativePos.y = 0.0f;
        Quaternion rotation = Quaternion.LookRotation(-relativePos);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * turnSpeed);
    }
}

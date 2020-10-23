﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Mirror;

public class PlayerAnterrogationBehaviour : NetworkBehaviour
{
    public CinemachineFreeLook myCam;

    public Anterrogation manager;

    public PlayerTeam player;

    public bool playerInRange;

    public GameObject playerInRangeOf, policeUI;

    public Collider hitbox;

    public Animator myAnim;

    void Start()
    {
        policeUI.SetActive(false);
        myCam = GetComponentInChildren<CinemachineFreeLook>();
        manager = FindObjectOfType<Anterrogation>();
        if (isLocalPlayer)
            hitbox.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (player.myTeam == TeamManager.PlayerTeams.police)
        {
            if (isLocalPlayer && manager.ableToAnterrogate && playerInRange)
            {
                myAnim.SetBool("IsIdle", false);
            }
            else
            {
                myAnim.SetBool("IsIdle", true);
            }

            if (isLocalPlayer && Input.GetKeyDown(KeyCode.E) && playerInRange && manager.ableToAnterrogate)
            {
                Debug.Log("Started anterrogation with " + this.gameObject + " as police, and " + playerInRangeOf.gameObject + "as other player.");
                CmdCallArrogation(playerInRangeOf);
            }
        }
    }

    public void OpenUI()
    {
        policeUI.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void PoliceOptions(bool yes)
    {
        if (yes)
        {
            CmdSayYes();
        }
        else
        {
            CmdSayNo();
        }
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    [Command]
    void CmdSayYes()
    {
        RpcSayYes();
    }
    [Command]
    void CmdSayNo()
    {
        RpcSayNo();
    }

    [ClientRpc]
    public void RpcSayYes()
    {
        manager.ThrowInJail();
        policeUI.SetActive(false);
    }
    [ClientRpc]
    public void RpcSayNo()
    {
        manager.LetGo();
        policeUI.SetActive(false);
    }


    [Command]
    void CmdCallArrogation(GameObject otherP)
    {
        RpcCallArrogation(otherP);
    }

    [ClientRpc]
    void RpcCallArrogation(GameObject otherP)
    {
        manager.DoAnAnterrogation(this.gameObject, otherP);
    }

    public void ChangeCam(CinemachineVirtualCamera newCam, int priority)
    {
        if (isLocalPlayer)
            newCam.Priority = priority;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerHitbox"))
        {
            if (player.myTeam == TeamManager.PlayerTeams.police && other.gameObject.GetComponentInParent<PlayerTeam>().myTeam != TeamManager.PlayerTeams.police)
            {
                Debug.Log("Trigger");
                playerInRange = true;
                playerInRangeOf = other.gameObject.GetComponentInParent<PlayerAnterrogationBehaviour>().gameObject;
            }
        }
        else
        {
            playerInRange = false;
            playerInRangeOf = null;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerHitbox") && player.myTeam == TeamManager.PlayerTeams.police)
        {
            playerInRange = false;
            playerInRangeOf = null;
        }
    }
}

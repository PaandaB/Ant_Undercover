﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeamTest : MonoBehaviour
{
    TeamTesting manager;
    public TeamTesting.TestingTeams myTeam;
    public GameObject MAURMustache;
    MeshFilter mesh;

    Camera mainCam;

    public LayerMask normalMask, spyMask;

    private void Awake()
    {
        MAURMustache.SetActive(false);
        mainCam = Camera.main;
        mesh = GetComponentInChildren<MeshFilter>();
        Debug.Log(mesh, mesh.gameObject);
        manager = FindObjectOfType<TeamTesting>();
        Debug.Log(manager, manager.gameObject);
        int role = Random.Range(0, 3);
        if (role == 0)
        {
            myTeam = TeamTesting.TestingTeams.police;
            mesh.mesh = manager.teamMeshes[0];
            mainCam.cullingMask = normalMask;
            GetComponentInParent<PlayerTestingTask>().enabled = false;
        }
        else if (role == 1)
        {
            myTeam = TeamTesting.TestingTeams.civillian;
            mesh.mesh = manager.teamMeshes[1];
            mainCam.cullingMask = normalMask;
        }
        else
        {
            myTeam = TeamTesting.TestingTeams.spy;
            mesh.mesh = manager.teamMeshes[1];
            mainCam.cullingMask = spyMask;
            MAURMustache.SetActive(true);
        }
    }

    public void SwitchTeam(int team)
    {
        switch (team)
        {
            case 0:
                myTeam = TeamTesting.TestingTeams.police;
                mesh.mesh = manager.teamMeshes[0];
                mainCam.cullingMask = normalMask;
                GetComponentInParent<PlayerTestingTask>().enabled = false;
                MAURMustache.SetActive(false);
                break;
            case 1:
                myTeam = TeamTesting.TestingTeams.civillian;
                mesh.mesh = manager.teamMeshes[1];
                mainCam.cullingMask = normalMask;
                GetComponentInParent<PlayerTestingTask>().enabled = true;
                MAURMustache.SetActive(false);
                break;
            case 2:
                myTeam = TeamTesting.TestingTeams.spy;
                mesh.mesh = manager.teamMeshes[1];
                mainCam.cullingMask = spyMask;
                GetComponentInParent<PlayerTestingTask>().enabled = true;
                MAURMustache.SetActive(true);
                break;
        }
    }

    void Start()
    {
        manager.AddToList(this);
    }
}

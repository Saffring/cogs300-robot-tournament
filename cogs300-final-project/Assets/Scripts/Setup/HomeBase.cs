﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeBase : MonoBehaviour
{
    // Start is called before the first frame update
    public int team;
    private GameObject player;

    private List<Vector3> positionsInBase = new List<Vector3>();
    private List<bool> openSpots = new List<bool>();
    private List<GameObject> capturedTargets;
    private GameObject[] players;

    void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");

        player = players[team - 1];

        for (int i = 0; i < 9; i++){
            openSpots.Add(true);
        }

        float x = transform.localPosition.x;
        float z = transform.localPosition.z;
        float y = 0.5f;
        
        positionsInBase.Add(new Vector3(x + 3,y,z + 3));
        positionsInBase.Add(new Vector3(x + 3,y,z));
        
        positionsInBase.Add(new Vector3(x,y,z + 3));
        
        positionsInBase.Add(new Vector3(x + 3,y,z - 3));

        positionsInBase.Add(new Vector3(x,y,z));

        positionsInBase.Add(new Vector3(x - 3,y,z + 3));
        
        positionsInBase.Add(new Vector3(x,y,z - 3));
        
        positionsInBase.Add(new Vector3(x - 3,y,z));
        positionsInBase.Add(new Vector3(x - 3,y,z - 3));

        if(team == 1){
            positionsInBase.Reverse();
        }

        Material mat;
        if (team == 1) {
            mat = (Material) Resources.Load<Material>(WorldConstants.agent1ID + "/HomeBaseMat"); 
        }
        else {
            mat = (Material) Resources.Load<Material>(WorldConstants.agent2ID + "/HomeBaseMat"); 
        }
        gameObject.GetComponentInChildren<Renderer>().material = mat;
        capturedTargets = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Reset(){
        for (int i = 0; i < 9; i++){
            openSpots[i] = true;
        }
        capturedTargets.Clear();
    }

    public int AddToFirstSpotInBase(){
        for (int i = 0; i < 9; i++){
                if(openSpots[i]){
                    openSpots[i] = false;
                    return i;
                }
            }
        return -1;
    }

    public Vector3 GetPosition(int i){
        return positionsInBase[i];
    }

    void OnTriggerEnter(Collider collision){
        if (collision.gameObject.CompareTag("Player"))
        {
            MyAgent player = collision.gameObject.GetComponent<MyAgent>();
            if (player.GetTeam() == team){
            for (int i = player.GetCarrying() - 1; i > -1; i--)
                {
                    GameObject currentTarget = player.GetCarry(i);
                    capturedTargets.Add(currentTarget);
                    int spot = AddToFirstSpotInBase();
                    Vector3 position = GetPosition(spot);
                    currentTarget.GetComponent<Target>().AddToBase(spot, team, position);
                    player.RemoveCarry(currentTarget);

                }
            }
        }
    }
    void OnTriggerExit(Collider collision){
         if(collision.gameObject.CompareTag("Target")){ 
            capturedTargets.Remove(collision.gameObject);
            openSpots[collision.gameObject.GetComponent<Target>().GetSpotInBase()] = true;             
         }

         
    }

    // --------------GETTERS----------------
    public int GetCaptured() {return capturedTargets.Count;}
}
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerManager : MonoBehaviour
{
    public GameObject Player;
    public CinemachineVirtualCamera CarCamera, BoatCamera, HelicopterCamera;

    public GameObject PSpawnBoat, PSpawnCar, PSpawnHeli;

    public CarController carController;

    public GameObject PlayerMesh;
    public void EnterCar()
    {
        StartCoroutine(EnterCarFunc());
        
    }

    IEnumerator EnterCarFunc()
    {
        Player.GetComponent<PlayerController>().inVehicle = true;
        Player.GetComponent<PlayerController>().HideChar();        
        CarCamera.gameObject.SetActive(true);
        yield return null;
        carController.inCar = true;
        
    }
    public void ExitCar() 
    {
        StartCoroutine(ExitCarFunc());
    }
    IEnumerator ExitCarFunc()
    {       
             
        CarCamera.gameObject.SetActive(false );        
        carController.inCar = false;
        yield return null;
        Player.GetComponent <PlayerController>().UnhideChar();
        Player.GetComponent<PlayerController>().inVehicle = false;
        //Player.GetComponent<PlayerController>().BackToPlayer(PSpawnCar.transform);

        Debug.Log("Executed");

    }
}

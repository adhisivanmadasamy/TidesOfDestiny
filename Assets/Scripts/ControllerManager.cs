using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerManager : MonoBehaviour
{
    public GameObject Player;
    public CinemachineVirtualCamera CarCamera, BoatCamera, HeliCamera, TruckCamera;

    public GameObject PSpawnBoat, PSpawnCar, PSpawnHeli, PSpawnTruck;

    public CarController carController;
    public BoatController boatController;
    public HeliController heliController;
    public TruckController truckController;
    
    public Animator CharAnimator;

    public GameObject PlayerMesh;
    public void EnterCar()
    {
        StartCoroutine(EnterCarFunc());
        
    }

    IEnumerator EnterCarFunc()
    {
        Player.GetComponent<PlayerController>().inVehicle = true;
        Player.GetComponent<PlayerController>().inCar = true;
        Player.GetComponent<PlayerController>().HideChar();        
        CarCamera.gameObject.SetActive(true);
        yield return null;
        carController.inCar = true;
        
    }

    public void EnterTruck()
    {
        StartCoroutine(EnterTruckFunc());

    }

    IEnumerator EnterTruckFunc()
    {
        Player.GetComponent<PlayerController>().inVehicle = true;
        Player.GetComponent<PlayerController>().inTruck = true;
        Player.GetComponent<PlayerController>().HideChar();
        TruckCamera.gameObject.SetActive(true);
        yield return null;
        truckController.inTruck = true;

    }

    public void EnterBoat()
    {
        StartCoroutine (EnterBoatFunc());
    }
    IEnumerator EnterBoatFunc()
    {
        Player.GetComponent<PlayerController>().inVehicle = true;
        Player.GetComponent<PlayerController>().inBoat = true;
        Player.GetComponent<PlayerController>().HideChar();
        Player.GetComponent<PlayerController>().inWater = false;
        Player.GetComponent<PlayerController>().isUnderwater = false;
        Player.GetComponent<PlayerController>().isFloating = false;
        CharAnimator.SetInteger("WeaponState",0);
        BoatCamera.gameObject.SetActive(true);
        yield return null;
        boatController.inBoat = true;
    }

    public void EnterHeli() 
    {
        StartCoroutine(EnterHeliFunc());
    }
    IEnumerator EnterHeliFunc()
    {
        Player.GetComponent<PlayerController>().inVehicle = true;
        Player.GetComponent<PlayerController>().inHeli = true;
        Player.GetComponent<PlayerController>().HideChar();
        HeliCamera.gameObject.SetActive(true);
        yield return null;
        heliController.inHeli = true;
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
        Player.GetComponent<PlayerController>().inCar = false;
        //Player.GetComponent<PlayerController>().BackToPlayer(PSpawnCar.transform);

        Debug.Log("Executed");

    }

    public void ExitTruck()
    {
        StartCoroutine(ExitTruckFunc());
    }
    IEnumerator ExitTruckFunc()
    {

        TruckCamera.gameObject.SetActive(false);
        truckController.inTruck = false;
        yield return null;
        Player.GetComponent<PlayerController>().UnhideChar();
        Player.GetComponent<PlayerController>().inVehicle = false;
        Player.GetComponent<PlayerController>().inTruck = false;
        //Player.GetComponent<PlayerController>().BackToPlayer(PSpawnCar.transform);

        Debug.Log("Executed");

    }

    public void ExitBoat()
    {
        StartCoroutine(ExitBoatFunc());
    }
    IEnumerator ExitBoatFunc()
    {
        BoatCamera.gameObject.SetActive (false );
        boatController.inBoat=false;
        yield return null;
        Player.GetComponent<PlayerController>().UnhideChar();
        Player.GetComponent<PlayerController>().inVehicle = false;        
        Player.GetComponent<PlayerController>().inBoat = false;
        CharAnimator.SetInteger("WeaponState",0 );
        Player.GetComponent<PlayerController>().inWater = false;
        Player.GetComponent<PlayerController>().isUnderwater = false;
        Player.GetComponent<PlayerController>().isFloating = false;
    }

    public void ExitHeli()
    {
        StartCoroutine (ExitHeliFunc());
    }

    IEnumerator ExitHeliFunc()
    {
        HeliCamera.gameObject.SetActive(false);
        heliController.inHeli = false;
        yield return null;
        Player.GetComponent<PlayerController>().UnhideChar();
        Player.GetComponent<PlayerController>().inVehicle = false;
        Player.GetComponent<PlayerController>().inHeli = false;

    }
}

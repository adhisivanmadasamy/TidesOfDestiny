using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuScript : MonoBehaviour
{
    [SerializeField] GameObject PanelWeaponWheel;
    [SerializeField] GameObject HighlightSelected;
    public bool inWheel = false;

    public static MenuScript menu;

    [SerializeField] GameObject Crosshair;
    private void Awake()
    {
        menu = this;
    }
    public void WheelEnable()
    {
        PanelWeaponWheel.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        inWheel = true;
    }

    public void WheelDisable()
    {
        PanelWeaponWheel.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        inWheel= false;
    }


    private void Update()
    {
        WeaponChange();
        CrosshairVisibility();

    }

    public void CrosshairVisibility()
    {
        if (PlayerController.i.isAiming)
        {
            Crosshair.SetActive(true);
        }
        else
        {
            Crosshair.SetActive(false);
        }
    }
    public void WeaponChange()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            WheelEnable();
        }

        if (Input.GetKeyUp(KeyCode.Q))
        {
            WheelDisable();
        }

        RotateHightlight();
    }
    public void RotateHightlight()
    {
        Vector3 mousePosition = Input.mousePosition;

        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

        Vector3 direction = mousePosition - screenCenter;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                
        HighlightSelected.transform.rotation = Quaternion.Euler(0, 0, angle -90f);
    }
}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlanetResourceInfoPanel : MonoBehaviour
{
    PlanetResource planetResource;
    GameObject depositInfoPanel;
    
    public void SetVariables(PlanetResource rsrc, GameObject panel)
    {
        planetResource = rsrc;
        depositInfoPanel = panel;
    }
    
    private void OnMouseEnter()
    {
        DepositInfoPanel dip = depositInfoPanel.GetComponent<DepositInfoPanel>();
        dip.depositType.text = planetResource.resource.ToString();
        dip.depositQuantity.text = planetResource.resourceQuantity.ToString();

        depositInfoPanel.SetActive(true);
    }

    private void OnMouseOver()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            depositInfoPanel.SetActive(true);

            // track mouse
            Mouse mouse = Mouse.current;
            Vector3 mousePosition = mouse.position.ReadValue();
            mousePosition += depositInfoPanel.GetComponent<DepositInfoPanel>().infoPanelOffset;
            depositInfoPanel.transform.position = mousePosition;
            depositInfoPanel.GetComponent<DepositInfoPanel>().depositQuantity.text = planetResource.resourceQuantity.ToString("F0");
        }
        else
        {
            depositInfoPanel.SetActive(false);
        }
    }

    private void OnMouseExit()
    {
        depositInfoPanel.SetActive(false);
    }
}

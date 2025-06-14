using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeleteBuildingButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector]
    public Building building;

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        building.render.material.SetColor("_BaseColor", Color.red);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        building.render.material.SetColor("_BaseColor", building.ogColor);
    }
}

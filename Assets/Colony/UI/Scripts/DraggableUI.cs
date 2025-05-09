using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableUI : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    Vector2 dragOffset;
    float width;
    float height;

    void Start()
    {
        width = gameObject.GetComponent<RectTransform>().rect.width;
        height = gameObject.GetComponent<RectTransform>().rect.height;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Vector2 uiPosition = transform.position;
        dragOffset = eventData.position - uiPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position - dragOffset;

        // stop menus from being dragged off screen
        if (transform.position.x - width / 2 < 0)
            transform.position = new Vector3(width / 2, transform.position.y, transform.position.z);
        
        if (transform.position.x + width / 2 > Screen.width)
            transform.position = new Vector3(Screen.width - width / 2, transform.position.y, transform.position.z);

        if (transform.position.y - height / 2 < 0)
            transform.position = new Vector3(transform.position.x, height / 2, transform.position.z);

        if (transform.position.y + height / 2 > Screen.height)
            transform.position = new Vector3(transform.position.x, Screen.height - height / 2, transform.position.z);
    }
}

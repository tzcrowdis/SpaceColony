using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class LockScrollOverScrollableUI : MonoBehaviour
{
    [Header("Scrollable UI")]
    public GameObject scrollView;

    private void Update()
    {
        DisableOtherScrollControlsOverScrollView();
    }

    void DisableOtherScrollControlsOverScrollView()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);

        if (raycastResults.Count > 0)
        {
            foreach (RaycastResult result in raycastResults)
                if (result.gameObject == scrollView) 
                    ColonyControls.instance.altitudeLock = true;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ColonistList : MonoBehaviour
{
    [Header("List Menu")]
    public GameObject scrollView;
    public Transform contentContainer;

    [Header("List Item")]
    public GameObject colonistListItemPrefab;

    public static ColonistList instance { get; private set; }
    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

    void Start()
    {
        
    }

    void Update()
    {
        DisableOtherScrollControlsOverScrollView();
    }

    public ColonistListItem AddColonistToList(Colonist clnst)
    {
        GameObject colonistListItem = Instantiate(colonistListItemPrefab, contentContainer);
        colonistListItem.transform.GetChild(0).GetComponent<Image>().sprite = clnst.headshot;
        colonistListItem.transform.GetChild(1).GetComponent<TMP_Text>().text = clnst.characterName;
        colonistListItem.GetComponent<ColonistListItem>().colonist = clnst;
        return colonistListItem.GetComponent<ColonistListItem>();
    }

    void DisableOtherScrollControlsOverScrollView() // NOTE locks scroll for all UI not just scrollable...
    {
        bool overScrollableUI = false;
        
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);

        if (raycastResults.Count > 0)
        {
            foreach (RaycastResult result in raycastResults)
                if (result.gameObject == scrollView) overScrollableUI = true;
        }

        if (overScrollableUI)
            ColonyControls.instance.altitudeLock = true;
        else
            ColonyControls.instance.altitudeLock = false;
    }
}

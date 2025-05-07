using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingList : MonoBehaviour
{
    [Header("List Menu")]
    public Transform contentContainer;

    [Header("List Item")]
    public GameObject listItemPrefab;

    public static BuildingList instance { get; private set; }
    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

    public BuildingListItem AddBuildingToList(Building bldg)
    {
        GameObject buildingListItem = Instantiate(listItemPrefab, contentContainer);
        buildingListItem.transform.GetChild(0).GetComponent<Image>().sprite = bldg.icon;
        buildingListItem.transform.GetChild(1).GetComponent<TMP_Text>().text = bldg.title;
        buildingListItem.GetComponent<BuildingListItem>().building = bldg;
        return buildingListItem.GetComponent<BuildingListItem>();
    }
}

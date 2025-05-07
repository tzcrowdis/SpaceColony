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

    public ColonistListItem AddColonistToList(Colonist clnst)
    {
        GameObject colonistListItem = Instantiate(colonistListItemPrefab, contentContainer);
        colonistListItem.transform.GetChild(0).GetComponent<Image>().sprite = clnst.headshot;
        colonistListItem.transform.GetChild(1).GetComponent<TMP_Text>().text = clnst.characterName;
        colonistListItem.GetComponent<ColonistListItem>().colonist = clnst;
        return colonistListItem.GetComponent<ColonistListItem>();
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BuildingPanel : MonoBehaviour
{
    public Button[] buildings;
    List<Button> unlockedBuildings;

    public float height;
    public float gapWidth;
    
    void Start()
    {
        foreach (Button b in buildings)
        {
            b.gameObject.SetActive(false);
        }

        UpdateUnlockedBuildingsArray();
    }

    public void UpdateUnlockedBuildingsArray()
    {
        // update list of unlocked buildings
        unlockedBuildings = new List<Button>();
        foreach (Button building in buildings)
        {
            if (building.GetComponent<BuildingButton>().unlocked)
            {
                building.gameObject.SetActive(true);
                unlockedBuildings.Add(building);
            }
            else
                building.gameObject.SetActive(false);
        }

        // early exit if no unlocked buildings
        if (unlockedBuildings.Count == 0)
            return;

        // update panel positioning
        unlockedBuildings[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(100, height);
        for (int i = 1; i < unlockedBuildings.Count; i++)
        {
            Vector2 prevPos = unlockedBuildings[i - 1].GetComponent<RectTransform>().anchoredPosition;
            unlockedBuildings[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(prevPos.x + gapWidth, height);
        }
    }
}

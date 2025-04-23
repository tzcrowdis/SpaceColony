using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollRectNoItemDrag : ScrollRect
{
    public override void OnDrag(PointerEventData eventData)
    {
        // do nothing
    }
}

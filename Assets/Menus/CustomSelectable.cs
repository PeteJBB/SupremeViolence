using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CustomSelectable : Selectable
{
    public override Selectable FindSelectableOnUp()
    {
        var sel = GetSelectablesInScope().OrderBy(x => x.transform.position.y).ToList();
        var index = Mathf.Clamp(sel.IndexOf(this) + 1, 0, sel.Count - 1);
        return sel[index];
    }

    public override void OnMove(AxisEventData eventData)
    {
        Debug.Log("OnMove");
        base.OnMove(eventData);
    }

    public override Selectable FindSelectableOnDown()
    {
        var sel = GetSelectablesInScope().OrderBy(x => x.transform.position.y).ToList();
        var index = Mathf.Clamp(sel.IndexOf(this) - 1, 0, sel.Count - 1);
        return sel[index];
    }

    public override Selectable FindSelectableOnLeft()
    {
        var sel = GetSelectablesInScope().OrderBy(x => x.transform.position.x).ToList();
        var index = Mathf.Clamp(sel.IndexOf(this) - 1, 0, sel.Count - 1);
        return sel[index];
    }
    public override Selectable FindSelectableOnRight()
    {
        var sel = GetSelectablesInScope().OrderBy(x => x.transform.position.x).ToList();
        var index = Mathf.Clamp(sel.IndexOf(this) + 1, 0, sel.Count - 1);
        return sel[index];
    }

    private List<Selectable> GetSelectablesInScope()
    {
        var canvas = GetCanvasForScope();
        return canvas.gameObject.GetComponentsInChildren<Selectable>().Where(x => x.IsInteractable()).ToList();
    }

    private Canvas GetCanvasForScope()
    {
        var parent = transform.parent;
        while(parent != null)
        {
            var c = parent.GetComponent<Canvas>();
            if(c != null)
                return c;

            parent = parent.parent;
        }

        return null;
    }
}
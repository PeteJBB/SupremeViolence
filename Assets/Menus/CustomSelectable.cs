using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CustomSelectable : Selectable
{
    public override void OnMove(AxisEventData eventData)
    {
        Debug.Log("OnMove");
        base.OnMove(eventData);
    }

    public override Selectable FindSelectableOnUp()
    {
        var options = GetSelectablesInScope();
        var sel = options
            .Where(x => x.transform.position.y > transform.position.y)
            .OrderBy(x => CalculateMoveScore(x.transform.position, false))
            .ToList();

        return sel.FirstOrDefault();
    }

    public override Selectable FindSelectableOnDown()
    {
        var options = GetSelectablesInScope();
        var sel = options
            .Where(x => x.transform.position.y < transform.position.y)
            .OrderBy(x => CalculateMoveScore(x.transform.position, false))
            .ToList();

        return sel.FirstOrDefault();
    }

    public override Selectable FindSelectableOnLeft()
    {
        var options = GetSelectablesInScope();
        var sel = options
            .Where(x => x.transform.position.x < transform.position.x)
            .OrderBy(x => CalculateMoveScore(x.transform.position, true))
            .ToList();

        return sel.FirstOrDefault();
    }

    public override Selectable FindSelectableOnRight()
    {
       var options = GetSelectablesInScope();
       var sel = options
            .Where(x => x.transform.position.x > transform.position.x)
            .OrderBy(x => CalculateMoveScore(x.transform.position, true))
            .ToList();

        return sel.FirstOrDefault();
    }

    private float CalculateMoveScore(Vector3 pos, bool xaxis)
    {
        var offset = pos - transform.position;
        if (xaxis)
            offset.y *= 2;
        else
            offset.y *= 1;

        return offset.sqrMagnitude;
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
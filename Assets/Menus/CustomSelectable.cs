using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CustomSelectable : Selectable
{
    public Color BackgroundColor = new Color(0,0,0,0);
    public Color HighlightedBackgroundColor = Color.white;
    public bool UseBackgroundColor = true;

    [HideInInspector]
    public bool IsSelected;

    [HideInInspector]
    public Image background;
    
    public override void OnMove(AxisEventData eventData)
    {
        //Debug.Log("OnMove");
        base.OnMove(eventData);
    }

    public override Selectable FindSelectableOnUp()
    {
        if(navigation.mode == Navigation.Mode.Horizontal)
            return this;

        var options = GetSelectablesInScope();
        var sel = options
            .Where(x => x.transform.position.y > transform.position.y)
            .OrderBy(x => CalculateMoveScore(x, MoveDir.Up))
            .ToList();

        return sel.FirstOrDefault();
    }

    public override Selectable FindSelectableOnDown()
    {
        if(navigation.mode == Navigation.Mode.Horizontal)
            return this;

        var options = GetSelectablesInScope();
        var sel = options
            .Where(x => x.transform.position.y < transform.position.y)
            .OrderBy(x => CalculateMoveScore(x, MoveDir.Up))
            .ToList();

        return sel.FirstOrDefault();
    }

    public override Selectable FindSelectableOnLeft()
    {
        if(navigation.mode == Navigation.Mode.Vertical)
            return this;

        var options = GetSelectablesInScope();
        var sel = options
            .Where(x => x.transform.position.x < transform.position.x)
            .OrderBy(x => CalculateMoveScore(x, MoveDir.Left))
            .ToList();

        return sel.FirstOrDefault();
    }

    public override Selectable FindSelectableOnRight()
    {
        if(navigation.mode == Navigation.Mode.Vertical)
            return this;

       var options = GetSelectablesInScope();
       var sel = options
            .Where(x => x.transform.position.x > transform.position.x)
            .OrderBy(x => CalculateMoveScore(x, MoveDir.Right))
            .ToList();

        return sel.FirstOrDefault();
    }

    private float CalculateMoveScore(Selectable sel, MoveDir moveDir)
    {
        //var myRt = gameObject.GetComponent<RectTransform>();
        //var rt = sel.gameObject.GetComponent<RectTransform>();

        //Vector2 start;
        //Vector2 end;

        //switch (moveDir)
        //{
        //    case MoveDir.Up:
        //        start = new Vector2(myRt.rect.center.x, myRt.rect.center.y);
        //        end = rt.rect.min;
        //        break;
        //}
        
        var offset = sel.transform.position - transform.position;
        if (navigation.mode == Navigation.Mode.Vertical)
            offset.x = 0;
        else if(navigation.mode == Navigation.Mode.Horizontal)
            offset.y = 0;
        else if(moveDir == MoveDir.Left || moveDir == MoveDir.Right)
            offset.y *= 5;
        else
            offset.x *= 5;

        return offset.sqrMagnitude;
    }

    private enum MoveDir
    {
        Up,
        Down,
        Left,
        Right
    }

    private List<CustomSelectable> GetSelectablesInScope()
    {
        var canvas = GetCanvasForScope();
        return canvas.gameObject.GetComponentsInChildren<CustomSelectable>().Where(x => x.IsInteractable()).ToList();
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

    public override void OnSelect(BaseEventData eventData)
    {
        if (UseBackgroundColor)
        {
            if (background == null)
                CreateBackgroundElem();

            iTween.ValueTo(background.gameObject, iTween.Hash("from", 0, "to", 1, "time", 0.1f, "onupdate", (System.Action<object>)((obj) =>
            {
                var val = (float)obj;
                var col = background.color;
                col.r = Mathf.Lerp(BackgroundColor.r, HighlightedBackgroundColor.r, val);
                col.g = Mathf.Lerp(BackgroundColor.g, HighlightedBackgroundColor.g, val);
                col.b = Mathf.Lerp(BackgroundColor.b, HighlightedBackgroundColor.b, val);
                col.a = Mathf.Lerp(BackgroundColor.a, HighlightedBackgroundColor.a, val);
                background.color = col;
            })));
            //background.color = HighlightedBackgroundColor; 
        }

        foreach (var text in Helper.GetComponentsInChildrenRecursive<Text>(transform))
        {
            text.color = colors.highlightedColor;
        }

        IsSelected = true;
        base.OnSelect(eventData);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        if (UseBackgroundColor)
        {
            if (background == null)
                CreateBackgroundElem();

            iTween.ValueTo(background.gameObject, iTween.Hash("from", 0, "to", 1, "time", 0.1f, "onupdate", (System.Action<object>)((obj) =>
            {
                var val = (float)obj;
                var col = background.color;
                col.r = Mathf.Lerp(HighlightedBackgroundColor.r, BackgroundColor.r, val);
                col.g = Mathf.Lerp(HighlightedBackgroundColor.g, BackgroundColor.g, val);
                col.b = Mathf.Lerp(HighlightedBackgroundColor.b, BackgroundColor.b, val);
                col.a = Mathf.Lerp(HighlightedBackgroundColor.a, BackgroundColor.a, val);
                background.color = col;
            })));
            

            foreach (var text in Helper.GetComponentsInChildrenRecursive<Text>(transform))
            {
                text.color = colors.normalColor;
            }
        }

        IsSelected = false;
        base.OnDeselect(eventData);
    }

    public void CreateBackgroundElem()
    {
        background = new GameObject().AddComponent<Image>();
        background.gameObject.name = "background";
        background.transform.SetParent(transform.parent);
        background.transform.SetAsFirstSibling();

        var thisRect = GetComponent<RectTransform>();

        var rectTrans = background.GetComponent<RectTransform>();
        rectTrans.anchorMin = thisRect.anchorMin;
        rectTrans.anchorMax = thisRect.anchorMax;
        rectTrans.offsetMin = thisRect.offsetMin;
        rectTrans.offsetMax = thisRect.offsetMax;

        background.sprite = Resources.Load<Sprite>("Textures/square");
        background.color = BackgroundColor;
        
    }

    float lastBgFlashTime = 0;
    public void FlashBackground()
    {
        if (background == null)
            CreateBackgroundElem();

        var time = Time.time;
        lastBgFlashTime = time;
                
        background.color = HighlightedBackgroundColor;

        var texts = Helper.GetComponentsRecursive<Text>(transform);
        foreach (var t in texts)
        {
            t.color = colors.highlightedColor;
        }

        Helper.Instance.WaitAndThenCall(0.1f, () =>
        {
            if (lastBgFlashTime == time)
            {
                background.color = BackgroundColor;
                foreach (var t in texts)
                {
                    t.color = colors.normalColor;
                }
            }
        });
    }
}
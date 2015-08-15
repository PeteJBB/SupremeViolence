using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CustomButton : CustomSelectable, ISubmitHandler
{
    public UnityEvent OnClick;

    public virtual void OnSubmit (BaseEventData eventData)
    {
        this.DoStateTransition (Selectable.SelectionState.Pressed, false);
        if(OnClick != null)
            OnClick.Invoke();
    }
}
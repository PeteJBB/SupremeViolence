using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class KeepOnTop : MonoBehaviour 
{
	void Update () 
	{
        if(transform.GetSiblingIndex() != transform.parent.childCount - 1)
            transform.SetAsLastSibling();
	}
}

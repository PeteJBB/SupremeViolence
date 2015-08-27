using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public interface IReflector
{
    bool DoesReflectMe(GameObject obj);
}
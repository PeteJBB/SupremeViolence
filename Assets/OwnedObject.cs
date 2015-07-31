using UnityEngine;
using System.Collections;

public class OwnedObject : MonoBehaviour, IOwnedObject
{
    public GameObject Owner { get; set; }
}

public interface IOwnedObject
{
    GameObject Owner { get; set; }
}

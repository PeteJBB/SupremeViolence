using UnityEngine;
using UnityEditor;
using System.Collections;

[ExecuteInEditMode]
public class PrefabLoader: MonoBehaviour 
{
    public GameObject Prefab;
    public Orientation Orientation;

    private GameObject instance;

    void Awake()
    {
        LoadPrefab();
    }

    [ContextMenu("Update Prefab")]
	public void LoadPrefab()
    {
        // delete the old one first
        var t = transform.Find("instance");
        if(t != null)
        {
            if(GameBrain.IsEditMode())
                DestroyImmediate(t.gameObject);
            else
                Destroy(t.gameObject);
        }

        instance = Instantiate(Prefab);
        instance.name = "instance";
        instance.transform.SetParent(transform);
        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = Quaternion.identity;

        Helper.SetHideFlags(instance.gameObject, HideFlags.HideInHierarchy);

        instance.BroadcastMessage("SetOrientation", Orientation, SendMessageOptions.DontRequireReceiver);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0,0,0,0.01f);
        Gizmos.DrawCube(transform.position, transform.localScale * 0.3f);
    }
}
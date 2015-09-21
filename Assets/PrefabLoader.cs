using UnityEngine;
//using UnityEditor;
using System.Collections;

[ExecuteInEditMode]
public class PrefabLoader : MonoBehaviour
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
        Helper.SetHideFlags(gameObject, HideFlags.None);

        // delete the old one first
        Helper.DestroyAllChildren(transform, Helper.IsEditMode());

        instance = Instantiate(Prefab);
        instance.name = "instance";
        instance.transform.SetParent(transform);
        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = Quaternion.identity;
        instance.transform.localScale = Prefab.transform.localScale;

        Helper.SetHideFlags(instance.gameObject, HideFlags.HideInHierarchy);

        instance.BroadcastMessage("SetOrientation", Orientation, SendMessageOptions.DontRequireReceiver);
    }

    [ContextMenu("Show Instance")]
    public void ShowInstance()
    {
        Helper.SetHideFlags(gameObject, HideFlags.None);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 0, 0.01f);
        Gizmos.DrawCube(transform.position, transform.localScale * 0.3f);
    }
}
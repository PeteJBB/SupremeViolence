using UnityEngine;
using UnityEditor;
using System.Collections;

public class PrefabLoader: MonoBehaviour 
{
    public GameObject Prefab;
    private GameObject instance;

    void Start()
    {
        LoadPrefab();
    }

    [ContextMenu("Reload Prefab")]
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
        instance.transform.parent = transform;
        instance.transform.localPosition = Vector3.zero;
    }
}
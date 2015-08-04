using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MiniMap : MonoBehaviour 
{
    public GameObject GridSquarePrefab;
    public GameObject[,] GridMap;

    private float mapSize = 120;

	// Use this for initialization
	void Start () 
    {
        var gridSize = mapSize / Arena.Instance.ArenaSizeY;
        GridMap = new GameObject[Arena.Instance.ArenaSizeX, Arena.Instance.ArenaSizeY];

        // create the minimap grid squares to match arena size
        for(var x = 0; x < Arena.Instance.ArenaSizeX; x++)
        {
            for(var y = 0; y < Arena.Instance.ArenaSizeY; y++)
            {
                var sq = (GameObject)Instantiate(GridSquarePrefab);
                sq.transform.parent = transform;
                var rect = sq.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(gridSize, gridSize);
                var posx = (x * gridSize) + (x); //<-- leave a gap of 1px for every sq except the first one
                var posy = (y * gridSize) + (y); //<-- leave a gap of 1px for every sq except the first one
                sq.transform.localPosition = new Vector3(posx, posy, 0);

                GridMap[x,y] = sq;
            }
        }

        var myrect = GetComponent<RectTransform>();
        myrect.sizeDelta = new Vector2((Arena.Instance.ArenaSizeX * gridSize) + Arena.Instance.ArenaSizeX, (Arena.Instance.ArenaSizeY * gridSize) + Arena.Instance.ArenaSizeY);
        myrect.localPosition = new Vector3(-myrect.sizeDelta.x / 2, myrect.localPosition.y + 16, 0);

        Arena.Instance.OnGridContentsChanged += Arena_GridContentsChanged;
	}

    public void Arena_GridContentsChanged(int gridX, int gridY, List<GameObject> objList)
    {
        var sq = GridMap[gridX, gridY];
        var img = sq.GetComponent<Image>();

        if(objList.Count == 0)
            img.color = new Vector4(0,0,0,0.5f);

        foreach(var o in objList)
        {
            if(o.tag == "Wall")
            {
                img.color = new Vector4(0.33f,0.33f,0.33f,0.5f);
            }
            else if(o.tag == "Player")
            {
                img.color = Color.red;
            }
            else if(o.GetComponent<Pickup>() != null)
            {
                img.color = Color.yellow;
            }
            else
            {
                img.color = new Vector4(0,0,0,0.5f);
            }
        }

    }

	// Update is called once per frame
	void Update () 
    {
	    
	}
}

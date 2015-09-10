using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public class MiniMap : MonoBehaviour 
{
    public GameObject GridSquarePrefab;
    public GameObject[,] GridMap;

    private float mapSize = 120;

	// Use this for initialization
	void Start () 
    {        
        var arenaSize = Arena.Instance.GetArenaSize();
        var gridSize = mapSize / arenaSize.y;
        GridMap = new GameObject[(int)arenaSize.x, (int)arenaSize.y];
        
        // create the minimap grid squares to match arena size
        for(var x = 0; x < arenaSize.x; x++)
        {
            for(var y = 0; y < arenaSize.y; y++)
            {
                var sq = (GameObject)Instantiate(GridSquarePrefab);
                sq.transform.SetParent(transform);
                var rect = sq.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(gridSize, gridSize);
                var posx = (x * gridSize) + (x); //<-- leave a gap of 1px for every sq except the first one
                var posy = (y * gridSize) + (y); //<-- leave a gap of 1px for every sq except the first one
                sq.transform.localPosition = new Vector3(posx, posy, 0);
                
                GridMap[x,y] = sq;
            }
        }
        
        var myrect = GetComponent<RectTransform>();
        myrect.sizeDelta = new Vector2((arenaSize.x * gridSize) + arenaSize.x, (arenaSize.y * gridSize) + arenaSize.y);
        myrect.localPosition = new Vector3(-myrect.sizeDelta.x / 2, myrect.localPosition.y + 16, 0);

        Arena.Instance.OnGridContentsChanged.AddListener(GridContentsChanged);
    }

    public void GridContentsChanged(GridSquareInfo info)
    {
        var sq = GridMap[info.x, info.y];
        var img = sq.GetComponent<Image>();

        var obj = info.Objects.OrderByDescending(x => x.MinimapPriority).FirstOrDefault();
        if(obj == null)
            img.color = new Vector4(0,0,0,0.75f);
        else
            img.color = obj.MapColor;

        //foreach(var o in info.Objects)
        //{
        //    if (o == null)
        //        continue;

            //if(o.tag == "Wall")
            //{
            //    img.color = new Vector4(0.33f,0.33f,0.33f,0.5f);
            //    break;
            //}
            //else if(o.tag == "Player")
            //{
            //    img.color = Color.red;
            //    break;
            //}
            //else if(o.GetComponent<PickupIcon>() != null)
            //{
            //    img.color = Color.yellow;
            //}
            //else
            //{
            //    img.color = new Vector4(0,0,0,0.5f);
            //}
        //}

    }

	// Update is called once per frame
	void Update () 
    {
	    
	}
}

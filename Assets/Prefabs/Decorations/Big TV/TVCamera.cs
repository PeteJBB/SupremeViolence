//using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class TVCamera: MonoBehaviour 
{
    public SpriteRenderer tvscreen;
    public int textureWidth;
    public int textureHeight;

    private Camera camera;
    private GameObject trackTarget;

    private RenderTexture renderTexture;
    private Texture2D screenTexture;

    private float switchTargetInterval = 5;
    private float lastSwitchTargetTime;

	void Awake() 
    {
        renderTexture = new RenderTexture(textureWidth, textureHeight, 16);
        screenTexture = new Texture2D(textureWidth, textureHeight);
        
        tvscreen.sprite = Sprite.Create(screenTexture, new Rect(0, 0, textureWidth, textureHeight), new Vector2(0.5f, 0f), 128);

        camera = GetComponent<Camera>();
        camera.targetTexture = renderTexture;
	}

	void Update() 
    {
        if (trackTarget == null || Time.time - lastSwitchTargetTime > switchTargetInterval)
        {
            FindNewTarget();
        }

        if (trackTarget != null)
        {
            var pos = trackTarget.transform.position;
            pos.z = -10;
            camera.transform.position = pos;
        }
	}

    private void FindNewTarget()
    {
        var potentialTargets = GameObject.FindGameObjectsWithTag("Player");
        trackTarget = potentialTargets[Random.Range(0, potentialTargets.Length)];
        camera.orthographicSize = Random.Range(0.1f, 1f);

        lastSwitchTargetTime = Time.time;
    }

    void OnPostRender() 
    {
        RenderTexture.active = renderTexture;
        screenTexture.ReadPixels(new Rect(0, 0, textureWidth, textureHeight), 0, 0, false);
        screenTexture.Apply();
        RenderTexture.active = null;
    }

    void OnDrawGizmos()
    {
        if (tvscreen != null)
        {
            Gizmos.color = Color.white;
            var pos = tvscreen.transform.position;
            pos.y += (textureHeight / 256f);
            Gizmos.DrawWireCube(pos, new Vector3(textureWidth / 128f, textureHeight / 128f, 0.1f));
        }
    }
}
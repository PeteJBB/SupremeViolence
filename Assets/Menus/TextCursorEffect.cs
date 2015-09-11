using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TextCursorEffect : MonoBehaviour
{
    public Color CursorColor = new Color(1,1,1,1);

    private Image image;
    private Text text;
    private RectTransform rt;

    private float blinkTime = 0.5f;
    private float lastBlink;

    // Use this for initialization
    void Awake()
    {
        text = GetComponent<Text>();

        image = new GameObject().AddComponent<Image>();
        image.sprite = Resources.Load<Sprite>("Textures/square");
        image.transform.SetParent(transform);
        image.color = CursorColor;

        rt = image.GetComponent<RectTransform>();
    }

    private void SetPosition()
    {
        var left = text.text.Length * text.fontSize;

        rt.anchorMin = new Vector2(0,0);
        rt.anchorMax = new Vector2(0,1);
        rt.offsetMin = new Vector2(left, 0);
        rt.offsetMax = new Vector2(left + text.fontSize,0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastBlink >= blinkTime)
        {
            lastBlink = Time.time;

            if (image.color.a == 0)
                image.color = CursorColor;
            else
                image.color = new Color(0, 0, 0, 0);

        }
        SetPosition();
    }
}

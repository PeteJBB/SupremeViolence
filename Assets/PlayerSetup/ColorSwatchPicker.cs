using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ColorSwatchPicker : MonoBehaviour
{
    public GameObject SwatchPrefab;
    public Color[] Colors;

    void Start()
    {
        foreach (var c in Colors)
        {
            AddSwatch(c);
        }

        var playerIndex = Helper.GetComponentInParentsRecursive<CustomMenuInputController>(transform).PlayerIndex;
        switch (playerIndex)
        {
            case 0:
                SetColor(new Color(0, 0.5f, 1)); // blue
                break;
            case 1:
                SetColor(Color.red); // red
                break;
            case 2:
                SetColor(new Color(0, 1, 0)); // green
                break;
            case 3:
                SetColor(new Color(0.7f, 0, 1)); // purple
                break;
        }
    }

    private void AddSwatch(Color c)
    {
        var swatch = Instantiate(SwatchPrefab);
        swatch.transform.SetParent(transform);

        var img = swatch.transform.Find("selectable/image").GetComponent<Image>();
        img.color = c;

        var btn = swatch.GetComponentInChildren<CustomButton>();
        btn.OnClick.AddListener(() => SetColor(img.color));
    }

    public void SetColor(Color c)
    {
        transform.parent.Find("Torso").GetComponent<Image>().color = c;
    }
}

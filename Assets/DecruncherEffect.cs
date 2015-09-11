using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DecruncherEffect : MonoBehaviour
{
    public UnityEvent OnDecrunchComplete;

    Sprite square;
    List<Image> images = new List<Image>();

    float minHeight = 0.01f;
    float maxHeight = 0.05f;
    float loadTime = 5;

    bool hasEnded = false;

    void Start()
    {
        square = Resources.Load<Sprite>("Textures/square");

        var y = 0f;
        while (y < 1)
        {
            var img = CreateNewImage();
            var h = RandomizeImage(img, y);
            y += h;
        }

        iTween.ValueTo(gameObject, iTween.Hash("from", 0, "to", 1, "time", loadTime / 2f, "easetype", iTween.EaseType.easeInCubic, "onupdate", (System.Action<object>)((obj) =>
        {
            var val = (float)obj;
            //minHeight = Mathf.Lerp(0.01f, 0.5f, val);
            //maxHeight = Mathf.Lerp(0.05f, 1f, val);
            minHeight = Mathf.Lerp(0.01f, 0.001f, val);
            maxHeight = Mathf.Lerp(0.05f, 0.005f, val);
        }), 
        "oncomplete", (System.Action)(() =>
        {
            iTween.ValueTo(gameObject, iTween.Hash("from", 0, "to", 1, "time", loadTime / 2f, "easetype", iTween.EaseType.easeInCubic, "onupdate", (System.Action<object>)((obj) =>
            {
                var val = (float)obj;
                minHeight = Mathf.Lerp(0.001f, 0.5f, val);
                maxHeight = Mathf.Lerp(0.005f, 1f, val);
            }), 
            "oncomplete", (System.Action)(End)));
        })));
    }

    private void End()
    {
        hasEnded = true;
        Helper.DestroyAllChildren(transform);

        if (OnDecrunchComplete != null)
            OnDecrunchComplete.Invoke();
    }

    private Image CreateNewImage()
    {
        var img = new GameObject().AddComponent<Image>();
        img.transform.SetParent(transform);
        img.sprite = square;
        img.color = new Color(Random.value, Random.value, Random.value, 1);

        images.Add(img);

        return img;
    }

    void Update()
    {
        if (!hasEnded)
        {
            // randomize list
            images = images.OrderBy(x => System.Guid.NewGuid()).ToList();

            var y = 0f;
            foreach (var img in images)
            {
                var h = RandomizeImage(img, y);
                y += h;
            }

            while (y < 1)
            {
                var img = CreateNewImage();
                var h = RandomizeImage(img, y);
                y += h;
            }
        }
    }

    private float RandomizeImage(Image img, float y)
    {
        var h = Random.Range(minHeight, maxHeight);
        var rt = img.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, y);
        rt.anchorMax = new Vector2(1, y + h);
        rt.offsetMin = new Vector2(0, 0);
        rt.offsetMax = new Vector2(0, 0);

        return h;
    }
}

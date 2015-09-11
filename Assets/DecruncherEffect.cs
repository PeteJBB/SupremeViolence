using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DecruncherEffect : MonoBehaviour
{
    public bool StartImmediately = true;
    public UnityEvent OnDecrunchComplete;

    private List<Image> images = new List<Image>();

    private float minHeight = 0.01f;
    private float maxHeight = 0.05f;
    private float loadTime = 3;

    private bool hasStarted = false;
    private bool hasEnded = false;

    void Start()
    {
        if (StartImmediately)
            StartDecrunching();
    }

    public void StartDecrunching()
    {
        hasStarted = true;
        minHeight = 0.01f;
        maxHeight = 0.05f;

        iTween.ValueTo(gameObject, iTween.Hash("from", 0, "to", 1, "time", loadTime, "easetype", iTween.EaseType.easeInCubic, "onupdate", (System.Action<object>)((obj) =>
        {
            var val = (float)obj;
            //minHeight = Mathf.Lerp(0.01f, 0.5f, val);
            //maxHeight = Mathf.Lerp(0.05f, 1f, val);
            minHeight = Mathf.Lerp(0.01f, 0.001f, val);
            maxHeight = Mathf.Lerp(0.05f, 0.005f, val);
        }), 
        "oncomplete", (System.Action)(() =>
        {
            StopDecrunching();
        })));
    }

    private void StopDecrunching()
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
        //img.sprite = square;
        img.color = new Color(Random.value, Random.value, Random.value, 1);

        images.Add(img);

        return img;
    }

    void Update()
    {
        if (hasStarted && !hasEnded)
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

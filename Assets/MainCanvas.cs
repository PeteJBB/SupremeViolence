using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainCanvas : MonoBehaviour 
{
    private GameObject TextTemplate;
    public static MainCanvas Instance;

    void Awake()
    {
        Instance = this;
    }

	// Use this for initialization
	void Start () 
    {
        TextTemplate = transform.FindChild("TextTemplate").gameObject;
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    public Text ShowMessage(string msg, float seconds)
    {
        var textObj = (GameObject)Instantiate(TextTemplate);
        textObj.transform.SetParent(transform, false);

        var text = textObj.GetComponent<Text>();
        text.text = msg;

        var rect = textObj.GetComponent<RectTransform>();
        rect.localPosition = Vector3.zero;

        if(seconds > 0)
        {
            GameBrain.Instance.WaitAndThenCall(seconds, () => 
            {
                Destroy(text.gameObject);
            });
        }
        return text;
    }
}

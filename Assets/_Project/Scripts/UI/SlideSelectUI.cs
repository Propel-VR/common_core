using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class SlideSelectElement
{
    public Sprite icon;
    public string name;
    public int value;
}

public class SlideSelectUI : MonoBehaviour
{
    [SerializeField] SlideSelectElement[] elements;
    [SerializeField] RectTransform parent;
    [SerializeField] RectTransform template;
    [SerializeField] float spacing = 0.1f;

    [SerializeField] AnimationCurve scaleOverDistance;
    [SerializeField] float scrollSpeed = 4f;
    [SerializeField] float smoothTime = 0.5f;
    [SerializeField] string inputAxis = "XRI_Left_Secondary2DAxis_Vertical";
    [SerializeField] float flickDeadzone = 0.5f;
    [SerializeField] float timeBeforeScroll = 0.6f;

    private List<RectTransform> childs = new();
    private int selected = 0;
    private float continuousValue;
    private float smoothedValue;
    private float smoothedVel;
    private float lastAxis;
    private float timeOutOfDeadzone;

    private void Awake ()
    {
        RebuildUI();
    }

    public void SetElements (SlideSelectElement[] elements)
    {
        this.elements = elements;
        RebuildUI();
    }

    private void RebuildUI ()
    {
        template.gameObject.SetActive(false);
        foreach (RectTransform child in childs) Destroy(child.gameObject);
        childs.Clear();

        int index = 0;
        foreach(SlideSelectElement element in elements)
        {
            RectTransform newChild = Instantiate(template, parent);
            newChild.gameObject.SetActive(true);
            Image image = newChild.GetComponentInChildren<Image>();
            TextMeshProUGUI label = newChild.GetComponentInChildren<TextMeshProUGUI>();
            image.sprite = element.icon;
            label.text = element.name;
            newChild.anchoredPosition = new Vector2(0f, (newChild.sizeDelta.y + spacing) * -index);

            childs.Add(newChild);
            index++;
        }

        selected = Mathf.Clamp(selected, 0, elements.Length - 1);
    }

    public int GetValue ()
    {
        return elements[selected].value;
    }

    public void Update ()
    {
        // Flick
        float axis = Input.GetAxisRaw(inputAxis);
        float absAxis = Mathf.Abs(axis);
        float absLastAxis = Mathf.Abs(lastAxis);
        if (absAxis > flickDeadzone && absLastAxis < flickDeadzone)
        {
            continuousValue = Mathf.RoundToInt(continuousValue) + Mathf.Sign(axis);
        }
        if(absAxis < flickDeadzone)
        {
            timeOutOfDeadzone = 0f;
        } else
        {
            timeOutOfDeadzone += Time.deltaTime;
        }

        // Do not start the continuous scroll constantly, wait a few milliseconds
        if(timeOutOfDeadzone > timeBeforeScroll)
        {
            continuousValue += axis * Time.deltaTime * scrollSpeed;
        }
        continuousValue = Mathf.Clamp(continuousValue, 0, childs.Count - 1);
        selected = Mathf.RoundToInt(continuousValue);
        smoothedValue = Mathf.SmoothDamp(smoothedValue, selected, ref smoothedVel, smoothTime);

        float height = template.sizeDelta.y + spacing;
        for (int i = 0; i < childs.Count; i++)
        {
            RectTransform child = childs[i];
            child.anchoredPosition = new Vector2(0f, height * -i + smoothedValue * height);
            child.localScale = Vector3.one * scaleOverDistance.Evaluate(Mathf.Abs(smoothedValue - i));
        }
        lastAxis = axis;
    }
}

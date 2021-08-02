using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HideAndShowMenu : MonoBehaviour
{

    public RectTransform rectTransform;
    public float showPos;
    public float hidePos;
    public bool show;

    // Start is called before the first frame update
    void Start()
    {
        show = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(show == false)
        {
            rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, -hidePos);
        }else if(show == true)
        {
            rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, showPos);
        }
    }

    public void setShow()
    {
        show = !show;
    }

}

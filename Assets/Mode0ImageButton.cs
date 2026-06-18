using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mode0ImageButton : MonoBehaviour
{
    //꾹누르기
    float clickTime;
    float minClickTime = 0.5f;
    bool isClick;
    bool zoomPanelOn; //줌패널을 켜고 있는지 여부

    //줌패널
    ZoomPanelManager zoomPanelManager;

    private void Start()
    {
        zoomPanelManager = GameObject.Find("ZoomPanelManager").GetComponent<ZoomPanelManager>();
        clickTime = 0f;
        isClick = false;
    }
    private void Update()
    {
        //꾹 누르기 시간 제한
        if (isClick)
        {
            clickTime += Time.deltaTime;

            if (!zoomPanelOn && clickTime >= minClickTime)
            {
                zoomPanelOn = true;
                zoomPanelManager.ZoomPanelOpen(this.gameObject);
            }
        }
        else
        {
            clickTime = 0f;
        }
    }

    public void SetZoomPanelOn(bool value) {
        zoomPanelOn = value;
    }

    public void ButtonDown()
    {
        isClick = true;
    }

    public void ButtonUp() {
        isClick = false;
        zoomPanelOn = false;
    }
}

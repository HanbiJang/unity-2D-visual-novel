using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZoomPanelManager : MonoBehaviour
{
    public GameObject zoomPanel;

    //child
    public GameObject QuitButton;
    public GameObject Image;

    //터치이벤트
    ScreenTouchManager screenTouchManager;


    //일러스트 줌 패널 열기 (이미지 클릭 이벤트)
    public void ZoomPanelOpen(GameObject clickedObject)
    {

        //이미지 애니메이션 재생
        zoomPanel.GetComponentInChildren<Animation>().Play("GameZoomPanelOpen");

        screenTouchManager = GameObject.Find("ScreenTouchManager").GetComponent<ScreenTouchManager>();
        screenTouchManager.SetTouchEnable(false); //터치 막기

        //누른 newContent 이미지를 가져온다
        Image image = clickedObject.GetComponent<Image>();

        //이미지를 zoompanel에 설정함
        Image.GetComponent<Image>().sprite = image.sprite;

        //show
        zoomPanel.SetActive(true);

    }

    //quit버튼에 할당
    public void ZoomPanelClose() {
        zoomPanel.SetActive(false);
        screenTouchManager.SetTouchEnable(true);
    }
}

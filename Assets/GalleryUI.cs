using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GalleryUI : MonoBehaviour
{

    public GameObject GalleryPanel; //패널
    public GameObject ZoomPanel; //이미지 클릭시 확대하는 패널
    public Image ZoomImage; //줌 패널의 이미지 부분
    public GalleryManager galleryManager;
    public Scrollbar scrollbar_vertical;

    public void GalleryPanelOpen()
    {
        galleryManager.ShowGalleryIllusts();//일러스트 설정
        //galleryManager.PlayAnim(); //애니메이션 재생
        GalleryPanel.SetActive(true);
    }

    //갤러리 스크롤바 위치 상단 설정 = 애니메이션의 이벤트로 들어감
    public void SetScrollbar_vertical_1() {
        scrollbar_vertical.value = 1;
    }

    public void GalleryPanelClose()
    {
        GalleryPanel.SetActive(false);
    }

    public void ZoomPanelOpen(Sprite sprite)
    {
        ZoomImage.sprite = sprite; //이미지 설정
        ZoomPanel.SetActive(true);
    }

    public void ZoomPanelClose()
    {
        ZoomPanel.SetActive(false);
    }
}

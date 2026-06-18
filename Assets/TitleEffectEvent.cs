using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleEffectEvent : MonoBehaviour
{
    GameObject TitleImage;
    public Image[] imageList;
    int ImageNum = 1;
    float changeSpeed = 0.005f;
    float changeTime = 0.1f;
    float LoopTime = 2.0f;

    bool isAlpha1 = false; //마지막 이미지의 투명도 1 여부

    int i = 0; //컬러 컴포넌트 가르키는 인덱스

    private void Start()
    {
        makeAlpha0(); //1번 이미지 제외 투명도 0 만들기
        

    }

    void Update()
    {
        if (isAlpha1 == true)
        { //마지막 이미지의 투명도가 1되면, 2초 뒤에 실행
            Invoke("LoopIncTrans", LoopTime);
        }
        else Invoke("IncTrans", changeTime); 
    }

    void IncTrans() //서서히 i번째 이미지의 투명도를 높임
    {
        Color c = imageList[i].color;

        if (c.a < 1)
        { //1이 될때까지 실행
            c.a += changeSpeed;
            imageList[i].color = c;
        }

        else if (i < ImageNum - 1)
        {
            i++;
        }

        else {
            isAlpha1 = true;
        }
    }

    void LoopIncTrans()
    {
        Color c = imageList[i].color;

        if (c.a > 0) //0이 될때까지 실행
        {
            for (int i = 1; i < ImageNum; i++)
            { //모든 이미지의 투명도를 낮춤
                c.a -= changeSpeed;
                imageList[i].color = c;
            }
        }

        else {
            i = 1;
            isAlpha1 = false;
        } //0이 되면 인덱스 초기화
    }


    void makeAlpha0() {
        for (int i = 1; i < ImageNum; i++)
        { 
            Color c = new Color(1, 1, 1);
            c.a = 0;
            imageList[i].color = c;
        }
        isAlpha1 = false;
    }
}

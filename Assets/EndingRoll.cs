using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Endings
{
    Ending1 = 0,
    Ending2,
    Ending3,
    Ending4,
/*    Part6Ending1 = 0,
    Part6Ending2,
    HiddenEnding,
    NormalEnding,
    TrueEnding,*/
    GameOver,
}

public class EndingRoll : MonoBehaviour
{
    ScreenTouchManager screenTouchManager;
    public GameObject EndingRollFade, EndingRoll_, EndingTitleButton;

    public void ShowEndingRoll(int endingNum)
    {
        screenTouchManager = GameObject.Find("ScreenTouchManager").GetComponent<ScreenTouchManager>();
        //터치 막기
        screenTouchManager.SetTouchEnable(false);

        //페이드 활성화
        EndingRollFade.SetActive(true);
        //페이드의 애니재생
        EndingRollFade.GetComponent<Animation>().Play("EndingRollFadeIn");
        //엔딩롤 활성화
        EndingRoll_.SetActive(true);
        //엔딩롤 애니재생
        EndingRoll_.GetComponent<Animation>().Play();
    }

    public void CloseEndingRoll()
    {
        screenTouchManager = GameObject.Find("ScreenTouchManager").GetComponent<ScreenTouchManager>();


        //페이드 비활성화
        EndingRollFade.SetActive(false);
        StartCoroutine(Delay());

    }

    IEnumerator Delay() {
        yield return new WaitForSeconds(3.0f);
        //페이드 비활성화
        EndingRoll_.SetActive(false);
        //터치 가능하게 하기
        screenTouchManager.SetTouchEnable(true);
    }

    public void ShowTitleButton() {
        EndingTitleButton.SetActive(true);
        EndingTitleButton.GetComponent<Animation>().Play();
    }

}

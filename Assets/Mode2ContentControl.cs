using System.Collections;
using UnityEngine;



public class Mode2ContentControl : MonoBehaviour
{
    public const int ChildNum = 3; //유지할 자식(내용물)의 개수
    ScreenTouchManager screenTouchManager;

    private void Start()
    {
        screenTouchManager = GameObject.Find("ScreenTouchManager").GetComponent<ScreenTouchManager>();
    }
    //애들 수를 ChildNum로 맞춘다
    public void MakeChildNum()
    {
        string a = this.gameObject.transform.name;
        if (this.gameObject.transform.childCount >= ChildNum)
        {
            screenTouchManager.SetTouchEnable(false); //터치 막기

            //모두 삭제
            for (int i = 0; i < ChildNum; i++) {
                GameObject child = this.gameObject.transform.GetChild(i).gameObject;

                child.GetComponent<Animation>().Play("Mode2DestoyAnim");

                StartCoroutine(Mode2DestoyAnimEnd(child));
            }
            StartCoroutine(MakeDelay());
        }
    }


    IEnumerator Mode2DestoyAnimEnd(GameObject child)
    {
        yield return new WaitForSeconds(0.3f);
        child.SetActive(false);
        //Destroy(child);
    }

    IEnumerator MakeDelay() {
        yield return new WaitForSeconds(2.0f);
        screenTouchManager.SetTouchEnable(true);
    }

}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScreenTouchManager : MonoBehaviour
{
    public GameManager GameManager;
    public GameObject Canvas_main;

    bool TouchEnable;

    Vector3 TouchStart; //누르는 순간
    Vector3 TouchEnd; //떼는 순간


    public void SetTouchEnable(bool value)
    {
        if (GameManager.choiceNow) { TouchEnable = false; return; }
        TouchEnable = value;
    }
    public bool GetTouchEnable() { return TouchEnable; }

    private void Start()
    {
        TouchEnable = true;
        TouchStart = Vector3.zero;
        TouchEnd = Vector3.zero;

    }

    private void Update()
    {
        if (TouchEnable)
        {
            if (Input.GetMouseButtonDown(0)) {
                TouchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            if(Input.GetMouseButtonUp(0))
            {
                TouchEnd = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                if (TouchStart != Vector3.zero || TouchEnd != Vector3.zero)
                {
                    //터치 길이에 따른 터치 구분
                    if (Vector3.Distance(TouchStart, TouchEnd) <= 1f && Vector3.Distance(TouchStart, TouchEnd) >= 0)
                    {
                        NextButton(FindTouchedObject());
                    }
                }
            }
        }

    }

    /// <summary>
    /// 기존 NextButton 역할 수행
    /// </summary>
    private void NextButton(List<RaycastResult> touchResult)
    {
        for (int i = 0; i < touchResult.Count; i++)
        {
            if (i == touchResult.Count - 1)
            {
                if (!touchResult[i].gameObject.name.Equals("UpPanel") && !touchResult[i].gameObject.CompareTag("ContentImage")) {
                    //기존 NextButton 기능 실행
                    GameManager.PreprocessNewTexts();
                    GameManager.QuikSave();
                }
            }

            else {
                if (touchResult[i].gameObject.CompareTag("ContentImage")) return;
                if (touchResult[i].gameObject.name.Equals("UpPanel")) return;
            }

        }
    }

    /// <summary>
    /// ui 레이캐스트 검출
    /// </summary>
    /// <returns></returns>
    private List<RaycastResult> FindTouchedObject()
    {
        /*using UnityEngine.UI;
        using UnityEngine.EventSystems;*/


        GraphicRaycaster m_gr = Canvas_main.GetComponent<GraphicRaycaster>();
        PointerEventData m_ped = new PointerEventData(null);

        m_ped.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        m_gr.Raycast(m_ped, results);

        if (results.Count > 0)
        {
            foreach (RaycastResult touchedObj in results)
            {
            }

        }


        return results;
    }
}

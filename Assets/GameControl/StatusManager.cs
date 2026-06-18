using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StatusManager : MonoBehaviour
{
    //mental => 의지
    //rel => 물망초
    //dre => 목표(타블렛)
    //fam => 이해(커피)
    private const int MAX_MEN = 4;
    private const int MAX_MONEY = 4;


    public Image meGauge;//정신력
    public Text meText;
    public Image moGauge;//돈
    public Text moText;
    //능력치
    public int mePoint;//정신력
    public int moPoint;//돈
    //public int rel;
    public int dre;
    public int fam;

    public Text monthText;
    public Text dayText;

    public Gradient gradient;

    [Range(0, 1)]
    public float graVal_me;
    [Range(0, 1)]
    public float graVal_mo;

    public GameObject incDecText;
    void Start()
    {
        //meImgArr = Resources.LoadAll<Sprite>("Images/Icon");
        CheckStatus();
        //CheckMoney();
        //UpdateMe();
        
    }
    void Update()
    {
        monthText.text = ScrollViewManager.month;
        dayText.text = ScrollViewManager.day;
    }

    void UpdateMe()
    {
        float destFillAmount = 0f;
        float currentFillAmount = meGauge.fillAmount;
        destFillAmount = (float)mePoint / MAX_MEN;
        
        StartCoroutine(UpdateMeGaugeCorutine(destFillAmount));
        meText.text = mePoint + " / " + MAX_MEN;
    }

    void UpdateMo()
    {
        float destFillAmount = 0f;
        float currentFillAmount = moGauge.fillAmount;
        
        destFillAmount = (float)moPoint / MAX_MONEY;

        StartCoroutine(UpdateMoGaugeCorutine(destFillAmount));
        moText.text = moPoint + " / " + MAX_MONEY;
    }

    IEnumerator UpdateMeGaugeCorutine(float _dest)
    {
        float fillVal;
        for (int i = 0; i < 50; i++)
        {
            fillVal = Mathf.Lerp(meGauge.fillAmount, _dest, 0.2f);
            meGauge.fillAmount = fillVal;
            yield return null;

        }
        meGauge.fillAmount = _dest;
    }

    IEnumerator UpdateMoGaugeCorutine(float _dest)
    {
        float fillVal;
        for (int i = 0; i < 50; i++)
        {
            fillVal = Mathf.Lerp(moGauge.fillAmount, _dest, 0.2f);
            moGauge.fillAmount = fillVal;
            yield return null;

        }
        moGauge.fillAmount = _dest;
    }

    IEnumerator UpdateMeGaugeColorCorutine(float _dest)
    {
        for(int i = 0; i < 50; i++)
        {
            graVal_me = Mathf.Lerp(graVal_me, _dest, 0.1f);
            meGauge.color = gradient.Evaluate(graVal_me);
            yield return null;
        }
        graVal_me = _dest;
        gradient.Evaluate(graVal_me);
    }

    IEnumerator UpdateMoGaugeColorCorutine(float _dest)
    {
        for (int i = 0; i < 50; i++)
        {
            graVal_mo = Mathf.Lerp(graVal_mo, _dest, 0.1f);
            moGauge.color = gradient.Evaluate(graVal_mo);
            yield return null;
        }
        graVal_mo = _dest;
        gradient.Evaluate(graVal_mo);
    }

    public void CheckStatus()
    {
        //정신력
        float destColor_me = 0f;
        switch(mePoint)
        {
            case 4:
                destColor_me = 1f;
                break;
            case 3:
                destColor_me = 0.75f;
                break;
            case 2:
                destColor_me = 0.5f;
                break;
            case 1:
                destColor_me = 0.25f;
                break;
            case 0:
                destColor_me = 0f;
                break;
            default:
                break;
        }

        //돈
        float destColor_mo = 0f;
        switch (moPoint)
        {
            case 4:
                destColor_mo = 1f;
                break;
            case 3:
                destColor_mo = 0.75f;
                break;
            case 2:
                destColor_mo = 0.5f;
                break;
            case 1:
                destColor_mo = 0.25f;
                break;
            case 0:
                destColor_mo = 0f;
                break;
            default:
                break;
        }


        StartCoroutine(UpdateMeGaugeColorCorutine(destColor_me));
        StartCoroutine(UpdateMoGaugeColorCorutine(destColor_mo));
        UpdateMe();
        UpdateMo();
    }

    //public void DecMo()
    //{
    //    moPoint--;

    //    CheckMoney();
    //}

    //public void DecMe()
    //{
    //    mePoint--;

    //    CheckStatus();
    //}

    //public void IncMo()
    //{
    //    moPoint++;

    //    CheckMoney();
    //}
    //public void IncMe()
    //{
    //    mePoint++;

    //    CheckStatus();
    //}
    public void Men(int _count)
    {
        mePoint += _count;

        if (mePoint > MAX_MEN)
            mePoint = MAX_MEN;

        if (mePoint < 0)
            mePoint = 0;

        CheckStatus();
    }
    public void Mo(int _count)
    {
        moPoint += _count;

        if (moPoint > MAX_MONEY)
            moPoint = MAX_MONEY;

        if (moPoint < 0)
            moPoint = 0;

        CheckStatus();
    }
    public void Dre(int _count)
    {
        dre += _count;
        CheckStatus();
    }
    public void Fam(int _count)
    {
        fam += _count;
        CheckStatus();
    }

}

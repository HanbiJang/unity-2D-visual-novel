using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
class SettingValues
{
    public int textSize;
    public int textGap;
    public float bgmSize;
    public float effectSize;
}
public class SettingManager : MonoBehaviour
{
    public static SettingManager instance;

    public Text textSizeStr;
    public int textSize;
    public int textGap;
    public GameObject settingsPanel;


    public Button textGapNarrow;
    public Button textGapMiddle;
    public Button textGapWide;

    public Slider bgmSlider;
    public Slider effectSlider;

    MobileTalkManager mobileTalkManager;

    public ScreenTouchManager screenTouchManager;//화면터치 인스펙터 할당

    // Start is called before the first frame update
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }


    }
    void Start()
    {
        SettingValuesLoad(); // 저장된 세팅값 불러오기
        textSizeStr.GetComponent<Text>().text = textSize.ToString();
        SetTextGaPColor();
    }

    public void SetTextGaPColor()
    {
        if(textGap == 1)
        {
            textGapNarrow.GetComponent<Image>().color = Color.red;
            textGapMiddle.GetComponent<Image>().color = Color.white;
            textGapWide.GetComponent<Image>().color = Color.white;
        }
        else if (textGap == 2)
        {
            textGapNarrow.GetComponent<Image>().color = Color.white;
            textGapMiddle.GetComponent<Image>().color = Color.red;
            textGapWide.GetComponent<Image>().color = Color.white;
        }
        else if (textGap == 3)
        {
            textGapNarrow.GetComponent<Image>().color = Color.white;
            textGapMiddle.GetComponent<Image>().color = Color.white;
            textGapWide.GetComponent<Image>().color = Color.red;
        }
    }
    //public void SetSize_1()
    //{
    //    SetTextSize(1); // 작게
    //}
    //public void SetSize_2()
    //{
    //    SetTextSize(2); // 중간
    //}
    //public void SetSize_3()
    //{
    //    SetTextSize(3); // 크게
    // }
    //public void SetSize()
    //{
    //    textSize = Int32.Parse(textSizeStr.GetComponent<Text>().text);
    //    SetTextSize(textSize);
    //}

    public void TextSizeInc()
    {
        if(textSize >= 40 && textSize <= 100)
        {
            textSize = Int32.Parse(textSizeStr.GetComponent<Text>().text);
            textSize+=5;
            if(textSize < 40)
                textSize = 40;
            if (textSize > 100)
                textSize = 100;
            textSizeStr.GetComponent<Text>().text = textSize.ToString();
        }
    }
    public void TextSizeDec()
    {
        if (textSize >= 40 && textSize <= 100)
        {
            textSize = Int32.Parse(textSizeStr.GetComponent<Text>().text);
            textSize-=5;
            if (textSize < 40)
                textSize = 40;
            if (textSize > 100)
                textSize = 100;
            textSizeStr.GetComponent<Text>().text = textSize.ToString();
        }
    }
    public void SetGap_1()
    {
        textGap = 1;
        SetTextGaPColor();
    }
    public void SetGap_2()
    {
        textGap = 2;
        SetTextGaPColor();
    }
    public void SetGap_3()
    {
        textGap = 3;
        SetTextGaPColor();
    }

    public void SettingsClose() // 세팅 닫기
    {
        SettingValuesSave();
        settingsPanel.SetActive(false);
        if(screenTouchManager != null)
            screenTouchManager.SetTouchEnable(true);
        Time.timeScale = 1;
    }

    public void BGMVolumeChanged()
    {
        SoundManager.instance.audioSourceBGM.volume = bgmSlider.value;
    }

    public void EffectVolumeChanged()
    {
        foreach(var audioSourceEffect in SoundManager.instance.audioSourceEffects)
        {
            audioSourceEffect.volume = effectSlider.value;
        }
    }

    public void SettingValuesSave() // 설정창을 닫으면 세팅값이 저장됨
    {
        SettingValues settingValues = new SettingValues();
        settingValues.textSize = this.textSize;
        settingValues.textGap = this.textGap;
        settingValues.bgmSize = this.bgmSlider.value;
        settingValues.effectSize = this.effectSlider.value;

        string jsonData = JsonUtility.ToJson(settingValues);

        FileStream fs = new FileStream(string.Format("{0}_SettingValues.json", Application.persistentDataPath), FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(jsonData);
        fs.Write(data, 0, data.Length);
        fs.Close();
    }

    public void SettingValuesLoad() // 게임을 시작할 때 세팅값을 불러옴
    {
        if (!File.Exists(string.Format("{0}_SettingValues.json", Application.persistentDataPath)))
            return;
        FileStream fs = new FileStream(string.Format("{0}_SettingValues.json", Application.persistentDataPath), FileMode.Open);
        byte[] data = new byte[fs.Length];
        fs.Read(data, 0, data.Length);
        fs.Close();
        string jsonData = Encoding.UTF8.GetString(data);

        SettingValues settingValues = JsonUtility.FromJson<SettingValues>(jsonData);

        this.textSize = settingValues.textSize;
        this.textGap = settingValues.textGap;
        this.bgmSlider.value = settingValues.bgmSize;
        this.effectSlider.value = settingValues.effectSize;
    }

    public void FindScreenTouchManager()
    {
        screenTouchManager = FindObjectOfType<ScreenTouchManager>();
    }
}

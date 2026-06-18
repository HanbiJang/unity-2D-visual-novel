using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioSource[] audioSourceEffects;
    public AudioSource audioSourceBGM;

    public string[] playSoundName;

    public Sound[] effectSounds;
    public Sound[] bgmSounds;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);
    }

    public void Start()
    {
        playSoundName = new string[audioSourceEffects.Length];
        ChangeValueBGM();
        ChangeValueEffect();
    }

    public void PlaySE(string _name)
    {
        foreach(var effectSound in effectSounds)
        {
            if(_name == effectSound.name)
            {
                for(int i = 0; i < audioSourceEffects.Length; i++)
                {
                    if (!audioSourceEffects[i].isPlaying)
                    {
                        playSoundName[i] = effectSound.name;
                        audioSourceEffects[i].clip = effectSound.clip;
                        audioSourceEffects[i].Play();
                        return;
                    }
                }
                return;
                
            }
        }
    }

    public void PlayBGM(string _name)
    {
        foreach(var bgmSound in bgmSounds)
        {
            if(_name == bgmSound.name)
            {
                audioSourceBGM.Stop(); // 기존에 재생되던 BGM을 정지하고
                audioSourceBGM.volume = SettingManager.instance.bgmSlider.value;
                audioSourceBGM.clip = bgmSound.clip; // _name을 추가한 다음
                audioSourceBGM.Play(); // 새로 재생
                return;
            }
        }
        return;
    }

    public void StopAllSE()
    {
        foreach(var audioSourceEffect in audioSourceEffects)
        {
            audioSourceEffect.Stop();
        }
    }

    public void StopBGM()
    {
        //audioSourceBGM.Stop();
        StartCoroutine(BGMFadeOut());
    }

    public void StopSE(string _name)
    {
        for (int i = 0; i < audioSourceEffects.Length; i++)
        {
            if(playSoundName[i] == _name)
            {
                audioSourceEffects[i].Stop();
                return;
            }
        }
    }

    public void ChangeValueBGM()
    {
        audioSourceBGM.volume = SettingManager.instance.bgmSlider.value;
    }

    public void ChangeValueEffect()
    {
        foreach (var audioSourceEffect in audioSourceEffects)
            audioSourceEffect.volume = SettingManager.instance.effectSlider.value;
    }

    public IEnumerator BGMFadeOut()
    {
        float soundVol;
        for (int i = 0; i < 5; i++)
        {
            soundVol = Mathf.Lerp(audioSourceBGM.volume, 0, 0.2f);
            audioSourceBGM.volume = soundVol;
            yield return new WaitForSeconds(0.1f);
        }
        audioSourceBGM.volume = 0;
    }
}

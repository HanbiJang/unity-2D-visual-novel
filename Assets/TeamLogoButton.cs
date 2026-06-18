using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamLogoButton : MonoBehaviour
{
    LevelChanger levelChanger;
    public GameObject STZPanel;


    private void Start()
    {
        levelChanger = GameObject.Find("LevelChanger").GetComponent<LevelChanger>();
    }

    public void SceneChange() {
        levelChanger.FadeToLevel(3); //3: 스텝롤 씬
    }

    //업데이트 내용 확인
    public void BtnCheckSTZ() {
        STZPanel.SetActive(true);
    }
    public void BtnCloseSTZ() {
        STZPanel.SetActive(false);
    }
}

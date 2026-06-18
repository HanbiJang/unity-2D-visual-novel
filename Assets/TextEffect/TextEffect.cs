using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextEffect : MonoBehaviour
{
    public bool BTextEffectOn = true;

    GameManager gameManager;
    public bool isTextEffect = false;
    int oldScriptIdx = -1;
    Text oldText_ = null;
    List<ScriptData> oldScripts;

    //public const int lineAddNum = 27; //27글자를 줄바꿈 수로 계산함

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    /// <summary>
    /// 글자 타이핑 효과
    /// </summary>
    /// <param name="scriptIdx">스크립트 읽은 진도</param>
    /// <param name="scripts">스크립트 리스트 객체</param>
    /// <param name="text_">Text가 들어가는 UI의 Text 컴포넌트</param>
    /// <param name="isClicked">화면이 눌렸냐를 판별하는 변수</param>
    public void StartTextEffectCoroutine(int scriptIdx, List<ScriptData> scripts, Text text_)
    {
        if (BTextEffectOn)
        {
            if (!isTextEffect) {
                if (scripts[scriptIdx].mode != 2)
                {
                    StartCoroutine(TypingEffectMode0(scriptIdx, scripts, text_));
                }
                else {//mode2
                    text_.text = ReplaceSpace(scripts[scriptIdx].script);
                    isTextEffect = false;
                }
                
            } 
            else
            {
                StopAllCoroutines();
                //즉시 모든 텍스트 표시
                if (oldScriptIdx != -1)
                {
                    oldText_.text = ReplaceSpace(scripts[oldScriptIdx].script);
                }

                StartCoroutine(AddTmpContentSeconds());
                isTextEffect = false;
            }
        }
        /*        if (scripts[oldScriptIdx].mode != 2)
                {

                }

                else {
                    oldText_.text = ReplaceSpace(scripts[oldScriptIdx].script);
                    isTextEffect = false;
                }*/

    }

    /// <summary>
    /// 텍스트 라인 변경을 단어 단위가 아니라 글자 단위로 하게 하는 처리 (스페이스를 다르게 처리하게 바꿔줌)
    /// </summary>
    string ReplaceSpace(string str)
    {
        str = str.Replace(' ', '\u00A0');
        return str;
    }

    IEnumerator AddTmpContentSeconds()
    {
        yield return new WaitForSeconds(0.01f); //텍스트가 프레임 넘겨서 적용돼서 이렇게 했음
        gameManager.AddTmpContent(); //유령컴포넌트를 넣었다 빼서 간격 맞춤
    }


    IEnumerator TypingEffectMode0(int scriptIdx, List<ScriptData> scripts, Text text_)
    {

        isTextEffect = true;
        oldScriptIdx = scriptIdx;
        oldText_ = text_;
        oldScripts = scripts;
        for (int i = 0; i <= scripts[scriptIdx].script.Length; i++)
        {
            //텍스트 타이핑 효과
            text_.text = ReplaceSpace(scripts[scriptIdx].script).Substring(0, i);
            // 레이아웃을 즉시 동기 갱신하여 content 높이 변경으로 인한 스크롤 위치 튐 방지
            Canvas.ForceUpdateCanvases();
            gameManager.ScrollToBottom(scripts[scriptIdx].mode);
            yield return new WaitForSeconds(0.05f);
        }
        isTextEffect = false;
        yield break;
    }


}

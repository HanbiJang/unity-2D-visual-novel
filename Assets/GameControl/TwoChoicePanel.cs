using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TwoChoicePanel : MonoBehaviour
{
    public GameObject twoChoicePanel;
    public Text How;
    public Button[] buttons = new Button[2];
    public Text[] buttonTexts = new Text[2];
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ViewTrue()
    {
        twoChoicePanel.SetActive(true);
    }
    public void ViewFalse()
    {
        twoChoicePanel.SetActive(false);
    }
    public void DisableButton(Text buttonText)
    {
        GameObject button = buttonText.transform.parent.gameObject;
        button.GetComponent<Button>().interactable = false;
    }
    public void ActiveAllButton()
    {

        for (int i = 0; i < 2; ++i)
        {
            GameObject button = buttonTexts[i].transform.parent.gameObject;
            button.GetComponent<Button>().interactable = true;
        }
    }
}

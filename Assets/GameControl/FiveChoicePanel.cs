using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FiveChoicePanel : MonoBehaviour
{
    public GameObject fiveChoicePanel;
    public Text How;
    public Button[] buttons = new Button[5];
    public Text[] buttonTexts = new Text[5];
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
        fiveChoicePanel.SetActive(true);
    }
    public void ViewFalse()
    {
        fiveChoicePanel.SetActive(false);
    }
    public void DisableButton(Text buttonText)
    {
        GameObject button = buttonText.transform.parent.gameObject;
        button.GetComponent<Button>().interactable = false;
    }
    public void ActiveAllButton()
    {

        for (int i = 0; i < 5; ++i)
        {
            GameObject button = buttonTexts[i].transform.parent.gameObject;
            button.GetComponent<Button>().interactable = true;
        }
    }
}

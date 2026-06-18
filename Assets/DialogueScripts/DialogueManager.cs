using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Text dialogueText;
    private string tempText;   


    private Queue<string> sentences;
    public LevelChanger levelChanger;
    public GameObject newGameBtn;
    public GameObject loadGameBtn;
    public GameObject continueBtn;
    
    void Start()
    {
        sentences = new Queue<string>();
        
    }
    public void StartDialogue(Dialogue dialogue)
    {
        newGameBtn.SetActive(false);
        loadGameBtn.SetActive(false);
        continueBtn.SetActive(true);


        
        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence+"\n");
        }
        DisplayNextSentence();
    }
    public void DisplayNextSentence()
    { 
        if (sentences.Count == 1)
        {
            EndDialogue();
            return;
        }
        string sentence = sentences.Dequeue();
        //dialogueText.text += sentence;
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        tempText = "";
        dialogueText.text += tempText;
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            tempText += letter;
            yield return null;
        }
       
    }
    public void EndDialogue()
    {
        levelChanger.Next_Clicked();
      
    }
}

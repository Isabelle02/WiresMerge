using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _dialogueText;
    [SerializeField] private TextMeshProUGUI[] _buttonsText;
    [SerializeField] private Button[] _buttons;

    private DialogueNode currentNode;

    public void StartDialogue(DialogueNode startingNode)
    {
        currentNode = startingNode;
        DisplayCurrentDialogue();
    }

    public void DisplayCurrentDialogue()
    {
        Debug.Log(currentNode.DialogueText);
        _dialogueText.text = currentNode.DialogueText;
        for (int i = 0; i < _buttons.Length /*currentNode.Choices.Count*/; i++)
        {
            //for (int j = 0; j < _buttons.Length; j++)
            //{
            //    int index = j;
            //    _buttons[j].onClick.AddListener(() => SelectChoice(index));
            //}
            int index = i;
            _buttons[i].onClick.AddListener(() => SelectChoice(index));

            Debug.Log($"{i + 1}: {currentNode.Choices[i].ChoiceText}");
            _buttonsText[i].text = currentNode.Choices[i].ChoiceText;
        }
    }

    public void SelectChoice(int choiceIndex)
    {
        if (choiceIndex < 0 || choiceIndex >= currentNode.Choices.Count)
        {
            Debug.LogError("Invalid choice index");
            return;
        }

        currentNode = currentNode.Choices[choiceIndex].NextNode;
        DisplayCurrentDialogue();
    }
}

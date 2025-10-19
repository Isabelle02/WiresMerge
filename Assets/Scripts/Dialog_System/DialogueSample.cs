using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSample : MonoBehaviour
{
    private DialogueManager dialogueManager;

    void Start()
    {
        dialogueManager = GetComponent<DialogueManager>();

        // Create dialogue nodes
        DialogueNode node1 = new DialogueNode("Hello, adventurer! How can I help you today?");
        DialogueNode node2 = new DialogueNode("I have a quest for you.");
        DialogueNode node3 = new DialogueNode("Goodbye, safe travels!");

        // Create choices
        node1.Choices.Add(new Choice("Ask about a quest", node2));
        node1.Choices.Add(new Choice("Say goodbye", node3));

        node2.Choices.Add(new Choice("Accept quest", node3));
        node2.Choices.Add(new Choice("Decline quest", node1));

        // Start dialogue
        dialogueManager.StartDialogue(node1);
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private GameObject uiButtons;

    [Serializable]
    public struct DialogueEntry
    {
        public string DialogueName;
        public string DialogueText;
    }

    [SerializeField]
    private List<DialogueEntry> dialogueEntries; // ������ �������� ��� ��������� � ����������

    private Dictionary<string, string> dialogueDictionary; // ������� ��� �������� ������� � ��������
    private Coroutine typingCoroutine;

    private void Start()
    {
        // ������������� �������
        dialogueDictionary = new Dictionary<string, string>();
        foreach (var entry in dialogueEntries)
        {
            if (!dialogueDictionary.ContainsKey(entry.DialogueName))
            {
                dialogueDictionary.Add(entry.DialogueName, entry.DialogueText);
            }
            else
            {
                Debug.LogWarning($"������ � ������ {entry.DialogueName} ��� ���������� � �������.");
            }
        }

        dialogueText.text = ""; // ������� ����� � ������
        uiButtons.SetActive(false); // �������� ������ �� ���������
    }

    public void ShowDialogue(string dialogueName)
    {
        if (dialogueDictionary.TryGetValue(dialogueName, out string dialogue))
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
            typingCoroutine = StartCoroutine(TypeText(dialogue));
            uiButtons.SetActive(true); // ���������� ������, ���� �����
        }
        else
        {
            Debug.LogError($"������ � ������ {dialogueName} �� ������.");
        }
    }

    private IEnumerator TypeText(string text)
    {
        dialogueText.text = "";
        foreach (char letter in text)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public void SkipTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        // ���������� ���� ����� �����
        if (dialogueDictionary.TryGetValue(dialogueText.text, out string fullText))
        {
            dialogueText.text = fullText;
        }
    }
}

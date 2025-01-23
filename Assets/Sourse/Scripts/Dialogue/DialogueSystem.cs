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
    private List<DialogueEntry> dialogueEntries; // Список диалогов для настройки в инспекторе

    private Dictionary<string, string> dialogueDictionary; // Словарь для быстрого доступа к диалогам
    private Coroutine typingCoroutine;

    private void Start()
    {
        // Инициализация словаря
        dialogueDictionary = new Dictionary<string, string>();
        foreach (var entry in dialogueEntries)
        {
            if (!dialogueDictionary.ContainsKey(entry.DialogueName))
            {
                dialogueDictionary.Add(entry.DialogueName, entry.DialogueText);
            }
            else
            {
                Debug.LogWarning($"Диалог с именем {entry.DialogueName} уже существует в словаре.");
            }
        }

        dialogueText.text = ""; // Очищаем текст в начале
        uiButtons.SetActive(false); // Скрываем кнопки по умолчанию
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
            uiButtons.SetActive(true); // Показываем кнопки, если нужно
        }
        else
        {
            Debug.LogError($"Диалог с именем {dialogueName} не найден.");
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

        // Отображаем весь текст сразу
        if (dialogueDictionary.TryGetValue(dialogueText.text, out string fullText))
        {
            dialogueText.text = fullText;
        }
    }
}

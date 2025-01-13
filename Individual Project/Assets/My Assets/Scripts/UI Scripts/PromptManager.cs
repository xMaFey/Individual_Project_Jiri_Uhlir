using UnityEngine;
using TMPro;

public class PromptManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI interactionPromptText;

    public static PromptManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void ShowPrompt(string prompt)
    {
        interactionPromptText.text = prompt;
        interactionPromptText.gameObject.SetActive(true);
    }

    public void HidePrompt()
    {
        interactionPromptText.gameObject.SetActive(false);
    }
}

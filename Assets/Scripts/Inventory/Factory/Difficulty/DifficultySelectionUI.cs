using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DifficultySelectionUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject difficultyPanel;
    [SerializeField] private Button easyButton;
    [SerializeField] private Button normalButton;
    [SerializeField] private Button hardButton;
    [SerializeField] private TextMeshProUGUI descriptionText;


    private void Start()
    {
        // Configurar botones
        easyButton.onClick.AddListener(() => SelectDifficulty(GameDifficulty.Easy));
        normalButton.onClick.AddListener(() => SelectDifficulty(GameDifficulty.Normal));
        hardButton.onClick.AddListener(() => SelectDifficulty(GameDifficulty.Hard));

        // Mostrar panel al inicio
        Show();
    }


    private void UpdateDescription(string description)
    {
        descriptionText.text = description;
    }

    private void SelectDifficulty(GameDifficulty difficulty)
    {
        DifficultyManager.Instance.SetDifficulty(difficulty);

        // Aquí puedes inicializar el inventario del jugador
        var playerInventory = FindObjectOfType<Inventory>();
        if (playerInventory != null)
        {
            playerInventory.InitializeWithDifficulty();
        }

        Hide();
    }

    public void Show()
    {
        difficultyPanel.SetActive(true);
        UpdateDescription("Selecciona la dificultad");
    }

    public void Hide()
    {
        difficultyPanel.SetActive(false);
    }
}
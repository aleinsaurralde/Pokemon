using System.Collections.Generic;
using UnityEngine;

public enum GameDifficulty
{
    Easy,
    Normal,
    Hard
}
public class DifficultyManager : MonoBehaviour
{
    private static DifficultyManager _instance;
    public static DifficultyManager Instance => _instance;

    [Header("Factory References")]
    [SerializeField] private ItemFactory itemFactory;
    [SerializeField] private EasyDifficultyConfig easyConfig;
    [SerializeField] private NormalDifficultyConfig normalConfig;
    [SerializeField] private HardDifficultyConfig hardConfig;

    private IDifficultyItemsFactory _currentFactory;
    public GameDifficulty CurrentDifficulty { get; private set; }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        // Inicializar el item factory
        if (itemFactory != null)
            itemFactory.Initialize();

        SetDifficulty(GameDifficulty.Normal);
    }

    public void SetDifficulty(GameDifficulty difficulty)
    {
        CurrentDifficulty = difficulty;

        _currentFactory = difficulty switch
        {
            GameDifficulty.Easy => easyConfig,
            GameDifficulty.Normal => normalConfig,
            GameDifficulty.Hard => hardConfig,
            _ => normalConfig
        };

        Debug.Log($"Difficulty set to: {difficulty}");
    }

    public List<ItemSlot> GetStartingItems()
    {
        if (_currentFactory == null || itemFactory == null)
        {
            Debug.LogError("Factory or ItemFactory not initialized!");
            return new List<ItemSlot>();
        }

        return _currentFactory.GetStartingItems(itemFactory);
    }

    public IDifficultyItemsFactory GetCurrentFactory()
    {
        return _currentFactory;
    }

    public ItemFactory GetItemFactory()
    {
        return itemFactory;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestGenerator : MonoBehaviour
{
    #region Singleton

    public static ChestGenerator Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Init();
    }

    #endregion

    #region Global Members

    [Header("References")]
    [SerializeField] private List<Transform> _chestAnchors;

    [Header("Prefabs")]
    [SerializeField] private GameObject _chestPrefab;

    #endregion

    public const int ChestCount = 10;

    public string StrSeed
    {
        set
        {
            if (int.TryParse(value, out int seed))
            {
                Seed = seed;
            }
        }
    }
    public int Seed { get; private set; }

    public enum ChestID
    {
        A = 0,
        B = 1,
        C = 2,
        D = 3,
        E = 4,
        F = 5,
        G = 6,
        H = 7,
        I = 8,
        J = 9,
    }

    public enum Key
    {
        NO_CONDITION = 0,
        KEY1 = 1,
        KEY2 = 2,
        KEY3 = 3,
        KEY4 = 4,
        KEY5 = 5,
        KEY6 = 6,
        KEY7 = 7,
        KEY8 = 8,
        KEY9 = 9,
    }


    private void Init()
    {
        SetSeed(1);
    }

    public void Generate()
    {
        SetSeed(Seed);

        List<Key> keys = GetKeyList();

        for (int i = 0; i < ChestCount; i++)
        {
            GenerateChest((ChestID)i, keys[i]);
        }
    }
    private void SetSeed(int seed)
    {
        Random.InitState(seed);
    }

    /// <summary>
    /// Get a list of 10 keys including at least one NO_CONDITION to make sure you can start,
    /// and 9 other unique keys (including possible other NO_CONDITION) in random order
    /// </summary>
    /// <returns></returns>
    private List<Key> GetKeyList()
    {
        var pickList = new List<Key>()
        {
            Key.NO_CONDITION,
            Key.NO_CONDITION,
            Key.KEY1,
            Key.KEY2,
            Key.KEY3,
            Key.KEY4,
            Key.KEY5,
            Key.KEY6,
            Key.KEY7,
            Key.KEY8,
            Key.KEY9,
        };
        var finalList = new List<Key>() { Key.NO_CONDITION };

        int randomPick;
        for (int i = 1; i < ChestCount; i++)
        {
            randomPick = Random.Range(0, pickList.Count);
            finalList.Add(pickList[randomPick]);
            pickList.RemoveAt(randomPick);
        }

        return finalList;
    }

    /// <summary>
    /// Generate a chest with a unique Key to access or no condition
    /// </summary>
    /// <param name="chestID"></param>
    /// <param name="key"></param>
    private Chest GenerateChest(ChestID chestID, Key key)
    {
        Chest chest = Instantiate(_chestPrefab, _chestAnchors[(int)chestID]).GetComponent<Chest>();

        chest.SetTitleAndKey(chestID, key);

        return chest;
    }
}

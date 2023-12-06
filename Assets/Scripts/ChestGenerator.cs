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


    public List<Chest> Chests { get; private set; }

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
        Seed = 1;

        Generate();
    }

    public void Generate()
    {
        Clean();

        SetSeed(Seed);

        List<Key> keys = GetKeyList();

        Chests = new();
        for (int i = 0; i < ChestCount; i++)
        {
            Chests.Add(GenerateChest((ChestID)i, keys[i]));
        }

        FillChests();
    }
    private void SetSeed(int seed)
    {
        Random.InitState(seed);
    }

    private List<Key> GenerateChestContent()
    {
        return new List<Key>();
    }
    private void SetSeed(string seed)
    {
        if (int.TryParse(seed, out int intSeed))
        {
            SetSeed(intSeed);
        }
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
        Key key;
        for (int i = 1; i < ChestCount; i++)
        {
            randomPick = Random.Range(0, pickList.Count);
            key = pickList[randomPick];

            if (finalList.Contains(key))
            {
                pickList.RemoveAt(randomPick);
            }
            finalList.Add(key);
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


    private void FillChests()
    {
        List<Chest> chests = new(Chests);

        // Remove First Chest
        chests.RemoveAt(0);

        // Get Random Final Chest
        Chest finalChest;
        finalChest = chests[chests.Count - 1];
        //do
        //{
        //    finalChest = chests[Random.Range(0, chests.Count)];
        //} while (finalChest.HasCondition);
        chests.Remove(finalChest);

        FillChest(chests[Random.Range(0, chests.Count)], finalChest.Key, chests);
    }
    private void FillChest(Chest chest, Key key, List<Chest> chests)
    {
        // Fill chest
        chest.SetContainingKeyList(new() { key });

        // If list null --> return
        if (chests == null) return;

        // If nothing left --> first chest
        if (chests.Count == 0)
        {
            FillChest(Chests[0], chest.Key, null);
        }

        // Get random chest
        else
        {
            Chest nextChest = chests[Random.Range(0, chests.Count)];
            chests.Remove(nextChest);
            FillChest(nextChest, chest.Key, chests);
        }
    }


    private void Clean()
    {
        if (Chests == null || Chests.Count == 0) return;

        foreach (var chest in Chests)
        {
            if (chest != null)
            {
                Destroy(chest.gameObject);
            }
        }
    }
}

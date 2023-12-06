using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

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

        CleanFillChest(keys);
        DisplayPaths();
    }
    private void SetSeed(int seed)
    {
        Random.InitState(seed);
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

        Chest chest = chests[Random.Range(0, chests.Count)];
        chests.Remove(chest);
        FillChest(chest, finalChest.Key, chests);
    }
    private void FillChest(Chest chest, Key key, List<Chest> chests)
    {
        // Fill chest
        chest.SetContainingKey(key);

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
            if (nextChest.Key == chest.Key)
            {
                nextChest = chests[Random.Range(0, chests.Count)];
            }

            chests.Remove(nextChest);
            FillChest(nextChest, chest.Key, chests);
        }
    }

    private void CleanFillChest(IReadOnlyList<Key> keys)
    {
        foreach (var chest in Chests.Where(chest => chest.HasCondition && !chest.IsFinalChest).Where(chest => chest.ContainedKey == chest.Key))
        {
            Key newKey;
            do
            {
                newKey = keys[Random.Range(0, keys.Count)];
            } while (newKey == chest.Key || newKey == Key.NO_CONDITION);
            chest.SetContainingKey(newKey);
        }

        ConstructChestGraph();
        //ConstructChestGraph2();
    }
    
    private void ConstructChestGraph1()
    {
        foreach (var chest in Chests)
        {
            if (chest.ContainedKey == Key.NO_CONDITION) continue;

            foreach (var chest1 in Chests.Where(chest1 => chest1 != chest && chest.ContainedKey == chest1.Key))
            {
                chest.Childs.Add(chest1);
            }
        }

        //Chests[0].CreateArrows();
    }

    public void DisplayPaths()
    {
        StringBuilder sb = new();
        foreach (var path in PossiblePaths())
        {
            foreach (var chest in path)
            {
                sb.Append(chest.ChestID.ToString());
                sb.Append(" ");
            }
            sb.AppendLine();
        }
        Debug.Log(sb.ToString());
    }

    private List<List<Chest>> PossiblePaths()
    {
        List<List<Chest>> paths = new();
        List<Chest> path = new();
        Chest start = Chests[^1];
        Chest end = Chests[0];
        bool[] visited = new bool[Chests.Count];
        DFS(start, end, visited, path, paths);
        return paths;
    }
    
    private void DFS(Chest current, Chest end, bool[] visited, List<Chest> path, List<List<Chest>> paths)
    {
        visited[(int)current.ChestID] = true;
        path.Add(current);

        if (current == end)
        {
            paths.Add(new List<Chest>(path));
        }
        else
        {
            foreach (var chest in current.Childs.Where(chest => !visited[(int)chest.ChestID]))
            {
                DFS(chest, end, visited, path, paths);
            }
        }

        path.Remove(current);
        visited[(int)current.ChestID] = false;
    }

    private void ConstructChestGraph()
    {
        List<Chest> chests = new(Chests);

        Chest chest = Chests[0];
        chests.Remove(chest);
        chests.Remove(Chests[^1]);

        List<Chest> childList = new();
        List<Chest> formerChildList;
        List<Chest> nextChildList = new();

        foreach (var child in chests.Where(c => c != chest && chest.ContainedKey == c.Key))
        {
            childList.Add(child);
            nextChildList.Add(child);
            child.Parent.Add(chest);
        }
        chest.ChildsDict.Add(0, childList);
        chest.CreateArrows();
        chest.HasChilds = true;

        int rank = 0;
        while (chests.Count > 0)
        {
            rank++;
            formerChildList = new(nextChildList);
            nextChildList.Clear();

            for (int i = 0; i < formerChildList.Count; i++)
            {
                childList.Clear();
                chest = formerChildList[i];
                if (!chest.HasChilds && chest.ContainedKey != Key.NO_CONDITION)
                {
                    chest.HasChilds = true;

                    chests.Remove(chest);

                    foreach (var child in Chests.Where(c => c != chest && chest.ContainedKey == c.Key))
                    {
                        childList.Add(child);
                        nextChildList.Add(child);
                        child.Parent.Add(chest);
                    }

                    chest.ChildsDict.Add(rank, childList);
                    chest.CreateArrows();
                }
            }
        }
    }
    
    private void ConstructChestGraph2()
    {
        var chest = Chests[0];
        var rank = 0;
        
        ConstructChestGraph2Worker(chest, rank);
    }
    
    private void ConstructChestGraph2Worker(Chest chest, int rank)
    {
        if (chest.ContainedKey == Key.NO_CONDITION) return;
        
        //chest.HasChilds = true;

        var childList = new List<Chest>();
        
        foreach (var chest1 in Chests.Where(chest1 => chest1 != chest && chest.ContainedKey == chest1.Key))
        {
            childList.Add(chest1);
        }
        
        if (childList.Count == 0) return;
        
        chest.ChildsDict.Add(rank, childList);
        chest.CreateArrows();
        
        foreach (var child in childList)
        {
            ConstructChestGraph2Worker(child, rank + 1);
        }
    }



    private void Clean()
    {
        Arrow.CleanArrows();

        if (Chests == null || Chests.Count == 0) return;

        foreach (var chest in Chests.Where(chest => chest != null))
        {
            Destroy(chest.gameObject);
        }
    }
}

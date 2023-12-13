using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class ChestGenerator : MonoBehaviour
{
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
        J = 9
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
        KEY9 = 9
    }

    private const int ChestCount = 10;

    public string StrSeed
    {
        set
        {
            if (int.TryParse(value, out var seed)) Seed = seed;
        }
    }

    private int Seed { get; set; }

    public string StrMaxRep
    {
        set
        {
            if (int.TryParse(value, out var maxRep)) MaxRep = Mathf.Max(1, maxRep);
        }
    }

    private int MaxRep { get; set; } = 2;


    private List<Path> Paths { get; set; }
    public Path CurrentPath => Paths[PathIndex];
    private int PathIndex { get; set; }


    private void Init()
    {
        Seed = 1;

        Generate();
    }

    public void Generate()
    {
        Clean();

        SetSeed(Seed);

        var keys = GetKeyList();

        Chests = new List<Chest>();
        for (var i = 0; i < ChestCount; i++) Chests.Add(GenerateChest((ChestID)i, keys[i]));

        FillChests();

        CleanFillChest(keys);
        ConstructChestGraph();
        DisplayPaths();
    }

    private static void SetSeed(int seed)
    {
        Random.InitState(seed);
    }

    /// <summary>
    ///     Get a list of 10 keys including at least one NO_CONDITION to make sure you can start,
    ///     and 9 other unique keys (including possible other NO_CONDITION) in random order
    /// </summary>
    /// <returns></returns>
    private List<Key> GetKeyList()
    {
        var pickList = new List<Key>
        {
            Key.KEY1,
            Key.KEY2,
            Key.KEY3,
            Key.KEY4,
            Key.KEY5,
            Key.KEY6,
            Key.KEY7,
            Key.KEY8,
            Key.KEY9
        };
        var finalList = new List<Key> { Key.NO_CONDITION };

        Dictionary<Key, int> keyDico = new();

        for (var i = 1; i < ChestCount; i++)
        {
            var randomPick = Random.Range(0, pickList.Count);
            var key = pickList[randomPick];

            if (keyDico.ContainsKey(key))
                keyDico[key]++;
            else
                keyDico[key] = 1;

            if (keyDico[key] == MaxRep) pickList.RemoveAt(randomPick);

            finalList.Add(key);
        }

        return finalList;
    }

    /// <summary>
    ///     Generate a chest with a unique Key to access or no condition
    /// </summary>
    /// <param name="chestID"></param>
    /// <param name="key"></param>
    private Chest GenerateChest(ChestID chestID, Key key)
    {
        var chest = Instantiate(chestPrefab, chestAnchors[(int)chestID]).GetComponent<Chest>();

        chest.SetTitleAndKey(chestID, key);

        return chest;
    }

    public bool ShortChest(Chest chest)
    {
        if (chest.IsFinalChest || chest == Chests[0]) return false;

        if (chest.ContainedKey != Chests[0].ContainedKey)
            foreach (var child in chest.ChildsDict.SelectMany(rank => rank.Value))
                child.SetTitleAndKey(child.ChestID,
                    chest.Key == child.ContainedKey ? Chests[0].ContainedKey : chest.Key);

        Chests.Remove(chest);

        Arrow.CleanArrows();
        ConstructChestGraph();
        DisplayPaths();

        return true;
    }


    private void FillChests()
    {
        List<Chest> chests = new(Chests);

        // Remove First Chest
        chests.RemoveAt(0);

        // Get Random Final Chest
        var finalChest = chests[^1];
        chests.Remove(finalChest);

        var chest = chests[Random.Range(0, chests.Count)];
        chests.Remove(chest);
        FillChest(chest, finalChest.Key, chests);
    }

    private void FillChest(Chest chest, Key key, List<Chest> chests)
    {
        while (true)
        {
            // Fill chest
            chest.SetContainingKey(key);

            // If list null --> return
            if (chests == null) return;

            // If nothing left --> first chest
            if (chests.Count == 0)
            {
                var chest1 = chest;
                chest = Chests[0];
                key = chest1.Key;
                chests = null;
            }

            // Get random chest
            else
            {
                var nextChest = chests[Random.Range(0, chests.Count)];
                if (nextChest.Key == chest.Key) nextChest = chests[Random.Range(0, chests.Count)];

                chests.Remove(nextChest);
                var chest1 = chest;
                chest = nextChest;
                key = chest1.Key;
            }
        }
    }

    private void CleanFillChest(IReadOnlyList<Key> keys)
    {
        foreach (var chest in Chests.Where(chest =>
                     chest.HasCondition && !chest.IsFinalChest && chest.ContainedKey == chest.Key))
        {
            Key newKey;
            do
            {
                newKey = keys[Random.Range(0, keys.Count)];
            } while (newKey == chest.Key || newKey == Key.NO_CONDITION);

            chest.SetContainingKey(newKey);
        }
    }

    private void DisplayPaths()
    {
        Paths = PossiblePaths();
        Paths.Sort((p1, p2) => p1.Length.CompareTo(p2.Length));

        pathsDropdown.ClearOptions();

        var options = Paths.Select(t => t.Display).ToList();
        options.Add("All");
        pathsDropdown.AddOptions(options);

        if (Paths.Count <= 0) return;
        DisplayPathsArrow(0);

        minLengthText.text = "Min : " + (Paths[0].Length - 1);
        maxLengthText.text = "Max : " + (Paths[^1].Length - 1);
    }

    public void DisplayPathsArrow(int pathIndex)
    {
        var all = pathIndex == Paths.Count;

        foreach (var chest in Chests)
        {
            chest.SetArrowsActive(all);
            chest.IsOpenable = all;
        }

        if (all) return;

        PathIndex = pathIndex;
        Paths[PathIndex].SetActive(true);
    }

    private List<Path> PossiblePaths()
    {
        List<Path> paths = new();
        Path path = new();
        var start = Chests[^1];
        var end = Chests[0];
        var visited = new bool[ChestCount];
        DFS(start, end, visited, path, paths);

        foreach (var p in paths) p.Complete();

        return paths;
    }

    private void DFS(Chest current, Chest end, IList<bool> visited, Path path, ICollection<Path> paths)
    {
        visited[(int)current.ChestID] = true;
        path.Add(current);

        if (current == end)
            paths.Add(new Path(path));
        else
            foreach (var chest in current.Parent.Where(chest => !visited[(int)chest.ChestID]))
                DFS(chest, end, visited, path, paths);

        path.Remove(current);
        visited[(int)current.ChestID] = false;
    }

    private void ConstructChestGraph()
    {
        List<Chest> chests = new(Chests);

        var chest = Chests[0];
        chests.Remove(chest);
        //chests.Remove(Chests[^1]);

        List<Chest> childList = new();
        List<Chest> nextChildList = new();

        // reset
        foreach (var ch in Chests.Where(ch => ch != null))
        {
            ch.Parent.Clear();
            ch.ChildsDict.Clear();
            ch.HasChilds = false;
        }

        foreach (var child in chests.Where(c => c != chest && chest.ContainedKey == c.Key))
        {
            childList.Add(child);
            nextChildList.Add(child);
            child.Parent.Add(chest);
        }

        chest.ChildsDict[0] = new List<Chest>(childList);
        chest.CreateArrows();
        chest.HasChilds = true;

        var rank = 0;
        while (chests.Count > 0 && nextChildList.Count > 0)
        {
            rank++;
            var formerChildList = new List<Chest>(nextChildList);
            nextChildList.Clear();

            foreach (var childChest in formerChildList)
            {
                childList.Clear();
                chest = childChest;
                if (chest.HasChilds || chest.ContainedKey == Key.NO_CONDITION) continue;
                chest.HasChilds = true;

                chests.Remove(chest);

                foreach (var child in Chests.Where(c => c != chest && chest.ContainedKey == c.Key))
                {
                    childList.Add(child);
                    nextChildList.Add(child);
                    child.Parent.Add(chest);
                }

                chest.ChildsDict[rank] = new List<Chest>(childList);
                chest.CreateArrows();
            }
        }
    }

    private void Clean()
    {
        Arrow.CleanArrows();

        if (Chests == null || Chests.Count == 0) return;

        foreach (var chest in Chests.Where(chest => chest != null)) Destroy(chest.gameObject);
    }

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

    [FormerlySerializedAs("_chestAnchors")] [Header("References")] [SerializeField]
    private List<Transform> chestAnchors;

    [FormerlySerializedAs("_pathsDropdown")] [SerializeField]
    private TMP_Dropdown pathsDropdown;

    [FormerlySerializedAs("_maxLengthText")] [SerializeField]
    private TextMeshProUGUI maxLengthText;

    [FormerlySerializedAs("_minLengthText")] [SerializeField]
    private TextMeshProUGUI minLengthText;

    [FormerlySerializedAs("_chestPrefab")] [Header("Prefabs")] [SerializeField]
    private GameObject chestPrefab;


    private List<Chest> Chests { get; set; }

    public bool IsBinActive { get; set; }

    #endregion
}
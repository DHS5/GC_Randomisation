using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Chest : MonoBehaviour
{
    private static readonly int Open = Animator.StringToHash("Open");

    [Header("Chest")] [Header("References")] [SerializeField]
    private Animator animator;

    [SerializeField] private Renderer _topRenderer;
    [SerializeField] private Renderer _bottomRenderer;

    [Space(10f)] [SerializeField] private TextMeshProUGUI titleText;

    [SerializeField] private TextMeshPro letterText;
    [SerializeField] private TextMeshProUGUI conditionText;
    [SerializeField] private TextMeshProUGUI containsText;

    [Header("Prefabs")] [SerializeField] private GameObject _arrowPrefab;

    [Header("Materials")] [SerializeField] private Material _lockedMat;

    [SerializeField] private Material _unlockedMat;

    public ChestGenerator.ChestID ChestID { get; private set; }
    public ChestGenerator.Key Key { get; private set; }
    public List<Chest> Parent { get; private set; } = new();
    public bool HasChilds { get; set; }

    public Dictionary<int, List<Chest>> ChildsDict { get; } = new();

    public bool HasCondition => Key != ChestGenerator.Key.NO_CONDITION;
    public bool IsFinalChest => ContainedKey == ChestGenerator.Key.NO_CONDITION;

    public ChestGenerator.Key ContainedKey { get; private set; } = ChestGenerator.Key.NO_CONDITION;

    #region Animation

    private void OpenAnim()
    {
        animator.SetTrigger(Open);
    }

    #endregion

    public void SetTitleAndKey(ChestGenerator.ChestID chestID, ChestGenerator.Key key)
    {
        ChestID = chestID;
        Key = key;

        BillboardSetTitleAndKey(chestID, key);
    }

    public void SetContainingKey(ChestGenerator.Key key)
    {
        ContainedKey = key;
        BillboardSetContainingKeys();
    }

    #region Billboard

    private void BillboardSetTitleAndKey(ChestGenerator.ChestID chestID, ChestGenerator.Key key)
    {
        titleText.text = "Chest " + chestID;
        letterText.text = chestID.ToString();
        conditionText.text =
            key == ChestGenerator.Key.NO_CONDITION
                ? "Open"
                : "Key : " + key;
    }

    private void BillboardSetContainingKeys()
    {
        containsText.text = IsFinalChest ? "Final chest" : "Contains " + ContainedKey;
    }

    private void BillboardResetContainingKeys()
    {
        containsText.text = "";
    }

    #endregion

    #region Open

    private bool _isOpenable;

    public bool IsOpenable
    {
        get => _isOpenable;
        set
        {
            _isOpenable = value;
            if (value == false) BillboardResetContainingKeys();

            _topRenderer.sharedMaterial = value ? _unlockedMat : _lockedMat;
            _bottomRenderer.sharedMaterial = value ? _unlockedMat : _lockedMat;
        }
    }

    private void OnMouseDown()
    {
        if (ChestGenerator.Instance.IsBinActive)
        {
            if (ChestGenerator.Instance.ShortChest(this))
                Destroy(gameObject);
        }
        else if (IsOpenable)
        {
            ChestGenerator.Instance.CurrentPath.OpenChest(this);
            OpenAnim();
            containsText.gameObject.SetActive(true);
            BillboardSetContainingKeys();
        }
    }

    #endregion

    #region Arrow

    private readonly Dictionary<ChestGenerator.ChestID, Arrow> _arrows = new();

    public void CreateArrows()
    {
        if (ChildsDict.Count == 0) return;

        _arrows.Clear();

        foreach (var rank in ChildsDict)
        foreach (var child in rank.Value.Where(child => child != null))
            CreateArrow(child, rank.Key);
    }

    private void CreateArrow(Chest chest, int rank)
    {
        var arrow = Instantiate(_arrowPrefab).GetComponent<Arrow>();

        _arrows.Add(chest.ChestID, arrow);
        arrow.Init(transform, chest.transform, rank);
    }

    public void SetArrowsActive(bool active)
    {
        foreach (var pair in _arrows.Where(pair => pair.Value != null))
            pair.Value.SetActive(active);
    }

    public void SetArrowsActive(bool active, ChestGenerator.ChestID chestID, int rank)
    {
        if (_arrows.TryGetValue(chestID, out var arrow)) arrow.SetActive(active, rank);
    }

    public void DisplayChild()
    {
        foreach (var child in ChildsDict.SelectMany(pair => pair.Value))
            Debug.Log(ChestID + " has child " + child.ChestID);
    }

    #endregion
}
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [Header("Chest")]

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Renderer _topRenderer;
    [SerializeField] private Renderer _bottomRenderer;
    [Space(10f)]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshPro letterText;
    [SerializeField] private TextMeshProUGUI conditionText;
    [SerializeField] private TextMeshProUGUI containsText;
    private static readonly int Open = Animator.StringToHash("Open");

    [Header("Prefabs")]
    [SerializeField] private GameObject _arrowPrefab;

    [Header("Materials")]
    [SerializeField] private Material _lockedMat;
    [SerializeField] private Material _unlockedMat;

    public ChestGenerator.ChestID ChestID { get; private set; }
    public ChestGenerator.Key Key { get; private set; }
    public List<Chest> Parent { get; private set; } = new();
    public bool HasChilds { get; set; }
    
    public Dictionary<int, List<Chest>> ChildsDict { get; set; } = new();

    public bool HasCondition => Key != ChestGenerator.Key.NO_CONDITION;
    public bool IsFinalChest => ContainedKey == ChestGenerator.Key.NO_CONDITION;

    public ChestGenerator.Key ContainedKey { get; private set; } = ChestGenerator.Key.NO_CONDITION;

    #region Animation 

    public void OpenAnim()
    {
        animator.SetTrigger(Open);
    }

    #endregion

    #region Billboard

    private void BillboardSetTitleAndKey(ChestGenerator.ChestID chestID, ChestGenerator.Key key)
    {
        titleText.text = "Chest " + chestID;
        letterText.text = chestID.ToString();
        conditionText.text =
            key == ChestGenerator.Key.NO_CONDITION ?
            "Open"
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

    private bool isOpenable = false;
    public bool IsOpenable
    {
        get => isOpenable;
        set
        {
            isOpenable = value;
            if (value == false)
            {
                BillboardResetContainingKeys();
            }

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

    #region Arrow

    private Dictionary<ChestGenerator.ChestID, Arrow> arrows = new();

    public void CreateArrows()
    {
        if (ChildsDict.Count == 0) return;

        arrows.Clear();

        foreach (var rank in ChildsDict)
        {
            foreach (var child in rank.Value)
            {
                if (child != null)
                {
                    CreateArrow(child, rank.Key);
                }
            }
        }
    }
    private void CreateArrow(Chest chest, int rank)
    {
        Arrow arrow = Instantiate(_arrowPrefab).GetComponent<Arrow>();

        arrows.Add(chest.ChestID, arrow);
        arrow.Init(transform, chest.transform, rank);
    }

    public void SetArrowsActive(bool active)
    {
        foreach (var pair in arrows)
        {
            if (pair.Value != null)
            {
                pair.Value.SetActive(active);
            }
        }
    }
    public void SetArrowsActive(bool active, ChestGenerator.ChestID chestID, int rank)
    {
        if (arrows.ContainsKey(chestID))
        {
            arrows[chestID].SetActive(active, rank);
        }
    }

    public void DisplayChild()
    {
        foreach (var pair in ChildsDict)
        {
            foreach (var child in pair.Value)
            {
                Debug.Log(ChestID + " has child " + child.ChestID);
            }
        }
    }

    #endregion
}

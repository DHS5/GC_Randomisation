using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.XR;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [Header("Chest")]

    [Header("References")]
    [SerializeField] private Animator animator;
    [Space(10f)]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshPro letterText;
    [SerializeField] private TextMeshProUGUI conditionText;
    [SerializeField] private TextMeshProUGUI containsText;
    private static readonly int Open = Animator.StringToHash("Open");

    [Header("Prefabs")]
    [SerializeField] private GameObject _arrowPrefab;

    public ChestGenerator.ChestID ChestID { get; private set; }
    public ChestGenerator.Key Key { get; private set; }
    public List<Chest> Childs { get; private set; } = new();
    public List<Chest> Parent { get; private set; } = new();
    public bool HasChilds { get; set; }
    
    public Dictionary<int, List<Chest>> ChildsDict { get; private set; } = new();

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

    #endregion

    #region Open

    private void OnMouseDown()
    {
        OpenAnim();
        containsText.gameObject.SetActive(true);
        BillboardSetContainingKeys();
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

    public void CreateArrows()
    {
        if (ChildsDict.Count == 0) return;

        foreach (var rank in ChildsDict)
        {
            foreach (var child in rank.Value)

            if (child != null)
            {
                CreateArrow(child, rank.Key);
            }
        }
    }
    private void CreateArrow(Chest chest, int rank)
    {
        Arrow arrow = Instantiate(_arrowPrefab).GetComponent<Arrow>();

        arrow.Init(transform, chest.transform, rank);

        //chest.CreateArrows(rank + 1);
    }

    #endregion
}

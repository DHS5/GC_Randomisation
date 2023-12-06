using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [Header("Chest")]

    [Header("References")]
    [SerializeField] private Animator animator;
    [Space(10f)]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI conditionText;
    private static readonly int Open = Animator.StringToHash("Open");

    public ChestGenerator.ChestID ChestID { get; private set; }
    public ChestGenerator.Key Key { get; private set; }

    public bool HasCondition => Key != ChestGenerator.Key.NO_CONDITION;

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
        conditionText.text =
            key == ChestGenerator.Key.NO_CONDITION ?
            "No condition"
            : "Open with key " + key;
    }

    #endregion

    #region Open

    private void OnMouseDown()
    {
        OpenAnim();
    }

    #endregion

    public void SetTitleAndKey(ChestGenerator.ChestID chestID, ChestGenerator.Key key)
    {
        ChestID = chestID;
        Key = key;

        BillboardSetTitleAndKey(chestID, key);
    }
}

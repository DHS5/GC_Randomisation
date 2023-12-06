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


    #region Animation 

    public void OpenAnim()
    {
        animator.SetTrigger("Open");
    }

    #endregion

    #region Billboard

    public void SetTitle(ChestGenerator.ChestID chestID)
    {
        titleText.text = "Chest " + chestID;
    }
    public void SetCondition(ChestGenerator.Key key)
    {
        conditionText.text =
            key == ChestGenerator.Key.NO_CONDITION ?
            "No condition"
            : "Open with key " + key;
    }

    #endregion
}

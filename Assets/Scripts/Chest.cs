using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [Header("Chest")]

    [Header("References")]
    [SerializeField] private Animator animator;


    #region Animation 

    public void OpenAnim()
    {
        animator.SetTrigger("Open");
    }

    #endregion
}

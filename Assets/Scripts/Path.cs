using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path
{
    public int Length { get; private set; }

    private List<Chest> chests;

    public void AddChest(Chest chest)
    {
        chests.Add(chest);
    }
}

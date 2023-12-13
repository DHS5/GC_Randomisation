using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Path
{
    public Path()
    {
        chests = new();
    }
    public Path(Path other)
    {
        chests = new(other.chests);
    }

    public int Length => chests.Count;

    private List<Chest> chests;

    public void Add(Chest chest)
    {
        chests.Add(chest);
    }
    public void Remove(Chest chest)
    {
        chests.Remove(chest);
    }
    public void Complete()
    {
        chests.Reverse();
    }

    public void SetActive(bool active)
    {
        chests[0].IsOpenable = active;
        for (int i = 0; i < chests.Count - 1; i++)
        {
            if (chests[i] != null)
            {
                chests[i].SetArrowsActive(active, chests[i + 1].ChestID, i);
            }
        }
    }

    public void OpenChest(Chest chest)
    {
        if (chests.Contains(chest))
        {
            int index = chests.IndexOf(chest);
            if (index < chests.Count - 1)
            {
                chests[index + 1].IsOpenable = true;
            }
        }
    }

    public string Display
    {
        get
        {
            StringBuilder sb = new();

            foreach (var chest in chests)
            {
                sb.Append(chest.ChestID + " ");
            }
            return sb.ToString();
        }
    }
}

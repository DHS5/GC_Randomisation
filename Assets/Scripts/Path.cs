using System.Collections.Generic;
using System.Text;

public class Path
{
    private readonly List<Chest> _chests;

    public Path()
    {
        _chests = new List<Chest>();
    }

    public Path(Path other)
    {
        _chests = new List<Chest>(other._chests);
    }

    public int Length => _chests.Count;

    public string Display
    {
        get
        {
            StringBuilder sb = new();

            foreach (var chest in _chests) sb.Append(chest.ChestID + " ");
            return sb.ToString();
        }
    }

    public void Add(Chest chest)
    {
        _chests.Add(chest);
    }

    public void Remove(Chest chest)
    {
        _chests.Remove(chest);
    }

    public void Complete()
    {
        _chests.Reverse();
    }

    public void SetActive(bool active)
    {
        _chests[0].IsOpenable = active;
        for (var i = 0; i < _chests.Count - 1; i++)
            if (_chests[i] != null)
                _chests[i].SetArrowsActive(active, _chests[i + 1].ChestID, i);
    }

    public void OpenChest(Chest chest)
    {
        if (!_chests.Contains(chest)) return;
        var index = _chests.IndexOf(chest);
        if (index < _chests.Count - 1) _chests[index + 1].IsOpenable = true;
    }
}
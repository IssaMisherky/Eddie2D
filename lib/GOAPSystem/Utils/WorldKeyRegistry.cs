using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "WorldKeyRegistry", menuName = "GOAP/World Key Registry")]
public class WorldKeyRegistry : ScriptableObject
{
    [SerializeField] private List<string> keys = new List<string>();

    private Dictionary<string, int> _stringToId = new Dictionary<string, int>();

    public void Initialize()
    {
        _stringToId.Clear();
        for (int i = 0; i < keys.Count; i++)
        {
            if (!string.IsNullOrEmpty(keys[i]))
                _stringToId[keys[i]] = i;
        }
    }

    public int GetId(string key)
    {
        if (_stringToId.Count == 0) Initialize();
        return _stringToId.TryGetValue(key, out int id) ? id : -1;
    }

    private void OnValidate() => Initialize();
}
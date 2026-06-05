using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SoundDefinition")]
public class SoundDefinitionSO : ScriptableObject
{
    [Serializable]
    public class BGM
    {
        public AudioClip clip;
        public List<string> names;
    }

    public List<BGM> bgms;

    public AudioClip Find(string name)
    {
        foreach (BGM bgm in bgms)
        {
            if(bgm.names.Contains(name))
            {
                return bgm.clip;
            }
        }
        return null;
    }
}

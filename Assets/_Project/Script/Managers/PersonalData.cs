using System;
using System.Collections.Generic;
using UnityEngine;

public partial class ServerSaveDataManager
{
    [Serializable]
    public class PersonalData
    {
        [SerializeField]
        string nickName = "(NoName)";
        [SerializeField]
        int clearCount = 0;
        [SerializeField]
        int maxScore = 0;
        [SerializeField]
        int playCount = 0;

        [NonSerialized]
        bool nickNameDirty = false;
        [NonSerialized]
        bool clearCountDirty = false;
        [NonSerialized]
        bool maxScoreDirty = false;
        [NonSerialized]
        bool playCountDirty = false;

        public string NickName
        {
            get => nickName;
            set
            {
                if(nickName == value) return;
                nickName = value;
                nickNameDirty = true;
            }
        }
        public int ClearCount
        {
            get => clearCount;
            set
            {
                if(clearCount== value) return;
                clearCount= value;
                clearCountDirty = true;
            }
        }
        public int MaxScore
        {
            get=> maxScore;
            set
            {
                if(maxScore== value) return;
                maxScore= value;
                maxScoreDirty = true;
            }
        }
        public int PlayCount
        {
            get=> playCount;
            set
            {
                if(playCount== value) return;
                playCount = value;
                playCountDirty = true;
            }
        }

        public bool IsDirty
        {
            get
            {
                return nickNameDirty || clearCountDirty || playCountDirty || maxScoreDirty;
            }
        }

        public Dictionary<string, object> GetUpdateDict()
        {
            Dictionary<string, object> result = new();
            if (nickNameDirty)
                result.Add("nickName", nickName);
            if (clearCountDirty)
                result.Add("clearCount", clearCount);
            if (maxScoreDirty)
                result.Add("maxScore", maxScore);
            if (playCountDirty)
                result.Add("playCount", playCount);

            return result;
        }

        public void ResetDirtyFlags()
        {
            nickNameDirty = false;
            clearCountDirty = false;
            maxScoreDirty = false;
            playCountDirty = false;
        }

        public static PersonalData FromJson(string json)
        {
            return JsonUtility.FromJson<PersonalData>(json);
        }

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }
    }
}

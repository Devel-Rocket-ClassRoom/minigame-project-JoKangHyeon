using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GlobalSaveData
{
    public RunData currentRun;

    public GlobalSaveData()
    {
        
    }
}

[Serializable]
public class RunData
{
    public int version;
    public RunData currentRuns;

    public RunData()
    {
        
    }

    public RunData UpdateVersion()
    {
        throw new NotImplementedException();
    }
}
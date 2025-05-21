using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static Jobs;
using static AgentManager;
using Unity.VisualScripting;

public class Tags
{
    public enum Tag
    {
        None,
        FREE,
        DICE,
        CHOICE,
        ENIGME,
        COMBAT,
        NEXT,
        END
    }   
}

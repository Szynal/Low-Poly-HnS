using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FE_ObjectHealth : FE_Health
{
    public List<FE_ActionContainer> AfterDeathChanges = new List<FE_ActionContainer>();

    protected override void die(bool _silentDeath)
    {
        base.die(_silentDeath);
        if(GetComponentInParent<FE_MultipleStateObject>() != null)
        {
            GetComponentInParent<FE_MultipleStateObject>().ChangeState(EStateMessage.ObjectDestroyed, true);
        }

        foreach (FE_ActionContainer _state in AfterDeathChanges)
        {
            FE_StateChanger.HandleObjectChange(_state);
        }

        Destroy(gameObject);
    }
}

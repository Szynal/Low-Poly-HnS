using System.Collections.Generic;
using LowPolyHnS;

public class ObjectHealth : Health
{
    public List<FE_ActionContainer> AfterDeathChanges = new List<FE_ActionContainer>();

    protected override void Die()
    {
        base.Die();
        if (GetComponentInParent<FE_MultipleStateObject>() != null)
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
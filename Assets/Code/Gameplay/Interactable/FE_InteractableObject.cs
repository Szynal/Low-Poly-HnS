using System;
using System.Threading.Tasks;
using LowPolyHnS;
using UnityEngine;

[RequireComponent(typeof(Collider))]
//Abstract, base class for all interactable objects in game
public abstract class FE_InteractableObject : MonoBehaviour
{
    [Header("Inherited from InteractableObject", order = 2)] [SerializeField]
    protected string interactionString = default;

    public string GetInteractionString()
    {
        return interactionString;
    }

    [SerializeField] protected Color stringColor = default;

    public Color GetStringColor()
    {
        return stringColor;
    }

    protected bool canInteract = true;
    [SerializeField] protected bool isMultipleUse = false;

    [Header("Properties for starting cutscenes", order = 2)]
    public bool StartCutsceneOnUse = false;

    public string CutsceneName = "";

    [Header("Used for objects that require items to use", order = 4)] [SerializeField]
    private bool needItem = false;

    public bool RequireItem()
    {
        return needItem;
    }
    
    protected bool needToUnholsterAfter;

    public delegate void OnInteractableUsed(FE_InteractableObject _usedInteractable);

    public OnInteractableUsed UseDelegate;

    virtual protected void Awake()
    {
        if (GetComponent<Collider>() == null)
        {
            Debug.LogError("No collider found for interactable: " + gameObject.name);
        }
        else
        {
            GetComponent<Collider>().isTrigger = true;
            gameObject.layer = 11; //11 = InteractTrigger
        }
    }


    public virtual void Activate()
    {
    }

    virtual protected void onActivation()
    {
        if (StartCutsceneOnUse)
        {
            SceneLoader.Instance.OpenCutscene(CutsceneName);
        }
    }

    virtual protected void onInteractionEnd()
    {
        UseDelegate?.Invoke(this);

        if (needToUnholsterAfter)
        {
            needToUnholsterAfter = false;
        }

        if (isMultipleUse)
        {
            SetCanInteract(true);
        }
    }

    protected async void failActivation()
    {
        SetCanInteract(false);

        await Task.Delay(TimeSpan.FromSeconds(0.5f));

        SetCanInteract(true);
    }

    public void SetCanInteract(bool _newVal)
    {
        if (_newVal)
        {
            canInteract = true;
            GetComponent<Collider>().enabled = true;
        }
        else
        {
            canInteract = false;
            GetComponent<Collider>().enabled = false;
        }
    }
}
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

    public int RequiredItemID;
    [SerializeField] private bool removeItemAfterUse = false;

    protected FE_PlayerInventoryInteraction playerRef;
    protected bool isInProperSpot = false;

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

    virtual protected void OnTriggerEnter(Collider _other)
    {
        if (canInteract)
        {
            if (_other.tag == "Player")
            {
                playerRef = _other.GetComponent<FE_PlayerInventoryInteraction>();
                if (playerRef.GetObjectsToUse().Contains(this) == false)
                {
                    playerRef.AddInteraction(this);
                }
            }
        }
    }

    virtual protected void OnTriggerExit(Collider _other)
    {
        if (_other.tag == "Player" &&
            _other.GetComponent<FE_PlayerInventoryInteraction>().GetObjectsToUse().Contains(this))
        {
            playerRef.RemoveInteraction(this);
            playerRef = null;
        }
    }

    protected virtual void OnDestroy()
    {
        if (playerRef != null)
        {
            playerRef.RemoveInteraction(this);
            playerRef = null;
        }
    }

    protected virtual void OnDisable()
    {
        if (playerRef != null)
        {
            playerRef.RemoveInteraction(this);
            playerRef = null;
        }
    }

    public virtual void Activate(FE_PlayerInventoryInteraction _instigator, FE_Item _itemUsed = null,
        bool _bypassItemUsage = false)
    {
        if (needItem == false || _itemUsed != null && _itemUsed.itemID == RequiredItemID || _bypassItemUsage)
        {
            onActivation(_instigator);
        }
        else
        {
            failActivation();
        }
    }

    virtual protected void onActivation(FE_PlayerInventoryInteraction _instigator)
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

        if (RequireItem() && removeItemAfterUse)
        {
            FE_UseableItem _useableToRemove =
                FE_PlayerInventoryInteraction.Instance.GetInventoryItemByID(RequiredItemID) as FE_UseableItem;
            if (_useableToRemove != null && _useableToRemove.IsStackable)
            {
                if (_useableToRemove.UseStack())
                {
                    FE_PlayerInventoryInteraction.Instance.RemoveItem(_useableToRemove);
                }
            }
            else
            {
                FE_PlayerInventoryInteraction.Instance.RemoveItem(
                    GameManager.Instance.ItemDatabase.GetItemByID(RequiredItemID));
            }
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
            if (playerRef != null)
            {
                playerRef.RemoveInteraction(this);
                playerRef = null;
            }
        }
    }
}
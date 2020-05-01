using System.Collections;
using UnityEngine;

public class FE_OpenableDoor : FE_InteractableObject
{
    [Header("Openable door properties")] [SerializeField]
    private bool staysOpen = true;

    [SerializeField] private float selfCloseTime = 30f;
    [SerializeField] private bool isOpenedAtStart = false;

    private Animation openAnim;
    private bool isOpen;
    private Collider triggerCol;

    private FE_MultipleStateObject parentScript;

    protected override void Awake()
    {
        base.Awake();

        openAnim = GetComponent<Animation>();
        parentScript = GetComponentInParent<FE_MultipleStateObject>();
        triggerCol = GetComponent<Collider>();

        if (openAnim == false)
        {
            Debug.LogError("Openable door " + name + " has no animation component!");
        }

        if (staysOpen == false && selfCloseTime <= 0f)
        {
            Debug.LogWarning("Openable door " + name +
                             " is set to close itself, but the closing time is <= 0. Setting to default closing time...");
            selfCloseTime = 30f;
        }

        if (triggerCol == null)
        {
            Debug.LogError("Openable Door " + name + " have no trigger collider!");
        }

        if (parentScript == null)
        {
            Debug.LogError("Openable Door " + name + " couldn't find FE_MultipleStateObject script in parent!");
        }

        if (isOpenedAtStart)
        {
            instantOpen();
        }
    }

    protected override void onActivation(FE_PlayerInventoryInteraction _instigator)
    {
        base.onActivation(_instigator);

        if (isOpen == false)
        {
            openAnim.Play("ANIM_openDoor");
            isOpen = true;
            parentScript.ChangeState(EStateMessage.DoorOpened, false);

            SetCanInteract(false);

            if (staysOpen == false)
            {
                StopAllCoroutines();
                StartCoroutine(closeTheDoor());
            }
        }

        onInteractionEnd();
    }

    private IEnumerator closeTheDoor()
    {
        yield return new WaitForSeconds(selfCloseTime);

        openAnim.Play("ANIM_closeDoor");
        parentScript.ChangeState(EStateMessage.DoorClosed, false);
        isOpen = false;
        SetCanInteract(true);

        if (isOpenedAtStart)
        {
            openAnim["ANIM_openDoor"].time = 0f;
        }
    }

    private void instantOpen()
    {
        if (isOpen == false)
        {
            openAnim["ANIM_openDoor"].time = openAnim["ANIM_openDoor"].clip.length;
            openAnim.Play("ANIM_openDoor");
            isOpen = true;

            SetCanInteract(false);

            if (staysOpen == false)
            {
                StopAllCoroutines();
                StartCoroutine(closeTheDoor());
            }
        }
    }
}
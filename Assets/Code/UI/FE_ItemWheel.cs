using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class FE_ItemWheel : MonoBehaviour
{
    private struct SlotTransform
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;
    }

    [SerializeField] FE_ItemWheelItem[] slotObjects = null;
    public bool CanBeOpened = true;
    [Space(5f)]
    [SerializeField] List<FE_Item> listOfItems = null;

    private SlotTransform[] slotPositions = null;
    private int middleIndex = 0;
    private int selectedIndex = 0;

    private void Awake()
    {
        if(slotObjects.Length < 1)
        {
            return;
        }

        slotPositions = new SlotTransform[slotObjects.Length];

        for(int i = 0; i < slotObjects.Length; i++)
        {
            slotPositions[i].Position = slotObjects[i].transform.localPosition;
            slotPositions[i].Rotation = slotObjects[i].transform.localRotation;
            slotPositions[i].Scale = slotObjects[i].transform.localScale;
        }

       // middleIndex = slotObjects.Length / 2;
    }

    private void OnDisable()
    {
        for (int i = 0; i < slotObjects.Length; i++)
        {
            slotObjects[i].transform.localPosition = slotPositions[i].Position;
            slotObjects[i].transform.localRotation = slotPositions[i].Rotation;
            slotObjects[i].transform.localScale = slotPositions[i].Scale;
        }

        //We disable them so they aren't visible when enabling the inventory screen
        slotObjects[0].gameObject.SetActive(false);
        slotObjects[slotObjects.Length - 1].gameObject.SetActive(false);
    }

    private Task calculateMidPoint()
    {
        middleIndex = slotObjects.Length / 2;
        return Task.CompletedTask;
    }

    public async void Initialize(List<FE_Item> _itemList, FE_Item _firstSelected)
    {
        if (_itemList.Count < 1)
        {
            CanBeOpened = false;
            return;
        }

        CanBeOpened = true;
        await calculateMidPoint(); //We do it like that, because we've had some problems with Awake() not happening quick enough and middleIndex not being calculated on time

        listOfItems = _itemList;

        if(_firstSelected == null)
        {
            _firstSelected = _itemList[0];
        }

        for(int i = 0; i < _itemList.Count; i++)
        {
            if(_itemList[i] == _firstSelected)
            {
                selectedIndex = i;
                break;
            }
        }

        if (_itemList.Count > 1)
        {
            slotObjects[middleIndex - 2].gameObject.SetActive(true);
            slotObjects[middleIndex - 1].gameObject.SetActive(true);
            slotObjects[middleIndex + 1].gameObject.SetActive(true);
            slotObjects[middleIndex + 2].gameObject.SetActive(true);
        }
        else
        {
            slotObjects[middleIndex - 2].gameObject.SetActive(false);
            slotObjects[middleIndex - 1].gameObject.SetActive(false);
            slotObjects[middleIndex + 1].gameObject.SetActive(false);
            slotObjects[middleIndex + 2].gameObject.SetActive(false);
        }

        setObjectsToSlots();
    }

    public void Rotate(int _input, float _inputDelay)
    {
        if (listOfItems.Count > 1)
        {
            StartCoroutine(animateWheel(_input, _inputDelay));
        }
    }

    public void UseCurrent()
    {
        FE_PlayerInventoryInteraction.Instance.UseItem(listOfItems[selectedIndex]);

        //if(listOfItems[selectedIndex] is FE_Weapon) //Refresh weapon
        //{
        //    setObjectsToSlots();
        //}
    }

    private void setObjectsToSlots()
    {
        for (int i = 0; i < slotObjects.Length; i++)
        {
            if (slotObjects[i].gameObject.activeSelf == false)
            {
                continue;
            }

            slotObjects[i].SetItem(listOfItems[arrayIndexWrapped(selectedIndex - (middleIndex - i), listOfItems.Count)]);
        }
    }

    private IEnumerator animateWheel(int _input, float _animTime)
    {
        int _itemToActivateIndex = 0;

        if (_input < 0)
        {
            for(int i = middleIndex; i > 0; i --)
            {
                if(slotObjects[i].gameObject.activeSelf == false)
                {
                    _itemToActivateIndex = i;
                    break;
                }
            }
        }
        else
        {
            for (int i = middleIndex; i < slotObjects.Length; i++)
            {
                if (slotObjects[i].gameObject.activeSelf == false)
                {
                    _itemToActivateIndex = i;
                    break;
                }
            }
        }

        slotObjects[_itemToActivateIndex].gameObject.SetActive(true);
        slotObjects[_itemToActivateIndex].SetItem(listOfItems[arrayIndexWrapped(selectedIndex - (middleIndex - _itemToActivateIndex), listOfItems.Count)]);

        float _lerpProgress = 0f;
        while(_lerpProgress < 1f)
        {
            _lerpProgress += Time.unscaledDeltaTime / _animTime;

            for(int i = 0; i < slotObjects.Length; i++)
            {
                slotObjects[i].transform.localPosition = Vector3.Lerp(slotPositions[i].Position, slotPositions[arrayIndexWrapped(i - _input, slotPositions.Length)].Position, _lerpProgress);
                slotObjects[i].transform.localRotation = Quaternion.Lerp(slotPositions[i].Rotation, slotPositions[arrayIndexWrapped(i - _input, slotPositions.Length)].Rotation, _lerpProgress);
                slotObjects[i].transform.localScale = Vector3.Lerp(slotPositions[i].Scale, slotPositions[arrayIndexWrapped(i - _input, slotPositions.Length)].Scale, _lerpProgress);
            }

            yield return null;
        }

        slotObjects[_itemToActivateIndex].gameObject.SetActive(false);

        selectedIndex = arrayIndexWrapped(selectedIndex + _input, listOfItems.Count);
        setObjectsToSlots();

        for (int i = 0; i < slotObjects.Length; i++)
        {
            slotObjects[i].transform.localPosition = slotPositions[i].Position;
            slotObjects[i].transform.localRotation = slotPositions[i].Rotation;
            slotObjects[i].transform.localScale = slotPositions[i].Scale;
        }
    }

    private int arrayIndexWrapped(int _index, int _arrayCount)
    {
        if(_arrayCount == 1)
        {
            return 0;
        }

        if(_index >= _arrayCount)
        {
            return Mathf.Clamp(_index - _arrayCount, 0, _arrayCount - 1);        
        }

        if(_index < 0)
        {
            return Mathf.Clamp(_arrayCount + _index, 0, _arrayCount - 1);
        }

        return _index;
    }
}

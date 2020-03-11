using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FE_ButtonControlledCabels : MonoBehaviour
{
    [SerializeField]public List<Button> controlledButtons;
    [SerializeField] public GameObject cursorImage;
    [SerializeField] public Sprite newImage;
    [SerializeField] public int[] completeCut = { 1, 0, 1 };
    private int[] cuttedArray = {0,0,0};
    private bool completed = true;
    
    // Start is called before the first frame update
    void Start()
    {
        if (cursorImage != null)
            cursorImage.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void HideCables()
    {
        foreach (var cable in controlledButtons)
        {
            cable.GetComponent<FE_ButtonControlledCabels>().cursorImage.SetActive(false);
        }
    }
    public void ShowCable(int cableIndex)
    {
        controlledButtons[cableIndex].GetComponent<FE_ButtonControlledCabels>().cursorImage.SetActive(true);
    }
    public void HideCable(int cableIndex)
    {
        controlledButtons[cableIndex].GetComponent<FE_ButtonControlledCabels>().cursorImage.SetActive(false);
    }
    public void CutCable(int cableIndex)
    {
        
            //cutted = true;
            controlledButtons[cableIndex].GetComponent<Image>().sprite = controlledButtons[cableIndex].GetComponent<FE_ButtonControlledCabels>().newImage;
            controlledButtons[cableIndex].GetComponent<FE_ButtonControlledCabels>().cursorImage.GetComponent<Image>().sprite = controlledButtons[cableIndex].GetComponent<FE_ButtonControlledCabels>().newImage;
            cuttedArray[cableIndex] = 1;
        
    }
    public int checkCables()
    {
        completed = true;
        for (int i = 0; i<completeCut.Length;i++)
        {
            if (cuttedArray[i] == 1 && completeCut[i] == 0)
                return -1;
            if (cuttedArray[i] != completeCut[i])
                completed = false;
        }
        if (completed)
            return 1;
        return 0;
    }
}

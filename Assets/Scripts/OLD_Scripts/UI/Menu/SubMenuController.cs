using LowPolyHnS;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SubMenuController : MonoBehaviour
{
    [Header("SubMenu Properties")] [Tooltip("Defaults to first child Selectable.")] [SerializeField]
    private Selectable firstToSelect;

    [SerializeField] private bool rememberSelection = true;
    [SerializeField] private bool canGoBack = true;

    protected SubMenuController parent;

    private bool initialized = false;
    private bool resetParent = true;

    private void Start()
    {
        init();
    }

    private void Update()
    {
        Input.GetKeyDown(KeyCode.Escape);
        if (GameManager.Instance.IsInCutscene == false && Input.GetKeyDown(KeyCode.Escape))
        {
            Exit();
        }
    }

    public virtual void Show(SubMenuController _parent)
    {
        _parent.Hide();

        parent = _parent;
        resetParent = false;

        Show();
    }

    public virtual void Show()
    {
        if (IsVisible())
        {
            return;
        }

        if (initialized == false)
        {
            init();
        }

        if (resetParent)
        {
            parent = null;
        }
        else
        {
            resetParent = true;
        }

        gameObject.SetActive(true);
        selectFirstToSelect();
    }

    public virtual void Hide() // Called when going into child menu
    {
        if (IsVisible() == false)
        {
            return;
        }

        if (rememberSelection)
        {
        //    setCurrentlySelectedAsFirstToSelect();
        }

        gameObject.SetActive(false);

        if (parent != null)
        {
            parent.Show();
        }
    }

    public virtual void Exit() // Called when going back to parent menu
    {
        if (canGoBack == false)
        {
            return;
        }

        if (parent == null)
        {
            ExitCompletely();
        }
        else
        {
            Hide();
        }
    }

    public virtual void ExitCompletely() // Called when closing all menus
    {
        parent = null;
        Hide();
    }

    public virtual bool IsVisible()
    {
        return gameObject.activeInHierarchy;
    }

    protected virtual void init()
    {
        if (firstToSelect == null)
        {
            firstToSelect = GetComponentInChildren<Selectable>();
        }

        selectFirstToSelect();
    }

    protected void setCurrentlySelectedAsFirstToSelect()
    {
        if (EventSystem.current == null)
        {
        }

        GameObject _currentSelected = EventSystem.current.currentSelectedGameObject;
        if (_currentSelected == null)
        {
            return;
        }

        firstToSelect = _currentSelected.GetComponent<Selectable>();
    }

    protected void selectFirstToSelect()
    {
        firstToSelect.Select();
        firstToSelect.OnSelect(null); // Fix for not highlighting the selected button
    }
}
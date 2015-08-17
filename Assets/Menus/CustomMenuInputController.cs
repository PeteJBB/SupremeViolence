using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using XInputDotNetPure;

public class CustomMenuInputController: MonoBehaviour 
{
    [Tooltip("-1 means take input from all players")]
    public int PlayerIndex = -1;
    public GameObject CurrentSelectedObject;

    private Stack<Canvas> navStack = new Stack<Canvas>();
    public Canvas ActiveCanvas;
    public bool ToggleScreenSpaceOnActivateCanvas = false;

	// Use this for initialization
	public void Start () 
    {
        if(ActiveCanvas != null)
            NavigateForwards(ActiveCanvas);

        if(CurrentSelectedObject != null)
            CurrentSelectedObject.SendMessage("OnSelect", new BaseEventData(null), SendMessageOptions.DontRequireReceiver);
	}

    public virtual void NavigateForwards(Canvas canvas)
    {
        Debug.Log("NavigateForwards to " + canvas.name);
        if(ActiveCanvas != null)
        {
            navStack.Push(ActiveCanvas);

            if(ToggleScreenSpaceOnActivateCanvas)
                ActiveCanvas.renderMode = RenderMode.WorldSpace;
        }

        ActiveCanvas = canvas;

        if(ActiveCanvas != null)
        {
            if(ToggleScreenSpaceOnActivateCanvas)
                ActiveCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

            SelectFirstVisibleItem();
        }
    }
    
    public virtual void GoBack()
    {
        Debug.Log("GoBack");
        if(navStack.Count > 0)
        {
            if(ActiveCanvas != null)
            {
                if(ToggleScreenSpaceOnActivateCanvas)
                    ActiveCanvas.renderMode = RenderMode.WorldSpace;
            }
            ActiveCanvas = navStack.Pop();

            if(ActiveCanvas != null)
            {
                if(ToggleScreenSpaceOnActivateCanvas)
                    ActiveCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

                SelectFirstVisibleItem();
            }
        }
    }
	
	// Update is called once per frame
	public void Update () 
    {
        if(ActiveCanvas != null)
        {
            if(CurrentSelectedObject == null)
                SelectFirstVisibleItem();

            var action = GetMenuInputAction();
            if(action == MenuInputAction.MoveUp)
            {
                var sel = CurrentSelectedObject.GetComponent<Selectable>().FindSelectableOnUp();
                if(sel != null && IsObjectInMyScope(sel.gameObject))
                    SetSelectedGameObject(sel.gameObject);
            }
            else if(action == MenuInputAction.MoveDown)
            {
                var sel = CurrentSelectedObject.GetComponent<Selectable>().FindSelectableOnDown();
                if(sel != null && IsObjectInMyScope(sel.gameObject))
                    SetSelectedGameObject(sel.gameObject);
            }
            else if(action == MenuInputAction.MoveLeft)
            {
                var sel = CurrentSelectedObject.GetComponent<Selectable>().FindSelectableOnLeft();
                if(sel != null && IsObjectInMyScope(sel.gameObject))
                    SetSelectedGameObject(sel.gameObject);
            }
            else if(action == MenuInputAction.MoveRight)
            {
                var sel = CurrentSelectedObject.GetComponent<Selectable>().FindSelectableOnRight();
                if(sel != null && IsObjectInMyScope(sel.gameObject))
                    SetSelectedGameObject(sel.gameObject);
            }
            else if(action == MenuInputAction.Submit)
            {
                var ev = new BaseEventData(null);
                CurrentSelectedObject.GetComponent<Selectable>().SendMessage("OnSubmit", ev, SendMessageOptions.DontRequireReceiver);
            }
            else if(action == MenuInputAction.Cancel)
            {
                
            }
            else if(action == MenuInputAction.PrevPage)
            {
                PrevPage();
            }
            else if(action == MenuInputAction.NextPage)
            {
                NextPage();
            }
        }
	}

    public virtual void NextPage()
    {

    }

    public virtual void PrevPage()
    {
        
    }

    public void SelectFirstVisibleItem()
    {
        if(ActiveCanvas != null)
        {
            var selectables = ActiveCanvas.GetComponentsInChildren<Selectable>();
            var first = selectables.OrderByDescending(x => x.transform.position.y).FirstOrDefault();
            SetSelectedGameObject(first.gameObject);
        }
    }

    public void SetSelectedGameObject(GameObject obj)
    {
        if(CurrentSelectedObject != null)
            CurrentSelectedObject.SendMessage("OnDeselect", new BaseEventData(null), SendMessageOptions.DontRequireReceiver);

        if(obj != null)
        {
            CurrentSelectedObject = obj;
            CurrentSelectedObject.SendMessage("OnSelect", new BaseEventData(null), SendMessageOptions.DontRequireReceiver);
        }
    }

    public bool IsObjectInMyScope(GameObject obj)
    {
        if(ActiveCanvas != null)
        {
            var parent = obj.transform.parent;
            while(parent != null)
            {
                if(parent == ActiveCanvas.transform)
                    return true;

                parent = parent.parent;
            }
        }

        return false;
    }

    GamePadState lastGamepadState;
    private MenuInputAction GetMenuInputAction()
    {
        var action = MenuInputAction.None;

        // gamepad input
        var state = GetGamePadInput();
        if(state.ThumbSticks.Left.Y > 0 && lastGamepadState.ThumbSticks.Left.Y <= 0
                || state.DPad.Up == ButtonState.Pressed && lastGamepadState.DPad.Up == ButtonState.Released)
        {
            action = MenuInputAction.MoveUp;
        }
        else if(state.ThumbSticks.Left.Y < 0 && lastGamepadState.ThumbSticks.Left.Y >= 0
                || state.DPad.Down == ButtonState.Pressed && lastGamepadState.DPad.Down == ButtonState.Released)
        {
            action = MenuInputAction.MoveDown;
        }
        else if(state.ThumbSticks.Left.X < 0 && lastGamepadState.ThumbSticks.Left.X >= 0
                || state.DPad.Left == ButtonState.Pressed && lastGamepadState.DPad.Left == ButtonState.Released)
        {
            action = MenuInputAction.MoveLeft;
        }
        else if(state.ThumbSticks.Left.X > 0 && lastGamepadState.ThumbSticks.Left.X <= 0
                || state.DPad.Right == ButtonState.Pressed && lastGamepadState.DPad.Right == ButtonState.Released)
        {
            action = MenuInputAction.MoveRight;
        }
        else if(state.Buttons.A == ButtonState.Pressed && lastGamepadState.Buttons.A == ButtonState.Released)
        {
            action = MenuInputAction.Submit;
        }
        else if(state.Buttons.B == ButtonState.Pressed && lastGamepadState.Buttons.B == ButtonState.Released)
        {
            action = MenuInputAction.Cancel;
        }
        else if(state.Buttons.LeftShoulder == ButtonState.Pressed && lastGamepadState.Buttons.LeftShoulder == ButtonState.Released)
        {
            action = MenuInputAction.PrevPage;
        }
        else if(state.Buttons.RightShoulder == ButtonState.Pressed && lastGamepadState.Buttons.RightShoulder == ButtonState.Released)
        {
            action = MenuInputAction.NextPage;
        }

        lastGamepadState = state;

        if(action != MenuInputAction.None)
            return action;

        // keyboard input (for debug)
        if(Input.GetKeyDown(KeyCode.UpArrow))
            action = MenuInputAction.MoveUp;
        else if(Input.GetKeyDown(KeyCode.DownArrow))
            action = MenuInputAction.MoveDown;
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
            action = MenuInputAction.MoveLeft;
        else if(Input.GetKeyDown(KeyCode.RightArrow))
            action = MenuInputAction.MoveRight;
        else if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            action = MenuInputAction.Submit;
        else if(Input.GetKeyDown(KeyCode.Escape))
            action = MenuInputAction.Cancel;

        return action;
    }

    private GamePadState GetGamePadInput()
    {
        switch(PlayerIndex)
        {
            case 0:
            default:
                return GamePad.GetState(XInputDotNetPure.PlayerIndex.One);
            case 1:
                return GamePad.GetState(XInputDotNetPure.PlayerIndex.Two);
            case 2:
                return GamePad.GetState(XInputDotNetPure.PlayerIndex.Three);
            case 3:
                return GamePad.GetState(XInputDotNetPure.PlayerIndex.Four);
        }
    }
//
//    void HighlightSelectedObject()
//    {
//        foreach(var t in CurrentSelectedObject.GetComponentsInChildren<Text>())
//        {
//            t.color = Color.yellow;
//        }
//    }
//
//    void UnhighlightSelectedObject()
//    {
//        foreach(var t in CurrentSelectedObject.GetComponentsInChildren<Text>())
//        {
//            t.color = Color.white;
//        }
//    }
}

public enum MenuInputAction
{
    None,
    MoveUp,
    MoveDown,
    MoveLeft,
    MoveRight,
    Submit,
    Cancel,
    PrevPage,
    NextPage

}
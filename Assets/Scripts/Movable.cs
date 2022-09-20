using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public class Movable : MonoBehaviour
{
    // Start is called before the first frame update
    
    public List<Vector2> availablePosList;
    private Transform _current;
    public Fist fist;
    public Scissor scissor;
    public Paper paper;
    private bool _selectable = true;
    public bool Selectable
    {
        get => (fist._selectable && scissor._selectable && paper._selectable);
        set
        {
            _selectable = value;
        }
    }
    public int hp = 2;
    public SpriteRenderer spriteRenderer;

    private void Start()
    {
        Selectable = CompareTag("Player1");
        var color = (Vector4)spriteRenderer.color;
        spriteRenderer.color = new Vector4(color.x, color.y, color.z, Selectable ? 1f : 0.6f);
    }
    

    public void SetSelectable()
    {
        var flag = CompareTag(transform.parent.tag);
        if (flag)
        {
            fist.Selectable = true;
            scissor.Selectable = true;
            paper.Selectable = true;
            fist.hp = hp;
            paper.hp = hp;
            scissor.hp = hp;
        }
        else
        {
            Selectable = false;
        }
        var color = (Vector4)spriteRenderer.color;
        spriteRenderer.color = new Vector4(color.x, color.y, color.z, Selectable ? 1f : 0.6f);
    }

    void Update()
    {
        if (isActiveAndEnabled)
        {
            if (Input.GetMouseButtonDown(1) && Selectable)
            {
                if (GridPrinter.GetGridPos(transform.position) == GridPrinter.GetMouseGridPos())
                {
                    ChangeCommand command = new ChangeCommand(this);
                    Storage.UndoList.AddLast(command);
                    Storage.RedoStack.Clear();
                    command.Execute();
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (GridPrinter.GetGridPos(transform.position) == GridPrinter.GetMouseGridPos())
                {
                    if (_current == null && Selectable)
                    {
                        ReturnQueryAvailable(false); // select self as current
                    }
                    else if (_current != transform && Selectable)
                    { 
                        // query if self is in current's move range, if no reselect current to self
                        _current.SendMessage("QueryAvailable", transform); 
                    }
                }
                else
                {
                    if (_current == transform)
                    {
                        foreach (Vector2 available in availablePosList)
                        {
                            if (GridPrinter.GetGridPos((Vector2)transform.position + available) == GridPrinter.GetMouseGridPos())
                            {
                                Storage.UndoList.AddLast(TryMove(available));
                                Storage.UndoList.Last.Value.Execute();
                                Storage.RedoStack.Clear();
                                break;
                            }
                        }
                    }
                }
            }            
        }
    }

    void ReturnQueryAvailable(bool result)
    {
        if (result == false) 
        {
            transform.parent.BroadcastMessage("SetCurrentUnit", transform);
        }
    }

    void QueryAvailable(Transform sender)
    {
        if (isActiveAndEnabled)
        {
            foreach (Vector2 available in availablePosList)
            {
                if (GridPrinter.GetGridPos(sender.position) == 
                    GridPrinter.GetGridPos((Vector2)transform.position + available))
                {
                    sender.SendMessage("ReturnQueryAvailable", true);
                    return;
                }
            }
            sender.SendMessage("ReturnQueryAvailable", false);
        }
    }

    Command TryMove(Vector2 vector)
    {
        foreach (Transform child in transform.parent.GetComponentsInChildren<Transform>())
        {
            if (child != transform && child != transform.parent)
            {
                if (GridPrinter.GetGridPos((Vector2)(child.position - transform.position) - vector) ==
                    GridPrinter.GetGridPos(Vector2.zero))
                {
                    Debug.Log("Something on Move Position");
                    if (child.CompareTag(transform.tag) == false) return new AttackCommand(this, child);
                    else
                    {
                        UnselectCurrent();
                        transform.parent.BroadcastMessage("SetCurrentUnit", child);
                        return new VoidCommand(this);
                    }
                }
            }
        }
        return new MoveCommand(this,vector);
    }
    

    public virtual void Move(Vector2 vector, bool isUndo = false)
    {
        transform.position = (Vector2)transform.position + vector;
        UnselectCurrent();
    }

    public void CheckWin(string tag)
    {
        int liveCount = 0;
        foreach (var child in transform.parent.GetComponentsInChildren<Transform>())
        {
            if (child.gameObject.activeSelf && child.CompareTag(tag))
            {
                liveCount++;
            }
        }

        if (liveCount == 0)
        {
            Camera.main.SendMessage("GameOver", transform.parent.tag);
        }
    }

    public void Attack(Transform target)
    {
        target.SendMessage("DealDamage",this);
        Debug.Log($"{transform.name} is launching attack on {target.name}");
        UnselectCurrent();
    }

    public void UnselectCurrent()
    {
        transform.parent.BroadcastMessage("ResetCurrent");
        Camera.main.SendMessage("DeselectGrid");
    }

    public virtual Movable Change(bool isUndo)
    {
        return this;
    }

    void ResetCurrent()
    {
        _current = null;
    }

    void SetCurrentUnit(Transform unit)
    {
        _current = unit;
        if (unit != transform) return;
        if (!(isActiveAndEnabled && Selectable)) return;
        Debug.Log($"{transform.name} is selected");
        Camera.main.SendMessage("SelectGrid", this);
    }
}

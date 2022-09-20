using UnityEngine;

public class Command
{
    public Movable Avatar;
    
    public virtual void Execute()
    {
        Avatar.Selectable = false;
        var color = (Vector4)Avatar.spriteRenderer.color;
        Avatar.spriteRenderer.color = new Vector4(color.x, color.y, color.z, 0.6f);
    }

    public virtual void Undo() {
        if (Avatar.isActiveAndEnabled == false)
        {
            Avatar.gameObject.SetActive(true);
        }
        Avatar.Selectable = true;
        var color = (Vector4)Avatar.spriteRenderer.color;
        Avatar.spriteRenderer.color = new Vector4(color.x, color.y, color.z, 1f);
    }
}

public class AttackCommand : Command
{
    Transform _target;
    
    public AttackCommand(Movable avatar, Transform target)
    {
        _target = target;
        Avatar = avatar;
    }

    public override void Execute()
    {
        Avatar.Attack(_target);
        base.Execute();
    }

    public override void Undo()
    {
        base.Undo();
        if(_target.gameObject.activeSelf == false) _target.gameObject.SetActive(true);
        _target.SendMessage("Restore",Avatar);
    }
}

public class MoveCommand : Command
{
    public Vector2 Delta;

    public MoveCommand(Movable avatar, Vector2 delta)
    {
        Delta = delta;
        Avatar = avatar;
    }

    public override void Execute()
    {
        base.Execute();
        Avatar.Move(Delta,false);
    }

    public override void Undo()
    {
        base.Undo();
        Avatar.Move(-Delta,true);
    }
}

public class ChangeCommand : Command
{
    public Movable Target;

    public ChangeCommand(Movable avatar)
    {
        Avatar = avatar;
    }

    public override void Execute()
    {
        base.Execute();
        Target = Avatar.Change(false);
    }

    public override void Undo()
    {
        base.Undo();
        Target.Change(true);
    }
}

public class VoidCommand : Command
{
    public VoidCommand(Movable avatar)
    {
        Avatar = avatar;
    }

    public override void Execute() {
        //do nothing
        Debug.Log($"{Avatar.name} did nothing.");
    }

    public override void Undo()
    {
        //do nothing
    }
}

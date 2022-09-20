using UnityEngine;

public class Scissor : Movable
{
    public Sprite verticalSprite;
    

    private void OnEnable()
    {
        if (isActiveAndEnabled)
        {
            spriteRenderer.sprite = verticalSprite;
        }
    }

    public override void Move(Vector2 vector, bool isUndo = false)
    {
        base.Move(vector,isUndo);
        if (!isUndo)
        {
            SetSprite(vector);
        }
        else
        {
            var lastMoveNode = Storage.UndoList.Last;
            do
            {
                lastMoveNode = lastMoveNode.Previous;
            } while (lastMoveNode != null 
                     && lastMoveNode.Value.GetType() != typeof(MoveCommand) 
                     && lastMoveNode.Value.Avatar != this);

            if (lastMoveNode != null 
                && lastMoveNode.Value.GetType() == typeof(MoveCommand) 
                && lastMoveNode.Value.Avatar == this)
            {
                SetSprite(((MoveCommand)lastMoveNode.Value).Delta);
            }
            else
            {
                SetSprite(Vector2.zero);
            }
        }
    }

    private void OnDisable()
    {
        spriteRenderer.flipY = false;
        spriteRenderer.flipX = false;
    }

    protected virtual void SetSprite(Vector2 vector)
    {
        float angle = Mathf.Atan2(vector.x, vector.y)*Mathf.Rad2Deg;
        transform.localEulerAngles =  Vector3.back * angle;
    }
    
    public override Movable Change(bool isUndo)
    {
        scissor.enabled = false;
        if (isUndo)
        {
            fist.enabled = true;
            return fist;
        }
        else
        {
            paper.enabled = true;
            UnselectCurrent();
            return paper;
        }
    }
    
    public virtual void DealDamage(Movable source)
    {
        if (isActiveAndEnabled)
        {
            int damage = 0;
            if (source.GetType() == typeof(Scissor))
            {
                damage = 1;
                Debug.Log("Scissor attacked by Scissor");
            }
            else if (source.GetType() == typeof(Paper))
            {
                Debug.Log("Scissor attacked by Paper, no effect.");
            }
            else
            {
                Debug.Log("Scissor attacked by Rock, it's super effective");
                damage = 2;
            }

            hp -= damage;
            Camera.main.SendMessage("PlaySound",damage);
            
            if (hp <= 0)
            {
                Debug.Log($"{name} Died due to ${source.name}'s Attack");
                gameObject.SetActive(false);
                CheckWin(tag);
            }
        }
    }

    public virtual void Restore(Movable source)
    {
        if (isActiveAndEnabled)
        {
            if (source.GetType() == typeof(Scissor))
            {
                hp += 1;
            }
            else if (source.GetType() == typeof(Paper))
            {
            }
            else
            {
                hp += 2;
            }
        }
    }
}

using UnityEngine;

public class Fist : Scissor
{
    public Sprite horizontalSprite ;

    protected override void SetSprite(Vector2 vector)
    {
        transform.localEulerAngles = Vector3.zero;
        spriteRenderer.sprite = vector.x != 0 ? horizontalSprite : verticalSprite;
        spriteRenderer.flipX = vector.x < 0;
        spriteRenderer.flipY = vector.y < 0;
    }

    public override Movable Change(bool isUndo)
    {
        fist.enabled = false;
        if (isUndo)
        {
            paper.enabled = true;
            return paper;
        }
        else
        {
            scissor.enabled = true;
            UnselectCurrent();
            return scissor;
        }
        
    }
    
    public override void DealDamage(Movable source)
    {
        if (isActiveAndEnabled)
        {
            int damage = 0;
            if (source.GetType() == typeof(Scissor))
            {
                Debug.Log("Rock attacked by Scissor, no effect");
            }
            else if (source.GetType() == typeof(Paper))
            {
                damage = 2;
                Debug.Log("Rock attacked by Paper, it's super effective.");
                
            }
            else
            {
                damage = 1;
                Debug.Log("Rock attacked by Rock.");
                
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

    public override void Restore(Movable source)
    {
        if (isActiveAndEnabled)
        {
            if (source.GetType() == typeof(Scissor))
            {
                
            }
            else if (source.GetType() == typeof(Paper))
            {
                hp += 2;
            }
            else
            {
                hp += 1;
            }
        }
    }
}

using UnityEngine;

public class Paper : Scissor
{
    public override Movable Change(bool isUndo)
    {
        paper.enabled = false;
        if (isUndo)
        {
            scissor.enabled = true;
            return scissor;
        }
        else
        {
            fist.enabled = true;
            UnselectCurrent();
            return fist;
        }
    }

    public override void DealDamage(Movable source)
    {
        if (isActiveAndEnabled)
        {
            int damage = 0;
            if (source.GetType() == typeof(Scissor))
            {
                damage = 2;
                Debug.Log("Paper attacked by Scissor, it's super effective.");
            }
            else if (source.GetType() == typeof(Paper))
            {
                damage = 1;
                Debug.Log("Paper attacked by Paper.");
            }
            else
            {
                Debug.Log("Paper attacked by Rock, no effect");
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
                hp += 2;
            }
            else if (source.GetType() == typeof(Paper))
            {
                hp += 1;
            }
        }
    }
}
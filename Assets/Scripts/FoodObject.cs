using UnityEngine;

public class FoodObject : CellObject
{
    private int m_FoodPoints = 4;
   public override void PlayerEntered()
   {
       Destroy(gameObject);
       GameManager.Instance.ChangeFoodAmount(m_FoodPoints);
   }
}
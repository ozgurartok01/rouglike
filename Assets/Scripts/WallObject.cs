
using UnityEngine;
using UnityEngine.Tilemaps;

public class WallObject : CellObject
{
   public Tile ObstacleTile;
   public int MaxHealth = 3;
   private int m_HealthPoint;
   private Tile m_OriginalTile;

   public Tile m_damagedTile;
  
   public override void Init(Vector2Int cell)
   {
       base.Init(cell);
       m_HealthPoint = MaxHealth;
       
       m_OriginalTile = GameManager.Instance.BoardManager.GetCellTile(cell);
       GameManager.Instance.BoardManager.SetCellTile(cell, ObstacleTile);
   }

   public override bool PlayerWantsToEnter()
   {
         GameManager.Instance.PlayerController.Attack();

         m_HealthPoint--;
         if(m_HealthPoint == 1 && m_damagedTile != null)
         {
            GameManager.Instance.BoardManager.SetCellTile(m_Cell, m_damagedTile);
         }
         if(m_HealthPoint <= 0)
         {
              GameManager.Instance.BoardManager.SetCellTile(m_Cell, m_OriginalTile);
              Destroy(gameObject);
              return true;
         }
         return false;
   }
}
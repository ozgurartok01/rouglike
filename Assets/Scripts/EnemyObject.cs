using UnityEngine;

public class EnemyObject : CellObject
{
   public int Health = 3;
  
   private int m_CurrentHealth;

   private void Awake()
   {
      GameManager.Instance.TurnManager.OnTick += TurnHappened;
   }

    private void OnDestroy()
    {
        GameManager.Instance.TurnManager.OnTick -= TurnHappened;
    }
   private void DestroyTick()
   {
       GameManager.Instance.TurnManager.OnTick -= TurnHappened;
   }

   public override void Init(Vector2Int coord)
   {
      base.Init(coord);
      m_CurrentHealth = Health;
   }

   public override bool PlayerWantsToEnter()
   {
       m_CurrentHealth -= 1;

       if (m_CurrentHealth <= 0)
       {
          DestroyTick();
          Destroy(gameObject);
       }

       return false;
   }

   public override void TakeDamage(int damage)
    {
        m_CurrentHealth -= damage;

        if (m_CurrentHealth <= 0)
        {
            DestroyTick();
            Destroy(gameObject);
        }
    }

   bool MoveTo(Vector2Int coord)
    {
        var board = GameManager.Instance.BoardManager;
        var targetCell = board.GetCellData(coord);

        if (targetCell == null || !targetCell.Passable || targetCell.ContainedObject != null)
        {
            return false;
        }

        var currentCell = board.GetCellData(m_Cell);
        currentCell.ContainedObject = null;

        targetCell.ContainedObject = this;
        m_Cell = coord;
        transform.position = board.CellToWorld(coord);

        return true;
    }

   void TurnHappened()
   {
      var playerCell = GameManager.Instance.PlayerController.Cell;

      int distX = playerCell.x - m_Cell.x;
      int distY = playerCell.y - m_Cell.y;

      int absDistX = Mathf.Abs(distX);
      int absDistY = Mathf.Abs(distY);

        if((absDistX == 0 && absDistY == 1)||(absDistX == 1 && absDistY == 0))
        {
            GameManager.Instance.ChangeFoodAmount(-3);
        }
        else{
            if(absDistX > absDistY)
            {
                if(!TryMoveInX(distX))
                    TryMoveInY(distY);

                
            }
            else
            {
                if(!TryMoveInY(distY))
                    TryMoveInX(distX);
            }
        }
   }

   bool TryMoveInX(int xDist)
   {
        if (xDist > 0)
        {
            return MoveTo(m_Cell + Vector2Int.right);
        }

        return MoveTo(m_Cell + Vector2Int.left);
   }

   bool TryMoveInY(int yDist)
   {
        if (yDist > 0)
        {
            return MoveTo(m_Cell + Vector2Int.up);
        }

        return MoveTo(m_Cell + Vector2Int.down);
   }
}

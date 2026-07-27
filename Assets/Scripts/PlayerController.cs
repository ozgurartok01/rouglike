using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float MoveSpeed = 5.0f;

    private Animator m_Animator;
    private BoardManager m_Board;
    private Vector2Int m_CellPosition;
    private Vector3 m_MoveTarget;
    private bool m_isGameOver = false;
    private bool m_IsMoving = false;
    public Vector2Int Cell => m_CellPosition;
    
    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void Init()
    {
        m_isGameOver = false;
    }

    public void Spawn(BoardManager boardManager, Vector2Int cell)
    {
        m_Board = boardManager;

        MoveTo(cell, true);
    }

    public void MoveTo(Vector2Int cell, bool immediate = false)
    {
        m_CellPosition = cell;

        if (immediate)
        {
            m_IsMoving = false;
            transform.position = m_Board.CellToWorld(m_CellPosition);
        }
        else
        {
            m_IsMoving = true;
            m_MoveTarget = m_Board.CellToWorld(m_CellPosition);
        }
        
        m_Animator.SetBool("Moving", m_IsMoving);
    }

    public void gameover()
    {
        m_isGameOver = true;
    }

    public void Attack()
    {
        m_Animator.SetTrigger("Attacking");
    }

    // Update is called once per frame
    void Update()
    {
        
        if (m_isGameOver)
        {
            if (Keyboard.current.enterKey.wasPressedThisFrame && m_isGameOver)
                {
                    GameManager.Instance.StartNewGame();
                    m_isGameOver = false;
                }
            return;
        }
        if (m_IsMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, m_MoveTarget, MoveSpeed * Time.deltaTime);
            
            if (transform.position == m_MoveTarget)
            {
                m_IsMoving = false;
                m_Animator.SetBool("Moving", false);
                var cellData = m_Board.GetCellData(m_CellPosition);
                if(cellData.ContainedObject != null)
                    cellData.ContainedObject.PlayerEntered();
            }

            return;
        }

        Vector2Int newCellTarget = m_CellPosition;
        bool hasMoved = false;

        if(Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            newCellTarget.y += 1;
            hasMoved = true;
        }
        else if(Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            newCellTarget.y -= 1;
            hasMoved = true;
        }
        else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            newCellTarget.x += 1;
            hasMoved = true;
        }
        else if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            newCellTarget.x -= 1;
            hasMoved = true;
        }
        else if (Keyboard.current.xKey.wasPressedThisFrame)
        {
            newCellTarget.x += 1;
            BoardManager.CellData cellData = m_Board.GetCellData(newCellTarget);
            if (cellData.ContainedObject == null)
                return;
            cellData.ContainedObject.TakeDamage(5);

            GameManager.Instance.TurnManager.Tick();
        }

        if(hasMoved)
        {
            //check if the new position is passable, then move there if it is.
            BoardManager.CellData cellData = m_Board.GetCellData(newCellTarget);

            if(cellData != null && cellData.Passable)
            {
                
                
                if (cellData.ContainedObject == null)
                {
                    MoveTo(newCellTarget);
                }

                else if(cellData.ContainedObject.PlayerWantsToEnter())
                {
                    MoveTo(newCellTarget);
                    cellData.ContainedObject.PlayerEntered();
                }
                GameManager.Instance.TurnManager.Tick();
            }
        }
    }
}

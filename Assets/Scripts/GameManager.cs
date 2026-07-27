using UnityEngine;
using UnityEngine.UIElements;
public class GameManager : MonoBehaviour
{
   public UIDocument UIDoc;
   private Label m_FoodLabel;
   private VisualElement m_GameOverPanel;
   private Label m_GameOverMessage;
   private int m_FoodAmount;
   public static GameManager Instance { get; private set; }
   public TurnManager TurnManager { get; private set;}  
   public BoardManager BoardManager;
   public PlayerController PlayerController;

   private int m_CurrentLevel = 0;

   

   private void Awake()
   {
       if (Instance != null)
       {
           Destroy(gameObject);
           return;
       }
      
       Instance = this;
   }
   void Start()
    {
        m_GameOverPanel = UIDoc.rootVisualElement.Q<VisualElement>("GameOverPanel");
        m_GameOverMessage = m_GameOverPanel.Q<Label>("GameOverMessage");

        m_GameOverPanel.style.visibility = Visibility.Hidden;

        m_FoodLabel = UIDoc.rootVisualElement.Q<Label>("FoodLabel");
        m_FoodLabel.text = "Food : " + m_FoodAmount;

        TurnManager = new TurnManager();
        TurnManager.OnTick += OnTurnHappen;

        StartNewGame();
    }
    void OnTurnHappen()
    {
        ChangeFoodAmount(-1);
    }

    public void ChangeFoodAmount(int amount)
    {
        m_FoodAmount += amount;
        m_FoodLabel.text = "Food : " + m_FoodAmount;

        if (m_FoodAmount <= 0)
        {
            PlayerController.gameover();
            m_GameOverPanel.style.visibility = Visibility.Visible;
            m_GameOverMessage.text = "Game Over!\n\nYou traveled through " + m_CurrentLevel + " levels\n\nPress Enter to restart";
        }
    }

    public void NewLevel()
    {
        
        BoardManager.cleanBoard();
        BoardManager.Init();
        PlayerController.Spawn(BoardManager, new Vector2Int(1,1));
        
        m_CurrentLevel++;
    }

    public void StartNewGame()
    {
        m_GameOverPanel.style.visibility = Visibility.Hidden;
        
        m_CurrentLevel = 1;
        m_FoodAmount = 100;
        m_FoodLabel.text = "Food : " + m_FoodAmount;
        
        BoardManager.cleanBoard();
        BoardManager.Init();
        
        PlayerController.Init();
        PlayerController.Spawn(BoardManager, new Vector2Int(1,1));
    }
}

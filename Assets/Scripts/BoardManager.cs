using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour
{
    List<Vector2Int> m_EmptyCellsList;
    public PlayerController Player;
    public EnemyObject Enemy;
    public FoodObject[] FoodPrefabs;
    public WallObject[] WallPrefabs;
    public ExitCellObject exitPrefab;
    private Grid m_Grid;
    public class CellData
   {
      public bool Passable;
      public CellObject ContainedObject;
   }
   private CellData[,] m_BoardData;

    private Tilemap m_Tilemap;
   public int Width;
   public int Height;
   public Tile[] GroundTiles;
   public Tile[] WallTiles;

   // Start is called before the first frame update
   public void Init()
   {
       m_Tilemap = GetComponentInChildren<Tilemap>();
       
       m_Grid = GetComponentInChildren<Grid>();

       m_EmptyCellsList = new List<Vector2Int>();

       m_BoardData = new CellData[Width, Height];


       for (int y = 0; y < Height; ++y)
       {
           for(int x = 0; x < Width; ++x)
           {
               Tile tile;
               m_BoardData[x, y] = new CellData();

              
               if(x == 0 || y == 0 || x == Width - 1 || y == Height - 1)
               {
                   tile = WallTiles[Random.Range(0, WallTiles.Length)]; 
                   m_BoardData[x, y].Passable = false;
               }
               else
               {
                   tile = GroundTiles[Random.Range(0, GroundTiles.Length)];
                   m_BoardData[x, y].Passable = true;
                   m_EmptyCellsList.Add(new Vector2Int(x, y));
               }
              
               m_Tilemap.SetTile(new Vector3Int(x, y, 0), tile);
           }
       }
       m_EmptyCellsList.Remove(new Vector2Int(1, 1));

       Vector2Int endCoord = new Vector2Int(Width - 2, Height - 2);
       addObject(Instantiate(exitPrefab), endCoord);
       m_EmptyCellsList.Remove(endCoord);

       

       GenerateWall();
       GenerateFood();

       GenerateEnemy();
   }

   public Vector3 CellToWorld(Vector2Int cellIndex)
    {
        return m_Grid.GetCellCenterWorld((Vector3Int)cellIndex);
    }
    
    public void SetCellTile(Vector2Int cellIndex, Tile tile)
    {
        m_Tilemap.SetTile(new Vector3Int(cellIndex.x, cellIndex.y, 0), tile);
    }

    public Tile GetCellTile(Vector2Int cellIndex)
    {
        return m_Tilemap.GetTile<Tile>(new Vector3Int(cellIndex.x, cellIndex.y, 0));
    }

    public CellData GetCellData(Vector2Int cellIndex)
    {
        if (cellIndex.x < 0 || cellIndex.x >= Width || cellIndex.y < 0 || cellIndex.y >= Height)
            {
                return null;
            }

        return m_BoardData[cellIndex.x, cellIndex.y];
    }

    void GenerateFood()
    {
        int foodCount = 5;
        for(int i = 0; i< foodCount; i++)
        {
            int randomIndex = Random.Range(0, m_EmptyCellsList.Count);
            Vector2Int coord = m_EmptyCellsList[randomIndex];
            m_EmptyCellsList.RemoveAt(randomIndex);
            FoodObject newFood = Instantiate(FoodPrefabs[Random.Range(0, FoodPrefabs.Length)]);
            addObject(newFood, coord);
        }
    }

    void GenerateWall()
    {
        int wallCount = Random.Range(5, 10);
        for(int i = 0; i< wallCount; i++)
        {
            int randomIndex = Random.Range(0, m_EmptyCellsList.Count);
            Vector2Int coord = m_EmptyCellsList[randomIndex];
            m_EmptyCellsList.RemoveAt(randomIndex);
            WallObject newWall = Instantiate(WallPrefabs[Random.Range(0, WallPrefabs.Length)]);
            addObject(newWall, coord);
        }
    }

    void GenerateEnemy()
    {
        int enemyCount = 1;
        for(int i = 0; i< enemyCount; i++)
        {
            int randomIndex = Random.Range(0, m_EmptyCellsList.Count);
            Vector2Int coord = m_EmptyCellsList[randomIndex];
            m_EmptyCellsList.RemoveAt(randomIndex);
            EnemyObject newEnemy = Instantiate(Enemy);
            addObject(newEnemy, coord);
        }
    }

    void addObject(CellObject obj, Vector2Int cell)
    {
        m_BoardData[cell.x, cell.y].ContainedObject = obj;
        obj.transform.position = CellToWorld(cell);
        obj.Init(cell);
    }

    public void cleanBoard()
    {
        if(m_BoardData == null)
            return;

        for(int y = 0; y < Height; ++y)
        {
            for(int x = 0; x < Width; ++x)
            {
                var cellData = m_BoardData[x,y];
                if(cellData.ContainedObject != null)
                {
                    Destroy(cellData.ContainedObject.gameObject);
                }

                SetCellTile(new Vector2Int(x,y), null);
            }
        }
    }
}

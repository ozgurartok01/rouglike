using UnityEngine;

public class TurnManager
{
    public event System.Action OnTick;
    private int m_TurnCount = 0;

    public TurnManager()
    {
        m_TurnCount = 0;
    }

    public void Tick()
    {
        OnTick?.Invoke();
        m_TurnCount += 1;
    }
    
}

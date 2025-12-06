using UnityEngine;

public static class PlayerPositionManager
{
    // Dictionary to store positions by scene index
    private static System.Collections.Generic.Dictionary<int, Vector3> scenePositions = 
        new System.Collections.Generic.Dictionary<int, Vector3>();
    
    // Store current health (not scene-specific, persists across all scenes)
    private static int currentHealth = -1; // -1 means not set
    
    public static void SavePosition(int sceneIndex, Vector3 position)
    {
        scenePositions[sceneIndex] = position;
    }
    
    public static bool TryGetPosition(int sceneIndex, out Vector3 position)
    {
        return scenePositions.TryGetValue(sceneIndex, out position);
    }
    
    public static void SaveHealth(int health)
    {
        currentHealth = health;
    }
    
    public static bool TryGetHealth(out int health)
    {
        health = currentHealth;
        return currentHealth != -1;
    }
    
    public static void ClearPositions()
    {
        scenePositions.Clear();
    }
    
    public static void ClearAll()
    {
        scenePositions.Clear();
        currentHealth = -1;
    }
}
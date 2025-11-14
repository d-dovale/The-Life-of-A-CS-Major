using UnityEngine;

public static class PlayerPositionManager
{
    // Dictionary to store positions by scene index
    private static System.Collections.Generic.Dictionary<int, Vector3> scenePositions = 
        new System.Collections.Generic.Dictionary<int, Vector3>();
    
    public static void SavePosition(int sceneIndex, Vector3 position)
    {
        scenePositions[sceneIndex] = position;
    }
    
    public static bool TryGetPosition(int sceneIndex, out Vector3 position)
    {
        return scenePositions.TryGetValue(sceneIndex, out position);
    }
    
    public static void ClearPositions()
    {
        scenePositions.Clear();
    }
}
using UnityEngine;
using System.Collections.Generic;

public class LevelCreator : MonoBehaviour
{
    [Header("Prefab Settings")]
    [Tooltip("The 3D tile platform prefab to instantiate.")]
    public GameObject tilePrefab;

    [Header("Area Settings")]
    [Tooltip("The width of the area (X axis).")]
    public float areaWidth = 10f;
    [Tooltip("The height of the area (Z axis).")]
    public float areaHeight = 20f;
    [Tooltip("The size of each tile/step along the path.")]
    public float tileSize = 1f;

    [Header("Path Settings")]
    [Tooltip("Probability to move forward (0-1). Higher values make the path straighter.")]
    [Range(0f, 1f)]
    public float forwardBias = 0.6f;
    [Tooltip("Probability to widen the path to 2 tiles at a given step.")]
    [Range(0f, 1f)]
    public float widePathChance = 0.2f;
    [Tooltip("Random seed. Change this to explore different paths.")]
    public int seed = 0;

    private void Start()
    {
        GeneratePath();
    }

    [ContextMenu("Generate Path")]
    public void GeneratePath()
    {
        DeleteChildren();

        if (tilePrefab == null)
        {
            Debug.LogWarning("LevelCreator: No Tile Prefab assigned.");
            return;
        }

        if (seed != 0) Random.InitState(seed);

        // Grid-based logic to handle "touch exactly 2 tiles" rule strictly.
        // Coordinate system: X (lateral), Y (longitudinal/depth).
        // Y increases as we go "Down" (-Z in World).

        HashSet<Vector2Int> occupied = new HashSet<Vector2Int>();

        int stepsY = Mathf.RoundToInt(areaHeight / tileSize);
        float startZ = areaHeight / 2f;

        // Start at (0,0) -> World (0, 0, startZ)
        Vector2Int currentGrid = Vector2Int.zero;

        // Place Start
        AddTile(currentGrid, startZ, occupied);

        int safety = 0;
        int maxSafety = 10000;

        while (currentGrid.y < stepsY && safety < maxSafety)
        {
            safety++;

            // Determine possible moves: Forward (0,1), Left (-1,0), Right (1,0)
            List<Vector2Int> candidates = new List<Vector2Int>();

            Vector2Int forwardMove = currentGrid + new Vector2Int(0, 1);
            Vector2Int leftMove = currentGrid + new Vector2Int(-1, 0);
            Vector2Int rightMove = currentGrid + new Vector2Int(1, 0);

            // Check candidates
            bool forwardValid = IsValidMove(forwardMove, occupied, stepsY);
            bool leftValid = IsValidMove(leftMove, occupied, stepsY);
            bool rightValid = IsValidMove(rightMove, occupied, stepsY);

            // Decision logic with Bias
            bool moved = false;

            // Bias logic
            if (forwardValid && Random.value < forwardBias)
            {
                currentGrid = forwardMove;
                moved = true;
            }
            else
            {
                // Try sideways, or fallback to forward
                List<Vector2Int> sideMoves = new List<Vector2Int>();
                if (leftValid) sideMoves.Add(leftMove);
                if (rightValid) sideMoves.Add(rightMove);

                if (sideMoves.Count > 0)
                {
                    currentGrid = sideMoves[Random.Range(0, sideMoves.Count)];
                    moved = true;
                }
                else if (forwardValid)
                {
                    // If sideways failed/blocked but forward is open
                    currentGrid = forwardMove;
                    moved = true;
                }
            }

            if (!moved)
            {
                Debug.Log("Path generation stuck at " + currentGrid);
                break;
            }

            AddTile(currentGrid, startZ, occupied);

            // Widen the path occasionally
            if (Random.value < widePathChance)
            {
                // Candidates: Forward, Left, Right
                Vector2Int[] checkDirs = { new Vector2Int(0, 1), new Vector2Int(-1, 0), new Vector2Int(1, 0) };
                List<Vector2Int> extras = new List<Vector2Int>();

                foreach (var d in checkDirs)
                {
                    Vector2Int neighbor = currentGrid + d;
                    // Start 'safety' check so we don't go way out of bounds, though IsValidMove checks Width
                    if (neighbor.y <= stepsY && IsValidMove(neighbor, occupied, stepsY))
                    {
                        extras.Add(neighbor);
                    }
                }

                if (extras.Count > 0)
                {
                    Vector2Int target = extras[Random.Range(0, extras.Count)];
                    AddTile(target, startZ, occupied);
                }
            }
        }

        // Final connection to Middle Bot if needed
        while (currentGrid.x != 0 && safety < maxSafety)
        {
            safety++;
            Vector2Int stepDir = new Vector2Int(currentGrid.x > 0 ? -1 : 1, 0);
            Vector2Int next = currentGrid + stepDir;

            if (IsValidMove(next, occupied, stepsY))
            {
                currentGrid = next;
                AddTile(currentGrid, startZ, occupied);
            }
            else
            {
                break;
            }
        }
    }

    private bool IsValidMove(Vector2Int pos, HashSet<Vector2Int> occupied, int maxY)
    {
        // Check Bounds
        float xWorld = pos.x * tileSize;
        if (Mathf.Abs(xWorld) > areaWidth / 2f) return false;

        // Y bound check 
        if (pos.y < 0) return false;

        // Check Occupied
        if (occupied.Contains(pos)) return false;

        // Neighbor Check
        int occupiedNeighbors = 0;

        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        foreach (var d in dirs)
        {
            if (occupied.Contains(pos + d)) occupiedNeighbors++;
        }

        if (occupiedNeighbors == 1) return true;

        return false;
    }

    private void AddTile(Vector2Int gridPos, float startZ, HashSet<Vector2Int> occupied)
    {
        occupied.Add(gridPos);
        Vector3 worldPos = new Vector3(gridPos.x * tileSize, 0f, startZ - gridPos.y * tileSize);
        SpawnTile(worldPos);
    }

    private void SpawnTile(Vector3 localPos)
    {
        GameObject tile = Instantiate(tilePrefab, transform);
        tile.transform.localPosition = localPos;
    }

    [ContextMenu("Delete Previous Path")]
    private void DeleteChildren()
    {
        // Safely destroy children in Editor or Play mode
        int childCount = transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (Application.isPlaying)
            {
                Destroy(child);
            }
            else
            {
                DestroyImmediate(child);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        // Draw the Area bounds
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(areaWidth, 1f, areaHeight));

        // Indicate Start and End
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(new Vector3(0, 0, areaHeight / 2f), 0.5f); // Top Mid
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector3(0, 0, -areaHeight / 2f), 0.5f); // Bot Mid
    }
}


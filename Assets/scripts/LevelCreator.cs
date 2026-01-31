using UnityEngine;
using System.Collections.Generic;
using System;

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
    [Tooltip("Random seed. Change this to explore different paths.")]
    public int seed = 0;

    public int pathCreatedLength = 0;

    List<Vector2Int> placedPositions = new List<Vector2Int>();
    List<GameObject> placedTiles = new List<GameObject>();
    HashSet<Vector2Int> occupied = new HashSet<Vector2Int>();

    private void Start()
    {
        UpdatePlayerPosition(0);
    }

    [ContextMenu("Generate Path")]
    public void UpdatePath(int maxYLength)
    {
        //DeleteChildren();

        if (tilePrefab == null)
        {
            Debug.LogWarning("LevelCreator: No Tile Prefab assigned.");
            return;
        }

        if (seed != 0) UnityEngine.Random.InitState(seed);

        // Grid-based logic to handle "touch exactly 2 tiles" rule strictly.
        // Coordinate system: X (lateral), Y (longitudinal/depth).
        // Y increases as we go "Down" (-Z in World).

        int stepsY = Mathf.RoundToInt(areaHeight / tileSize);
        float startZ = areaHeight / 2f;

        int safety = 0;
        int maxSafety = 10000;

        int currentMaxY = -1;
        for (int i = 0; i < placedPositions.Count; i++)
        {
            currentMaxY = Mathf.Max(currentMaxY, placedPositions[i].y);
        }

        if (currentMaxY == -1)
        {
            Vector2Int currentGrid = Vector2Int.zero;
            AddTile(currentGrid, startZ, occupied);
            currentMaxY = 0;
        }

        while (currentMaxY < maxYLength && safety < maxSafety)
        {
            for (int i = 0; i < placedPositions.Count; i++)
            {
                currentMaxY = Mathf.Max(currentMaxY, placedPositions[i].y);
            }

            Vector2Int currentGrid = placedPositions[placedPositions.Count - 1];

            safety++;

            // Determine possible moves: Forward (0,1), Left (-1,0), Right (1,0)
            List<Vector2Int> candidates = new List<Vector2Int>();

            Vector2Int forwardMove = currentGrid + Vector2Int.up;
            Vector2Int leftMove = currentGrid + Vector2Int.left;
            Vector2Int rightMove = currentGrid + Vector2Int.right;

            // Check candidates
            bool forwardValid = IsValidMove(forwardMove, occupied, stepsY);
            bool leftValid = IsValidMove(leftMove, occupied, stepsY);
            bool rightValid = IsValidMove(rightMove, occupied, stepsY);

            // Decision logic with Bias
            bool moved = false;

            // Bias logic
            if (forwardValid && UnityEngine.Random.value < forwardBias)
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
                    currentGrid = sideMoves[UnityEngine.Random.Range(0, sideMoves.Count)];
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
        }

        // Final connection to Middle Bot if needed
        // while (currentGrid.x != 0 && safety < maxSafety)
        // {
        //     safety++;
        //     Vector2Int stepDir = new Vector2Int(currentGrid.x > 0 ? -1 : 1, 0);
        //     Vector2Int next = currentGrid + stepDir;

        //     if (IsValidMove(next, occupied, stepsY))
        //     {
        //         currentGrid = next;
        //         AddTile(currentGrid, startZ, occupied);
        //     }
        //     else
        //     {
        //         break;
        //     }
        // }

        pathCreatedLength = currentMaxY;

        UpdateTileVisuals();
    }

    public void UpdatePlayerPosition(int playerY)
    {
        int newtargetY = playerY + 10;
        if (newtargetY > pathCreatedLength)
        {
            UpdatePath(newtargetY);
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
        Vector3 worldPos = new Vector3(gridPos.x * tileSize, 0f, gridPos.y * tileSize);
        SpawnTile(worldPos, gridPos);
    }

    private void SpawnTile(Vector3 localPos, Vector2Int gridPos)
    {
        GameObject tile = Instantiate(tilePrefab, transform);
        tile.transform.localPosition = localPos;

        placedTiles.Add(tile);
        placedPositions.Add(gridPos);
    }

    public void UpdateTileVisuals()
    {
        for (int i = 0; i < placedTiles.Count; i++)
        {
            GameObject tileObj = placedTiles[i];
            Vector2Int gridPos = placedPositions[i];

            GameObject tileComp = tileObj;//tileObj.GetComponentInChildren<GameObject>();
            if (tileComp != null)
            {
                // Determine neighbors
                bool up = placedPositions.Contains(gridPos + Vector2Int.up);
                bool down = placedPositions.Contains(gridPos + Vector2Int.down);
                bool left = placedPositions.Contains(gridPos + Vector2Int.left);
                bool right = placedPositions.Contains(gridPos + Vector2Int.right);

                int neighbourCount = (up ? 1 : 0) + (down ? 1 : 0) + (left ? 1 : 0) + (right ? 1 : 0);

                Transform[] childrens = tileComp.GetComponentsInChildren<Transform>();

                if (up && down || left && right)
                {
                    // straight piece
                    // Debug.Log("Tile at " + gridPos + " is a straight piece.");

                    foreach (var child in childrens)
                    {
                        // Debug.Log("Child name: " + child.name + ", " + child.);
                        if (child.name == "block_corner")
                        {
                            child.gameObject.SetActive(false);
                        }
                    }
                }
                else if ((up && right) || (right && down) || (down && left) || (left && up))
                {
                    // corner piece
                    // Debug.Log("Tile at " + gridPos + " is a corner piece.");

                    foreach (var child in childrens)
                    {
                        // Debug.Log("Child name: " + child.name);
                        if (child.name == "block_straight")
                        {
                            child.gameObject.SetActive(false);
                        }
                    }
                }
                else
                {
                    // end piece
                    // Debug.Log("Tile at " + gridPos + " is something else." + up + ", " + down + ", " + left + ", " + right);
                }
            }
        }
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

        placedTiles.Clear();
        placedPositions.Clear();
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


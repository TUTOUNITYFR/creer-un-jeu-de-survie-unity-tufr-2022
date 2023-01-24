using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField]
    private float sizeX = 7.2f;

    [SerializeField]
    private float sizeY = 6f;

    [SerializeField]
    private float sizeZ = 7.2f;

    public Vector3 GetNearestPointOnGrid(Vector3 position)
    {
        int xCount = Mathf.RoundToInt(position.x / sizeX);
        int yCount = Mathf.RoundToInt(position.y / sizeY);
        int zCount = Mathf.RoundToInt(position.z / sizeZ);

        return new Vector3(xCount * sizeX, yCount * sizeY, zCount * sizeZ);
    }
}

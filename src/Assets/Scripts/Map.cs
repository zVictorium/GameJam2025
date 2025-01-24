using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] private bool isWall = false;

    public bool IsWall()
    {
        return isWall;
    }
}

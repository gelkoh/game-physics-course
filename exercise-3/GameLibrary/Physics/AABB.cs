namespace GameLibrary.Physics;

public struct AABB
{
    public float MinX;
    public float MaxX;
    public float MinY;
    public float MaxY;

    public static bool intersects(AABB a, AABB b)
    {
        return a.MinX < b.MaxX &&
               a.MaxX > b.MinX &&
               a.MinY < b.MaxY &&
               a.MaxY > b.MinY;
    }
}
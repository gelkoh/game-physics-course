namespace GameLibrary.Physics;

using Microsoft.Xna.Framework;

public interface IConvexPolygonCollider
{
    public Vector2[] GetWorldVertices();
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameLibrary.Physics;

using System;

public static class DebugPhysicsRenderer
{
    private static Texture2D _pixel;

    public static void DrawColliders(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
    {
        // 1x1 Pixel Textur "on the fly" erstellen, falls noch nicht da
        if (_pixel == null)
        {
            _pixel = new Texture2D(graphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }

        // Alle aktiven Collider zeichnen
        foreach (var collider in PhysicsWorld.ActiveColliders)
        {
            // 1. AABB zeichnen (Gelb) - Hilft zu sehen, ob die Broadphase stimmt
            AABB aabb = collider.GetAABB();
            DrawHollowRect(spriteBatch, aabb.MinX, aabb.MinY, aabb.MaxX - aabb.MinX, aabb.MaxY - aabb.MinY, Color.Yellow * 0.3f);

            // 2. Die tatsächliche Form zeichnen (Rot)
            if (collider is CircleCollider circle)
            {
                // Kreis zeichnen (vereinfacht als Box oder Polygon)
                // Hier nur Mittelpunkt markieren für Quick-Debug
                spriteBatch.Draw(_pixel, new Rectangle((int)circle.Position.X - 2, (int)circle.Position.Y - 2, 4, 4), Color.Red);
            }
            else if (collider is IConvexPolygonCollider poly)
            {
                Vector2[] verts = poly.GetWorldVertices();
                DrawPolygon(spriteBatch, verts, Color.Red);
            }
        }
    }

    private static void DrawPolygon(SpriteBatch spriteBatch, Vector2[] verts, Color color)
    {
        for (int i = 0; i < verts.Length; i++)
        {
            Vector2 p1 = verts[i];
            Vector2 p2 = verts[(i + 1) % verts.Length]; // Verbindung zum nächsten (und letzter zum ersten)
            DrawLine(spriteBatch, p1, p2, color);
        }
    }

    private static void DrawHollowRect(SpriteBatch spriteBatch, float x, float y, float w, float h, Color color)
    {
        DrawLine(spriteBatch, new Vector2(x, y), new Vector2(x + w, y), color);
        DrawLine(spriteBatch, new Vector2(x + w, y), new Vector2(x + w, y + h), color);
        DrawLine(spriteBatch, new Vector2(x + w, y + h), new Vector2(x, y + h), color);
        DrawLine(spriteBatch, new Vector2(x, y + h), new Vector2(x, y), color);
    }

    private static void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color)
    {
        Vector2 edge = end - start;
        float angle = (float)Math.Atan2(edge.Y, edge.X);
        float length = edge.Length();

        spriteBatch.Draw(_pixel, start, null, color, angle, Vector2.Zero, new Vector2(length, 2), SpriteEffects.None, 0);
    }
}
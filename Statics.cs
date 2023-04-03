using UnityEngine;

public static class Statics
{
    public static bool CheckBox(this Vector2 p1, Vector2 p2, Vector2 size)
    {
        size /= 2;
        float x1 = p2.x - size.x;
        float x2 = p2.x + size.x;
        float y1 = p2.y - size.y;
        float y2 = p2.y + size.y;
        return !(p1.x > x2 | p1.x < x1 | p1.y > y2 | p1.y < y1);
    }

    public static bool CheckBox(this Vector2 p1, Vector2 p2, Vector2 size, float pivotX, float pivotY)
    {
        float x1 = p2.x - size.x * pivotX;
        float x2 = p2.x + size.x * (1 - pivotX);
        float y1 = p2.y - size.y * pivotY;
        float y2 = p2.y + size.y * (1 - pivotY);
        return !(p1.x > x2 | p1.x < x1 | p1.y > y2 | p1.y < y1);
    }
}
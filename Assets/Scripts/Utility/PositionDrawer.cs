using UnityEngine;

namespace AFSInterview
{
    public static class PositionDrawer
    {
        public static void DrawPosition(Vector3 position)
        {
            Debug.DrawRay(position, Vector3.up, Color.green, 2);
        }
    }
}
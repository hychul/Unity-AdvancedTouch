using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdvancedTouch
{
    public struct Touch
    {
        public int fingerId;

        public Vector2 point;
        public Vector2 prevPoint;

        public TouchPhase phase;

        public float holdTime;
        public float dragLength;
    }
}
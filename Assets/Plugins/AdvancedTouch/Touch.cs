using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdvancedTouch
{
	public class Touch
	{
		private int fingerId;
		
		public Vector2 point;
		public Vector2 prevPoint;

		public TouchPhase phase;

		public float holdTime;
		public float dragLength;

		public int FingerId
		{
			get { return fingerId; }
		}

		public Touch(int fingerId, Vector2 point)
		{
			this.fingerId = fingerId;
			this.point = point;
			prevPoint = point;
			phase = TouchPhase.Down;
		}

		public void UpdatePoint(Vector2 newPoint)
		{
			prevPoint = point;
			point = newPoint;
		}
	}
}
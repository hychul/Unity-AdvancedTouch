using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace AdvancedTouch 
{
	public class AdvancedTouchInputModule : MonoBehaviour
    {
        private class AdvancedTouch
        {
            public int fingerId;

            public Vector2 point;
            public Vector2 prevPoint;

            public TouchPhase phase;

            public float holdTime;
            public float dragLength;

			public Touch ToTouch() {
				Touch touch = new Touch() {
                    fingerId = this.fingerId,
                	point = this.point,
                	prevPoint = this.prevPoint,
                	phase = this.phase,
                	holdTime = this.holdTime,
                	dragLength = this.dragLength
				};

				return touch;
			}
        }

        [Serializable]
        public class TouchEvent : UnityEvent<List<Touch>> { }

		private const float CRITERIA_DPI = 320.0f;

		[SerializeField]
		private float holdTriggerDelay = 0.5f;
		[SerializeField]
		private float dragTriggerLength = 10.0f;
		[SerializeField]
		private float tremorRevision = 10.0f;

		[SerializeField]
		private bool ignoreOverUI = true;

		public TouchEvent onTouched;

		private readonly Stack<AdvancedTouch> touchPool = new Stack<AdvancedTouch>();
		private readonly Dictionary<int, AdvancedTouch> touchById = new Dictionary<int, AdvancedTouch>();
		private readonly List<Touch> touchEventList = new List<Touch>();

		private void Start()
		{
			var dpiFactor = Screen.dpi / CRITERIA_DPI;
			tremorRevision *= dpiFactor;
		}

		private void Update()
		{
			if (ignoreOverUI && EventSystem.current.IsPointerOverGameObject())
				return;
			
#if UNITY_EDITOR
			MouseInput();
#else
			TouchInput();
#endif
		}

		private void MouseInput()
		{
			AdvancedTouch touch;
			if (Input.GetMouseButtonDown(0))
			{
				if (touchById.ContainsKey(0))
					return;

				if (ignoreOverUI && EventSystem.current.IsPointerOverGameObject())
					return;
				
				touch = GetTouch(0, Input.mousePosition);
				touch.phase = TouchPhase.None;

				touchById.Add(0, touch);

				touchEventList.Add(touch.ToTouch());
			}
			else if (Input.GetMouseButton(0))
			{
				if (!touchById.ContainsKey(0))
					return;

				touch = touchById[0];

				UpdateTouch(touch, Input.mousePosition);

				if (0.0f < (touch.point - touch.prevPoint).magnitude)
					touch.dragLength += (touch.point - touch.prevPoint).magnitude;
				else
					touch.holdTime += Time.deltaTime;

				if (dragTriggerLength < touch.dragLength)
					touch.phase = TouchPhase.Drag;
				else if (holdTriggerDelay < touch.holdTime)
					touch.phase = TouchPhase.Hold;
				else
					touch.phase = TouchPhase.Down;

				touchEventList.Add(touch.ToTouch());
			}
			else if (Input.GetMouseButtonUp(0))
			{
				if (!touchById.ContainsKey(0))
					return;

				touch = touchById[0];
				touchById.Remove(0);
                PutTouch(touch);
				UpdateTouch(touch, Input.mousePosition);

				switch (touch.phase)
				{
					case TouchPhase.Hold:
						touch.phase = TouchPhase.HoldUp;
						break;
					case TouchPhase.Drag:
						touch.phase = TouchPhase.DragUp;
						break;
					default:
						touch.phase = TouchPhase.Up;
						break;
				}

				touchEventList.Add(touch.ToTouch());
			}

			onTouched.Invoke(touchEventList);
			touchEventList.Clear();
		}

		private void TouchInput()
		{
			int touchCount = Input.touchCount;
			var touches = Input.touches;
			for (int i = 0; i < touchCount; i++)
			{
				var input = touches[i];

				AdvancedTouch touch;
				switch (input.phase)
				{
					case UnityEngine.TouchPhase.Began:
						if (touchById.ContainsKey(input.fingerId))
							break;

						if (ignoreOverUI && EventSystem.current.IsPointerOverGameObject(input.fingerId))
							break;

						touch = GetTouch(input.fingerId, input.position);
						touch.phase = TouchPhase.None;

						touchById.Add(input.fingerId, touch);

						touchEventList.Add(touch.ToTouch());
						break;
					case UnityEngine.TouchPhase.Stationary:
					case UnityEngine.TouchPhase.Moved:
						if (!touchById.ContainsKey(input.fingerId))
							break;

						touch = touchById[input.fingerId];
						UpdateTouch(touch, input.position);

						if (tremorRevision < input.deltaPosition.magnitude)
							touch.dragLength += input.deltaPosition.magnitude;
						else
							touch.holdTime += Time.deltaTime;

						if (dragTriggerLength < touch.dragLength)
							touch.phase = TouchPhase.Drag;
						else if (holdTriggerDelay < touch.holdTime)
							touch.phase = TouchPhase.Hold;
						else
							touch.phase = TouchPhase.Down;

						touchEventList.Add(touch.ToTouch());
						break;
					default:
						if (!touchById.ContainsKey(input.fingerId))
							break;

						touch = touchById[input.fingerId];
						touchById.Remove(input.fingerId);
						PutTouch(touch);
                        UpdateTouch(touch, input.position);

						switch (touch.phase)
						{
							case TouchPhase.Hold:
								touch.phase = TouchPhase.HoldUp;
								break;
							case TouchPhase.Drag:
								touch.phase = TouchPhase.DragUp;
								break;
							default:
								touch.phase = TouchPhase.Up;
								break;
						}

						touchEventList.Add(touch.ToTouch());
						break;
				}
			}

			onTouched.Invoke(touchEventList);
			touchEventList.Clear();
		}

		private void UpdateTouch(AdvancedTouch touch, Vector2 point)
		{
			touch.prevPoint = touch.point;
			touch.point = point;
		}

		private AdvancedTouch GetTouch(int fingerId, Vector2 point)
		{
			AdvancedTouch touch;

			if (0 < touchPool.Count)
			{
				touch = touchPool.Pop();
			}
			else
			{
				touch = new AdvancedTouch();
			}

            touch.fingerId = fingerId;
            touch.point = point;
            touch.prevPoint = point;
            touch.phase = TouchPhase.None;
            touch.holdTime = 0;
            touch.dragLength = 0;

			return touch;
		}

		private void PutTouch(AdvancedTouch touch)
		{
			touchPool.Push(touch);
		}
	}
}

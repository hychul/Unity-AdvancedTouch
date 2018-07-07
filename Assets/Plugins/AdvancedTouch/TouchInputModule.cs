using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace AdvancedTouch 
{
	public class TouchInputModule : MonoBehaviour
	{
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

		private readonly Dictionary<int, Touch> touchById = new Dictionary<int, Touch>();
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
			Touch touch;
			if (Input.GetMouseButtonDown(0))
			{
				if (touchById.ContainsKey(0))
					return;

				if (ignoreOverUI && EventSystem.current.IsPointerOverGameObject())
					return;
				
				touch = new Touch(0, Input.mousePosition);
				touchById.Add(0, touch);

				touchEventList.Add(touch);
			}
			else if (Input.GetMouseButton(0))
			{
				if (!touchById.ContainsKey(0))
					return;

				touch = touchById[0];

				var previousPos = touch.point;
				touch.UpdatePoint(Input.mousePosition);

				if (0.0f < (touch.point - previousPos).magnitude)
					touch.dragLength += (touch.point - previousPos).magnitude;
				else
					touch.holdTime += Time.deltaTime;

				if (dragTriggerLength < touch.dragLength)
					touch.phase = TouchPhase.Drag;
				else if (holdTriggerDelay < touch.holdTime)
					touch.phase = TouchPhase.Hold;
				else
					touch.phase = TouchPhase.None;

				touchEventList.Add(touch);
			}
			else if (Input.GetMouseButtonUp(0))
			{
				if (!touchById.ContainsKey(0))
					return;

				touch = touchById[0];
				touchById.Remove(0);
				touch.UpdatePoint(Input.mousePosition);

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

				touchEventList.Add(touch);
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

				Touch touch;
				switch (input.phase)
				{
					case UnityEngine.TouchPhase.Began:
						if (touchById.ContainsKey(input.fingerId))
							break;

						if (ignoreOverUI && EventSystem.current.IsPointerOverGameObject(input.fingerId))
							break;

						touch = new Touch(input.fingerId, input.position);
						touchById.Add(input.fingerId, touch);

						touchEventList.Add(touch);
						break;
					case UnityEngine.TouchPhase.Stationary:
					case UnityEngine.TouchPhase.Moved:
						if (!touchById.ContainsKey(input.fingerId))
							break;

						touch = touchById[input.fingerId];
						touch.UpdatePoint(input.position);

						if (tremorRevision < input.deltaPosition.magnitude)
							touch.dragLength += input.deltaPosition.magnitude;
						else
							touch.holdTime += Time.deltaTime;

						if (dragTriggerLength < touch.dragLength)
							touch.phase = TouchPhase.Drag;
						else if (holdTriggerDelay < touch.holdTime)
							touch.phase = TouchPhase.Hold;
						else
							touch.phase = TouchPhase.None;

						touchEventList.Add(touch);
						break;
					default:
						if (!touchById.ContainsKey(input.fingerId))
							break;

						touch = touchById[input.fingerId];
						touch.UpdatePoint(input.position);
						touchById.Remove(input.fingerId);
						
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

						touchEventList.Add(touch);
						break;
				}
			}

			onTouched.Invoke(touchEventList);
			touchEventList.Clear();
		}
	}
}

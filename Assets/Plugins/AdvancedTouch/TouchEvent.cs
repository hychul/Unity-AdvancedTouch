using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AdvancedTouch
{
		[Serializable]
		public class TouchEvent : UnityEvent<List<Touch>> { }
}

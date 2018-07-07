using AdvancedTouch;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class SimpleTouchPresenter : MonoBehaviour 
{
	private const string TOUCH_COUNT_FORMAT = "Touch Count : {0}\n";
	private const string TOUCH_PHASE_FORMAT = "Touch Phase : {0}";

	[SerializeField] private Text txtPresent;

	public void OnTouchInput(List<AdvancedTouch.Touch> touchList) {
		var stringBuilder = new StringBuilder();
		
		var touchCount = touchList.Count;

		stringBuilder.Append(string.Format(TOUCH_COUNT_FORMAT, touchCount));

		if (0 < touchCount)
			stringBuilder.Append(string.Format(TOUCH_PHASE_FORMAT, touchList[0].phase));

		txtPresent.text = stringBuilder.ToString();
	}
}

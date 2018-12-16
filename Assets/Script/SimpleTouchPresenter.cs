using AdvancedTouch;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class SimpleTouchPresenter : MonoBehaviour 
{
	private const string TOUCH_TEXT_FORMAT = "Touch Count : {0}\nTouch Phase : {1}";

	[SerializeField] private Text txtPresent;

	public void OnTouchInput(List<AdvancedTouch.Touch> touchList) {
		var stringBuilder = new StringBuilder();
		
		var touchCount = touchList.Count;

		stringBuilder.Append(string.Format(TOUCH_TEXT_FORMAT, touchCount, 0 < touchCount ? touchList[0].phase : AdvancedTouch.TouchPhase.None));

		txtPresent.text = stringBuilder.ToString();
	}
}

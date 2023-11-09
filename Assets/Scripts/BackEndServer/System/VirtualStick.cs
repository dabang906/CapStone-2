using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VirtualStick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler {

	public Image bgImg {get; private set;}
	private Image stickImg;
	private Vector3 inputVector;
	public bool isInputEnable {get;set;}

	public Vector3 joyStickInputPos {get; private set;}

	void Start () {
		isInputEnable = false;
		joyStickInputPos = Vector3.zero;
		bgImg = this.transform.GetChild(0).gameObject.GetComponent<Image>();
		stickImg = this.transform.GetChild(1).gameObject.GetComponent<Image>();
	}

	public virtual void OnDrag(PointerEventData pointEvent) {
		Vector2 pos;
		joyStickInputPos = pointEvent.position;
		if(RectTransformUtility.ScreenPointToLocalPointInRectangle(
			bgImg.rectTransform, pointEvent.position, pointEvent.pressEventCamera, out pos)) {
				isInputEnable = true;
				pos.x -=150;
				pos.y+=150;
				pos.x /= bgImg.rectTransform.sizeDelta.x;
				pos.y /= bgImg.rectTransform.sizeDelta.y;

				inputVector = new Vector3(pos.x * 2 + 1, pos.y * 2 - 1, 0);
				inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;
				stickImg.rectTransform.anchoredPosition =
					new Vector3(inputVector.x * (bgImg.rectTransform.sizeDelta.x / 3),
								inputVector.y * (bgImg.rectTransform.sizeDelta.y / 3));
		}
	}

	public virtual void OnPointerDown(PointerEventData pointEvent) {
		joyStickInputPos = Vector3.zero;
		OnDrag(pointEvent);
	}

	public virtual void OnPointerUp(PointerEventData pointEvent) {
		isInputEnable = false;
		inputVector = Vector3.zero;
		stickImg.rectTransform.anchoredPosition = Vector3.zero;
	}

	public float GetHorizontalValue() {
		return inputVector.x;
	}
	public float GetVerticalValue() {
		return inputVector.y;
	}

}

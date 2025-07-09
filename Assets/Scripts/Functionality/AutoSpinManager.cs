using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AutoSpinManager : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
	[SerializeField]
	private SlotBehaviour slotManager;
	[SerializeField]
	private Button StartBTN;


	public void OnPointerDown(PointerEventData eventData)
    {
		Debug.Log("run on pointer Down");
		slotManager.StartSpinRoutine();
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		Debug.Log("run on pointer UP");
		if (!slotManager.IsHoldSpin)
		{

			slotManager.StopSpinRoutine();
			slotManager.WasAutoSpinOn = false;
			Debug.Log("auto spin stopp");
		}
	}
}

	


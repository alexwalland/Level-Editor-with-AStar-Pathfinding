using UnityEngine;
using System.Collections;

	public class InterfaceManager : MonoBehaviour
	{
	public bool mouseOverUIElement;

	private static InterfaceManager instance = null;

	void Awake()
	{
		instance = this;
	}

	public static InterfaceManager GetInstance()
	{
		return instance;
	}

	public void MouseEnter()
	{
		mouseOverUIElement = true;
	}

	public void MouseExit()
	{
		mouseOverUIElement = false;
	}


}


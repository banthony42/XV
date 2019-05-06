﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class XV_UI : MonoBehaviour
{

	public bool isGUILocked { get { return mIsGUILocked; } }

	[SerializeField]
	private UINotifier notifier;

	[SerializeField]
	private CanvasGroup UIModelManagerLockerScreen;

	[SerializeField]
	private CanvasGroup UIToolBarLockerScreen;

	private UINotifier Notifier { get { return notifier; } }

	private static XV_UI sInstance;

	private static bool sLockInstance;

	private bool mIsGUILocked;

	static public XV_UI Instance
	{
		get
		{
			if (sInstance == null) {
				sLockInstance = true;
				sInstance = Resources.Load<GameObject>(GameManager.UI_TEMPLATE_PATH + "XV_UI").GetComponent<XV_UI>();
			}
			return sInstance;
		}
	}

	private void Start()
	{
		if (sInstance == null)
			sInstance = this;
		else if (!sLockInstance) {
			Destroy(gameObject);
			throw new Exception("An instance of this singleton already exists.");
		}
		sLockInstance = false;
		enabled = false;
		mIsGUILocked = true;
	}

	public void Notify(float iDuration, string iMessage)
	{
		if (Notifier != null)
			Notifier.Notify(iDuration, iMessage);
	}

	public void LockGUI()
	{
		if (!mIsGUILocked) {
			mIsGUILocked = true;
			UIModelManagerLockerScreen.alpha = 0F;
			UIToolBarLockerScreen.alpha = 0F;
			UIModelManagerLockerScreen.gameObject.SetActive(true);
			UIToolBarLockerScreen.gameObject.SetActive(true);
			StartCoroutine(Utils.FadeToAsync(1F, 0.5F, UIModelManagerLockerScreen));
			StartCoroutine(Utils.FadeToAsync(1F, 0.5F, UIToolBarLockerScreen));
		}
	}

	public void UnlockGUI()
	{
		if (mIsGUILocked) {
			mIsGUILocked = false;
			UIModelManagerLockerScreen.alpha = 1F;
			UIToolBarLockerScreen.alpha = 1f;
			StartCoroutine(Utils.FadeToAsync(0F, 0.5F, UIModelManagerLockerScreen,
											 () => { UIModelManagerLockerScreen.gameObject.SetActive(false); }));
			StartCoroutine(Utils.FadeToAsync(0F, 0.5F, UIToolBarLockerScreen,
											 () => { UIToolBarLockerScreen.gameObject.SetActive(false); }));
		
		}
	}
}

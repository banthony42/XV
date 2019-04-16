﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectEntity : MonoBehaviour
{
	public static string TAG = "ObjectEntity";
	public static List<ObjectEntity> sAllEntites;

	public static ObjectEntity[] AllEntities
	{
		get
		{
			if (sAllEntites != null)
				return sAllEntites.ToArray();
			else
				return new ObjectEntity[0];
		}
	}

	public static int InstantiatedEntity { get { return sAllEntites.Count; } }

	private DataScene mDataScene;
	private ObjectDataScene mODS;
	private bool mSelected;
	private bool mDestroyingObject;
	private bool mPoppingObject;

	private UIBubbleInfo mUIBubbleInfo;
	private Vector3 mCenter;
	private Vector3 mSize;

	public bool Selected
	{
		get
		{
			return mSelected;
		}
		set
		{
			if (!value) {
				Debug.Log("ObjectEntity : " + mODS.Name + "Has been unselected");
				mUIBubbleInfo.Hide();
			}
			mSelected = value;
		}
	}

	void Start()
	{
		if (sAllEntites == null)
			sAllEntites = new List<ObjectEntity>();
		sAllEntites.Add(this);

		gameObject.tag = TAG;
		Transform[] lTransforms = GetComponentsInChildren<Transform>();

		foreach (Transform childObject in lTransforms) {
			MeshFilter meshFilter = childObject.gameObject.GetComponent<MeshFilter>();

			if (meshFilter != null) {

				ColliderMouseHandler lCMH = childObject.gameObject.AddComponent<ColliderMouseHandler>();
				lCMH.OnMouseDownAction = OnMouseDown;
			}
		}
	}

	void Update()
	{

	}

	// Called by unity only !
	public void OnDestroy()
	{
		sAllEntites.Remove(this);
	}

	// Called by XV
	public void Dispose()
	{
		if (!mDestroyingObject && !mPoppingObject)
			StartCoroutine(DestroyObjectsTimed());
	}

	private IEnumerator DestroyObjectsTimed()
	{
		mDestroyingObject = true;

		Transform[] lTransforms = gameObject.GetComponentsInChildren<Transform>();
		Array.Reverse(lTransforms);

		if (lTransforms.Length > 0) {
			float lWaiting = 0.05F / lTransforms.Length;

			foreach (Transform lTransform in lTransforms) {
				if (lTransform.gameObject.tag == TAG) {
					Destroy(lTransform.gameObject);
					yield return new WaitForSeconds(lWaiting);
				}
			}
		}
		mDestroyingObject = false;
	}

	private void OnMouseDown()
	{
		if (!Selected) {
			GameManager.Instance.SelectedEntity = this;
			Debug.Log("ObjectEntity : " + mODS.Name + " has been selected");
			mUIBubbleInfo.Display();
		}
	}

	public ObjectEntity InitDataScene(DataScene iDataScene)
	{
		mDataScene = iDataScene;
		return this;
	}

	public ObjectEntity StartAnimation(bool iAnimatedPopping)
	{
		if (iAnimatedPopping && !mPoppingObject) {

			Transform[] lTransforms = gameObject.GetComponentsInChildren<Transform>();
			Array.Reverse(lTransforms);

			if (lTransforms.Length > 0) {
				float lWaiting = 0.05F / lTransforms.Length;

				foreach (Transform lTransform in lTransforms) {
					if (lTransform.gameObject.tag == TAG) {
						lTransform.gameObject.SetActive(false);
					}
				}
			}

			gameObject.SetActive(true);
			StartCoroutine(PoppingObject(lTransforms));
		}
		return this;
	}

	private IEnumerator PoppingObject(Transform[] iTransforms)
	{
		mPoppingObject = true;

		if (iTransforms.Length > 0) {
			float lWaiting = 0.05F / iTransforms.Length;

			foreach (Transform lTransform in iTransforms) {
				if (lTransform.gameObject.tag == TAG) {
					lTransform.gameObject.SetActive(true);
					yield return new WaitForSeconds(lWaiting);
				}
			}
		}
		mPoppingObject = false;
	}

	public ObjectEntity SetObjectDataScene(ObjectDataScene iODS)
	{
		mODS = iODS;
		if (!mDataScene.DataObjects.Contains(mODS))
			mDataScene.DataObjects.Add(mODS);
		return this;
	}

	public ObjectEntity SetUIBubbleInfo(UIBubbleInfo iBubbleInfo)
	{
		mUIBubbleInfo = iBubbleInfo;
		return this;
	}

	public ObjectEntity SetCenter(Vector3 iVector)
	{
		mCenter = iVector;
		return this;
	}

	public ObjectEntity SetSize(Vector3 iVector)
	{
		mSize = iVector;
		return this;
	}

	public ObjectEntity SaveEntity()
	{
		if (mODS != null) {
			mODS.Position = transform.position;
			mODS.Rotation = transform.rotation.eulerAngles;
			mODS.Scale = transform.localScale;

			mDataScene.Serialize();
		}
		return this;
	}
}
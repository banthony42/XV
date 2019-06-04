using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class AEntity : MonoBehaviour
{

	public abstract bool Selected { get; set; }

	public abstract string Name { get; set; }

	public List<Action> PostPoppingAction { get; private set; }

	public virtual Vector3 Size { get { return mSize; } }

	public static List<AEntity> sAllEntites;

	public static AEntity[] AllEntities
	{
		get
		{
			if (sAllEntites != null)
				return sAllEntites.ToArray();
			else
				return new ObjectEntity[0];
		}
	}

	public static int InstantiatedEntity { get { return AllEntities.Length; } }

	protected UIBubbleInfo mUIBubbleInfo;

	protected DataScene mDataScene;

	protected Vector3 mSize;

	protected bool mBusy;

	private AObjectDataScene mODS;

	public abstract void Dispose();

	protected virtual void Awake()
	{
		PostPoppingAction = new List<Action>();
	}

	public AEntity InitDataScene(DataScene iDataScene)
	{
		mDataScene = iDataScene;
		return this;
	}

	public virtual void SetObjectDataScene(AObjectDataScene iODS)
	{
		mODS = iODS;
	}

	protected virtual void Start()
	{
		// Adding this to all ObjectEntities
		if (sAllEntites == null)
			sAllEntites = new List<AEntity>();
		sAllEntites.Add(this);
	}

	protected virtual void OnDestroy()
	{
		if (sAllEntites != null)
			sAllEntites.Remove(this);
	}

	public AEntity SetUIBubbleInfo(UIBubbleInfo iBubbleInfo)
	{
		mUIBubbleInfo = iBubbleInfo;
		mUIBubbleInfo.Parent = this;
		if (mODS != null)
			mUIBubbleInfo.SetUIName(mODS.Name);
		else
			Debug.LogError("[AENTITY] mODS is null when setting UIBubbleInfo");
		mUIBubbleInfo.RefreshCanvas();
		return this;
	}

	public Button CreateBubbleInfoButton(UIBubbleInfoButton iButtonInfo)
	{
		if (mUIBubbleInfo == null) {
			Debug.LogError("[AENTITY] mUIBubbleInfo is null when create button");
			return null;
		}
		return mUIBubbleInfo.CreateButton(iButtonInfo);
	}

	public void DestroyBubbleInfoButton(UIBubbleInfoButton iButtonInfo)
	{
		if (mUIBubbleInfo != null)
			mUIBubbleInfo.DestroyButton(iButtonInfo.Tag);
	}

	public void DestroyBubbleInfoButton(string iTag)
	{
		if (mUIBubbleInfo != null)
			mUIBubbleInfo.DestroyButton(iTag);
	}

	public static void ForEachEntities(Action<AEntity> iAction)
	{
		if (iAction == null)
			return;

		AEntity[] lEntities = AllEntities;
		foreach (AEntity lEntity in lEntities)
			iAction(lEntity);
	}


	private GameObject InstantiateMaterialObject(string iMaterialName)
	{
		return null;
	}

	public GameObject OutlineObject()
	{
		Utils.BrowseChildRecursively(gameObject, (iObject) => {
			Debug.Log("pass1");

			// Perform this action on each child of iObject, which is oGhostObject
			Renderer lR = iObject.GetComponent<Renderer>();
			if (lR != null) {


				// TODO
				// Supprimer les deux fonctions 
				// Pour mettre en evidence, on laisse activé tout ce qui est intertable (pour trouver ca, utiliser AInteraction.CanInteractWith())
				// Et desactiver tout ce qui n'est pas interactable. 
				// Creer son script Interactable a l'humain 
				// ps : supprimer le createButton dans HumanEntity et le mettre dans HumanInteractable comme dans ChariotPlateForm.cs



				//lR.material.shader = Shader.Find("Self-Illumin/Outlined Diffuse");

				//Material[] lMaterials = lR.materials;

				//foreach (Material lMaterial in lMaterials) {
				//	Debug.Log("pass2");
				//	Color lColor = lMaterial.color;
				//	lColor.a = 0.3F;
				//	lMaterial.color = lColor;

				//	lMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
				//	lMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
				//	lMaterial.SetInt("_ZWrite", 0);

				//	lMaterial.DisableKeyword("_ALPHATEST_ON");
				//	lMaterial.EnableKeyword("_ALPHABLEND_ON");
				//	lMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
				//	lMaterial.renderQueue = 10000;

				//}

				//lR.materials = lMaterials;
			}
		});


		return null;
	}

	public GameObject ResetOutlineObject()
	{
		Utils.BrowseChildRecursively(gameObject, (iObject) => {

			// Perform this action on each child of iObject, which is oGhostObject
			Renderer lR = iObject.GetComponent<Renderer>();
			if (lR != null) {


				Material[] lMaterials = lR.materials;

				foreach (Material lMaterial in lMaterials) {
					Debug.Log("pass2");
					Color lColor = lMaterial.color;
					lColor.a = 1F;
					lMaterial.color = lColor;

					lMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
					lMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
					lMaterial.SetInt("_ZWrite", 1);
					lMaterial.DisableKeyword("_ALPHATEST_ON");
					lMaterial.DisableKeyword("_ALPHABLEND_ON");
					lMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
					lMaterial.renderQueue = -1;

				}

				lR.materials = lMaterials;
			}
		});


		return null;
	}

	// This function Instantiate associated Model & make it child of OffsetRotation
	// Then all material are replace by GhostMaterial
	public GameObject CreateGhostObject()
	{
		GameObject lGameObject = null;
		GameObject oGhostObject = null;
		Material lGhostMaterial = null;

		if ((lGhostMaterial = Resources.Load<Material>(GameManager.UI_MATERIAL + "Ghost")) == null) {
			Debug.LogError("Load material : 'Ghost' failed.");
			return null;
		}

		if (mODS.Type == ObjectDataSceneType.BUILT_IN) {
			lGameObject = ModelLoader.Instance.GetModelGameObject(mODS.PrefabName);
			if (lGameObject == null) {
				Debug.LogError("Load prefab " + mODS.PrefabName + " failed.");
				return null;
			}
		} else {
			lGameObject = ModelLoader.Instance.GetModelGameObject(mODS.PrefabName);
			if (lGameObject == null) {
				Debug.LogError("Load model " + mODS.PrefabName + " failed.");
				return null;
			}
		}

		oGhostObject = Instantiate(lGameObject, transform.position, transform.rotation, transform.parent);

		Utils.BrowseChildRecursively(oGhostObject, (iObject) => {

			// Perform this action on each child of iObject, which is oGhostObject
			Renderer lR = iObject.GetComponent<Renderer>();
			if (lR != null) {
				Material[] lO = new Material[lR.materials.Length];
				for (int lIndex = 0; lIndex < lO.Length; lIndex++) {
					lO.SetValue(lGhostMaterial, lIndex);
				}
				lR.materials = lO;
			}

			MonoBehaviour[] lMBs = iObject.GetComponents<MonoBehaviour>();
			foreach (MonoBehaviour lMB in lMBs)
				lMB.enabled = false;
		});

		if (oGhostObject == null)
			Debug.LogError("[AENTITY] oGhostObject is null");

		return oGhostObject;
	}

	public virtual void ResetWorldState()
	{
		transform.position = mODS.Position;
		transform.transform.eulerAngles = mODS.Rotation;
	}

}
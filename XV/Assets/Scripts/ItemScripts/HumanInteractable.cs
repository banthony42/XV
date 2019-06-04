using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanInteractable : AInteraction
{
    protected override void PostPoppingEntity()
    {

        CreateInteraction(new ItemInteraction()
        {
            Name = "TakeObject",
            Help = "Take an object",
            InteractWith = new EntityParameters.EntityType[] { EntityParameters.EntityType.SMALL_ITEM, EntityParameters.EntityType.MEDIUM_ITEM },
            AnimationImpl = TakeObject,
            Button = new UIBubbleInfoButton() {
                Text = "TakeObject",
                Tag = name + "_TAKE_OBJECT",
                ClickAction = OnClickTakeObject
            }
        });
    }

    private void OnClickTakeObject(AEntity iEntity)
    {
        StartCoroutine(InteractionWaitForTargetAsync("TakeAnObject"));
    }

    private IEnumerator InteractionWaitForTargetAsync(string iInteractionName)
    {
        AEntity.HideNoInteractable(GetItemInteraction(iInteractionName).InteractWith);
        yield return new WaitWhile(() => {

            if (Input.GetMouseButtonDown(0))
            {

                RaycastHit lHit;
                Ray lRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(lRay, out lHit, 100000, LayerMask.GetMask("dropable")))
                {

                    Debug.DrawRay(lRay.origin, lRay.direction * lHit.distance, Color.red, 1);

                    // If we don't found EntityParameters, stop with an error.
                    EntityParameters lEntityParam;
                    if ((lEntityParam = lHit.collider.gameObject.GetComponentInParent<EntityParameters>()) == null)
                    {
                        Debug.LogError("[TARGET SELECTOR] Click on Entity !");
                        return false;
                    }

                    // If Subscribers contain the clicked Entity type use it as target and add animation to timeline
                    if (IsInteractionCanInteractType(iInteractionName, lEntityParam.Type))
                    {
                        AnimationParameters lAnimationParameters = new AnimationParameters()
                        {
                            TargetType = AnimationParameters.AnimationTargetType.ENTITY,
                            AnimationTarget = lEntityParam.gameObject,
                        };
                        //TimelineManager.Instance.AddAnimation(gameObject, TakeAnObject, lAnimationParameters);
                        TimelineManager.Instance.AddAnimation(gameObject, TakeObject);
                        return false;
                    }
                    Debug.LogError("[TARGET SELECTOR] The object you click on is not interactable with this object !");
                }

                return false;
            }
            return true;
        });
        AEntity.DisableHideNoInteractable();
    }

    private bool TakeObject(AnimationInfo iAnimInfo)
    {

        return true;
    }

}

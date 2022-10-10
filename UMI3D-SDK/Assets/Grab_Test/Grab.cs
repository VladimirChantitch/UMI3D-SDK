using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using umi3d.edk;
using umi3d.edk.userCapture;
using umi3d.common.userCapture;
using umi3d.common.interaction;
using UnityEngine.Events;
using umi3d.common;

public class Grab : MonoBehaviour
{
    public float HandDistActivation;
    public HandAnimation GrabAnimation;

    Vector3 tempLocalPos;
    Quaternion tempLocalRot;
    UMI3DBinding tempBinding;
    ulong tempUserID;

    Transform bindingAnchor;
    Vector3 localPosOffset;
    Quaternion localRotOffset;

    bool activation = false;

    //[SerializeField]  public InteractionEvent onRelease = new InteractionEvent();

    // Start is called before the first frame update
    void Start()
    {
        tempLocalPos = transform.localPosition;
        tempLocalRot = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (activation)
        {
            GetComponent<UMI3DNode>().objectPosition.SetValue(transform.parent.InverseTransformPoint(bindingAnchor.TransformPoint(localPosOffset)));
            GetComponent<UMI3DNode>().objectRotation.SetValue(Quaternion.Inverse(transform.parent.rotation) * bindingAnchor.rotation * localRotOffset);
        }
    }

    public void StartGrab(umi3d.edk.interaction.AbstractInteraction.InteractionEventContent content)
    {
        UMI3DTrackedUser user = content.user as UMI3DTrackedUser;
        uint bonetype = content.boneType;

        if (!activation)
        {
            Debug.Log("hey i'm active");
            Debug.Log(user);
            Debug.Log(user.Avatar.skeletonAnimator);
            Debug.Log(transform.position, user.Avatar.skeletonAnimator.GetBoneTransform(bonetype.ConvertToBoneType().GetValueOrDefault()));
            if (Vector3.Distance(transform.position, user.Avatar.skeletonAnimator.GetBoneTransform(bonetype.ConvertToBoneType().GetValueOrDefault()).transform.position) > HandDistActivation)
                return;

            Debug.Log("Grabbing");

            if (bonetype.Equals(BoneType.Head))
            {
                bonetype = BoneType.RightHand;
            }

            Matrix4x4 handToObjectMatrix;

            if (bonetype.Equals(BoneType.RightHand))
            {
                handToObjectMatrix = Matrix4x4.TRS(
                    GrabAnimation.HandPose.RightHandPosition,
                    Quaternion.Euler(GrabAnimation.HandPose.RightHandEulerRotation),
                     new Vector3(1f / transform.lossyScale.x, 1f / transform.lossyScale.y, 1f / transform.lossyScale.z)
                ).inverse;

            }
            else
            {
                handToObjectMatrix = Matrix4x4.TRS(
                    GrabAnimation.HandPose.LeftHandPosition,
                    Quaternion.Euler(GrabAnimation.HandPose.LeftHandEulerRotation),
                     new Vector3(1f / transform.lossyScale.x, 1f / transform.lossyScale.y, 1f / transform.lossyScale.z)
                ).inverse;
            }

            bindingAnchor = user.Avatar.skeletonAnimator.GetBoneTransform(bonetype.ConvertToBoneType().GetValueOrDefault()).transform;

            activation = true;

            localPosOffset = Vector3.Scale(
                handToObjectMatrix.MultiplyPoint3x4(Vector3.zero),
                new Vector3(1f / bindingAnchor.lossyScale.x, 1f / bindingAnchor.lossyScale.y, 1f / bindingAnchor.lossyScale.z)
            );

            localRotOffset = handToObjectMatrix.rotation;

            tempUserID = user.Id();

            tempBinding = new UMI3DBinding()
            {
                node = GetComponent<UMI3DNode>(),
                boneType = bonetype,
                isBinded = true,
                syncPosition = true,
                offsetPosition = localPosOffset,
                offsetRotation = localRotOffset
            };

            SetEntityProperty op = UMI3DEmbodimentManager.Instance.AddBinding(user.Avatar, tempBinding);

            Transaction transaction = new Transaction();
            transaction.AddIfNotNull(op);

            transaction.AddIfNotNull(GetComponent<UMI3DNode>().objectPosition.SetValue(transform.parent.InverseTransformPoint(bindingAnchor.TransformPoint(localPosOffset))));
            transaction.AddIfNotNull(GetComponent<UMI3DNode>().objectRotation.SetValue(Quaternion.Inverse(transform.parent.rotation) * bindingAnchor.rotation * localRotOffset));

            transaction.reliable = true;
            
            UMI3DServer.Dispatch(transaction);
        }

        // Tweening the grabbed object to the right position
        // ...
    }

    public void EndGrab(umi3d.edk.interaction.AbstractInteraction.InteractionEventContent content)
    {
        Debug.Log("Ungrabbing");

        UMI3DTrackedUser user = content.user as UMI3DTrackedUser;

        if (!user.Id().Equals(tempUserID))
            return;

        activation = false;

        Transaction transaction = new Transaction();

        transaction.AddIfNotNull(UMI3DEmbodimentManager.Instance.RemoveBinding(user.Avatar, tempBinding));
        transaction.AddIfNotNull(GetComponent<UMI3DNode>().objectPosition.SetValue(tempLocalPos));
        transaction.AddIfNotNull(GetComponent<UMI3DNode>().objectRotation.SetValue(tempLocalRot));

        transaction.reliable = true;

        UMI3DServer.Dispatch(transaction);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using umi3d.edk.userCapture;
using umi3d.common.userCapture;
using System.Linq;

public class HandPoseInterpolation : EditorWindow
{
    [MenuItem("Tools/UMI3D/Hand pose interpolation", false, 0)]
    static void ShowMenu()
    {
        HandPoseInterpolation window = CreateInstance<HandPoseInterpolation>();

        window.position = new Rect(Screen.width / 2, Screen.height / 2, 250, 150);
        window.ShowUtility();
    }

    public UMI3DHandPose LargeHandPose;
    public float LargeHandSize = 1f;
    public UMI3DHandPose InterpolatedHandPose;
    public UMI3DHandPose SmallHandPose;
    public float SmallHandSize = .5f;

    [Range(.25f,1.5f)]
    public float HandSize;


    private Editor WidgetsEditor;
    private Vector2 scrollPosition;


    void OnGUI()
    {
        if (!WidgetsEditor) { WidgetsEditor = Editor.CreateEditor(this); }

        GUILayout.Space(10);

        scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true, GUILayout.Height(160));

        WidgetsEditor.OnInspectorGUI();

        GUILayout.EndScrollView();

        GUILayout.Space(10);

        if (SmallHandPose == null || LargeHandPose == null || InterpolatedHandPose == null)
        {
            GUILayout.Label("Please reference the hand poses you want to edit.");
        }
        else
        {

            if (GUILayout.Button("Interpolate"))
            {
                Debug.Assert(SmallHandPose.isRelativeToNode == LargeHandPose.isRelativeToNode);
                InterpolatedHandPose.isRelativeToNode = SmallHandPose.isRelativeToNode;

                float t = Mathf.InverseLerp(SmallHandSize, LargeHandSize, HandSize);
                Debug.Log($"Interpolation factor : {t}");

                InterpolatedHandPose.RightHandPosition = Vector3.Lerp(
                    SmallHandPose.RightHandPosition,
                    LargeHandPose.RightHandPosition,
                    t
                );

                InterpolatedHandPose.LeftHandPosition = Vector3.Lerp(
                    SmallHandPose.LeftHandPosition,
                    LargeHandPose.LeftHandPosition,
                    t
                );

                InterpolatedHandPose.RightHandEulerRotation = Quaternion.Lerp(
                    Quaternion.Euler(SmallHandPose.RightHandEulerRotation),
                    Quaternion.Euler(LargeHandPose.RightHandEulerRotation),
                    t
                ).eulerAngles;

                InterpolatedHandPose.LeftHandEulerRotation = Quaternion.Lerp(
                    Quaternion.Euler(SmallHandPose.LeftHandEulerRotation),
                    Quaternion.Euler(LargeHandPose.LeftHandEulerRotation),
                    t
                ).eulerAngles;

                Debug.Assert(SmallHandPose.PhalanxRotations.Count == LargeHandPose.PhalanxRotations.Count);

                InterpolatedHandPose.PhalanxRotations = SmallHandPose.PhalanxRotations.Select((spr, i) => {
                        UMI3DHandPose.PhalanxRotation lpr = LargeHandPose.PhalanxRotations[i];
                        // Same ID, same name ?!
                        Debug.Assert(lpr.phalanxId == spr.phalanxId && lpr.Phalanx.CompareTo(spr.Phalanx) == 0);

                        return new UMI3DHandPose.PhalanxRotation(
                            spr.phalanxId,
                            spr.Phalanx,
                            Quaternion.Lerp(
                                Quaternion.Euler(spr.PhalanxEulerRotation),
                                Quaternion.Euler(lpr.PhalanxEulerRotation),
                                t
                            ).eulerAngles
                        );
                }).ToList();
            }
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Close")) Close();
    }

}

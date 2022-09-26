using System.Collections;
using System.Collections.Generic;
using umi3d.cdk;
using umi3d.cdk.volumes;
using umi3d.common.volume;
using UnityEngine;

public class MultiPointVolumeTracker : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// Framerate used to track volumes.
    /// </summary>
    public float frameRate = 30;

    [SerializeField]
    [Tooltip("Points used to detect volume collision.")]
    private Transform[] trackingPoints = null;

    [SerializeField]
    [Tooltip("Stores all points currently in a volume")]
    private Dictionary<AbstractVolumeCell, HashSet<Transform>> pointsByVolume = new Dictionary<AbstractVolumeCell, HashSet<Transform>>();

    private Coroutine trackCoroutine = null;

    #endregion

    private void Awake()
    {
        VolumePrimitiveManager.SubscribeToPrimitiveCreation(OnPrimitiveCreated, true);
        VolumePrimitiveManager.SubscribeToPrimitiveDelete(OnPrimitiveDeleted);
    }

    private void OnEnable()
    {
        trackCoroutine = StartCoroutine(TrackCoroutine());
    }

    private void OnDisable()
    {
        StopCoroutine(trackCoroutine);
        trackCoroutine = null;
    }

    private void OnPrimitiveCreated(AbstractVolumeCell cell)
    {
        if (!pointsByVolume.ContainsKey(cell))
        {
            pointsByVolume.Add(cell, new HashSet<Transform>());
        }
        else
        {
            Debug.LogError("A volume with id " + cell.Id() + " already exists");
        }
    }

    private void OnPrimitiveDeleted(AbstractVolumeCell cell)
    {
        if (pointsByVolume.ContainsKey(cell))
        {
            pointsByVolume.Remove(cell);
        }
        else
        {
            Debug.LogError("No volume found with id " + cell.Id() + ".");
        }
    }

    private IEnumerator TrackCoroutine()
    {
        var wait = new WaitForSeconds(1f / frameRate);

        while (true)
        {

            foreach (AbstractVolumeCell cell in pointsByVolume.Keys)
            {
                bool wasIn = pointsByVolume[cell].Count > 0;

                foreach (Transform t in trackingPoints)
                {
                    if (cell.IsInside(t.position, Space.World))
                        pointsByVolume[cell].Add(t);
                    else
                        pointsByVolume[(cell)].Remove(t);
                }

                int nbPointnbPoint = pointsByVolume[cell].Count;

                if ((nbPointnbPoint > 0) && !wasIn)
                    OnVolumeEnter(cell);
                else if ((nbPointnbPoint == 0) && wasIn)
                    OnVolumeExit(cell);
            }

            yield return wait;
        }
    }

    private void OnVolumeEnter(AbstractVolumeCell cell)
    {
        UMI3DClientServer.SendData(new VolumeUserTransitDto() { direction = true, volumeId = cell.Id() }, true);
    }

    private void OnVolumeExit(AbstractVolumeCell cell)
    {
        UMI3DClientServer.SendData(new VolumeUserTransitDto() { direction = false, volumeId = cell.Id() }, false);
    }
}

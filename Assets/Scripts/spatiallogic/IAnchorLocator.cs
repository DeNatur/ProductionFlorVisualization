using Microsoft.Azure.SpatialAnchors;
using Microsoft.Azure.SpatialAnchors.Unity;
using System;
using UnityEngine;


public delegate void CloudAnchorLocated(object sender, IAnchorLocator.CloudAnchorLocatedArgs args);
public interface IAnchorLocator
{
    public event CloudAnchorLocated CloudAnchorLocated;

    public class CloudAnchorLocatedArgs : EventArgs
    {
        public Pose pose { get; }
        public int type { get; }
        public string identifier { get; }

        public Vector3 scale { get; }

        public CloudAnchorLocatedArgs(Pose pose, int type, string identifier, Vector3 scale)
        {
            this.pose = pose;
            this.type = type;
            this.identifier = identifier;
            this.scale = scale;
        }
    }

    public void startLocatingAzureAnchors(string[] idsToFind);
}
public class AzureAnchorLocator : IAnchorLocator
{
    static string ANCHOR_TYPE_PROP = "ANCHOR_TYPE";
    static string ANCHOR_SCALE_X = "ANCHOR_SCALE_X";
    static string ANCHOR_SCALE_Y = "ANCHOR_SCALE_Y";
    static string ANCHOR_SCALE_Z = "ANCHOR_SCALE_Z";

    readonly SpatialAnchorManager _cloudManager;

    public AzureAnchorLocator(SpatialAnchorManager cloudManager)
    {
        _cloudManager = cloudManager;
        _cloudManager.AnchorLocated += CloudManager_AnchorLocated;

    }

    public event CloudAnchorLocated CloudAnchorLocated;

    public void startLocatingAzureAnchors(string[] idsToFind)
    {
        AnchorLocateCriteria anchorLocateCriteria = new AnchorLocateCriteria();
        anchorLocateCriteria.Identifiers = idsToFind;
        _cloudManager.Session.CreateWatcher(anchorLocateCriteria);
    }

    private void CloudManager_AnchorLocated(object sender, AnchorLocatedEventArgs args)
    {
        if (args.Status == LocateAnchorStatus.Located && args.Anchor != null)
        {
            Debug.Log($"Azure anchor located successfully");
            IAnchorLocator.CloudAnchorLocatedArgs anchorLocatedArgs =
                new IAnchorLocator.CloudAnchorLocatedArgs(
                    pose: args.Anchor.GetPose(),
                    type: int.Parse(args.Anchor.AppProperties[ANCHOR_TYPE_PROP]),
                    identifier: args.Identifier,
                    scale: new Vector3(
                        float.Parse(args.Anchor.AppProperties[ANCHOR_SCALE_X]),
                        float.Parse(args.Anchor.AppProperties[ANCHOR_SCALE_Y]),
                        float.Parse(args.Anchor.AppProperties[ANCHOR_SCALE_Z])
                    )
                );
            this.CloudAnchorLocated?.Invoke(this, anchorLocatedArgs);
        }
    }
}
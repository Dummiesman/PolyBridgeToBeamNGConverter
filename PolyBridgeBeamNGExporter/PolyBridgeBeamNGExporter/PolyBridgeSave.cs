using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyBridgeBeamNGExporter
{
    public enum ObjectType
    {
        Anchor,
        Road,
        Segment,
        SegmentStrong,
        Piston,
        Rope,
        Suspension,
        SuspensionSegment,
        CableStrong,
        Count
    }

    [System.Serializable]
    public class PolyBridgeObject
    {
        public ObjectType type;
        public float x;
        public float y;
        public int id;
        public int anchorAID = -1;
        public int anchorBID = -1;
        public int splitAID = -1;
        public int splitBID = -1;
        public int splitForDrawBridge;
        public float rate;
        public bool isKinematic;

        public bool SplitForDrawBridge => splitForDrawBridge > 0;

        public float BeamStrength
        {
            get
            {
                switch (type)
                {
                    case ObjectType.SegmentStrong:
                    case ObjectType.CableStrong:
                        return 65000f;
                    case ObjectType.Road:
                    case ObjectType.Segment:
                    case ObjectType.Rope:
                        return 45000f;
                    default:
                        return 0f;
                }
                return 0f;
            }
        }
    }

    [System.Serializable]
    public class PolyBridgeSave
    {
        public string DisplayName = "";
        public List<PolyBridgeObject> Objects = new List<PolyBridgeObject>();
    }
}

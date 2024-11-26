using JoeScan.Pinchot;

namespace F3H.ProfileShark.Helpers;

public class HeadCamPair : Tuple<uint, Camera>
{
    public HeadCamPair(uint item1, Camera item2) : base(item1, item2)
    {
    }
}
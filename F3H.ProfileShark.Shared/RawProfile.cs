using JoeScan.Pinchot;

namespace F3H.ProfileShark.Models;

public class RawProfile 
{
    public RawProfile()
    {
        
    }

    public RawProfile(RawProfile rp)
    {
        Index = rp.Index;
        ScanHeadId = rp.ScanHeadId;
        Camera = rp.Camera;
        Laser = rp.Laser;
        Flags = rp.Flags;
        SequenceNumber = rp.SequenceNumber;
        Data = new Point2D[rp.NumPts];
        for (var i = 0; i < rp.NumPts; i++)
        {
            Data[i] = rp.Data[i];
        }
        LaserOnTimeUs = rp.LaserOnTimeUs;
        EncoderValue = rp.EncoderValue;
        TimeStampNs = rp.TimeStampNs;
        ReducedTimeStampNs = rp.ReducedTimeStampNs;
        ReducedEncoder = rp.ReducedEncoder;
    }

    public int Index { get; set; }
    public uint ScanHeadId { get;set; } 
    public Camera Camera { get; set; }
    public Laser Laser { get; set; }
    public UInt32 Flags { get; set; }
    public UInt32 SequenceNumber { get; set; }
    
    
    public Point2D[] Data { get; set; } = Array.Empty<Point2D>();
    public int NumPts => Data.Length;
    public UInt32 LaserOnTimeUs { get; set; }
    public long EncoderValue { get; set; }
   
    public ulong TimeStampNs { get; set; }
    public ulong ReducedTimeStampNs { get; set; }
    public long ReducedEncoder { get; set; }
    
    public UInt32 DataLength { get; set; }
  
}




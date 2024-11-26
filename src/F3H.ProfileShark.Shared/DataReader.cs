using System.IO.Compression;
using F3H.ProfileShark.Models;
using JoeScan.Pinchot;
using K4os.Compression.LZ4.Streams;


namespace F3H.ProfileShark.Shared;

public static class DataReader
{
    public static Task<List<RawProfile>> ReadProfilesNative(string fileName)
    {
        return Task.Run(() =>
        {
            using var sr = File.OpenRead(fileName);
            using var decompressedStream = LZ4Stream.Decode(sr);
            using var br = new BinaryReader(decompressedStream);
            return ReadProfilesNative(br).ToList();
        });
    }

    public static IEnumerable<RawProfile> ReadProfilesNative(BinaryReader br)
    {
        var l = new List<RawProfile>();
        var idx = 0;
        try
        {
            if (0x02 != br.ReadInt32())
            {
                throw new InvalidDataException("Invalid file format");
            }
            while (true)
            {
                var rp = new RawProfile();
                rp.Index = idx++;
                rp.ScanHeadId = br.ReadUInt32();
                rp.Camera = (Camera)br.ReadInt32();
                rp.Laser = (Laser)br.ReadInt32();
                rp.TimeStampNs = br.ReadUInt64();
                int encoderCount = br.ReadInt32();
                for (var i = 0; i < encoderCount; i++)
                {
                    if (i == 0)
                    {
                        rp.EncoderValue = br.ReadInt64();
                    }
                }
                rp.LaserOnTimeUs = br.ReadUInt16();
                var numPoints = br.ReadUInt32();
                _ = br.ReadInt32(); // format
                rp.SequenceNumber = br.ReadUInt32(); 
                rp.Flags = (uint) br.ReadInt32();
                var pts = new List<Point2D>();
                for (var i = 0; i < numPoints; i++)
                {
                    var x = br.ReadSingle();
                    var y = br.ReadSingle();
                    var brightness = br.ReadByte();
                    pts.Add(new Point2D(x, y , brightness));
                }
                rp.DataLength = numPoints;
                rp.Data = pts.ToArray();
                l.Add(rp);
            }
        } catch (EndOfStreamException)
        {
            // ignored
        }
        return l;

    }
    
    public static Task<List<RawProfile>> ReadProfilesC(string fileName)
    {
        return Task.Run(() =>
        {
            using var sr = File.OpenRead(fileName);
            using var br = new BinaryReader(sr);
            return ReadProfilesC(br).ToList();
        });
    }

    public static IEnumerable<RawProfile> ReadProfilesC(BinaryReader br)
    {
        var l = new List<RawProfile>();
        var idx = 0;
        try
        {
            while (true)
            {
                var rp = new RawProfile();
                rp.Index = idx++;
                rp.ScanHeadId = br.ReadUInt32();
                rp.Camera = (Camera)br.ReadInt32();
                rp.Laser = (Laser)br.ReadInt32();
                rp.TimeStampNs = br.ReadUInt64();
                rp.Flags = br.ReadUInt32();
                rp.SequenceNumber = br.ReadUInt32();
                rp.EncoderValue = br.ReadInt64();
                var tmp = br.ReadInt64();
                tmp = br.ReadInt64();
                var numEncoderVals = br.ReadUInt32();
                rp.LaserOnTimeUs = br.ReadUInt32();
                var format = br.ReadInt32();
                var packetsReceived = br.ReadUInt32();
                var packetsExpected = br.ReadUInt32();
                rp.DataLength = br.ReadUInt32();
                var reserved0 = br.ReadUInt64();
                var reserved1 = br.ReadUInt64();
                var reserved2 = br.ReadUInt64();
                var reserved3 = br.ReadUInt64();

                var reserved4 = br.ReadUInt64();
                var reserved5 = br.ReadUInt64();

                var pts = new List<Point2D>();
                for (var i = 0; i < 1456; i++)
                {
                    var x = br.ReadInt32();
                    var y = br.ReadInt32();
                    var brightness = br.ReadInt32();
                    if (i< rp.DataLength)
                    {
                        pts.Add(new Point2D(x / 1000.0, y / 1000.0, brightness));
                    }
                }

                rp.Data = pts.ToArray();

                l.Add(rp);
            }
        }
        catch (EndOfStreamException)
        {
            // ignored
        }

        return l;
    }
}
// uint32_t scan_head_id;
// jsCamera camera;  // int32_t
// jsLaser laser; // int32_t
// uint64_t timestamp_ns;
// uint32_t flags;
// uint32_t sequence_number;
// int64_t encoder_values[JS_ENCODER_MAX];
// uint32_t num_encoder_values;
// uint32_t laser_on_time_us;
// jsDataFormat format; // int32_t
// uint32_t packets_received;
// uint32_t packets_expected;
// uint32_t data_len;
// uint64_t reserved_0;
// uint64_t reserved_1;
// uint64_t reserved_2;
// uint64_t reserved_3;
// uint64_t reserved_4;
// uint64_t reserved_5;
// jsProfileData data[JS_PROFILE_DATA_LEN];
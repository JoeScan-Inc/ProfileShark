using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.IO;
using F3H.ProfileShark.Helpers;
using JoeScan.Pinchot;
using K4os.Compression.LZ4.Streams;
using NLog;

namespace F3H.ProfileShark.Models;

public class LiveRecorder : IDisposable
{
    #region Private Fields

    private readonly ILogger logger;
    private CancellationTokenSource cancellationTokenSource = new();
    private BlockingCollection<IProfile> queue;
    private readonly long profileUpdateFrequency = 100;

    #endregion

    #region Events
    
    public event EventHandler<ProgressEventArgs>? ProgressUpdate;
    
    #endregion
    
    #region Lifecycle

    public LiveRecorder(ILogger logger)
    {
        this.logger = logger;
        logger.Trace("LiveRecorder created");
        queue = new BlockingCollection<IProfile>();
    }

    #endregion

    #region Public Properties

    public string OutputFileName { get; set; } = string.Empty;
    public ScanSystem? ScanSystem { get; set; } = null;
    public uint MinScanPeriod { get; set; }

    #endregion

    #region Public Methods

    public async Task StartRecording()
    {
        // create two tasks, one that produces IProfile data and one that consumes it
        var consumerTask = ConsumeAsync(cancellationTokenSource.Token);
        var producerTask = ProduceAsync(cancellationTokenSource.Token);


        // Wait for both tasks to complete
        await Task.WhenAll(producerTask, consumerTask);
        // await consumerTask;
    }

    public void StopRecording()
    {
        cancellationTokenSource.Cancel();
        logger.Trace("Cancellation requested");
        cancellationTokenSource.Dispose();
        cancellationTokenSource = new CancellationTokenSource();
    }

    #endregion

    #region Private Methods

    #region Consumer

    private Task ConsumeAsync(CancellationToken token)
    {
        return Task.Run(() =>
        {
            var bytesWritten = 0L;
            var profilesWritten = 0L;

            using var fs = new FileStream(OutputFileName, FileMode.Create);
            using Stream target = LZ4Stream.Encode(fs);
            using var bw = new BinaryWriter(target);

            // write a header to the file
            bw.Write(0x02); // version
            while (!queue.IsCompleted && !token.IsCancellationRequested)
            {
                if (queue.TryTake(out IProfile? profile, 3, token))
                {
                    WriteToStream(bw, profile);
                    profilesWritten++;
                    bytesWritten = fs.Position;
                    if (profilesWritten % profileUpdateFrequency == 0)
                    {
                        OnProgressUpdate(new ProgressEventArgs(bytesWritten, profilesWritten));
                        // logger.Info($"Profiles written: {profilesWritten}");
                        // logger.Info($"Bytes written: {bytesWritten}");
                    }
                }
            }
            logger.Info("Consumer Queue completed");
        }, token);
    }

    private void WriteToStream(BinaryWriter bw, IProfile p)
    {
        bw.Write(p.ScanHeadID);
        bw.Write((int)p.Camera);

        bw.Write((int)p.Laser);
        bw.Write(p.TimestampNs);
        bw.Write(p.EncoderValues.Keys.Count);
        for (uint i = 0; i < p.EncoderValues.Keys.Count; i++)
        {
            bw.Write(p.EncoderValues[(Encoder)i]);
        }

        bw.Write(p.LaserOnTimeUs);
        bw.Write(p.ValidPointCount);
        bw.Write((int)p.DataFormat);
        bw.Write(p.SequenceNumber);
        bw.Write((int)p.Flags);
        foreach (var t in p.GetValidXYPoints())
        {
            bw.Write(t.X);
            bw.Write(t.Y);
            // scale brightness to 8 bit
            bw.Write((byte)t.Brightness);
        }
    }

    #endregion
    #region Producer 
    private Task ProduceAsync(CancellationToken token)
    {
        if (ScanSystem == null)
        {
            logger.Error("ScanSystem is null");
            queue.CompleteAdding();
            return Task.CompletedTask;
        }

        if (OutputFileName == string.Empty)
        {
            logger.Error("OutputFileName is empty");
            queue.CompleteAdding();
            return Task.CompletedTask;
        }

        if (ScanSystem.Connect(TimeSpan.FromSeconds(3)).Count > 0)
        {
            logger.Error("Failed to connect to scan system");
            queue.CompleteAdding();
            return Task.CompletedTask;
        }

        var minScanPeriod = ScanSystem.GetMinScanPeriod();
        if (MinScanPeriod < minScanPeriod)
        {
            logger.Warn($"MinScanPeriod is less than the minimum scan period of {minScanPeriod}");
            MinScanPeriod = minScanPeriod;
        }


        return Task.Run(() =>
        {
            ScanSystem.StartScanning(MinScanPeriod, DataFormat.XYBrightnessFull,
                ScanningMode.Frame);
            logger.Info("Scanning started");
            while (!token.IsCancellationRequested)
            {
                var gotFrame = ScanSystem.TryTakeFrame(out var frame,
                    TimeSpan.FromMilliseconds(3), token);
                if (gotFrame)
                {
                    if (frame.IsComplete)
                    {
                        for (int i = 0; i < frame.Count; i++)
                        {
                            queue.Add(frame[i], token);
                        }
                    }
                }
            }
            
            ScanSystem.StopScanning();
            logger.Info("Scanning stopped");
            ScanSystem.Disconnect();
            logger.Info("ScanSystem disconnected");
            queue.CompleteAdding();
            logger.Info("Queue completed");
        }, token);
    }
#endregion
    #endregion

    #region Event Invocations

    protected virtual void OnProgressUpdate(ProgressEventArgs e)
    {
        // the Raise extension method handles the marshalling to the UI thread
        ProgressUpdate?.Raise(this, e);
    }

    #endregion

    public void Dispose()
    {
        cancellationTokenSource.Dispose();
        queue.Dispose();
        logger.Trace("LiveRecorder disposed");
    }
}

#region Event Types

public class ProgressEventArgs : EventArgs
{
    public ProgressEventArgs(long bytesWritten, long profilesWritten)
    {
        BytesWritten = bytesWritten;
        ProfilesWritten = profilesWritten;
    }

    public long BytesWritten { get; }
    public long ProfilesWritten { get; }
}

#endregion
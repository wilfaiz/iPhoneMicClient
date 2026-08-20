using AVFoundation;
using Foundation;
using System.Net.Sockets;

namespace iPhoneMicClient;

public class AudioStreamer
{
    private AVAudioEngine _engine;
    private UdpClient _udpClient;
    private string _targetIp;
    private int _targetPort;

    public AudioStreamer(string ip, int port)
    {
        _targetIp = ip;
        _targetPort = port;
        _udpClient = new UdpClient();
    }

    public void Start()
    {
        // 1. Meminta izin dan mengatur sesi audio perangkat iOS
        var session = AVAudioSession.SharedInstance();
        session.SetCategory(AVAudioSessionCategory.PlayAndRecord, AVAudioSessionCategoryOptions.DefaultToSpeaker);
        session.SetActive(true);

        _engine = new AVAudioEngine();
        var inputNode = _engine.InputNode;

        // 2. Format harus sama persis dengan PC: 44.1kHz, Mono
        var format = new AVAudioFormat(44100, 1);

        // 3. Menyadap suara mikrofon secara real-time (setiap frame/buffer)
        inputNode.InstallTapOnBus(0, 1024, format, (buffer, when) =>
        {
            try
            {
                var audioBuffer = buffer.AudioBufferList[0]; int length = (int)audioBuffer.DataByteSize;
                byte[] data = new byte[length];

                System.Runtime.InteropServices.Marshal.Copy(audioBuffer.Data, data, 0, length);

                // 4. Mengirim byte suara ke IP Address PC (Server)
                _udpClient.Send(data, data.Length, _targetIp, _targetPort);
            }
            catch { }
        });

        _engine.Prepare();
        _engine.StartAndReturnError(out NSError error);
    }

    public void Stop()
    {
        _engine?.InputNode?.RemoveTapOnBus(0);
        _engine?.Stop();
    }
}
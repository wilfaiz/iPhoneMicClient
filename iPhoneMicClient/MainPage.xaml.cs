namespace iPhoneMicClient;

public partial class MainPage : ContentPage
{
#if IOS
    private AudioStreamer _streamer;
#endif
    private bool _isStreaming = false;

    public MainPage()
    {
        InitializeComponent();
    }

    private void OnStreamClicked(object sender, EventArgs e)
    {
#if IOS
        if (!_isStreaming)
        {
            // Pastikan Port 5000 disamakan dengan port di server PC Anda
            _streamer = new AudioStreamer(IpEntry.Text, 5000);
            _streamer.Start();
            
            BtnStream.Text = "Hentikan Stream";
            BtnStream.BackgroundColor = Colors.Red;
            _isStreaming = true;
        }
        else
        {
            _streamer.Stop();
            
            BtnStream.Text = "Mulai Stream";
            BtnStream.BackgroundColor = Colors.Green;
            _isStreaming = false;
        }
#else
        DisplayAlert("Info", "Aplikasi ini dirancang khusus untuk iOS, tidak bisa jalan di Windows.", "OK");
#endif
    }
}
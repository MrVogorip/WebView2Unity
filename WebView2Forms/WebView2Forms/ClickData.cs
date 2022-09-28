namespace WebView2Forms
{
    public class ClickData
    {
        public readonly string X;
        public readonly string Y;
        public readonly int Scroll;

        public ClickData(string data)
        {
            var split = data.Split('|');
            X = split[0];
            Y = split[1];
            Scroll = int.Parse(split[2]);
        }
    }
}
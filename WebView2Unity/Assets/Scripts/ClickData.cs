using UnityEngine;

namespace WebView2
{
    public class ClickData
    {
        private readonly string _x;
        private readonly string _y;
        private readonly int _scroll;

        public ClickData(Vector2 vector, int scroll)
        {
            _x = Mathf.Abs((int)vector.x).ToString();
            _y = Mathf.Abs((int)vector.y).ToString();
            _scroll = scroll > 300 || scroll < -300 ? scroll : 0;
        }

        public override string ToString() =>
            $"{_x}|{_y}|{_scroll}";
    }
}
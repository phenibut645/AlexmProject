using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alexm_app.Models
{
    public delegate void ActiveChangedHandler(TrafficLightState trafficLightState);
    public class TrafficLightState
    {

        public event ActiveChangedHandler ActiveChanged;
        public Color ActiveColor { get; set; }
        public Color InactiveColor { get; set; }
        public Button Button { get; set; }
        public string Text { get; set; }
        private bool _active = false;
        public bool Active
        {
            get
            {
                return _active;
            }
            set
            {
                _active = value;
                ActiveChanged?.Invoke(this);
                if (Button != null) Button.BackgroundColor = CurrentColor;
            }
        }
        private Color _activeColor;
        public Color CurrentColor
        {
            get
            {
                if (Active) return ActiveColor;
                else return InactiveColor;
            }
        }
        public TrafficLightState(string activeColor, string inactiveColor, string text)
        {
            ActiveColor = Color.FromArgb(activeColor);
            InactiveColor = Color.FromArgb(inactiveColor);
            Text = text;
        }
    }
}
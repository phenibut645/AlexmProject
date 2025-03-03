using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alexm_app.Models
{
    public delegate void OptionChanged(Color color);
    public class Theme
    {
        public string Name { get; set; } = "Default";

        private Color _backgroundColor = Color.FromRgba("#2b2c36");
        public event OptionChanged onBackgroundColorChange;

        private Color _enemyCell = Color.FromRgba("#ff5447");
        public Color EnemyCell
            {
            get
            {
                return _enemyCell;
            }
            set
            {
                _enemyCell = value;
                OnEnemyCellColorChange?.Invoke(value);
            }
            } 
        public event OptionChanged OnEnemyCellColorChange;
        public event OptionChanged OnPlayerCellColorChange;
        private Color _playerCell = Color.FromRgba("#91ff87");
        public Color PlayerCell
        {
            get
            {
                return _playerCell;
            }
            set
            {
                _playerCell = value;
                OnPlayerCellColorChange?.Invoke(value);
            }
        }
        public Color BackgroundColor
        {
            get { return _backgroundColor; }
            set
            {
                _backgroundColor = value;
                onBackgroundColorChange?.Invoke(value);
            }
        }

        private Color _cellColor = Color.FromRgba("#343545");
        public event OptionChanged onCellColorChange;
        public Color CellColor
        {
            get { return _cellColor; }
            set
            {
                _cellColor = value;
                onCellColorChange?.Invoke(value);
            }
        }

        private Color _textColor = Color.FromRgba("#ffffff");
        public event OptionChanged onTextColorChanged;
        public Color TextColor
        {
            get { return _textColor; }
            set
            {
                _textColor = value;
                onTextColorChanged?.Invoke(value);
            }
        }

        private Color _frameColor = Color.FromRgba("#222336");
        public event OptionChanged onFrameColorChange;
        public Color FrameColor
        {
            get { return _frameColor; }
            set
            {
                _frameColor = value;
                onFrameColorChange?.Invoke(value);
            }
        }

        private Color _buttonColor = Color.FromRgba("#7a00cc");
        public event OptionChanged onButtonColorChange;
        public Color ButtonColor
        {
            get { return _buttonColor; }
            set
            {
                _buttonColor = value;
                onButtonColorChange?.Invoke(value);
            }
        }

        public void CallEveryEvent()
        {
            onBackgroundColorChange?.Invoke(BackgroundColor);
            onButtonColorChange?.Invoke(ButtonColor);
            onCellColorChange?.Invoke(CellColor);
            onFrameColorChange?.Invoke(FrameColor);
            onTextColorChanged?.Invoke(TextColor);
            OnPlayerCellColorChange?.Invoke(PlayerCell);
            OnEnemyCellColorChange?.Invoke(EnemyCell);
        }

        public Theme(Color backgroundColor, Color cellColor, Color textColor, Color frameColor, Color buttonColor)
        {
            BackgroundColor = backgroundColor;
            CellColor = cellColor;
            TextColor = textColor;
            FrameColor = frameColor;
            ButtonColor = buttonColor;
        }

        public Theme()
        {

        }
    }
}

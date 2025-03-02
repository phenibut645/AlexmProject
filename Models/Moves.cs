using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alexm_app.Models.TicTacToe;

namespace alexm_app.Utils.TicTacToe
{
    public delegate void OnMoveAddHandler(Move move);
    public class Moves
    {
        public event OnMoveAddHandler? OnMoveAdd;
        private List<Move> _moves = new List<Move>();
        public Move this[int index]
        {
            get
            {
                if(index < _moves.Count && index >= 0) return _moves[index];
                throw new IndexOutOfRangeException("Invalid index [Moves]");
            }
            set
            {
                if(index < _moves.Count && index >= 0) _moves[index] = value;
                else throw new IndexOutOfRangeException("Invalid index [Moves]");
            }
        }
        public void Add(Move move)
        {
            _moves.Add(move);
            OnMoveAdd?.Invoke(move);
        }
        public void Remove(Move move)
        {
            _moves.Remove(move);
        }
        public void Remove(int index)
        {
            if(index < _moves.Count && index >= 0)
            {
                _moves.RemoveAt(index);
                return;
            }
            throw new IndexOutOfRangeException("Invalid index [Moves]");
        }
    }
}

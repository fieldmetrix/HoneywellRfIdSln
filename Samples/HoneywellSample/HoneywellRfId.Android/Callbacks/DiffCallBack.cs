using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;

namespace HoneywellRfId.Callbacks
{
    public class DiffCallBack : DiffUtil.Callback
    {
        private List<Dictionary<string, string>> _current;
        private List<Dictionary<string, string>> _next;
        private Dictionary<string, string> _currentItem;
        private Dictionary<string, string> _nextItem;

        public override int NewListSize => _next.Count;

        public override int OldListSize => _current.Count;
        private App _app;

        public DiffCallBack(App app, List<Dictionary<string, string>> current, List<Dictionary<string, string>> next)
        {
            _app = app;
            this._current = current;
            this._next = next;
        }

        public override bool AreContentsTheSame(int oldItemPosition, int newItemPosition)
        {
            _currentItem = _current[oldItemPosition];
            _nextItem = _next[newItemPosition];

            return (_currentItem[_app.Coname[2]].Equals(_nextItem[_app.Coname[2]])) &&
                    (_currentItem[_app.Coname[5]]).Equals(_nextItem[_app.Coname[5]]);
        }

        public override bool AreItemsTheSame(int oldItemPosition, int newItemPosition)
        {
            _currentItem = _current[oldItemPosition];
            _nextItem = _next[newItemPosition];
            return _currentItem[_app.Coname[1]].Equals(_nextItem[_app.Coname[1]]);
        }
    }

}
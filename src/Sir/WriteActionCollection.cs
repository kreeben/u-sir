using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sir
{
    public class WriteActionCollection
    {
        private Dictionary<string, IList<IWriteAction>> _postActions;

        public WriteActionCollection()
        {
            _postActions = new Dictionary<string, IList<IWriteAction>>();
        }

        public void Add(string mediaType, IWriteAction postAction)
        {
            if (!_postActions.ContainsKey(mediaType))
            {
                _postActions.Add(mediaType, new List<IWriteAction>());
            }
            _postActions[mediaType].Insert(postAction.Ordinal, postAction);
        }

        public IList<IWriteAction> Get(string mediaType)
        {
            IList<IWriteAction> actions;
            if(_postActions.TryGetValue(mediaType, out actions))
            {
                return actions;
            }
            return new List<IWriteAction>();
        }
    }
}

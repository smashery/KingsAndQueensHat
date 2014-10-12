using KingsAndQueensHat.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KingsAndQueensHat.ViewModel
{
    public class PersistedTournament
    {
        public PersistedTournament(string name)
        {
            Name = name;
        }

        public event EventHandler Delete;
        public event EventHandler Open;

        public string Path
        {
            get
            {
                return System.IO.Path.Combine(Constants.StorageDirectory(), Name);
            }
        }

        private CommandHandler _deleteCommand;
        public CommandHandler Deleted
        {
            get
            {
                return _deleteCommand ?? (_deleteCommand = new CommandHandler(() => Delete(this, new EventArgs()), () => true));
            }
        }

        private CommandHandler _openCommand;
        public CommandHandler Opened
        {
            get
            {
                return _openCommand ?? (_openCommand = new CommandHandler(() => Open(this, new EventArgs()), () => true));
            }
        }

        public string Name { get; private set; }
    }
}

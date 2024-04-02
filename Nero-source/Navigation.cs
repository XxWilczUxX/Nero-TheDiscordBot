using System;

namespace Nero {

    public interface INavigation {
        public object Right();
        public object Left();
        public object Up();
        public object Down();

        public string GetContents();
    }

    public interface Saveable {
        public void Save(string guildID, string clientID, bool isTemp = false);
        public void Load(string guildID, string clientID, bool isTemp = false);
    }

}
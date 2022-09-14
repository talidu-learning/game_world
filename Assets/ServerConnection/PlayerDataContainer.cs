using System.Collections.Generic;

namespace ServerConnection
{
    public class PlayerDataContainer
    {
        public string _playerName = "Test";
        public string _credential = "0000";
        public string _ownedItems = "Box,Placed";
        public int Stars = 10;
        public List<string> _placedItems = new List<string>{"Placed"};
    }
}
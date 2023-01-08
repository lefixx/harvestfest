using System.Collections.Generic;

using UnityEngine;

namespace LevelGen {
    [CreateAssetMenu]
    public class AssetMap : ScriptableObject {
        public List<Sprite> HouseTop;
        public List<Sprite> Ground;
        public List<Sprite> Road;
        public List<Sprite> HouseDoor;
        public List<Sprite> HouseFront;
    }
}
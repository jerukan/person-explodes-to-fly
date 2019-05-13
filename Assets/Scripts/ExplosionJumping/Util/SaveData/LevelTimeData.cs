using System;
using System.Collections.Generic;

namespace ExplosionJumping.Util.SaveData {
    [Serializable]
    public class LevelTimeData {
        public string levelName;
        public List<float> times;
    }
}

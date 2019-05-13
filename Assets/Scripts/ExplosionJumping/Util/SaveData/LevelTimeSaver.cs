using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.SceneManagement;

namespace ExplosionJumping.Util.SaveData {
    public class LevelTimeSaver : MonoBehaviour {

        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {

        }

        public static void SaveData(LevelTimeData data, string levelName) {
            string path = Path.Combine(Application.persistentDataPath, levelName + ".json");
            string jsonString = JsonUtility.ToJson(data);

            using (StreamWriter streamWriter = File.CreateText(path)) {
                streamWriter.Write(jsonString);
            }
            Debug.Log($"Data written to {path}.");
        }

        public static LevelTimeData LoadData(string levelName) {
            string path = Path.Combine(Application.persistentDataPath, levelName + ".json");

            try {
                using (StreamReader streamReader = File.OpenText(path)) {
                    string jsonString = streamReader.ReadToEnd();
                    return JsonUtility.FromJson<LevelTimeData>(jsonString);
                }
            }
            catch(FileNotFoundException e) {
                SaveData(new LevelTimeData(), SceneManager.GetActiveScene().name);
                using (StreamReader streamReader = File.OpenText(path)) {
                    string jsonString = streamReader.ReadToEnd();
                    return JsonUtility.FromJson<LevelTimeData>(jsonString);
                }
            }
        }
    }
}
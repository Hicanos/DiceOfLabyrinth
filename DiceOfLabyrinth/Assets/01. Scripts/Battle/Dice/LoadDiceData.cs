using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Linq;

public class LoadDiceDataScript
{
    private const string jsonPath = "Assets/Resources/Json/DiceData.json";
    JObject root;

    public void LoadDiceJson()
    {
        if(File.Exists(jsonPath))
        {
            Debug.LogWarning("DiceData.json ¾øÀ½");
            return;
        }

        string jsonData = File.ReadAllText(jsonPath);
        root = JObject.Parse(jsonData);
    }

    public Vector3[] GetPoses()
    {         
        int i = 0;
        JToken dicePoses = root["DicePoses"];        
        Vector3[] loadPoses = new Vector3[dicePoses.Count()];

        foreach (JToken jtoken in dicePoses)
        {
            loadPoses[i].x = (float)jtoken["X"];
            loadPoses[i].y = (float)jtoken["Y"];
            loadPoses[i].z = (float)jtoken["Z"];
            i++;
        }       

        return loadPoses;
    }

    public float[] GetWeighting()
    {
        JToken damageWeighting = root["DamageWeighting"];
        float[] loadWeighting = JsonConvert.DeserializeObject<float[]>(damageWeighting.ToString());

        return loadWeighting;
    }

    public Vector3[] GetVectorCodes()
    {
        int i = 0;
        JToken vectorCodes = root["VectorCodes"];
        Vector3[] loadVectorCodes = new Vector3[vectorCodes.Count()];

        foreach (JToken jtoken in vectorCodes)
        {
            loadVectorCodes[i].x = (float)jtoken["X"];            
            loadVectorCodes[i].y = (float)jtoken["Y"];            
            loadVectorCodes[i].z = (float)jtoken["Z"];
            i++;
        }

        return loadVectorCodes;
    }
}



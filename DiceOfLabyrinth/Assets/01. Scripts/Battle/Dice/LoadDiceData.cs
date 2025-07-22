using UnityEngine;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class LoadDiceDataScript
{
    JObject root;

    public void LoadDiceJson()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Json/DiceData");
        if (textAsset == null)
        {
            Debug.LogWarning("DiceData.json을 찾을 수 없습니다.");
            return;
        }
        string jsonData = textAsset.text;
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

    public Vector3[] GetDiceDefaultPosition()
    {
        int i = 0;
        JToken dicePoses = root["RollDiceDefaultPosition"];
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



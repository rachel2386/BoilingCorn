using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(order = 1, fileName = "ListOfMonologues")]
public class MonologueScriptableObj : ScriptableObject
{
    public List<Monologue> MyMonologues = new List<Monologue>();

    public Monologue GetMonologueFromName(string name)
    {
        return MyMonologues.Find(x => x.Name == name);
    }
}

[System.Serializable]
public class Monologue
{
    public string Name;
    public bool displayText = false;
    [TextArea(3,100)]public string MonologueText;
    public bool WithVoiceOver = false;
    public AudioClip VoiceOverClip;
    public bool displaySprite = true;
    public List<Sprite> monologueSprite = new List<Sprite>();
    
   
    

}

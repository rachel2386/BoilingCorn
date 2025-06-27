using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    FileDataHandler fileDataHandler;

    private GameData gameData;
    public static DataPersistenceManager instance { get; private set; }
    List<IDataPersistence> dataPersistenceObjects = new List<IDataPersistence>();

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Data Persistence Manager in the scene.");
        
        }
        instance = this;
    
    }

    void Start()
    { 
       
    
    }

    public void InitializeGameDataReferences()
    {
        this.fileDataHandler = new FileDataHandler(Application.persistentDataPath,fileName);
        dataPersistenceObjects = FindAllDataPersistenceObjects();
    }

    public void NewGame()
    { 
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        //load any saved data from a file using the data handler
        gameData = fileDataHandler.Load();

        //if no data can be loaded, initialize to a new game
        if (this.gameData == null)
        {
            Debug.Log("No data was found. Initializing data to defaults");
            NewGame();
        
        }
        // push the loaded data to all other scripts that need it
        foreach (IDataPersistence dataPersistence in dataPersistenceObjects)
        {
            dataPersistence.LoadData(gameData);
        }
        
    }

    public void SaveGame()
    {
        // pass the data to other scripts so they can update it
        foreach (IDataPersistence dataPersistence in dataPersistenceObjects)
        { 
            dataPersistence.SaveData(ref gameData);
        
        }       
        // save that data to a file using the data handler
        fileDataHandler.Save(gameData);

    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceobjects = FindObjectsOfType<MonoBehaviour>()
            .OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceobjects);
    
    
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SaveManager : MonoBehaviour
{
    /*public static SaveManager Instance {get; private set;}

    private void Awake() 
    {
        //Make sure that this intance never exists twice
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            //We set the Instance to this script and now we can access it from everywhere
            Instance = this;
            //That the object that this script is attached to does not get destroyed if we load a new scene 
            DontDestroyOnLoad(gameObject);
        }
    }

    public void SaveAsPrefab(GameObject gameObjectToBecomePrefab)
    {
        string localPath = "Assets/Prefabs" + gameObjectToBecomePrefab.name + ".prefab";

        //Make sure the file name is unique, in case an existing prefab has the same name 
        localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

        //Create the new prefab 
        PrefabUtility.SaveAsPrefabAssetAndConnect(gameObjectToBecomePrefab, localPath, InteractionMode.UserAction);
    }*/
}

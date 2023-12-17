using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressablesManager : MonoBehaviour
{
    [SerializeField] private AssetReference playerClassReference;

    // Start is called before the first frame update
    void Start()
    {
        Addressables.InitializeAsync().Completed += AddressablesManager_Completed;
    }

    private void AddressablesManager_Completed(AsyncOperationHandle<IResourceLocator> obj)
    {
        Debug.Log("Initializing Addressables...");
        playerClassReference.LoadAssetAsync<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.U2D;
using UnityEngine.UI;

public class LoadDataMgs : MonoBehaviour
{
    public Image change_IMG;
    public Transform trans;

    string bundleUrl = "https://raw.githubusercontent.com/SONGPANGZHI/Test_MyAssetBundles/main/AssetBundles/Android/ui";
    //private string bundleUrl = "https://github.com/SONGPANGZHI/Test_MyAssetBundles.git";
    private string assetName = "/ui"; // AB 包内资源名
    private void Start()
    {
        ////Resources 加载---
        //ResourcesTest();

        ////本地加载 AB包
        //LocalLoadingAB();

        ////线上加载
        //StartCoroutine(OnlineLoadingAB());
    }

    //Resources 加载
    public void ResourcesTest()
    {
        Sprite restartSprite = Resources.Load<Sprite>("BTN_04");
        if (restartSprite != null)
        {
            change_IMG.sprite = restartSprite;
        }
        else
        {
            Debug.Log("GameAtlas_Restart 不存在！");
        }
    }

    //本地加载 AB包
    public void LocalLoadingAB()
    {
        // 路径拼接
        string path = Application.streamingAssetsPath + assetName;

        // 加载 AB 包
        AssetBundle bundle = AssetBundle.LoadFromFile(path);
        if (bundle == null)
        {
            Debug.LogError("AB 包加载失败！路径: " + path);
            return;
        }

        //生成
        //GameObject btn_GO = Instantiate(bundle.LoadAsset<GameObject>("BTN_03"), trans);

        // 加载资源
        Sprite restartSprite = bundle.LoadAsset<Sprite>("BTN_04");
        if (restartSprite != null)
        {
            change_IMG.sprite = restartSprite;
        }
        else
        {
            Debug.Log("GameAtlas_Restart 不存在！");
        }
    }

    //加载线上 AB包
    IEnumerator OnlineLoadingAB()
    {
        UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(bundleUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // 加载 AB 包
            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);

            // 加载资源
            Sprite restartSprite = bundle.LoadAsset<Sprite>("BTN_04");
            if (restartSprite != null)
            {
                change_IMG.sprite = restartSprite;
            }
            else
            {
                Debug.Log("GameAtlas_Restart 不存在！");
            }

        }
        else
        {
            Debug.LogError("下载失败: " + request.error);
        }
    }

    //加载依赖包
    public void LoadDependentPackages()
    {
        //加载AB包
        AssetBundle AB = AssetBundle.LoadFromFile(Application.streamingAssetsPath + assetName);

        //获取主包
        AssetBundle mainAB = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + "Android");

        //获取主包中固定文件
        AssetBundleManifest ABManifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

        //寻找依赖信息列表
        string[] packages = ABManifest.GetAllDependencies("BTN_04");

        //得到 依赖包的名字
        for (int i = 0; i < packages.Length; i++)
        {
            AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + packages[i]);
        }
    }

    public AssetBundle mainAB;
    public AssetBundleManifest manifest;
    public Dictionary<string,AssetBundle> abDict = new Dictionary<string,AssetBundle>();
    public string mainABName = "Android";

    public void LoadAB(string abName)
    {
        if (mainAB == null)
        {
            mainAB = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + mainABName);
            manifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }

        AssetBundle AB = null;

        string[] AB_MainName = manifest.GetAllDependencies(abName);

        for (int i = 0; i < AB_MainName.Length; i++)
        {
            if (!abDict.ContainsKey(AB_MainName[i]))
            {
                AB = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + AB_MainName[i]);
                abDict.Add(AB_MainName[i], AB);
            }
        }
    }

    //异步加载 
    public void LoadABAsync(string abName, string resName, UnityAction<object> ca11Back)
    {
        StartCoroutine(StartLoadABAsync(abName, resName, ca11Back));
    }

    IEnumerator StartLoadABAsync(string abName, string resName, UnityAction<object> ca11Back)
    {
        LoadAB(abName);

        //生成
        AssetBundleRequest Go = abDict[abName].LoadAssetAsync(resName);
        yield return Go;
        ca11Back(Instantiate(Go.asset));
    }

    //同步加载 根据类型
    public void LoadABAsync(string abName, string resName,System.Type TYPE, UnityAction<object> ca11Back)
    {
        StartCoroutine(StartLoadABAsync(abName, resName,TYPE, ca11Back));
    }

    IEnumerator StartLoadABAsync(string abName, string resName,System.Type TYPE, UnityAction<object> ca11Back)
    {
        LoadAB(abName);

        //生成
        AssetBundleRequest Go = abDict[abName].LoadAssetAsync(resName, TYPE);
        yield return Go;
        ca11Back(Instantiate(Go.asset));
    }

    //同步加载 泛型
    public void LoadABAsync<T>(string abName, string resName, UnityAction<object> ca11Back)
    {
        StartCoroutine(StartLoadABAsync(abName, resName,ca11Back));
    }

    IEnumerator StartLoadABAsync<T>(string abName, string resName,UnityAction<object> ca11Back)
    {
        LoadAB(abName);

        //生成
        AssetBundleRequest Go = abDict[abName].LoadAssetAsync<T>(resName);
        yield return Go;
        ca11Back(Instantiate(Go.asset));
    }


    ////同步加载 不限类型
    //public void SynchronousLoad(string abName,string resName)
    //{
    //    LoadAB(abName);

    //    //生成
    //    GameObject Go = abDict[abName].LoadAsset(resName) as GameObject;
    //    Instantiate(Go);
    //}

    ////同步加载 根据类型
    //public void SynchronousLoad(string abName, string resName,System.Type TYPE)
    //{
    //    LoadAB(abName);

    //    //生成
    //    GameObject Go = abDict[abName].LoadAsset(resName,TYPE) as GameObject;
    //    Instantiate(Go);
    //}

    ////同步加载 泛型
    //public T SynchronousLoad<T>(string abName, string resName) where T : Object
    //{
    //    LoadAB(abName);

    //    //生成
    //    T Go = abDict[abName].LoadAsset<T>(resName);
    //    Instantiate(Go);

    //    return Go;
    //}

    ////所有卸载
    //public void ClearAB()
    //{
    //    // 卸载 AB 包（false: 保留已实例化的资源；true: 彻底卸载）
    //    AssetBundle.UnloadAllAssetBundles(false);
    //    abDict.Clear();
    //    mainAB = null;
    //    manifest = null;
    //}
}

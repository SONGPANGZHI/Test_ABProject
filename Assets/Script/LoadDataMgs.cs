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
    private string assetName = "/ui"; // AB ������Դ��
    private void Start()
    {
        ////Resources ����---
        //ResourcesTest();

        ////���ؼ��� AB��
        //LocalLoadingAB();

        ////���ϼ���
        //StartCoroutine(OnlineLoadingAB());
    }

    //Resources ����
    public void ResourcesTest()
    {
        Sprite restartSprite = Resources.Load<Sprite>("BTN_04");
        if (restartSprite != null)
        {
            change_IMG.sprite = restartSprite;
        }
        else
        {
            Debug.Log("GameAtlas_Restart �����ڣ�");
        }
    }

    //���ؼ��� AB��
    public void LocalLoadingAB()
    {
        // ·��ƴ��
        string path = Application.streamingAssetsPath + assetName;

        // ���� AB ��
        AssetBundle bundle = AssetBundle.LoadFromFile(path);
        if (bundle == null)
        {
            Debug.LogError("AB ������ʧ�ܣ�·��: " + path);
            return;
        }

        //����
        //GameObject btn_GO = Instantiate(bundle.LoadAsset<GameObject>("BTN_03"), trans);

        // ������Դ
        Sprite restartSprite = bundle.LoadAsset<Sprite>("BTN_04");
        if (restartSprite != null)
        {
            change_IMG.sprite = restartSprite;
        }
        else
        {
            Debug.Log("GameAtlas_Restart �����ڣ�");
        }
    }

    //�������� AB��
    IEnumerator OnlineLoadingAB()
    {
        UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(bundleUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // ���� AB ��
            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);

            // ������Դ
            Sprite restartSprite = bundle.LoadAsset<Sprite>("BTN_04");
            if (restartSprite != null)
            {
                change_IMG.sprite = restartSprite;
            }
            else
            {
                Debug.Log("GameAtlas_Restart �����ڣ�");
            }

        }
        else
        {
            Debug.LogError("����ʧ��: " + request.error);
        }
    }

    //����������
    public void LoadDependentPackages()
    {
        //����AB��
        AssetBundle AB = AssetBundle.LoadFromFile(Application.streamingAssetsPath + assetName);

        //��ȡ����
        AssetBundle mainAB = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + "Android");

        //��ȡ�����й̶��ļ�
        AssetBundleManifest ABManifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

        //Ѱ��������Ϣ�б�
        string[] packages = ABManifest.GetAllDependencies("BTN_04");

        //�õ� ������������
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

    //�첽���� 
    public void LoadABAsync(string abName, string resName, UnityAction<object> ca11Back)
    {
        StartCoroutine(StartLoadABAsync(abName, resName, ca11Back));
    }

    IEnumerator StartLoadABAsync(string abName, string resName, UnityAction<object> ca11Back)
    {
        LoadAB(abName);

        //����
        AssetBundleRequest Go = abDict[abName].LoadAssetAsync(resName);
        yield return Go;
        ca11Back(Instantiate(Go.asset));
    }

    //ͬ������ ��������
    public void LoadABAsync(string abName, string resName,System.Type TYPE, UnityAction<object> ca11Back)
    {
        StartCoroutine(StartLoadABAsync(abName, resName,TYPE, ca11Back));
    }

    IEnumerator StartLoadABAsync(string abName, string resName,System.Type TYPE, UnityAction<object> ca11Back)
    {
        LoadAB(abName);

        //����
        AssetBundleRequest Go = abDict[abName].LoadAssetAsync(resName, TYPE);
        yield return Go;
        ca11Back(Instantiate(Go.asset));
    }

    //ͬ������ ����
    public void LoadABAsync<T>(string abName, string resName, UnityAction<object> ca11Back)
    {
        StartCoroutine(StartLoadABAsync(abName, resName,ca11Back));
    }

    IEnumerator StartLoadABAsync<T>(string abName, string resName,UnityAction<object> ca11Back)
    {
        LoadAB(abName);

        //����
        AssetBundleRequest Go = abDict[abName].LoadAssetAsync<T>(resName);
        yield return Go;
        ca11Back(Instantiate(Go.asset));
    }


    ////ͬ������ ��������
    //public void SynchronousLoad(string abName,string resName)
    //{
    //    LoadAB(abName);

    //    //����
    //    GameObject Go = abDict[abName].LoadAsset(resName) as GameObject;
    //    Instantiate(Go);
    //}

    ////ͬ������ ��������
    //public void SynchronousLoad(string abName, string resName,System.Type TYPE)
    //{
    //    LoadAB(abName);

    //    //����
    //    GameObject Go = abDict[abName].LoadAsset(resName,TYPE) as GameObject;
    //    Instantiate(Go);
    //}

    ////ͬ������ ����
    //public T SynchronousLoad<T>(string abName, string resName) where T : Object
    //{
    //    LoadAB(abName);

    //    //����
    //    T Go = abDict[abName].LoadAsset<T>(resName);
    //    Instantiate(Go);

    //    return Go;
    //}

    ////����ж��
    //public void ClearAB()
    //{
    //    // ж�� AB ����false: ������ʵ��������Դ��true: ����ж�أ�
    //    AssetBundle.UnloadAllAssetBundles(false);
    //    abDict.Clear();
    //    mainAB = null;
    //    manifest = null;
    //}
}

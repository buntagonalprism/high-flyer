using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PhotoLoader : MonoBehaviour
{

    public float padding;
    public int imgWidth;
    public int imgHeight;

    private float maxRotation = 5f;

    public GameObject imageHolderPrefab;

    private Canvas canvas;
    private RectTransform rect;
    private int rows;
    private int cols;
    private int totalPages;
    private int currentPage = 0;
    public delegate void PhotoLoadHandler(int photoPages);
    public event PhotoLoadHandler OnPhotosLoaded;

    class ImageHolder
    {
        public ImageHolder(GameObject holderObj, GameObject magnet, Image image, Text text, float centreX, float centreY)
        {
            this.holderObj = holderObj;
            this.magnet = magnet;
            this.image = image;
            this.text = text;
            this.centreX = centreX;
            this.centreY = centreY;
        }
        public GameObject holderObj;
        public GameObject magnet;
        public Image image;
        public Text text;
        public float centreX;
        public float centreY;
    }

    private List<ImageHolder> imageHolders = new List<ImageHolder>();
    private List<Texture2D> imgTextures = new List<Texture2D>();
    private List<Sprite> imgSprites = new List<Sprite>();
    private List<string> fileDescriptionList = new List<string>();
    private Dictionary<string, string> fileDescriptionMap;

    string[] files;
    string pathPreFix;

    // Use this for initialization
    private void Init()
    {
        canvas = GetComponent<Canvas>();
        rect = GetComponent<RectTransform>();
        cols = Mathf.FloorToInt(rect.rect.width / (imgWidth + 2 * padding));
        rows = Mathf.FloorToInt(rect.rect.height / (imgHeight + 2 * padding));

      


        float colWidth = rect.rect.width / ((float)cols);
        float rowHeight = rect.rect.height / ((float)rows);
        for (int col = 0; col < cols; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                float centreX = (rect.rect.width * -0.5f) + ((((float)col) + 0.5f) * colWidth);
                float centreY = (rect.rect.height * 0.5f) - ((((float)row) + 0.5f) * rowHeight);
                GameObject holderObj = Instantiate(imageHolderPrefab, transform);
                holderObj.GetComponent<RectTransform>().localPosition = new Vector3(centreX, centreY, 0f);
                imageHolders.Add(new ImageHolder(holderObj,
                    holderObj.GetComponentInChildren<MeshRenderer>().gameObject,
                    holderObj.GetComponentInChildren<Image>(),
                    holderObj.GetComponentInChildren<Text>(),
                    centreX, centreY
                    ));
            }
        }
    }

    public void LoadFolder(string path, Dictionary<string, string> fileDescriptions)
    {
        Init();
        fileDescriptionMap = fileDescriptions;
        //Change this to change pictures folder
        //string path = @"C:\Users\Alex\Pictures\TestImages\";

        //canvas = GetComponent<Canvas>();
        //rect = GetComponent<RectTransform>();
        //cols = Mathf.FloorToInt(rect.rect.width / (imgWidth + 2 * padding));
        //rows = Mathf.FloorToInt(rect.rect.height / (imgHeight + 2 * padding));




        //float colWidth = rect.rect.width / ((float)cols);
        //float rowHeight = rect.rect.height / ((float)rows);
        //for (int col = 0; col < cols; col++)
        //{
        //    for (int row = 0; row < rows; row++)
        //    {
        //        float centreX = (rect.rect.width * -0.5f) + ((((float)col) + 0.5f) * colWidth);
        //        float centreY = (rect.rect.height * 0.5f) - ((((float)row) + 0.5f) * rowHeight);
        //        GameObject holderObj = Instantiate(imageHolderPrefab, transform);
        //        holderObj.GetComponent<RectTransform>().localPosition = new Vector3(centreX, centreY, 0f);
        //        imageHolders.Add(new ImageHolder(holderObj,
        //            holderObj.GetComponentInChildren<MeshRenderer>().gameObject,
        //            holderObj.GetComponentInChildren<Image>(),
        //            holderObj.GetComponentInChildren<Text>(),
        //            centreX, centreY
        //            ));
        //    }
        //}

        pathPreFix = @"file://";

        files = System.IO.Directory.GetFiles(path.Replace("\\","/"), "*.png");
        totalPages = Mathf.CeilToInt((float)files.Length / (float)(cols * rows));

        StartCoroutine(LoadImages());
    }

    private IEnumerator LoadImages()
    {
        // Load all images in default folder as textures and apply dynamically to plane game objects.
        foreach (string tstring in files)
        {

            string pathTemp = pathPreFix + tstring;
            // WWW is generic class for loading data from a URI - so called because it is intended for web-loading, but can 
            // also be used for filesystem load 
            WWW www = new WWW(pathTemp);
            yield return www;
            Texture2D texTmp = new Texture2D(800, 600, TextureFormat.DXT1, false);
            www.LoadImageIntoTexture(texTmp);

            if (fileDescriptionMap.ContainsKey(tstring.Replace("\\","/")))
            {
                fileDescriptionList.Add(fileDescriptionMap[tstring.Replace("\\", "/")]);
            } else
            {
                //fileDescriptionList.Add(new string[] { "Giraffe", "Cow", "Horse", "Elephant", "Spider" }[Random.Range(0, 5)]);
                fileDescriptionList.Add("");
            }

            imgTextures.Add(texTmp);
            imgSprites.Add(Sprite.Create(texTmp, new Rect(0, 0, 800, 600), new Vector2(0.5f, 0.5f)));

            //gameObj[dummy].renderer.material.SetTexture("_MainTex", texTmp);
            //dummy++;
        }
        currentPage = 0;
        if (OnPhotosLoaded != null)
            OnPhotosLoaded(totalPages);
        LoadPage();
    }

    private void LoadPage()
    {
        int imgIdx = currentPage * cols * rows;
        foreach (ImageHolder holder in imageHolders)
        {
            if (imgIdx < imgSprites.Count)
            {
                holder.holderObj.SetActive(true);
                Vector3 rotation = holder.holderObj.transform.localEulerAngles;
                rotation.z = Random.Range(-maxRotation, maxRotation);
                holder.holderObj.transform.localEulerAngles = rotation;
                holder.holderObj.transform.localPosition = new Vector3(holder.centreX + Random.Range(-padding * 0.5f, padding * 0.5f), holder.centreY + Random.Range(-padding * 0.5f, padding * 0.5f), 0f);
                holder.magnet.transform.localPosition = new Vector3(Random.Range(-imgHeight * 0.07f, imgHeight * 0.07f), (0.4f * imgHeight) + Random.Range(-imgHeight * 0.07f, imgHeight * 0.07f), 0f);
                holder.image.sprite = imgSprites[imgIdx];
                holder.text.text = fileDescriptionList[imgIdx];
                imgIdx++;
            }
            else
                holder.holderObj.SetActive(false);
        }
    }
    public bool NextPage()
    {
        if (currentPage < totalPages - 1)
        {
            currentPage++;
            LoadPage();
        }
        return currentPage == totalPages - 1;
        //return false;
    }

    public int getPageCount()
    {
        return totalPages;
    }

    public bool BackPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            LoadPage();
        }
        return currentPage == 0;
        //return false;
    }
}
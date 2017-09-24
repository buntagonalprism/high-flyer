using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AnimalPhotoManager : MonoBehaviour {

    [Header("Camera Controls")]
    
    public float cameraLensDownTime;
    public float cameraLensUpTime;
    private RectTransform cameraLens;
    public int resWidth = 800;
    public int resHeight = 600;
    public int startX = 200;
    public int startY = 240;
    public float timeDialationFactor = 0.7f;

    private Camera photoCamera;

    // Camera lens controls
    private bool isLensAnimating = false;
    private bool isLensDown = false;
    private float cameraLensStartAnimTime;
    private float cameraLensAnimTime;
    private Vector2 lensUpPos = new Vector2(0, 1);
    private Vector2 lensDownPos = new Vector2(0, 0);
    private Vector3 lensUpScale = Vector3.one;
    private Vector3 lensDownScale = new Vector3(1.4f, 1.4f, 1f);
    private float baseFixedDeltaTime;

    private string folder;
    private bool photoCaptured = false;
    private Animal[] animals;

    //[Header("Photo Display")]
    //public float padding;
    //public int imgWidth;
    //public int imgHeight;
    //public GameObject imageHolderPrefab;

    private float maxRotation = 5f;

    private Canvas canvas;
    private RectTransform rect;
    private int rows;
    private int cols;
    private int totalPages;
    private int currentPage = 0;

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

    private Dictionary<string, string> animalPhotos = new Dictionary<string, string>();

    private float normWidth;
    private float normStartX;
    private float normHeight;
    private float normStartY;

    string[] files;
    string pathPreFix;
    private static AnimalPhotoManager instance;

    // Use this for initialization
    void Awake () {
        // Use static instance variable to implement singleton pattern
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
        baseFixedDeltaTime = Time.fixedDeltaTime;

        DontDestroyOnLoad(gameObject);
        OnLevelWasLoaded();
    }

    private void OnLevelWasLoaded()
    {
        // Recalculate normalised bounding rectangle
        normWidth = (float) resWidth / (float) (resWidth + startX + startX);
        normStartX = (float) startX / (float) (resWidth + startX + startX);
        normHeight = (float)resHeight / (float)(resHeight + startY + startY);
        normStartY = (float)startY / (float)(resHeight + startY + startY);

        // Might slip through depending on destroy timing
        if (instance != this)
            return;
        if (SceneManager.GetActiveScene().name == "End")
        {
            PhotoLoader loader = FindObjectOfType<PhotoLoader>();
            loader.LoadFolder(folder, animalPhotos);
        }
        else if (SceneManager.GetActiveScene().name == "Start" || (SceneManager.GetActiveScene().name == "Main" && folder == null))
        {
            folder = Application.dataPath;
            if (Application.isEditor)
            {
                // put screenshots in folder above asset path so unity doesn't index the files
                var stringPath = folder + "/..";
                folder = Path.GetFullPath(stringPath);
            }
            folder += "/screenshots/" + Guid.NewGuid();
            folder = folder.Replace("\\", "/");
            // make sure directoroy exists
            System.IO.Directory.CreateDirectory(folder);
        }
        photoCamera = GameObject.Find("Camera (eye)").GetComponent<Camera>();
        cameraLens = GameObject.Find("CameraLensPanel").GetComponent<RectTransform>();
        animals = FindObjectsOfType<Animal>();
    }

    public void Update()
    {

        // Run the lens down/up animation
        if (isLensAnimating)
        {
            AnimateCameraLens();
        }

        // Re-enable the camera lens once the phot has been captured
        if (photoCaptured)
        {
            photoCaptured = false;
            cameraLens.gameObject.SetActive(true);
        }
    }

    // Photo capture needs to happen in OnPostRender so set a flag to perform capture rather than
    // doing it here. Also hide the camera lens overlay while the picture is being taken
    public void InitiatePhotoGrab(float x, float y)
    {
        if (isLensDown && !isLensAnimating)
        {
            cameraLens.gameObject.SetActive(false);
            StartCoroutine(TakePhoto());
        }
    }

    public void ToggleCameraLens(float x, float y)
    {
        isLensDown = !isLensDown;
        isLensAnimating = true;
        cameraLensStartAnimTime = Time.time;
        cameraLensAnimTime = isLensDown ? cameraLensDownTime : cameraLensUpTime;
    }

    private void AnimateCameraLens()
    {
        float ratio = (Time.time - cameraLensStartAnimTime) / cameraLensAnimTime;
        if (isLensDown)
        {
            cameraLens.anchorMin = Vector2.Lerp(lensUpPos, lensDownPos, ratio);
            cameraLens.localScale = Vector3.Lerp(lensUpScale, lensDownScale, ratio);
            Time.timeScale = 1 - (ratio * timeDialationFactor);
            Time.fixedDeltaTime = baseFixedDeltaTime * Time.timeScale;
        }
        else
        {
            cameraLens.anchorMin = Vector2.Lerp(lensDownPos, lensUpPos, ratio);
            cameraLens.localScale = Vector3.Lerp(lensDownScale, lensUpScale, ratio);
            Time.timeScale = (1 - timeDialationFactor) + (ratio * timeDialationFactor);
            Time.fixedDeltaTime = baseFixedDeltaTime * Time.timeScale;
        }
        if (ratio > 1)
        {
            isLensAnimating = false;
            // Reset time to ensure no lerp boundary issues
            if (!isLensDown)
            {
                Time.timeScale = 1f;
                Time.fixedDeltaTime = baseFixedDeltaTime;
            }
        }
    }


    // Image captures runs in onPostRender so that we have screen pixels to grab on calling ReadPixels
    //void OnPostRender()
    //{
    //    if (shouldCapturePhoto)
    //    {
    //        shouldCapturePhoto = false;
    //        TakePhoto();
    //    }

    //}

    IEnumerator TakePhoto()
    {
        // We should only read the screen buffer after rendering is complete
        yield return new WaitForEndOfFrame();
        Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        screenShot.ReadPixels(new Rect(startX, startY, resWidth, resHeight), 0, 0);
        byte[] bytes = screenShot.EncodeToPNG();
        string filename = ScreenShotName(resWidth, resHeight);

        new System.Threading.Thread(() =>
        {
            // create file and write optional header with image bytes
            System.IO.File.WriteAllBytes(filename, bytes);
            Debug.Log(string.Format("Wrote screenshot {0} of size {1}", filename, bytes.Length));
            photoCaptured = true;
        }).Start();
        bool animalInView = false;
        Debug.Log("StartingAnimalAnalysis:" + filename);
        foreach (Animal animal in animals)
        {
            if (animalInView = IsInView(animal))
            {
                animalPhotos.Add(filename, animal.name);
                break;
            }
        }
        if (!animalInView)
            animalPhotos.Add(filename, "");
        Debug.Log("EndAnimalAnalysis:" + filename);
        yield return null;
    }

    public string ScreenShotName(int width, int height)
    {
        return string.Format("{0}/screen_{1}x{2}_{3}.png",
                             folder,
                             width, height,
                             System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    }

    private bool IsInView(Animal toCheck)
    {
        Vector3 pointOnScreen = photoCamera.WorldToScreenPoint(toCheck.renderer.bounds.center);

        //Is in front
        if (pointOnScreen.z < 0)
        {
            Debug.Log("Behind: " + toCheck.name);
            return false;
        }
        // Is too far away to see properly
        if (pointOnScreen.z > toCheck.maxVisibleRange)
        {
            Debug.Log("Too far: " + toCheck.name);
            return false;
        }


        //Is in FOV
        //if ((pointOnScreen.x < 0) || (pointOnScreen.x > Screen.width) ||
        //        (pointOnScreen.y < 0) || (pointOnScreen.y > Screen.height))
        float normX = pointOnScreen.x / photoCamera.pixelWidth;
        float normY = pointOnScreen.y / photoCamera.pixelHeight;
        if (normX < normStartX || (normX > normStartX + normWidth) ||
            normY < normStartY || (normY > normStartY + normHeight))
        {
            Debug.Log("Out of FOV: " + toCheck.name);
            return false;
        }

        RaycastHit hit;
        if (Physics.Linecast(photoCamera.transform.position, toCheck.renderer.bounds.center, out hit))
        {
            if (hit.transform.name != toCheck.name)
            {
                /* -->
                Debug.DrawLine(cam.transform.position, toCheck.GetComponentInChildren<Renderer>().bounds.center, Color.red);
                Debug.LogError(toCheck.name + " occluded by " + hit.transform.name);
                */
                Debug.Log(toCheck.name + " occluded by " + hit.transform.name);
                return false;
            }
        }
        Debug.Log("Detected: " + toCheck.name + " in view");
        return true;
    }


    //public void LoadFolder(string path)
    //{
    //    //Change this to change pictures folder
    //    //string path = @"C:\Users\Alex\Pictures\TestImages\";

    //    canvas = GetComponent<Canvas>();
    //    rect = GetComponent<RectTransform>();
    //    cols = Mathf.FloorToInt(rect.rect.width / (imgWidth + 2 * padding));
    //    rows = Mathf.FloorToInt(rect.rect.height / (imgHeight + 2 * padding));

    //    float colWidth = rect.rect.width / ((float)cols);
    //    float rowHeight = rect.rect.height / ((float)rows);
    //    for (int col = 0; col < cols; col++)
    //    {
    //        for (int row = 0; row < rows; row++)
    //        {
    //            float centreX = (rect.rect.width * -0.5f) + ((((float)col) + 0.5f) * colWidth);
    //            float centreY = (rect.rect.height * 0.5f) - ((((float)row) + 0.5f) * rowHeight);
    //            GameObject holderObj = Instantiate(imageHolderPrefab, transform);
    //            holderObj.GetComponent<RectTransform>().localPosition = new Vector3(centreX, centreY, 0f);
    //            imageHolders.Add(new ImageHolder(holderObj,
    //                holderObj.GetComponentInChildren<MeshRenderer>().gameObject,
    //                holderObj.GetComponentInChildren<Image>(),
    //                holderObj.GetComponentInChildren<Text>(),
    //                centreX, centreY
    //                ));
    //        }
    //    }

    //    pathPreFix = @"file://";

    //    files = System.IO.Directory.GetFiles(path, "*.png");
    //    totalPages = Mathf.CeilToInt((float)files.Length / (float)(cols * rows));

    //    StartCoroutine(LoadImages());
    //}


    //private IEnumerator LoadImages()
    //{
    //    // Load all images in default folder as textures and apply dynamically to plane game objects.
    //    foreach (string tstring in files)
    //    {

    //        string pathTemp = pathPreFix + tstring;
    //        // WWW is generic class for loading data from a URI - so called because it is intended for web-loading, but can 
    //        // also be used for filesystem load 
    //        WWW www = new WWW(pathTemp);
    //        yield return www;
    //        Texture2D texTmp = new Texture2D(imgWidth, imgHeight, TextureFormat.DXT1, false);
    //        www.LoadImageIntoTexture(texTmp);

    //        imgTextures.Add(texTmp);
    //        imgSprites.Add(Sprite.Create(texTmp, new Rect(0, 0, imgWidth, imgHeight), new Vector2(0.5f, 0.5f)));

    //        //gameObj[dummy].renderer.material.SetTexture("_MainTex", texTmp);
    //        //dummy++;
    //    }
    //    currentPage = 0;
    //    LoadPage();
    //}

    //private void LoadPage()
    //{
    //    int imgIdx = currentPage * cols * rows;
    //    foreach (ImageHolder holder in imageHolders)
    //    {
    //        if (imgIdx < imgSprites.Count)
    //        {
    //            holder.holderObj.SetActive(true);
    //            Vector3 rotation = holder.holderObj.transform.localEulerAngles;
    //            rotation.z = UnityEngine.Random.Range(-maxRotation, maxRotation);
    //            holder.holderObj.transform.localEulerAngles = rotation;
    //            holder.holderObj.transform.localPosition = new Vector3(holder.centreX + UnityEngine.Random.Range(-padding * 0.5f, padding * 0.5f), holder.centreY + UnityEngine.Random.Range(-padding * 0.5f, padding * 0.5f), 0f);
    //            holder.magnet.transform.localPosition = new Vector3(UnityEngine.Random.Range(-padding * 0.4f, padding * 0.4f), (0.4f * imgHeight) + UnityEngine.Random.Range(-padding * 0.4f, padding * 0.4f), 0f);
    //            holder.image.sprite = imgSprites[imgIdx];
    //            holder.text.text = files[imgIdx];
    //            imgIdx++;
    //        }
    //        else
    //            holder.holderObj.SetActive(false);
    //    }
    //}
    //public void NextPage()
    //{
    //    if (currentPage < totalPages - 1)
    //    {
    //        currentPage++;
    //        LoadPage();
    //        //return true;
    //    }
    //    //return false;
    //}

    //public void BackPage()
    //{
    //    if (currentPage > 0)
    //    {
    //        currentPage--;
    //        LoadPage();
    //        //return true;
    //    }
    //    //return false;
    //}

}

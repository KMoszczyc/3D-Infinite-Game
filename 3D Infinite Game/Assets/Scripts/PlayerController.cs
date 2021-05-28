using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public Texture xSignTexture;
    public Texture mapTexture;
    public Texture darkTexture;
    public Vector2 targetPosition;

    public AudioClip openSound;
    public AudioClip closeSound;
    public LayerMask chestLayer;

    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI chestsText;
    [SerializeField] private TextMeshProUGUI timeLeftText;
    [SerializeField] private TextMeshProUGUI timeOutText;
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private GameObject controls;
 
    [SerializeField] private GameController gameController;
    private bool isMapVisible=false;

    Vector2 middlePoint;
    Vector2 mapPosition;

    Animator animator;

    private AudioSource audioSource;

    float mapSize = 500;
    float x_sign_size = 30;
    int coins = 0;
    int chests = 0;

    float totalTime = 150;
    float timeLeft;
    float timeElapsed=0;
    float timeIncrement = 150;
    float timeStart;

    [HideInInspector] public bool isGameEnded = false;
    [HideInInspector] public bool isControlsViewOpen=false;

    void Start(){
        middlePoint = new Vector2(Screen.width/2, Screen.height/2);
        mapPosition = new Vector2(middlePoint.x-mapSize/2, middlePoint.y-mapSize/2);
        Debug.Log(mapPosition);
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();

        playAgainButton.onClick.AddListener(() => ReloadGame());
        playAgainButton.gameObject.SetActive(false);

        exitButton.onClick.AddListener(() => Application.Quit());
        exitButton.gameObject.SetActive(false);

        timeOutText.gameObject.SetActive(false);
        controls.SetActive(false);
        
        timeStart = Time.time;

        transform.position = gameController.RandomPositionOnTerrain(1);
    }

    void Update(){
        if(isControlsViewOpen) {
            timeStart+=Time.deltaTime;
            animator.SetFloat("Speed", 0);
        }

        if(!isGameEnded) {
            timeElapsed = Time.time - timeStart;
            timeLeft = totalTime - timeElapsed;
            timeLeftText.text = string.Format("{0:F1}",timeLeft) + " s";
        }


        if(timeLeft<=0){
            LostGameScreen();
        }

        if(!isGameEnded) {
            ShowMap();
            ShowSettings();
            HandleChests();
        }
    }

    void ShowMap() {
        if(Input.GetKeyDown("m")){
            if(isMapVisible){
                isMapVisible = false;
                audioSource.PlayOneShot(closeSound);
            }
            else {
                isMapVisible = true;
                audioSource.PlayOneShot(openSound);
            }
        }
    }

    void ShowSettings() {
        if(Input.GetKeyDown(KeyCode.Escape)){
            if(isControlsViewOpen){
                controls.SetActive(false);
                playAgainButton.gameObject.SetActive(false);
                exitButton.gameObject.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                isControlsViewOpen=false;
            }
            else{
                controls.SetActive(true);
                playAgainButton.gameObject.SetActive(true);
                exitButton.gameObject.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                isControlsViewOpen=true;
            }
        }
    }

    void HandleChests(){
        bool openCloseChest = Input.GetKeyDown("e");
        bool lootChest = Input.GetKeyDown("f");
        
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 3, chestLayer);
        foreach (var hitCollider in hitColliders)
        {
            ChestController chest = hitCollider.gameObject.GetComponent<ChestController>();
            chest.showText = true;

            if(openCloseChest){
                if(chest.isClosed)
                    chest.Open();
                else if(!chest.isClosed)
                    chest.Close();
            }

            if(!chest.isClosed && lootChest && !chest.isLooted) {
                coins += chest.Loot();
                chests++;
                totalTime+=timeIncrement;

                coinsText.text = coins.ToString();
                chestsText.text = chests.ToString();
                gameController.PlaceNewChestOnTerrain();
            }
        }
    }

    public Vector2 ChestPositionToMapCoords(Vector3 chestPosition){
        float x = map(chestPosition.x, 0, 1000, 0, 500);
        float y = map(chestPosition.z, 0, 1000, 500, 0);
        return new Vector2(x,y);
    }

    private float map(float value, float fromLow, float fromHigh, float toLow, float toHigh) 
    {
        return (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow;
    }

    void OnGUI()
    {   
        if(isMapVisible){
            DarkenScreen();
            // draw map
            GUI.DrawTexture(new Rect(mapPosition.x, mapPosition.y, mapSize, mapSize), mapTexture, ScaleMode.ScaleToFit);

            // draw x sign - target
            GUI.DrawTexture(new Rect(mapPosition.x - x_sign_size/2 + targetPosition.x,  mapPosition.y - x_sign_size/2 + targetPosition.y, x_sign_size, x_sign_size), xSignTexture, ScaleMode.ScaleToFit);
        }
    }

    void DarkenScreen(){
        Color originalColor = GUI.color;
        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, 0.6f);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), darkTexture, ScaleMode.StretchToFill);
        GUI.color = originalColor;  
    }

    void LostGameScreen(){
        animator.SetFloat("Speed", 0);
        Cursor.lockState = CursorLockMode.None;
        isGameEnded = true;
        playAgainButton.gameObject.SetActive(true);
        exitButton.gameObject.SetActive(true);
        timeOutText.gameObject.SetActive(true);
    }

    public void ReloadGame(){
        isGameEnded = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

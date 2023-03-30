using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MenuComponent : MonoBehaviour
{

    string[] pauseMenu = new string[3] { "OnPauseButton", "OnOptionsButton", "OnQuitButton" };
    string[] MainMenu = new string[3] { "OnPlayButton", "OnOptionsButton", "OnExitButton" };
    string[] GameOver = new string[3] { "OnPauseButton", "OnOptionsButton", "OnQuitButton" };
    string[] activeMenu;
    // Start is called before the first frame update
    [SerializeField]
    Button playAgainButton;

    public Color UITextColor = new Color(0.306f, 0.153f, 0.490f, 1.000f);
    [SerializeField] TextMeshProUGUI ScoreText;
    [SerializeField] TextMeshProUGUI[] MainMenuChoices= new TextMeshProUGUI[3];
    [SerializeField] TextMeshProUGUI[] PauseMenuChoices = new TextMeshProUGUI[3];
    [SerializeField] TextMeshProUGUI waveText;
    
    Button exitButton;
    [SerializeField]  GameObject newWaveText;

    [SerializeField]
    private GameObject DefeatHandle;

    [SerializeField]
    public GameObject pauseHandle;

    [SerializeField]
    private GameObject MenuHandle;

    private Entity player;
    private Entity gameManager;
    public static MenuComponent Instance;
    private List<World> worlds= new List<World>();
    private EntityManager _entityManager;
    private List<Entity> players;
    public bool isPlaying=true;
    bool toggle=true;
    public bool _onPause=false;
    bool Open=false;
    bool Close=true;
    public bool displayNextWave;
    int choiceMarker=0;
    private IEnumerator fadeText;
    MethodInfo methodChoice;

    float alpha=0f;
    private void Awake()
    {
        if(Instance==null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }
        
    }
    void Start()
    {
        waveText = newWaveText.GetComponentInChildren<TextMeshProUGUI>();
        MenuHandle.SetActive(true);
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }
 
    MethodInfo getSelectedFunction(string[] menuType)
    {
        MethodInfo mInfo = GetType().GetMethod(menuType[choiceMarker]);
        return mInfo;
    }
    private IEnumerator waitAndFade(float waitTime)
    {
        while (alpha < 1)
        {
            UITextColor.a = alpha;
            waveText.color = UITextColor;
            alpha += 0.05f;
            yield return new WaitForSeconds(waitTime);
        }

        while (alpha>0)
        {
           UITextColor.a = alpha;
           waveText.color = UITextColor;
           alpha -= 0.05f;
           yield return new WaitForSeconds(waitTime);
        }

        //turning off ui
        newWaveText.SetActive(false);
       

    }
    void Update()
    {
        if(displayNextWave)
        {
            displayNextWave = false;
            waveText.text = "Wave " + _entityManager.World.GetExistingSystem<EntitySpawnerSystem>().lvl.wave;
            newWaveText.SetActive(true);
            fadeText = waitAndFade(0.05f);
            StartCoroutine(fadeText);
        }
        
        if (MenuHandle.activeSelf|| pauseHandle.activeSelf)
        {
        if(MenuHandle.activeSelf)
        {
            activeMenu = MainMenu;
            for (int i = 0; i < 3; i++) { MainMenuChoices[i].color = (choiceMarker == i) ? Color.green : UITextColor; }

        }
        else if(pauseHandle.activeSelf)
        {
            activeMenu = pauseMenu;
            for (int i = 0; i < 3; i++) { PauseMenuChoices[i].color = (choiceMarker == i) ? Color.green : UITextColor; }

        }
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            choiceMarker = (choiceMarker <= 0) ? 2 : choiceMarker-1;
        

        }
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            choiceMarker = (choiceMarker >= 2) ? 0 : choiceMarker + 1;
           
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (choiceMarker < 0 || choiceMarker > 2)
            { choiceMarker = 0; }
            getSelectedFunction(activeMenu).Invoke(this, null);
        }
        }
        if(DefeatHandle.activeSelf)
        {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            OnPlayAgainButton();
        }
        }
    
        gameManager = _entityManager.World.GetExistingSystem<GameHandler>().GameHandlerEntity;
        player = _entityManager.World.GetExistingSystem<GameHandler>().playerEntity;
        if (_entityManager.HasComponent<isDeadTag>(player)){DefeatHandle.SetActive(true);}

        if (Input.GetKeyDown(KeyCode.Escape)&& isPlaying)
        {
            OnPauseButton();
        }


    }
    public void OnPauseButton()
    {
        toggle = !toggle;
        pauseHandle.SetActive(!toggle);
        _entityManager.World.GetExistingSystem<InputSystem>().Enabled = toggle;
        _entityManager.World.GetExistingSystem<EnemySystem>().Enabled = toggle;
        _entityManager.World.GetExistingSystem<EntitySpawnerSystem>().isPaused = !toggle;
        _entityManager.World.GetExistingSystem<ShootSystem>().Enabled = toggle;
    }
    public void OnPlayAgainButton()
    {
        DefeatHandle.SetActive(false);
        OnQuitButton();
        OnPlayButton(); 
    }
    public void OnOptionsButton(){}
    public void OnQuitButton()
    {
        toggle = true;
        isPlaying = false;
        _entityManager.World.GetExistingSystem<InputSystem>().Enabled = false;
        _entityManager.World.GetExistingSystem<EnemySystem>().Enabled = false;
        _entityManager.World.GetExistingSystem<EntitySpawnerSystem>().Enabled = false;
        _entityManager.World.GetExistingSystem<ShootSystem>().Enabled = false;
        _entityManager.World.GetExistingSystem<GameHandler>().Enabled = false;
        DefeatHandle.SetActive(false);
        MenuHandle.SetActive(true);
        List<Entity> enemies = _entityManager.World.GetExistingSystem<EntitySpawnerSystem>().EnemyList;
        List<Entity> ent = _entityManager.World.GetExistingSystem<ShootSystem>().EntitiesList;
       
        _entityManager.DestroyEntity(player);
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            _entityManager.DestroyEntity(enemies[i]);
        }
        for (int i = ent.Count - 1; i >= 0; i--)
        {
            _entityManager.DestroyEntity(ent[i]);
        }
  
        pauseHandle.SetActive(false);
        DefeatHandle.SetActive(false);
    }
    public void OnPlayButton()
    {
        isPlaying = true;
        MenuHandle.SetActive(false);
        _entityManager.World.GetExistingSystem<InputSystem>().Enabled = true;
        _entityManager.World.GetExistingSystem<EnemySystem>().Enabled = true;
        _entityManager.World.GetExistingSystem<EntitySpawnerSystem>().Enabled = true;
        _entityManager.World.GetExistingSystem<ShootSystem>().Enabled = true;
        _entityManager.World.GetExistingSystem<GameHandler>().Enabled = true;
    }
    public void OnExitButton(){Application.Quit();}
    public void UpdateScore(int score)
    {
        ScoreText.text = "Score:" + score.ToString();
    }
}

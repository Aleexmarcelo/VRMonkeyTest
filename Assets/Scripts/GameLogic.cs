using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.UI;
public class GameLogic : MonoBehaviour {

    public enum GameStates { gameplay,paused,messageBox,equipment,cutsceneFree,cutsceneHold,checkingObject,death,saving}
    public GameStates gameState= GameStates.gameplay;
    public AudioSource musicSource;
    public bool enableStamina = false;
    bool freezeFrame = false;
    public GameObject healthPickupSpawner;
    public GameObject keyObject;
    public float previousTimeScale = 1;
    StealthPlayerController player;

    public int keys = 0;
    Vector3 currentCheckpointPosition;

    public Text textBoxText;
    public GameObject textBox;

    public BatteryPickup[] batteries;
    public AIAgent[] enemies;

    public GameObject ShockText;
    public GameObject CloakText;
    public GameObject DrainText;
    public GameObject hoverText;
    public GameObject shootText;

    public GameObject enemyContainer;
    public GameObject enemyModel;
    //Here is a private reference only this class can access
    private static GameLogic _instance;

    int chasingEnemies = 0;

    public AudioSource calmMusicSource;
    public AudioSource chaseMusicSource;

    public AudioSource playerWheelSource;
    float wheelSound;

    public GameObject keyBoardKeys;
    public GameObject gamepadKeys;

    public Text keyText;


    public void FreezeFrame()
    {
        StartCoroutine(FreezeFrameRoutine());
    }

    IEnumerator FreezeFrameRoutine()
    {
        Debug.Log("FreezeFrame");
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(0.05f);
        Time.timeScale = 1;
    }

    public void AddChaser()
    {
        chasingEnemies++;

        Debug.Log("Chasers " + chasingEnemies);

        calmMusicSource.Pause();
        if (!chaseMusicSource.isPlaying)
        {
            chaseMusicSource.Play();
        }
        
    }

    public void RemoveChaser()
    {
        chasingEnemies--;
        if (chasingEnemies < 0)
        {
            chasingEnemies = 0;
        }
        Debug.Log("Chasers " + chasingEnemies);

        if (chasingEnemies == 0)
        {
            calmMusicSource.UnPause();
            chaseMusicSource.Stop();
        }
    }

	//This is the public reference that other classes will use
	public static GameLogic instance
	{
		get
		{
			//If _instance hasn't been set yet, we grab it from the scene!
			//This will only happen the first time this reference is used.
			if(_instance == null)
				_instance = GameObject.FindObjectOfType<GameLogic>();
			return _instance;
		}
	}

    [ContextMenu("GetStuff")]
    public void GetBatteries()
    {
        batteries = GameObject.FindObjectsOfType<BatteryPickup>();
        enemies = GameObject.FindObjectsOfType<AIAgent>();
    }

    public void Start()
    {
        if (Input.GetJoystickNames().Length == 0)
        {
            keyBoardKeys.SetActive(true);
            gamepadKeys.SetActive(false);
        }
        else
        {
            keyBoardKeys.SetActive(false);
            gamepadKeys.SetActive(true);
        }


        wheelSound = playerWheelSource.volume;
        if (batteries == null)
        {
            batteries = GameObject.FindObjectsOfType<BatteryPickup>();
        }
        if (enemies == null)
        {
            enemies = GameObject.FindObjectsOfType<AIAgent>();
        }

        player = StealthPlayerController.getInstance();
        currentCheckpointPosition = player.transform.position;
        
        switch (SceneManager.GetActiveScene().name)
        {
            case "gameSceneReversed":
                EnablePlayerSkill(Upgrade.Type.shock, false);
                EnablePlayerSkill(Upgrade.Type.cloak, false);
                EnablePlayerSkill(Upgrade.Type.drain, false);
                break;
        }
    }

    public void SetCheckpoint(Vector3 position)
    {
        currentCheckpointPosition = position;
        ConsoleText.getInstance().ShowMessage("Checkpoint Reached!");
        player.ResetEnergy();
    }

    public void GameOver()
    {
        chasingEnemies = 0;
        calmMusicSource.UnPause();
        chaseMusicSource.Stop();
        player.transform.position = currentCheckpointPosition;
        player.rb.velocity = Vector3.zero;
        player.ResetEnergy();
        player.warpParticles.Play();
        GameObject.Destroy(enemyContainer);
        enemyContainer= GameObject.Instantiate(enemyModel);

        for(int i = 0; i < batteries.Length; i++)
        {
            batteries[i].gameObject.SetActive(true);
        }
        /*
        for(int i = 0; i < enemies.Length; i++)
        {
            enemies[i].Restart();
        }
        */
    }

    public void useKey()
    {
        keys--;
        Debug.Log("using key, remaining " + keys);
        keyText.text = "x" + keys;
    }

    public void addKey()
    {
        keys++;
        keyText.text = "x" + keys;
        ConsoleText.getInstance().ShowMessage("Single use keycard acquired");
    }

   public void EnablePlayerSkill(Upgrade.Type upgradeType, bool showText = true)
    {
        switch (upgradeType)
        {
            case Upgrade.Type.shock:
                player.canShock = true;
                ShockText.SetActive(true);
                break;
            case Upgrade.Type.cloak:
                player.canCloak = true;
                CloakText.SetActive(true);
                break;
            case Upgrade.Type.drain:
                player.canDrain = true;
                DrainText.SetActive(true);
                break;
            case Upgrade.Type.hover:
                player.canHoverUpgrade = true;
                hoverText.SetActive(true);
                break;
            case Upgrade.Type.shoot:
                player.canShootUpgrade = true;
                shootText.SetActive(true);
                break;
        }

        StartCoroutine(EnablePlayerSkillRoutine(upgradeType, showText));
    }

    IEnumerator EnablePlayerSkillRoutine(Upgrade.Type upgradeType, bool showText)
    {
        yield return new WaitForSeconds(0.5f);
        if (Input.GetJoystickNames().Length == 0)
        {
            switch (upgradeType)
            {
                case Upgrade.Type.shock:

                    if (showText)
                    {
                        ShowMessageBox("You acquired the SHOCK power! Press X to stun enemies!");
                    }

                    break;
                case Upgrade.Type.cloak:
                    if (showText)
                    {
                        ShowMessageBox("You acquired the CLOAK power! Press C to become invisible!");
                    }

                    break;
                case Upgrade.Type.drain:
                    if (showText)
                    {
                        ShowMessageBox(
                            "You acquired the DRAIN power! Press V to drain enemies' energy! ONLY WORKS ON UNAWARE ENEMIES");
                    }

                    player.canDrain = true;
                    break;
                case Upgrade.Type.hover:
                    if (showText)
                    {
                        ShowMessageBox(
                            "You acquired the HOVER power! Press S to Hover! While in Hover mode, you can move and enemies can't see or harm you!");
                    }

                    player.canHoverUpgrade = true;
                    break;
                case Upgrade.Type.shoot:
                    if (showText)
                    {
                        ShowMessageBox(
                            "You acquired the SHOT power! Press A to Shoot! You can disable enemies and your shots go through walls!");
                    }

                    player.canShootUpgrade = true;
                    break;
            }
        }
        else
        {
            switch (upgradeType)
            {
                case Upgrade.Type.shock:

                    if (showText)
                    {
                        ShowMessageBox("You acquired the SHOCK power! Press X to stun enemies!");
                    }

                    break;
                case Upgrade.Type.cloak:
                    if (showText)
                    {
                        ShowMessageBox("You acquired the CLOAK power! Press C to become invisible!");
                    }

                    break;
                case Upgrade.Type.drain:
                    if (showText)
                    {
                        ShowMessageBox(
                            "You acquired the DRAIN power! Press V to drain enemies' energy! ONLY WORKS ON UNAWARE ENEMIES");
                    }

                    player.canDrain = true;
                    break;
                case Upgrade.Type.hover:
                    if (showText)
                    {
                        ShowMessageBox(
                            "You acquired the HOVER power! Press S to Hover! While in Hover mode, you can move and enemies can't see or harm you!");
                    }

                    player.canHoverUpgrade = true;
                    break;
                case Upgrade.Type.shoot:
                    if (showText)
                    {
                        ShowMessageBox(
                            "You acquired the SHOT power! Press A to Shoot! You can disable enemies and your shots go through walls!");
                    }

                    player.canShootUpgrade = true;
                    break;
            }
        }
    }
    bool unpausesound = false;
    public void ShowMessageBox(string text)
    {
        if (playerWheelSource.isPlaying)
        {
            playerWheelSource.Pause();
            unpausesound = true;
        }
        Time.timeScale = 0;
        gameState = GameStates.messageBox;
        textBoxText.text = text;
        textBox.SetActive(true);
    }

    public void Update()
    {
        if(gameState==GameStates.messageBox && Input.GetButtonDown("Run"))
        {
            textBox.SetActive(false);
            gameState = GameStates.gameplay;
            Time.timeScale = 1;
            if (unpausesound)
            {
                unpausesound = false;
                playerWheelSource.UnPause();
            }
        }
    }

    public void EndGame(string nextScene)
    {
        StartCoroutine(EndGameRoutine(nextScene));
    }

    IEnumerator EndGameRoutine(string nextScene)
    {
        yield return new WaitForSeconds(1.0f);
        player.warpParticles.Play();
        yield return new WaitForSeconds(2.0f);
        HUDManager.instance.FadeInWhite(1.0f);
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene(nextScene);
    }
}

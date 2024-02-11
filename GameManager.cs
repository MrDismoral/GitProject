using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public float levelStartDelay = 2f; //2 sec pause before starting next level //�������� � 2 ������� ����� �������� ������� ����
    public float turnDelay = 1f; //delay between player turns //�������� �� ������� ������
    public static GameManager instance = null; //to get access to public functions and variables of "GameManager" class from any other game script //��� ��������� ������� �� �������� ������� �� ������ ��������� "GameManager" ����-���� ����� ��������
    public BoardManager boardScript; //variable to store a reference for "BoardManager" class //����� ��� ���������� ��������� �� ���� "BoardManager"
    public int playerFoodPoints = 100; //player start score //�������� ���� ��������� ������
    [HideInInspector] public bool playersTurn = true; //to keep track of turns //��� ����������� �� �������

    private Text levelText;
    private GameObject levelImage;
    private int level = 1;
    private List<Enemy> enemies;
    private bool enemiesMoving;
    private bool doingSetup; //to prevent player to move during setup //���������� ���� ������ �� ��� ��������� ����

    void Awake()
    {
        if (instance == null) //to make sure that there is only one instance of GameManager //��� ����������, �� ��� �������� ���� ���� ��������� ���� GameManager
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject); //to make the GameManager will persist between levels to not to lose the player score //������� ��� ���������� ��'���� GameManager � ������� �� ���������� ����� ��� ���� ��� �������� ���� ��� ������� "���� ���������" ������
        enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManager>(); //get and store a component reference //��� ��������� �� ���������� ��������� �� ���������
        InitGame();
    }

    private void OnLevelWasLoaded(int index) //called every time the scene is loaded //����������� ������ �� ������������� �����
    {
        level++;
        InitGame();
    }

    void InitGame() //to setup the scene //��� ������������ �����
    {
        //UI //�������������� ���������
        doingSetup = true;
        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = "Day " + level;
        levelImage.SetActive(true);
        
        Invoke("HideLevelImage", levelStartDelay); //to hide UI //������� ���������

        enemies.Clear(); //to clear the scene from present enemies //������� ����� �� ������
        boardScript.SetupScene(level); //calling the "SetupScene" function from "BoardManager" script and passing the "level" parameter //������ ������� "SetupScene" � ������� "BoardManager" �� �������� ��������� "�����"
    }

    private void HideLevelImage()
    {
        levelImage.SetActive(false); //to hide UI image //������� ���������� ����������
        doingSetup = false; //setup is done, now the player can move //������������ ���������, ����� ������� ���� ��������
    }

    public void GameOver()
    {
        levelText.text = "You`ve survived for " + level + " days"; //last message with player stats //������� ������ ����������� � ����������� ������
        levelImage.SetActive(true); //shows up the UI image //������ ���������� ����������
        enabled = false; //ends the game session //��������� ������� ���
    }

    void Update()
    {
        if (playersTurn || enemiesMoving || doingSetup) //condition when the player still can move or enemies have moved, or scene setup not complete //����� ���� ������� ���� �������� ��� ����� ��� �������, ��� ������������ ����� �� ����������
            return; //ignores the code below //������ ��� �����

        StartCoroutine(MoveEnemies()); //enemies movement //��� ������
    }

    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }

    IEnumerator MoveEnemies()
    {
        enemiesMoving = true; //enemy can move //����� ���� ��������
        yield return new WaitForSeconds(turnDelay); //waits for turn delay //���������� �������� ����� ������
        if (enemies.Count == 0) //condition where there is no enemies on map /����� ���� �� ��� ������� ������
            yield return new WaitForSeconds(turnDelay); //still waits for turn delay //��� ���� �������� ����� ������

        for(int i = 0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy(); //calls a fuction to move for each enemy on the map //������ ������� ���� ������� ������ �� ����
            yield return new WaitForSeconds(enemies[i].moveTime);
        }

        playersTurn = true; //players turn //���� ������
        enemiesMoving = false; //enemies cannot move anymore //������ ����� �� ������ ��������
    }
}

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour
{
    private CubePos nowCube = new CubePos(0, 1, 0);
    public float cubeChangePlaceSpeed = 0.5f;
    public Transform cubeToPlace;
    public GameObject[] cubesToCreate;
    public GameObject allCubes, vfx;
    public GameObject[] canvasStartPage;
    public Text scoreTxt;
    private Rigidbody allCubesRB;
    private bool IsLose, firstCube;
    private Coroutine showCubePlace;
    private float camMoveToYPosition, camMoveSpeed = 2f;
    public Color[] bgColor;
    private Color toCameraColor;
    private List<GameObject> posibleCubesToCreate = new List<GameObject>();


    private List<Vector3> allCubesPositions = new List<Vector3>
    {
        new Vector3(0,0,0),
        new Vector3(1,0,0),
        new Vector3(-1,0,0),
        new Vector3(0,1,0),
        new Vector3(0,0,1),
        new Vector3(0,0,-1),
        new Vector3(1,0,1),
        new Vector3(-1,0,1),
        new Vector3(1,0,-1),
    };

    private Transform mainCam;
    private int prevCountMaxHorizontal;

    private void Start()
    {

        if (PlayerPrefs.GetInt("score") < 5)
            posibleCubesToCreate.Add(cubesToCreate[0]);
        else if (PlayerPrefs.GetInt("score") < 10)
            AddPosibleCubes(2);
        else if (PlayerPrefs.GetInt("score") < 20)
            AddPosibleCubes(3);
        else if (PlayerPrefs.GetInt("score") < 30)
            AddPosibleCubes(4);
        else if (PlayerPrefs.GetInt("score") < 40)
            AddPosibleCubes(5);
        else if (PlayerPrefs.GetInt("score") < 50)
            AddPosibleCubes(6);
        else if (PlayerPrefs.GetInt("score") < 60)
            AddPosibleCubes(7);
        else if (PlayerPrefs.GetInt("score") < 70)
            AddPosibleCubes(8);
        else if (PlayerPrefs.GetInt("score") < 80)
            AddPosibleCubes(9);
        else if (PlayerPrefs.GetInt("score") < 90)
            AddPosibleCubes(10);
        else if (PlayerPrefs.GetInt("score") < 100)
            AddPosibleCubes(11);
        else
            AddPosibleCubes(12);

        scoreTxt.text = $"<color=#D2421C>Best: {PlayerPrefs.GetInt("score")}</color>\n<size=40>Now:</size> 0";
        toCameraColor = Camera.main.backgroundColor;
        mainCam = Camera.main.transform;
        camMoveToYPosition = 6f + nowCube.y - 1f;

        allCubesRB = allCubes.GetComponent<Rigidbody>();
        showCubePlace = StartCoroutine(ShowCubePlace());
    }

    private void Update()
    {
        if ((Input.GetMouseButtonDown(0) || Input.touchCount > 0)
            && cubeToPlace != null && allCubes != null
            && !EventSystem.current.IsPointerOverGameObject())
        {
#if !UNITY_EDITOR
            if (Input.GetTouch(0).phase != TouchPhase.Began)
                return;
#endif


            if (!firstCube)
            {
                firstCube = true;
                foreach (GameObject obj in canvasStartPage)
                {
                    Destroy(obj);
                }
            }

            GameObject createCube = null;
            if (posibleCubesToCreate.Count == 1)
                createCube = posibleCubesToCreate[0];
            else
                createCube = posibleCubesToCreate[UnityEngine.Random.Range(0, posibleCubesToCreate.Count)];

            GameObject newCube = Instantiate(
                createCube,
                cubeToPlace.position,
                Quaternion.identity);

            newCube.transform.SetParent(allCubes.transform);
            nowCube.setVector(cubeToPlace.position);
            allCubesPositions.Add(nowCube.getVector());

            if (PlayerPrefs.GetString("music") != "No")
                GetComponent<AudioSource>().Play();

            GameObject newVfx = Instantiate(vfx, newCube.transform.position, Quaternion.identity);
            Destroy(newVfx, 1.5f);

            allCubesRB.isKinematic = true;
            allCubesRB.isKinematic = false;

            SpawnPositions();
            MoveCameraChangeBg();
        }

        if (!IsLose && allCubesRB.velocity.magnitude > 0.1f)
        {
            Destroy(cubeToPlace.gameObject);
            IsLose = true;
            StopCoroutine(showCubePlace);
        }

        mainCam.localPosition = Vector3.MoveTowards(mainCam.localPosition,
            new Vector3(mainCam.localPosition.x, camMoveToYPosition, mainCam.localPosition.z),
            camMoveSpeed * Time.deltaTime);

        if (Camera.main.backgroundColor != toCameraColor)
            Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor,
                toCameraColor,
                Time.deltaTime / 1.5f);
    }

    IEnumerator ShowCubePlace()
    {
        while (true)
        {
            SpawnPositions();
            yield return new WaitForSeconds(cubeChangePlaceSpeed);
        }
    }
    private void SpawnPositions()
    {
        List<Vector3> positions = new List<Vector3>();
        if (IsPositionEmpty(new Vector3(nowCube.x + 1, nowCube.y, nowCube.z))
            && nowCube.x + 1 != cubeToPlace.position.x)
            positions.Add(new Vector3(nowCube.x + 1, nowCube.y, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x - 1, nowCube.y, nowCube.z))
            && nowCube.x - 1 != cubeToPlace.position.x)
            positions.Add(new Vector3(nowCube.x - 1, nowCube.y, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y + 1, nowCube.z))
            && nowCube.y + 1 != cubeToPlace.position.y)
            positions.Add(new Vector3(nowCube.x, nowCube.y + 1, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y - 1, nowCube.z))
            && nowCube.y - 1 != cubeToPlace.position.y)
            positions.Add(new Vector3(nowCube.x, nowCube.y - 1, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y, nowCube.z + 1))
            && nowCube.z + 1 != cubeToPlace.position.z)
            positions.Add(new Vector3(nowCube.x, nowCube.y, nowCube.z + 1));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y, nowCube.z - 1))
            && nowCube.z - 1 != cubeToPlace.position.z)
            positions.Add(new Vector3(nowCube.x, nowCube.y, nowCube.z - 1));

        if (positions.Count > 1)
            cubeToPlace.position = positions[UnityEngine.Random.Range(0, positions.Count)];
        else if (positions.Count == 0)
            IsLose = true;
        else
            cubeToPlace.position = positions[0];
    }

    private bool IsPositionEmpty(Vector3 targetPos)
    {
        if (targetPos.y == 0)
            return false;

        foreach (Vector3 pos in allCubesPositions)
        {
            if (pos.x == targetPos.x && pos.y == targetPos.y && pos.z == targetPos.z)
                return false;
        }
        return true;
    }
    private void MoveCameraChangeBg()
    {
        int maxX = 0, maxY = 0, maxZ = 0, maxHor;

        foreach (Vector3 pos in allCubesPositions)
        {
            if (Mathf.Abs(Convert.ToInt32(pos.x)) > maxX)
                maxX = Convert.ToInt32(pos.x);

            if (Convert.ToInt32(pos.y) > maxY)
                maxY = Convert.ToInt32(pos.y);

            if (Mathf.Abs(Convert.ToInt32(pos.z)) > maxZ)
                maxZ = Convert.ToInt32(pos.z);
        }

        if (PlayerPrefs.GetInt("score") < maxY - 1)
            PlayerPrefs.SetInt("score", maxY - 1);

        scoreTxt.text = $"<color=#D2421C>Best: {PlayerPrefs.GetInt("score")}</color>\n<size=40>Now:</size> {maxY - 1}";

        camMoveToYPosition = 6f + nowCube.y - 1f;

        maxHor = maxX > maxZ ? maxX : maxZ;
        if (maxHor % 2 == 0 && prevCountMaxHorizontal != maxHor)
        {
            mainCam.localPosition += new Vector3(0, 0, -3f);
            prevCountMaxHorizontal = maxHor;
        }

        if (maxY >= 110)
            toCameraColor = bgColor[11];
        else if (maxY >= 100)
            toCameraColor = bgColor[10];
        else if (maxY >= 90)
            toCameraColor = bgColor[9];
        else if (maxY >= 80)
            toCameraColor = bgColor[8];
        else if (maxY >= 70)
            toCameraColor = bgColor[7];
        else if (maxY >= 60)
            toCameraColor = bgColor[6];
        else if (maxY >= 50)
            toCameraColor = bgColor[5];
        else if (maxY >= 40)
            toCameraColor = bgColor[4];
        else if (maxY >= 30)
            toCameraColor = bgColor[3];
        else if (maxY >= 20)
            toCameraColor = bgColor[2];
        else if (maxY >= 10)
            toCameraColor = bgColor[1];
        else if (maxY >= 5)
            toCameraColor = bgColor[0];
    }

    private void AddPosibleCubes(int till)
    {
        for (int i = 0; i < till; i++)
        {
            posibleCubesToCreate.Add(cubesToCreate[i]);
        }
    }



}

struct CubePos
{
    public int x, y, z;
    public CubePos(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3 getVector()
    {
        return new Vector3(x, y, z);
    }

    public void setVector(Vector3 pos)
    {
        x = Convert.ToInt32(pos.x);
        y = Convert.ToInt32(pos.y);
        z = Convert.ToInt32(pos.z);
    }

}

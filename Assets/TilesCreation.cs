using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TilesCreation : MonoBehaviour
{
    [SerializeField] GameObject greenTile;
    [SerializeField] GameObject redTile;
    [SerializeField] GameObject blueTile;
    [SerializeField] GameObject blackTile;
    [SerializeField] GameObject colorSignRed;
    [SerializeField] GameObject colorSignBlue;
    [SerializeField] GameObject colorSignGreen;
    
    private GameObject empty;
    private List<GameObject> tiles = new List<GameObject>();
    private List<GameObject> firstCol = new List<GameObject>();
    private List<GameObject> secondCol = new List<GameObject>();
    private List<GameObject> thirdCol = new List<GameObject>();

    private Vector2[,] fieldGrid;

    public float movingDistance;
    public Vector2 startingPoint;
    public Vector2 rect;

    public Vector2[,] CreateGrid(){
        rect = new Vector2();

        float spaceInBetween = 1 / 5f;
        float topBottomSpace = 1 / 2f;
        float colorSignSize = 1 / 2f;
        float signToTilesSpace = 1 / 3f;

        Vector2 [,] fieldGrid = new Vector2[5,6];

        Camera cam = Camera.main;
        float newScale = 0f;
        float smallerSide = (cam.orthographicSize<cam.orthographicSize*cam.aspect)?cam.orthographicSize:cam.orthographicSize*cam.aspect;
        if (cam.orthographicSize<=cam.orthographicSize*cam.aspect){
            newScale = cam.orthographicSize * 2 / (5 + spaceInBetween * 4 + topBottomSpace * 2 + colorSignSize + signToTilesSpace);
            rect.y = cam.orthographicSize*2;
            rect.x = newScale * (5 + spaceInBetween * 4 + topBottomSpace * 2);
        }else{
            newScale = cam.orthographicSize * cam.aspect * 2 / (5 + spaceInBetween * 4 + topBottomSpace * 2);
            rect.x = cam.orthographicSize * cam.aspect * 2;
            rect.y = newScale * (5 + spaceInBetween * 4 + topBottomSpace * 2 + colorSignSize + signToTilesSpace);
        }
        
        greenTile.transform.localScale = new Vector3(newScale, newScale, newScale);
        redTile.transform.localScale = new Vector3(newScale, newScale, newScale);
        blueTile.transform.localScale = new Vector3(newScale, newScale, newScale);
        blackTile.transform.localScale = new Vector3(newScale, newScale, newScale);
        colorSignRed.transform.localScale = new Vector3(newScale * colorSignSize, newScale * colorSignSize, newScale * colorSignSize);
        colorSignBlue.transform.localScale = new Vector3(newScale * colorSignSize, newScale * colorSignSize, newScale * colorSignSize);
        colorSignGreen.transform.localScale = new Vector3(newScale * colorSignSize, newScale * colorSignSize, newScale * colorSignSize); 
    
        float startingXpoint = cam.transform.position.x - rect.x/2 + newScale/2 + topBottomSpace * newScale;
        float startingYpoint = cam.transform.position.y + rect.y/2 - newScale * colorSignSize/2 - topBottomSpace * newScale;
        startingPoint = new Vector2(startingXpoint, startingYpoint);
        
        for (int i = 0; i < 5; i++){
            fieldGrid[i,0] = new Vector2(startingXpoint + newScale * i * (1 + spaceInBetween), startingYpoint);
            fieldGrid[i,1] = new Vector2(startingXpoint + newScale * i * (1 + spaceInBetween), startingYpoint - newScale * colorSignSize/2 - newScale * signToTilesSpace - newScale / 2);
            fieldGrid[i,2] = new Vector2(startingXpoint + newScale * i * (1 + spaceInBetween), fieldGrid[i,1].y - newScale - newScale*spaceInBetween);
            fieldGrid[i,3] = new Vector2(startingXpoint + newScale * i * (1 + spaceInBetween), fieldGrid[i,2].y - newScale - newScale*spaceInBetween);
            fieldGrid[i,4] = new Vector2(startingXpoint + newScale * i * (1 + spaceInBetween), fieldGrid[i,3].y - newScale - newScale*spaceInBetween);
            fieldGrid[i,5] = new Vector2(startingXpoint + newScale * i * (1 + spaceInBetween), fieldGrid[i,4].y - newScale - newScale*spaceInBetween);
        }

        movingDistance = newScale * spaceInBetween + newScale;
        
        return fieldGrid;
    }

    public void CreateTiles(){
        empty = new GameObject();     
        
        CreateAndAddTilesToTheList(tiles, firstCol, 1, colorSignGreen);
        CreateAndAddTilesToTheList(tiles, firstCol, 5, greenTile);

        AddSecondAndForthCol(tiles, blackTile);

        CreateAndAddTilesToTheList(tiles, secondCol, 1, colorSignBlue);
        CreateAndAddTilesToTheList(tiles, secondCol, 5, blueTile);

        AddSecondAndForthCol(tiles, blackTile);

        CreateAndAddTilesToTheList(tiles, thirdCol, 1, colorSignRed);
        CreateAndAddTilesToTheList(tiles, thirdCol, 5, redTile);

        foreach (GameObject tile in tiles){
            Debug.Log(tile.name);
        }

        DisableLeftovers();
    }

    public void FillTheGrid(Vector2[,] grid){
        int k = 0;
        for (int i = 0; i < grid.GetLength(0); i++){
            for (int j = 0; j < grid.GetLength(1); j++){
                tiles[k].transform.position = grid[i,j];
                k++;
            }
        }
    }

    public void ShuffleTiles(){
        List<GameObject> shuffledList = new List<GameObject>();
        shuffledList.InsertRange(0,firstCol);
        shuffledList.RemoveAt(0);
        shuffledList.InsertRange(0,secondCol);
        shuffledList.RemoveAt(0);
        shuffledList.InsertRange(0,thirdCol);
        shuffledList.RemoveAt(0);
        shuffledList = shuffledList.OrderBy( x => Random.value ).ToList( );

        int j = 0;
        for (int i = 0; i < tiles.Count; i++){
            if (tiles[i].layer != 6) continue;

            tiles[i] = shuffledList[j];
            j++;
        }
    }

    private void CreateAndAddTilesToTheList(List<GameObject> list, List<GameObject> list2, int n, GameObject tile){
        for (int i = 0; i < n; i++){
            GameObject newTile = Instantiate(tile, tile.transform.position, tile.transform.rotation);
            list.Add(newTile);
            list2.Add(newTile);
        }
    }


    private void AddSecondAndForthCol(List<GameObject> list, GameObject tile){
        for (int i = 0; i < 3; i++){
            GameObject newEmpty = Instantiate(empty, transform.position, Quaternion.identity);
            tiles.Add(newEmpty);
            GameObject newTile = Instantiate(tile, tile.transform.position, Quaternion.identity);
            tiles.Add(newTile);
        }
    }

    private void DisableLeftovers(){
        greenTile.SetActive(false);
        blueTile.SetActive(false);
        redTile.SetActive(false);
        colorSignBlue.SetActive(false);
        colorSignGreen.SetActive(false);
        colorSignRed.SetActive(false);
        blackTile.SetActive(false);
        empty.SetActive(false);
    }

    public List<GameObject> GetFirstCol(){
        return firstCol;
    }

    public List<GameObject> GetSecondCol(){
        return secondCol;
    }

    public List<GameObject> GetThirdCol(){
        return thirdCol;
    }
}

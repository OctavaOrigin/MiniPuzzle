using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTiles : MonoBehaviour
{
    TilesCreation tilesCreation;
    private Vector2[,] grid;
    private GameObject selectedTile;
    private Vector2 prevClickPos;
    private float movingDistance;
    private Vector2 rect;
    private Vector2 startCheckingPos;
    private List<GameObject> firstCol;
    private List<GameObject> secondCol;
    private List<GameObject> thirdCol;
    private CanvasMenu canvasMenu;
    private bool stopGame;

    void Start(){
        tilesCreation = GetComponent<TilesCreation>();
        grid = tilesCreation.CreateGrid();
        tilesCreation.CreateTiles();
        tilesCreation.ShuffleTiles();
        tilesCreation.FillTheGrid(grid);
        movingDistance = tilesCreation.movingDistance;
        rect = tilesCreation.rect;
        startCheckingPos = tilesCreation.startingPoint;
        firstCol = tilesCreation.GetFirstCol();
        secondCol = tilesCreation.GetSecondCol();
        thirdCol = tilesCreation.GetThirdCol();
        canvasMenu = FindObjectOfType<CanvasMenu>();
        stopGame = false;
    }

    void Update(){
        if (stopGame) return;

        if (Input.GetMouseButton(0)){ 
            Play();
        }
        if (Input.GetMouseButtonUp(0)){
            if (selectedTile != null)
                TryMoving();
            selectedTile = null;
        }
    }

    private void Play(){
        Vector3 clickpos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 click2D = new Vector2(clickpos.x, clickpos.y);
 
        RaycastHit2D hit = Physics2D.Raycast(click2D, Vector2.zero);

        if (hit.collider == null) return;
        if (selectedTile != null) return;

        if (hit.transform.gameObject.layer == 6){
            selectedTile = hit.transform.gameObject;
            prevClickPos = selectedTile.transform.position;
        }
    }

    private void TryMoving(){
        Vector3 clickpos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 currentClickPos = new Vector2(clickpos.x, clickpos.y);

        Vector2 direction = currentClickPos - prevClickPos;
        if (direction.magnitude < 0.02f){
            // no direcion
        }
        RaycastHit2D hit = new RaycastHit2D();
        Vector2 newLocation = new Vector2();
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y) && direction.x > 0)
        {
            hit = Physics2D.Raycast(selectedTile.transform.position + new Vector3(movingDistance, 0, 0), Vector2.zero);
            if (hit.collider == null) newLocation = selectedTile.transform.position + new Vector3(movingDistance, 0, 0);
        }
        else if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y) && direction.x < 0)
        {
            hit = Physics2D.Raycast(selectedTile.transform.position + new Vector3(-movingDistance, 0, 0), Vector2.zero);
            if (hit.collider == null) newLocation = selectedTile.transform.position + new Vector3(-movingDistance, 0, 0);
        }
        else if (Mathf.Abs(direction.x) < Mathf.Abs(direction.y) && direction.y > 0)
        {
            hit = Physics2D.Raycast(selectedTile.transform.position + new Vector3(0, movingDistance, 0), Vector2.zero);
            if (hit.collider == null) newLocation = selectedTile.transform.position + new Vector3(0, movingDistance, 0);
        }
        else if (Mathf.Abs(direction.x) < Mathf.Abs(direction.y) && direction.y < 0)
        {
            hit = Physics2D.Raycast(selectedTile.transform.position + new Vector3(0, -movingDistance, 0), Vector2.zero);
            if (hit.collider == null) newLocation = selectedTile.transform.position + new Vector3(0, -movingDistance, 0);
        }

        if (hit.collider != null) return;

        if (Mathf.Abs(newLocation.x) > rect.x/2 || Mathf.Abs(newLocation.y) > rect.y/2)
            return;

        selectedTile.transform.position = newLocation;
        CheckforComplition();
    }

    private void CheckforComplition(){
        float pos = 0f;
        foreach(GameObject tile in firstCol){
            float distance = Mathf.Abs(tile.transform.position.x - pos);
            if (distance < 0.002 || pos == 0){
                pos = tile.transform.position.x;
            }else{
                return;
            }
        }
        pos = 0f;
        foreach(GameObject tile in secondCol){
            float distance = Mathf.Abs(tile.transform.position.x - pos);
            if (distance < 0.002 || pos == 0){
                pos = tile.transform.position.x;
            }else{
                return;
            }
        }
        pos = 0f;
        foreach(GameObject tile in thirdCol){
            float distance = Mathf.Abs(tile.transform.position.x - pos);
            if (distance < 0.002 || pos == 0){
                pos = tile.transform.position.x;
            }else{
                return;
            }
        }
        stopGame = true;
        canvasMenu.ShowButton();
    }


}

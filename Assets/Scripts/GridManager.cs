using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//tableau de coordonnées de la map qui permet avec un X et un Y d'accéder aux coordonnées physiques sur Unity
//ceci pour spawn / despawn / déplacer des gameobjects
public struct GridCoord
{
    public GridCoord (float x, float y, int height)
    {
        _x = x;
        _y = y;
        _height = height;
    }

    public float _x { get; set; }
    public float _y { get; set; }
    public int _height { get; set; }
}

//tableau de déplacement avec les numéros de cellule du tableau (vraies coord) + le nombre de déplacement restant
//c'est pour les calculs de déplacements
public struct MoveGrid
{
    public MoveGrid(int x, int y, int moveLeft)
    {
        _x = x;
        _y = y;
        _moveLeft = moveLeft;

    }
    public int _x { get; set; }
    public int _y { get; set; }
    public int _moveLeft { get; set; }
}

public class GridManager : MonoBehaviour
{
    [SerializeField] public Grid _Grid;
    [SerializeField] private int gridheight, gridwidth;

    // Start is called before the first frame update
    void Start()
    {
        GenerateGrid();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GenerateGrid()
    {
        _Grid.grid = new GridCoord[gridwidth, gridheight];
        _Grid.grid[0,0] = new GridCoord(0, 0, 1);

        //Init of the first column
        for (int y =1; y<gridheight;y++ )
        {
            _Grid.grid[0, y] = new GridCoord(_Grid.grid[0, y - 1]._x + 0.8f, _Grid.grid[0, y - 1]._y - 0.4f, 1);
            //Debug.Log(_Grid.grid[0, y]);
        }

        //Init the raws based on the first value precedently initialized
        for (int j = 0; j < gridheight; j++)
        {
            for (int i = 1; i < gridwidth; i++)
            {
                _Grid.grid[i, j] = new GridCoord(_Grid.grid[i-1, j]._x + 0.8f, _Grid.grid[i-1, j]._y + 0.4f, 1);
                //if (j==4) Debug.Log(_Grid.grid[i, j]);
            }
        }

    }
}

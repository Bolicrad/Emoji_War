using System.Collections.Generic;
using UnityEngine;


public class GridPrinter : MonoBehaviour
{
    public Material lineMaterial;
    public static Color _lineColor = Color.white;
    public static Color _overlapColor = Color.red;
    public static Color _selectedColor = Color.blue;
    public static Color _availableColor = Color.green;

    private Vector2 _selectedGrid;
    private List<Vector2> _availableList;
    private Movable _currentMovable;

    void Start()
    {
        
    }
    
    void OnPostRender()
    {
        DrawGrid();
        if (_currentMovable!=null)
        {
            foreach (Vector2 pos in _availableList)
            {
                DrawSquare(pos+_selectedGrid,_availableColor);
            }
            DrawSquare(_selectedGrid,_selectedColor);
        }
        DrawSquare(GetMouseGridPos(),_overlapColor);
    }

    public static Vector2 GetGridPos(Vector2 worldPos)
    {
        return new Vector2(
            (int)(worldPos.x > 0 ? worldPos.x + 0.5 : worldPos.x - 0.5),
            (int)(worldPos.y > 0 ? worldPos.y + 0.5 : worldPos.y - 0.5));
    }

    public static Vector2 GetMouseGridPos()
    {
        return GetGridPos(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    void DrawLine(Vector2 start, Vector2 end, Color color)
    {
        GL.Begin(GL.LINES);
        lineMaterial.SetPass(0);
        GL.Color(color);
        GL.Vertex3(start.x,start.y,0);
        GL.Vertex3(end.x,end.y,0);
        GL.End();
    }
    
    void DrawSquare(Vector2 center, float width, float height, Color color)
    {
        Vector2 leftUp = new Vector2(center.x - width / 2f, center.y + height / 2f);
        Vector2 rightUp = new Vector2(center.x + width / 2f, center.y + height / 2f);
        Vector2 leftDown = new Vector2(center.x - width / 2f, center.y - height / 2f);
        Vector2 rightDown = new Vector2(center.x + width / 2f, center.y - height / 2f);
 
        DrawLine(rightUp, leftUp, color);
        DrawLine(leftUp, leftDown, color);
        DrawLine(leftDown, rightDown, color);
        DrawLine(rightDown, rightUp, color);
    }
    
    void DrawSquare(Vector2 center, Color color)
    {
        DrawSquare(center, 1, 1, color);
    }
    
    void DrawGrid()
    {
        Vector2 rightUp = Camera.main.ScreenToWorldPoint(new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight));
        Vector2 leftDown = Camera.main.ScreenToWorldPoint(new Vector2(0, 0));
        var viewSize = rightUp - leftDown;
        
        int x = (int)viewSize.x + 1;
        int y = (int)viewSize.y + 1;
        x = x / 2 * 2 + 1;
        y = y / 2 * 2 + 1;
        var center = new Vector2(x / 2 + 1, y / 2 + 1);

        Vector2 cameraPos = Camera.main.transform.position;
        var offset = new Vector2(cameraPos.x % 1, cameraPos.y % 1);

        for (var i = 0; i <= x; i++)
        {
            var posX = i - center.x + 0.5f + cameraPos.x - offset.x;
            DrawLine(new Vector2(posX, rightUp.y), new Vector2(posX, leftDown.y), _lineColor);
        }
        
        for (var i = 0; i <= y; i++)
        {
            var posY = i - center.y + 0.5f + cameraPos.y - offset.y;
            DrawLine(new Vector2(leftDown.x, posY), new Vector2(rightUp.x, posY), _lineColor);
        }
    }

    void SelectGrid(Movable movable)
    {
        _currentMovable = movable;
        _selectedGrid = GetGridPos(movable.transform.position);
        _availableList = movable.availablePosList;
    }

    void DeselectGrid()
    {
        _currentMovable = null;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Venctor5 = UnityEngine.Vector3;

public class BlockCreator : MonoBehaviour
{
    [SerializeField] private Vector2 _size;
    private Block[,] _blocks;
    private float _secondsToWait = 0.1f;
    private float _lastTime;
    private bool _playing = false;
    
    private void Start()
    {
        _blocks = new Block[(int)_size.x, (int)_size.y];

        for (int i = 0; i < (int) _size.y; i++)
        {
            for (int j = 0; j < (int) _size.x; j++)
            {
                var block = GameObject.CreatePrimitive(PrimitiveType.Cube);
                block.transform.localScale = new Venctor5(0.95f, 0.95f, 0.95f);
                //DestroyImmediate(block.GetComponent<BoxCollider>());
                _blocks[j,i] = block.AddComponent<Block>();
                _blocks[j, i].transform.position = new Vector3(j+0.5f, i+0.5f, 0);
            }
        }

        Camera.main.transform.position = new Venctor5(_size.x / 2, _size.y / 2, -10);
        Camera.main.orthographicSize = _size.y / 2;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit info;
            if (Physics.Raycast(ray, out info, Mathf.Infinity))
            {
                var comp = info.collider.GetComponent<Block>();
                comp.Dead = !comp.Dead;
            }

            /*var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            int xindex = (int) pos.x % (int) _size.x;
            int yindex = (int) pos.y % (int) _size.y;
            Toggle(xindex, yindex);*/
        }

        if (Input.GetKeyDown(KeyCode.Space)) _playing = !_playing;
        if (Input.GetKeyDown(KeyCode.A)) Step();

        if (_playing && !(_lastTime > Time.time)) Step();
    }
    
    private void Step()
    {
        List<GameOfLifeAction> actions = new List<GameOfLifeAction>();

        for (int i = 0; i < _blocks.GetLength(0); i++)
        {
            for (int j = 0; j < _blocks.GetLength(1); j++)
            {
                int neighbors = GetLifeNeighbors(i, j);
                var b = _blocks[i, j];
                if(!b.Dead && neighbors < 2) actions.Add(new GameOfLifeAction(i,j,false));
                else if (!b.Dead && neighbors > 3) actions.Add(new GameOfLifeAction(i, j, false));
                else if (b.Dead && neighbors == 3) actions.Add(new GameOfLifeAction(i, j, true));
            }
        }

        foreach (var action in actions)
        {
            _blocks[action.x, action.y].Dead = !action.state;
        }
        
        _lastTime = Time.time + _secondsToWait;
    }

    private int GetLifeNeighbors(int x, int y)
    {
        int[,] positions = new int[,]
        {
            {-1,-1}, {0,-1}, {1, -1},
            {-1, 0},         {1, 0},
            {-1, 1}, {0, 1}, {1, 1}
        };

        int c = 0;
        for (int p = 0; p < positions.GetLength(0); p++)
        {
            int nx = (int)Mathf.Repeat(x + positions[p, 0], (int)_size.x);
            int ny = (int) Mathf.Repeat(y + positions[p, 1], (int) _size.y);

            var comp = _blocks[nx, ny];
            if (!comp.Dead) c++;
        }
        return c;
    }

    private int Wrap(int val, int limit)
    {
        return 0;
    }

    private void Toggle(int x, int y)
    {
        if (x < 0 || x >= (int) _size.x || y < 0 || y >= (int) _size.y) return;
        var comp = _blocks[x, y];
        comp.Dead = !comp.Dead;
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(5, 10, 50, 50), System.Math.Round((double)_secondsToWait, 3).ToString());
        _secondsToWait = GUI.HorizontalSlider(new Rect(50, 15, 110, 200), _secondsToWait, 0.005f, 0.5f);
    }
}

struct GameOfLifeAction
{
    public GameOfLifeAction(int x, int y, bool newState)
    {
        this.x = x;
        this.y = y;
        this.state = newState;
    }
    
    public int x;
    public int y;
    public bool state;
}

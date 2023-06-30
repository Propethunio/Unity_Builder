using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class BuildableObjectSO : ScriptableObject {

    public enum Dir {
        Top,
        Down,
        Left,
        Right
    }

    public string nameString;
    public GameObject prefab;
    public GameObject visual;
    public int width;
    public int height;
    public bool isRoad;

    public static Dir GetNextDir(Dir dir) {
        switch(dir) {
            default:
            case Dir.Down: return Dir.Left;
            case Dir.Left: return Dir.Top;
            case Dir.Top: return Dir.Right;
            case Dir.Right: return Dir.Down;
        }
    }

    public int GetRotationAngle(Dir dir) {
        switch(dir) {
            default:
            case Dir.Down: return 0;
            case Dir.Left: return 90;
            case Dir.Top: return 180;
            case Dir.Right: return 270;
        }
    }

    public Vector2Int GetRotationOffset(Dir dir) {
        switch(dir) {
            default:
            case Dir.Down: return new Vector2Int(0, 0);
            case Dir.Left: return new Vector2Int(0, width);
            case Dir.Top: return new Vector2Int(width, height);
            case Dir.Right: return new Vector2Int(height, 0);
        }
    }

    public List<Vector2Int> GetGridPositionList(Vector2Int offset, Dir dir) {
        List<Vector2Int> gridPositionList = new List<Vector2Int>();
        switch(dir) {
            default:
            case Dir.Down:
            case Dir.Top:
                for(int x = 0; x < width; x++) {
                    for(int y = 0; y < height; y++) {
                        gridPositionList.Add(offset + new Vector2Int(x, y));
                    }
                }
                break;
            case Dir.Left:
            case Dir.Right:
                for(int x = 0; x < height; x++) {
                    for(int y = 0; y < width; y++) {
                        gridPositionList.Add(offset + new Vector2Int(x, y));
                    }
                }
                break;
        }
        return gridPositionList;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings.SplashScreen;

public class MovePlayer : MonoBehaviour
{
    enum MoveType
    {
        None = 0,
        MoveWait,
        MoveFloor,
        MoveJump,
        MoveWall,
        MoveFloorWall,
        MoveWallFloor,
        MoveFall,
    }
    enum PlayerStat
    {
        None = 0,
        StateWait,
        StateMove,
    }

    private int moveCount = 0;
    [SerializeField]
    int removeCount = 300;
    private float checkRadius = 0.01f;
    private float checkDistance = 0.98f;
    [SerializeField]
    LayerMask wallLayers = 0;

    private bool[] isObject = { false, false, false, false, false, false, false, false };   //上から8方向に時計回り
    private bool isCatchWallTop = false;
    private bool isCatchWallRight = false;
    private bool isCatchWallLeft = false;
    private bool isMoveEnd;

    private Vector3 oldPos = Vector3.zero;
    private Vector3 newPos = Vector3.zero;

    private MoveType moveType = MoveType.None;
    private PlayerStat playerState = PlayerStat.None;

    private bool moveLR = true;
    private bool moveUD = true;

    // Start is called before the first frame update
    void Start()
    {
        playerState = PlayerStat.StateWait;

        for (int i = 0; i < 8; i++)
        {
            isObject[i] = checkGroundStatus(i).collider != null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();

        //if (playerState == PlayerStat.StateWait)
        //{
        //    gameObject.GetComponent<Renderer>().material.color = Color.green;
        //}
        //else if (playerState == PlayerStat.StateMove)
        //{
        //    gameObject.GetComponent<Renderer>().material.color = Color.red;
        //}

        //Debug.Log("top=" + isObject[0] + ":bottom=" + isObject[1] + ":right=" + isObject[2] + ":left=" + isObject[3]);
        //Debug.Log(isObject[0] + "," + isObject[1] + "," + isObject[2] + "," + isObject[3] + "," + isObject[4] + "," + isObject[5] + "," + isObject[6] + "," + isObject[7]);
    }

    // 入力受付
    void GetInput()
    {
        switch (playerState)
        {
            case PlayerStat.StateWait:
                oldPos = transform.position;
                if (!isObject[4] && !isCatchWallTop && !isCatchWallRight && !isCatchWallLeft)
                {
                    //自然落下
                    Vector3 pos = transform.position;
                    pos.y -= 1;
                    newPos = pos;

                    playerState = PlayerStat.StateMove;
                    moveType = MoveType.MoveFall;
                    moveUD = false;
                    return;

                }
                if (Input.GetKey(KeyCode.D))
                {
                    moveLR = true;
                    Vector3 pos = transform.position;

                    if (isCatchWallLeft)
                    {
                        if (isCatchWallTop)
                        {
                            pos.x += 1.0f;
                            isCatchWallLeft = false;
                            moveType = MoveType.MoveWall;
                        }
                        else
                        {
                            isCatchWallLeft = false;
                            moveType = MoveType.MoveFall;
                            return;
                        }

                    }
                    else if (isCatchWallRight)
                    {
                        if (!isObject[1])
                        {
                            pos.x += 1.0f;
                            pos.y += 1.0f;
                            isCatchWallRight = false;
                            moveType = MoveType.MoveWallFloor;
                        }
                        else
                        {
                            return;
                        }
                    }
                    else if (isCatchWallTop)
                    {
                        if (!isObject[2])
                        {
                            pos.x += 1.0f;
                            isCatchWallRight = false;
                            moveType = MoveType.MoveWall;
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        if (isObject[2])
                        {
                            if (isObject[1])
                            {
                                pos.y += 1.0f;
                                isCatchWallRight = true;
                                moveType = MoveType.MoveFloorWall;
                                moveUD = true;
                            }
                            else
                            {
                                pos.x += 1.0f;
                                pos.y += 1.0f;
                                moveType = MoveType.MoveJump;
                            }
                        }
                        else
                        {
                            pos.x += 1.0f;
                            moveType = MoveType.MoveFloor;
                        }
                    }

                    newPos = pos;

                    playerState = PlayerStat.StateMove;
                    return;

                }
                if (Input.GetKey(KeyCode.A))
                {
                    moveLR = false;
                    Vector3 pos = transform.position;

                    if (isCatchWallRight)
                    {
                        if (isCatchWallTop)
                        {
                            pos.x -= 1.0f;
                            isCatchWallRight = false;
                            moveType = MoveType.MoveWall;
                        }
                        else
                        {
                            isCatchWallRight = false;
                            moveType = MoveType.MoveFall;
                            return;
                        }

                    }
                    else if (isCatchWallLeft)
                    {
                        if (!isObject[7])
                        {
                            pos.x -= 1.0f;
                            pos.y += 1.0f;
                            isCatchWallLeft = false;
                            moveType = MoveType.MoveWallFloor;
                        }
                        else
                        {
                            return;
                        }
                    }
                    else if (isCatchWallTop)
                    {
                        if (!isObject[6])
                        {
                            pos.x -= 1.0f;
                            isCatchWallLeft = false;
                            moveType = MoveType.MoveWall;
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        if (isObject[6])
                        {
                            if (isObject[7])
                            {
                                pos.y += 1.0f;
                                isCatchWallLeft = true;
                                moveType = MoveType.MoveFloorWall;
                                moveUD = true;
                            }
                            else
                            {
                                pos.x -= 1.0f;
                                pos.y += 1.0f;
                                moveType = MoveType.MoveJump;
                            }
                        }
                        else
                        {
                            pos.x -= 1.0f;
                            moveType = MoveType.MoveFloor;
                        }
                    }

                    newPos = pos;

                    playerState = PlayerStat.StateMove;
                    return;
                }
                if (Input.GetKey(KeyCode.W))
                {
                    Vector3 pos = transform.position;
                    moveUD = true;
                    if (isCatchWallRight && !isObject[1])
                    {
                        pos.x += 1.0f;
                        pos.y += 1.0f;
                        isCatchWallRight = false;
                        moveType = MoveType.MoveWallFloor;
                    }
                    else if (isCatchWallLeft && !isObject[7])
                    {
                        pos.x -= 1.0f;
                        pos.y += 1.0f;
                        isCatchWallLeft = false;
                        moveType = MoveType.MoveWallFloor;
                    }
                    else if ((isCatchWallRight || isCatchWallLeft) && !isObject[0])
                    {
                        pos.y += 1.0f;
                        moveType = MoveType.MoveWall;
                    }
                    else
                    {
                        return;
                    }
                    newPos = pos;

                    playerState = PlayerStat.StateMove;
                }
                if (Input.GetKey(KeyCode.S))
                {
                    moveUD = false;
                    Vector3 pos = transform.position;
                    if ((isCatchWallRight || isCatchWallLeft) && !isObject[4])
                    {
                        pos.y -= 1.0f;
                        newPos = pos;

                        playerState = PlayerStat.StateMove;
                        moveType = MoveType.MoveWall;
                        isCatchWallTop = false;
                    }
                    else if (isCatchWallTop)
                    {
                        isCatchWallTop = false;

                        pos.y -= 1.0f;

                        newPos = pos;

                        playerState = PlayerStat.StateMove;
                        moveType = MoveType.MoveFall;
                    }
                }
                break;
            case PlayerStat.StateMove:
                moveCount++;

                if (moveCount > removeCount)
                {
                    isMoveEnd = true;
                }

                DrawMove();

                if (moveCount > removeCount)
                {
                    playerState = PlayerStat.StateWait;
                    moveType = MoveType.MoveWait;
                    moveCount = 0;

                    if (isCatchWallTop)
                    {
                        if (isObject[2])
                        {
                            isCatchWallRight = true;
                        }
                        if (isObject[6])
                        {
                            isCatchWallLeft = true;
                        }
                    }
                    else if (isCatchWallRight)
                    {
                        if (isObject[0])
                        {
                            isCatchWallTop = true;
                        }
                        if (isObject[4])
                        {
                            isCatchWallRight = false;
                        }
                    }
                    else if (isCatchWallLeft)
                    {
                        if (isObject[0])
                        {
                            isCatchWallTop = true;
                        }
                        if (isObject[4])
                        {
                            isCatchWallLeft = false;
                        }
                    }

                }
                break;
        }
    }

    // 接触判定
    RaycastHit2D checkGroundStatus(int direction)
    {
        RaycastHit2D hit = new RaycastHit2D();
        switch (direction)
        {
            case 0: //上
                hit = Physics2D.CircleCast((Vector2)transform.position, checkRadius, Vector2.up, checkDistance, wallLayers);
                break;
            case 1: //右上
                hit = Physics2D.CircleCast((Vector2)transform.position, checkRadius, Vector2.up + Vector2.right, checkDistance, wallLayers);
                break;
            case 2: //右
                hit = Physics2D.CircleCast((Vector2)transform.position, checkRadius, Vector2.right, checkDistance, wallLayers);
                break;
            case 3: //右下
                hit = Physics2D.CircleCast((Vector2)transform.position, checkRadius, Vector2.down + Vector2.right, checkDistance, wallLayers);
                break;
            case 4: //下
                hit = Physics2D.CircleCast((Vector2)transform.position, checkRadius, Vector2.down, checkDistance, wallLayers);
                break;
            case 5: //左下
                hit = Physics2D.CircleCast((Vector2)transform.position, checkRadius, Vector2.down + Vector2.left, checkDistance, wallLayers);
                break;
            case 6: //左
                hit = Physics2D.CircleCast((Vector2)transform.position, checkRadius, Vector2.left, checkDistance, wallLayers);
                break;
            case 7: //左上
                hit = Physics2D.CircleCast((Vector2)transform.position, checkRadius, Vector2.up + Vector2.left, checkDistance, wallLayers);
                break;
            default:
                break;
        }
        return hit;
    }

    void DrawMove()
    {
        Vector3 pos = transform.position;
        switch (moveType)
        {
            case MoveType.None:
                break;
            case MoveType.MoveWait:
                break;
            case MoveType.MoveFloor:
                if (!moveLR)
                {
                    pos.x = (newPos.x + oldPos.x) / 2 + 0.5f * Mathf.Cos(Mathf.PI / removeCount * moveCount);
                }
                else if (moveLR)
                {
                    pos.x = (newPos.x + oldPos.x) / 2 - 0.5f * Mathf.Cos(Mathf.PI / removeCount * moveCount);
                }
                pos.y = (newPos.y + oldPos.y) / 2 + 0.5f * Mathf.Sin(Mathf.PI / removeCount * moveCount);

                if (moveLR)
                {
                    this.transform.eulerAngles = new Vector3(0, 180, 0);
                }
                else
                {
                    this.transform.eulerAngles = new Vector3(0, 0, 0);
                }
                break;
            case MoveType.MoveJump:
                if(moveCount <= (removeCount /2))
                {
                    pos.x += (((newPos.x - oldPos.x) / 2)/ (removeCount /2));
                    pos.y += ((newPos.y + 0.5f - oldPos.y) / (removeCount /2));
                }
                else
                {
                    if (!moveLR)
                    {
                        pos.x = (newPos.x + oldPos.x) / 2 - 0.5f * Mathf.Cos(Mathf.PI / 2 / (removeCount / 2) * (removeCount - moveCount));
                    }
                    else if (moveLR)
                    {
                        pos.x = (newPos.x + oldPos.x) / 2 + 0.5f * Mathf.Cos(Mathf.PI / 2 / (removeCount / 2) * (removeCount - moveCount));
                    }
                    pos.y = newPos.y + 0.5f * Mathf.Cos(Mathf.PI / 2 / (removeCount / 2) * (moveCount - removeCount /2));
                }
                if (moveLR)
                {
                    this.transform.eulerAngles = new Vector3(0, 180, 0);
                }
                else
                {
                    this.transform.eulerAngles = new Vector3(0, 0, 0);
                }
                break;
            case MoveType.MoveWall:
                pos.x += ((newPos.x - oldPos.x) / removeCount);
                pos.y += ((newPos.y - oldPos.y) / removeCount);

                if (isCatchWallTop)
                {
                    if (moveLR)
                    {
                        this.transform.eulerAngles = new Vector3(180, 180, 0);
                    }
                    else
                    {
                        this.transform.eulerAngles = new Vector3(180, 0, 0);
                    }
                }
                else if (isCatchWallRight)
                {
                    if (moveUD)
                    {
                        this.transform.eulerAngles = new Vector3(0, 180, -90);
                    }
                    else
                    {
                        this.transform.eulerAngles = new Vector3(0, 0, 90);
                    }
                }
                else if (isCatchWallLeft)
                {
                    if (moveUD)
                    {
                        this.transform.eulerAngles = new Vector3(0, 0, -90);
                    }
                    else
                    {
                        this.transform.eulerAngles = new Vector3(0, 180, 90);
                    }
                }
                break;
            case MoveType.MoveFloorWall:
                pos.x += ((newPos.x - oldPos.x) / removeCount);
                pos.y += ((newPos.y - oldPos.y) / removeCount);

                if (isCatchWallTop)
                {
                    if (moveLR)
                    {
                        this.transform.eulerAngles = new Vector3(180, 180, 0);
                    }
                    else
                    {
                        this.transform.eulerAngles = new Vector3(180, 0, 0);
                    }
                }
                else if (isCatchWallRight)
                {
                    if (moveUD)
                    {
                        this.transform.eulerAngles = new Vector3(0, 180, -90);
                    }
                    else
                    {
                        this.transform.eulerAngles = new Vector3(0, 0, 90);
                    }
                }
                else if (isCatchWallLeft)
                {
                    if (moveUD)
                    {
                        this.transform.eulerAngles = new Vector3(0, 0, -90);
                    }
                    else
                    {
                        this.transform.eulerAngles = new Vector3(0, 180, 90);
                    }
                }
                break;
            case MoveType.MoveWallFloor:
                if (!moveLR)
                {
                    pos.x = newPos.x + Mathf.Cos((Mathf.PI / 2) / removeCount * moveCount);
                }
                else if (moveLR)
                {
                    pos.x = newPos.x - Mathf.Cos((Mathf.PI / 2) / removeCount * moveCount);
                }
                pos.y = oldPos.y + Mathf.Sin((Mathf.PI / 2) / removeCount * moveCount);

                if (!moveLR)
                {
                    this.transform.eulerAngles = new Vector3(0, 0, -90 - (-90 * Mathf.Sin((Mathf.PI / 2) / removeCount * moveCount)));
                }
                else if (moveLR)
                {
                    this.transform.eulerAngles = new Vector3(0, 180, -90 - (-90 * Mathf.Sin((Mathf.PI / 2) / removeCount * moveCount)));
                }
                break;
            case MoveType.MoveFall:
                pos.x += ((newPos.x - oldPos.x) / removeCount);
                pos.y += ((newPos.y - oldPos.y) / removeCount);

                if (moveLR)
                {
                    this.transform.eulerAngles = new Vector3(0, 180, 0);
                }
                else
                {
                    this.transform.eulerAngles = new Vector3(0, 0, 0);
                }
                break;
        }

        if (isMoveEnd)
        {
            transform.position = newPos;
            isMoveEnd = false;
            for (int i = 0; i < 8; i++)
            {
                isObject[i] = checkGroundStatus(i).collider != null;
            }
        }
        else
        {
            transform.position = pos;
            Debug.Log(pos);
        }
    }
}

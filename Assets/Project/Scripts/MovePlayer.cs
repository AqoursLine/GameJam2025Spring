using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;
using static UnityEditor.PlayerSettings.SplashScreen;

public class MovePlayer : MonoBehaviour
{
	enum MoveType       //プレイヤ－の移動タイプ
	{
		None = 0,
		MoveWait,       //待機
		MoveFloor,      //床ジャンプ移動
		MoveJump,       //一段上にジャンプ
		MoveWall,       //壁、天井を這って移動
		MoveFloorWall,  //床から壁に移動
		MoveWallFloor,  //壁から床に移動
		MoveTopWall,    //天井から壁に移動
		MoveWallTop,    //壁から天井に移動
		MoveFall,       //落下
		MoveUpdraft,    //上昇気流
	}
	private enum PlayerState
	{
		None = 0,
		StateWait,
		StateMove,
	}

	[SerializeField]
	const int MoveCountFloor = 20;
	[SerializeField]
	const int MoveCountFall = 10;
	[SerializeField]
	const int MoveCountUpdraft = 20;

	[SerializeField]
	Animator animator;

	private int moveCount = 0;
	[SerializeField]
	int removeCount = 10;
	private float checkRadius = 0.01f;
	private float checkDistance = 0.98f;
	[SerializeField]
	LayerMask wallLayers = 0;
	[SerializeField]
	LayerMask updraftLayers = 0;
	[SerializeField]
	LayerMask healLayers = 0;
	[SerializeField]
	LayerMask boxLayers = 0;

	[Tooltip("上から8方向に時計回り")]
	private bool[] isObject = { false, false, false, false, false, false, false, false };
	private bool[] isBox = { false, false };
	private bool isCatchWallTop = false;        //天井掴んでいるか
	private bool isCatchWallRight = false;      //右の壁をつかんでいるか
	private bool isCatchWallLeft = false;       //左の壁をつかんでいるか
	private bool isMoveEnd;

	private Vector3 oldPos = Vector3.zero;
	private Vector3 newPos = Vector3.zero;

	private MoveType moveType = MoveType.None;
	private PlayerState playerState = PlayerState.None;

	[Tooltip("false:左移動　true:右移動")]
	private bool moveLR = true;
	[Tooltip("false:下移動　true:上移動")]
	private bool moveUD = true;

	private bool isUpdraftPanel = false;

	[SerializeField]
	int defaultHP = 10;

	int HP;

	[SerializeField]
	float CheckUpBlocks = 10;

	private bool isRockFlg = false;

	// Start is called before the first frame update
	void Start()
	{
		Application.targetFrameRate = 60;
		playerState = PlayerState.StateWait;

		HP = defaultHP;

		GetAroudObject();
	}

	// Update is called once per frame
	void Update()
	{
		UpdateAnimator();
		GetInput();
		//Debug.Log((int)moveType);
	}

	private void FixedUpdate()
	{
		if (moveType == MoveType.MoveWait || moveType == MoveType.None)
		{
			return;
		}
		if (moveType == MoveType.MoveWall || moveType == MoveType.MoveFloorWall || moveType == MoveType.MoveWallTop || (HP == 0 && moveType == MoveType.MoveFloor))
		{
			if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0)
			{
				return;
			}
		}

		moveCount++;

		Vector3 pos = this.transform.position;

		switch (moveType)
		{
			case MoveType.MoveFloor:
				removeCount = MoveCountFloor;
				if (!isRockFlg)
				{
					pos = MoveFloor(pos);
					isMoveEnd = removeCount <= moveCount;
				}
				else
				{
					pos = MoveSlideRock(pos);
					isMoveEnd = animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.80;
				}
				break;
			case MoveType.MoveJump:
				removeCount = MoveCountFloor;
				if (!isRockFlg)
				{
					pos = MoveJump(pos);
					isMoveEnd = removeCount <= moveCount;
				}
				else
				{
					pos = MoveJumpRock(pos);
					isMoveEnd = animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.80;
				}
				break;
			case MoveType.MoveWall:
				pos = MoveWall(pos);
				isMoveEnd = animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.80;
				break;
			case MoveType.MoveFloorWall:
				pos = FloorWall(pos);
				isMoveEnd = animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.80;
				break;
			case MoveType.MoveWallFloor:
				removeCount = MoveCountFloor;
				pos = WallFloor(pos);
				isMoveEnd = removeCount <= moveCount;
				break;
			case MoveType.MoveTopWall:
				removeCount = MoveCountFloor;
				pos = TopWall(pos);
				isMoveEnd = removeCount <= moveCount;
				break;
			case MoveType.MoveWallTop:
				pos = WallTop(pos);
				isMoveEnd = animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.80;
				break;
			case MoveType.MoveFall:
				removeCount = MoveCountFall;
				pos = MoveFall(pos);
				isMoveEnd = removeCount <= moveCount;
				break;
			case MoveType.MoveUpdraft:
				removeCount = MoveCountUpdraft;
				pos = MoveUpdraft(pos);
				isMoveEnd = removeCount <= moveCount;
				break;
			default:
				break;
		}

		if (isMoveEnd)
		{
			if (HP == 0)
			{
				isRockFlg = true;
			}
			transform.position = newPos;

			GetAroudObject();

			if (isObject[4])
			{
				isCatchWallRight = false;
				isCatchWallLeft = false;
				isCatchWallTop = false;
			}
			else
			{
				if (moveType != MoveType.MoveFall)
				{
					isCatchWallRight = isObject[2];
					isCatchWallLeft = isObject[6];
					isCatchWallTop = isObject[0];
				}
			}

			if (playerState == PlayerState.StateMove)
			{
				moveType = MoveType.None;
			}
			isMoveEnd = false;
			playerState = PlayerState.StateWait;
			moveCount = 0;

			if (!isCatchWallTop && !isCatchWallRight && !isCatchWallLeft)
			{
				transform.localScale = new Vector3(1, 1, 1);
			}
			if (isCatchWallRight)
			{
				transform.localScale = new Vector3(1, 1, 1);
			}
			else if (isCatchWallLeft)
			{
				transform.localScale = new Vector3(-1, 1, 1);
			}
			else if (isCatchWallTop)
			{
				transform.localScale = new Vector3(1, 1, 1);
			}
		}
		else
		{
			transform.position = pos;
			//Debug.Log(pos);
		}
	}

	// 入力受付
	void GetInput()
	{
		if(Physics2D.CircleCast((Vector2)transform.position, checkRadius, Vector2.zero, 0, wallLayers).collider != null)
		{
			return;
		}

		if (moveType == MoveType.None)
		{
			moveType = MoveType.MoveWait;
		}
		switch (playerState)
		{
			case PlayerState.StateWait:
				oldPos = transform.position;
				if (HP == 0)
				{
					isCatchWallTop = false;
					isCatchWallRight = false;
					isCatchWallLeft = false;
				}

				if (CheckFall())
				{
					return;
				}

				if (isUpdraftPanel)
				{
					RaycastHit2D hit = new RaycastHit2D();
					hit = Physics2D.CircleCast((Vector2)transform.position, checkRadius, Vector2.up, CheckUpBlocks, wallLayers);

					int targetY = (int)Mathf.Round(hit.centroid.y - 0.5f);

					newPos = new Vector3(transform.position.x, targetY, 0.0f);

					playerState = PlayerState.StateMove;
					moveType = MoveType.MoveUpdraft;
					return;
				}

				if (Input.GetKey(KeyCode.D))
				{
					if (isObject[2] && isObject[0])
					{
						return;
					}
					moveLR = true;
					Vector3 pos = transform.position;

					if (isCatchWallLeft)
					{
						if (isCatchWallTop)
						{
							pos.x += 1.0f;
							isCatchWallLeft = false;
							moveType = MoveType.MoveWallTop;
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
							if (!isObject[1])
							{
								pos.x += 1.0f;
								pos.y += 1.0f;
								isCatchWallTop = false;
								isCatchWallLeft = true;
								moveType = MoveType.MoveTopWall;
							}
							else
							{
								pos.x += 1.0f;
								isCatchWallRight = false;
								moveType = MoveType.MoveWall;
							}
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
								if (HP > 0)
								{
									pos.y += 1.0f;
									isCatchWallRight = true;
									moveType = MoveType.MoveFloorWall;
									moveUD = true;
								}
								else
								{
									return;
								}
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
							if (isBox[0])
							{
								RaycastHit2D hit = new RaycastHit2D();
								hit = Physics2D.CircleCast((Vector2)transform.position, checkRadius, Vector2.right, checkDistance, boxLayers);

								MoveWallScript moveWallScript = hit.transform.gameObject.GetComponent<MoveWallScript>();
								if (moveWallScript != null)
								{
									if (moveWallScript.Move(2))
									{
										pos.x += 1.0f;
										moveType = MoveType.MoveFloor;
									}
									else
									{
										pos.x += 1.0f;
										pos.y += 1.0f;
										moveType = MoveType.MoveJump;
									}
								}
							}
							else
							{
								pos.x += 1.0f;
								moveType = MoveType.MoveFloor;
							}
						}
					}

					newPos = pos;

					playerState = PlayerState.StateMove;

					if (HP > 0)
					{
						HP--;
					}

					//isMoveEnd = false;
					return;

				}
				if (Input.GetKey(KeyCode.A))
				{
					if (isObject[6] && isObject[0])
					{
						return;
					}
					Debug.Log("A");
					moveLR = false;
					Vector3 pos = transform.position;

					if (isCatchWallRight)
					{
						Debug.Log("B");
						if (isCatchWallTop)
						{
							pos.x -= 1.0f;
							isCatchWallRight = false;
							moveType = MoveType.MoveWallTop;
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
						Debug.Log("C");
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
						Debug.Log("D");
						if (!isObject[6])
						{
							if (!isObject[7])
							{
								pos.x -= 1.0f;
								pos.y += 1.0f;
								isCatchWallTop = false;
								isCatchWallRight = true;
								moveType = MoveType.MoveTopWall;
							}
							else
							{
								pos.x -= 1.0f;
								isCatchWallLeft = false;
								moveType = MoveType.MoveWall;
							}
						}
						else
						{
							return;
						}
					}
					else
					{
						Debug.Log("E");
						if (isObject[6])
						{
							if (isObject[7])
							{
								if (HP > 0)
								{
									pos.y += 1.0f;
									isCatchWallLeft = true;
									moveType = MoveType.MoveFloorWall;
									moveUD = true;
								}
								else
								{
									return;
								}
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
							if (isBox[1])
							{
								RaycastHit2D hit = new RaycastHit2D();
								hit = Physics2D.CircleCast((Vector2)transform.position, checkRadius, Vector2.left, checkDistance, boxLayers);

								MoveWallScript moveWallScript = hit.transform.gameObject.GetComponent<MoveWallScript>();
								if (moveWallScript != null)
								{
									if (moveWallScript.Move(6))
									{
										pos.x -= 1.0f;
										moveType = MoveType.MoveFloor;
									}
									else
									{
										pos.x -= 1.0f;
										pos.y += 1.0f;
										moveType = MoveType.MoveJump;
									}
								}
							}
							else
							{
								pos.x -= 1.0f;
								moveType = MoveType.MoveFloor;
							}
						}
					}

					newPos = pos;

					playerState = PlayerState.StateMove;

					if (HP > 0)
					{
						HP--;
					}
					//isMoveEnd = false;
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
						moveLR = true;
						moveType = MoveType.MoveWallFloor;
					}
					else if (isCatchWallLeft && !isObject[7])
					{
						pos.x -= 1.0f;
						pos.y += 1.0f;
						isCatchWallLeft = false;
						moveLR = false;
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

					playerState = PlayerState.StateMove;

					if (HP > 0)
					{
						HP--;
					}
					//isMoveEnd = false;
				}
				if (Input.GetKey(KeyCode.S))
				{
					moveUD = false;
					Vector3 pos = transform.position;
					if ((isCatchWallRight || isCatchWallLeft) && !isObject[4])
					{
						pos.y -= 1.0f;
						newPos = pos;

						playerState = PlayerState.StateMove;
						//isMoveEnd = false;
						moveType = MoveType.MoveWall;
						isCatchWallTop = false;
					}
					else if (isCatchWallTop)
					{
						isCatchWallTop = false;

						pos.y -= 1.0f;

						newPos = pos;

						playerState = PlayerState.StateMove;
						//isMoveEnd = false;
						moveType = MoveType.MoveFall;
					}
					else
					{
						return;
					}

					if (HP > 0)
					{
						HP--;
					}
				}
				break;
			case PlayerState.StateMove:

				//DrawMove();

				break;
		}
	}

	// 接触判定
	RaycastHit2D checkObjectStatus(int direction)
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
				if (hit.collider == null)
				{
					hit = Physics2D.CircleCast((Vector2)transform.position, checkRadius, Vector2.down, checkDistance, boxLayers);
				}
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

	public void GetAroudObject()
	{
		if (Physics2D.CircleCast((Vector2)transform.position, checkRadius, Vector2.down, checkDistance, healLayers).collider != null
			|| Physics2D.CircleCast((Vector2)transform.position, checkRadius, Vector2.up, checkDistance, healLayers).collider != null)
		{
			HP = defaultHP;
			isRockFlg = false;
		}
		for (int i = 0; i < 8; i++)
		{
			isObject[i] = checkObjectStatus(i).collider != null;
		}

		isBox[0] = Physics2D.CircleCast((Vector2)transform.position, checkRadius, Vector2.right, checkDistance, boxLayers).collider != null;
		isBox[1] = Physics2D.CircleCast((Vector2)transform.position, checkRadius, Vector2.left, checkDistance, boxLayers).collider != null;

		if (HP > 0)
		{
			isUpdraftPanel = Physics2D.CircleCast((Vector2)transform.position, checkRadius, Vector2.down, checkDistance, updraftLayers).collider != null;
		}
		else
		{
			isUpdraftPanel = false;
		}
	}

	//床を左右移動
	Vector3 MoveFloor(Vector3 pos)
	{
		if (!moveLR)
		{
			pos.x = (newPos.x + oldPos.x) / 2 + 0.5f * Mathf.Cos(Mathf.PI / removeCount * moveCount);
		}
		else if (moveLR)
		{
			pos.x = (newPos.x + oldPos.x) / 2 - 0.5f * Mathf.Cos(Mathf.PI / removeCount * moveCount);
		}
		pos.y = (newPos.y + oldPos.y) / 2 + 0.5f * Mathf.Sin(Mathf.PI / removeCount * moveCount);

		transform.localScale = new Vector3(1, 1, 1);
		return pos;
	}

	//一段上の床にジャンプ移動
	Vector3 MoveJump(Vector3 pos)
	{
		if (moveCount <= (removeCount / 2))
		{
			pos.x += (((newPos.x - oldPos.x) / 2) / (removeCount / 2));
			pos.y += ((newPos.y + 0.5f - oldPos.y) / (removeCount / 2));
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
			pos.y = newPos.y + 0.5f * Mathf.Cos(Mathf.PI / 2 / (removeCount / 2) * (moveCount - removeCount / 2));
		}

		transform.localScale = new Vector3(1, 1, 1);
		return pos;
	}

	//壁または天井を移動
	Vector3 MoveWall(Vector3 pos)
	{
		if (isCatchWallTop)
		{
			Vector3 scale = transform.localScale;
			if (moveLR)
			{
				scale.x = 1;
			}
			else
			{
				scale.x = -1;
			}
			transform.localScale = scale;
		}
		if (isCatchWallLeft || isCatchWallRight)
		{
			Vector3 scale = transform.localScale;
			if (moveUD)
			{
				scale.y = 1;
			}
			else
			{
				scale.y = -1;
			}
			transform.localScale = scale;
		}

		return pos;
	}

	//床から壁へ移動
	Vector3 FloorWall(Vector3 pos)
	{
		Vector3 scale = transform.localScale;
		if (moveLR)
		{
			scale.x = 1;
		}
		else
		{
			scale.x = -1;
		}
		transform.localScale = scale;

		return pos;
	}

	//壁から床へ移動
	Vector3 WallFloor(Vector3 pos)
	{
		if (!moveLR)
		{
			pos.x = newPos.x + Mathf.Cos((Mathf.PI / 2) / removeCount * moveCount);
		}
		else if (moveLR)
		{
			pos.x = newPos.x - Mathf.Cos((Mathf.PI / 2) / removeCount * moveCount);
		}
		pos.y = oldPos.y + Mathf.Sin((Mathf.PI / 2) / removeCount * moveCount);
		return pos;
	}

	//天井から壁へ移動
	Vector3 TopWall(Vector3 pos)
	{
		if (!moveLR)
		{
			pos.x = oldPos.x - Mathf.Cos(Mathf.PI / 2 / removeCount * (removeCount - moveCount));
		}
		else if (moveLR)
		{
			pos.x = oldPos.x + Mathf.Cos(Mathf.PI / 2 / removeCount * (removeCount - moveCount));
		}
		pos.y = newPos.y - Mathf.Sin((Mathf.PI / 2) / removeCount * (removeCount - moveCount));
		return pos;
	}

	//壁から天井へ移動
	Vector3 WallTop(Vector3 pos)
	{
		Vector3 scale = transform.localScale;
		if (moveLR)
		{
			scale.x = -1;
		}
		else
		{
			scale.x = 1;
		}
		transform.localScale = scale;

		return pos;
	}

	//落下モーション
	Vector3 MoveFall(Vector3 pos)
	{
		pos.y += ((newPos.y - oldPos.y) / removeCount);

		return pos;
	}

	//固体時移動モーション
	Vector3 MoveSlideRock(Vector3 pos)
	{
		Vector3 scale = transform.localScale;
		if (moveLR)
		{
			scale.x = 1;
		}
		else
		{
			scale.x = -1;
		}
		transform.localScale = scale;
		return pos;
	}

	//固体時ジャンプモーション
	Vector3 MoveJumpRock(Vector3 pos)
	{
		if (moveCount <= (removeCount / 2))
		{
			pos.y += ((newPos.y + 0.5f - oldPos.y) / (removeCount / 2));
		}
		else
		{
			pos.y -= (0.5f / (removeCount / 2));
		}

		Vector3 scale = transform.localScale;
		if (moveLR)
		{
			scale.x = 1;
		}
		else
		{
			scale.x = -1;
		}
		transform.localScale = scale;

		return pos;
	}

	// アニメータの更新
	void UpdateAnimator()
	{
		animator.SetInteger("MoveType", ((int)moveType));
		animator.SetBool("isTopCatch", (isCatchWallTop));
		animator.SetBool("isSideCatch", (isCatchWallRight || isCatchWallLeft));
		animator.SetInteger("PlayerHP", HP);
	}

	Vector3 MoveUpdraft(Vector3 pos)
	{
		pos.x += ((newPos.x - oldPos.x) / removeCount);
		pos.y += ((newPos.y - oldPos.y) / removeCount);
		return pos;
	}

	public bool GetPlayerState()
	{
		return !(playerState == PlayerState.StateWait);
	}

	public bool CheckFall()
	{
		if (!isObject[4] && !isCatchWallTop && !isCatchWallRight && !isCatchWallLeft)
		{
			//自然落下
			Vector3 pos = transform.position;
			pos.y -= 1;
			newPos = pos;

			playerState = PlayerState.StateMove;
			moveType = MoveType.MoveFall;
			moveUD = false;
			return true;

		}
		return false;
	}

	public void CatchWallFlg()
	{
		if (!isObject[4])
		{
			isCatchWallTop = isObject[0];
			isCatchWallRight = isObject[2];
			isCatchWallLeft = isObject[6];
		}
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor.UIElements;
using UnityEngine;
using Random = System.Random;

public class EnemyFormationController : MonoBehaviour
{
				[SerializeField] private float _shotSpeed = .01f;
				[SerializeField] private float _stepSpeed = .5f;
				[SerializeField] private float _stepHorizontalDistance = .5f;
				[SerializeField] private float _stepVerticalDistance = .5f;

				private int TotalColumns => transform.childCount;
				
				private float _timeSinceLastMove;
				private float _timeSinceLastShot;
				private List<EnemyColumn> _activeColumns;
				private EnemyColumn[] _orderedColumns;
				private bool _movingLeft;
				private int _columnsDestroyed;
				private int _rightMostActiveColumn;
				private int _leftMostActiveColumn;
				private float _halfEnemySpriteWidth;

				/*
				* Resets all enemies and moves formation to height
				*/
				public void ResetFormation(float height)
				{
					_timeSinceLastMove = 0f;
					_timeSinceLastShot = 0f;
					_movingLeft = false;
					ResetAllEnemies();
				}

				private void Awake()
				{
					_orderedColumns = new EnemyColumn[TotalColumns];
					for (int n = 0; n < TotalColumns; n++)
					{
						_orderedColumns[n] = transform.GetChild(n).GetComponent<EnemyColumn>();
						_orderedColumns[n].ColumnDestroyed += OnColumnDestroyed;
					}

					_halfEnemySpriteWidth =
						GetComponentInChildren<EnemyController>().GetComponent<SpriteRenderer>().size.x / 2;
					
					ResetFormation(5);
				}

				private void Update()
				{
					if (_columnsDestroyed == TotalColumns)
						return;
					UpdateMove();
					UpdateShot();
				}
				
				void OnColumnDestroyed(EnemyColumn column)
				{
					_activeColumns.Remove(column);
					_columnsDestroyed++;
					
					if (_columnsDestroyed < TotalColumns)
					{
						RecalculateColumnBoundaries();
					}
				}

				float GetLeftBoundary()
				{
					return _orderedColumns[_leftMostActiveColumn].transform.position.x - _halfEnemySpriteWidth;
				}
				
				float GetRightBoundary()
				{
					return _orderedColumns[_rightMostActiveColumn].transform.position.x + _halfEnemySpriteWidth;
				}

				void RecalculateColumnBoundaries()
				{
					for (int n = _leftMostActiveColumn; n < TotalColumns; n++)
					{
						if (_orderedColumns[n].IsColumnEmpty())
						{
							_leftMostActiveColumn++;
						}
					}

					for (int n = _rightMostActiveColumn; n >= 0; n--)
					{
						if (_orderedColumns[n].IsColumnEmpty())
						{
							_rightMostActiveColumn--;
						}
					}
				}

				void UpdateShot()
				{
					_timeSinceLastShot += Time.deltaTime;
					if (_timeSinceLastShot > _shotSpeed)
					{
						FireRandomShot();
						_timeSinceLastShot = 0;
					}
				}

				void UpdateMove()
				{
					_timeSinceLastMove += Time.deltaTime;
					if (_timeSinceLastMove > _stepSpeed)
					{
						Move();
					}
				}
				
				/*
				* Fires a shot from a random bottom enemy
				*/
				void FireRandomShot()
				{
					GetRandomActiveColumn().FireShot();
				}
				
				/*
				* Resets all enemies (IE restores them to life)
				*/
				void ResetAllEnemies()
				{
					for (int n = 0; n < _orderedColumns.Length; n++)
					{
						_orderedColumns[n].ResetColumn();
					}

					_columnsDestroyed = 0;
					_rightMostActiveColumn = _orderedColumns.Length - 1;
					_leftMostActiveColumn = 0;
					_activeColumns = new List<EnemyColumn>(_orderedColumns);
				}

				EnemyColumn GetRandomActiveColumn()
				{
					int randomActiveColumnIndex = UnityEngine.Random.Range(0, _activeColumns.Count);
					return _activeColumns[randomActiveColumnIndex];
				}

				/*
				* Moves formation to next position
				*/
				public void Move()
				{
					Vector3 delta = transform.position;
					if (_movingLeft)
					{
						float leftWall = Camera.main.ViewportToWorldPoint(Vector3.zero).x;
						if (Mathf.Approximately(leftWall, GetLeftBoundary()))
						{
							_movingLeft = false;
							delta += new Vector3(0f, -1 * _stepVerticalDistance, 0f);
						}
						else
						{
							float deltaLeftBoundary = GetLeftBoundary() - _stepHorizontalDistance;

							float deltaLeft;
							if (deltaLeftBoundary > leftWall)
							{
								deltaLeft = _stepHorizontalDistance;
							}
							else
							{
								deltaLeft = _stepHorizontalDistance - (leftWall - deltaLeftBoundary);
							}
							delta += new Vector3(-1 * deltaLeft, 0f, 0f);
						}
					}
					else
					{

						float rightWall = Camera.main.ViewportToWorldPoint(Vector3.one).x;
						
						if (Mathf.Approximately(rightWall, GetRightBoundary()))
						{
							delta += new Vector3(0f, -1 * _stepVerticalDistance, 0f);
							_movingLeft = true;
						}
						else
						{
							float deltaRightBoundary = GetRightBoundary() + _stepHorizontalDistance;
							float deltaRight;
							if (deltaRightBoundary < rightWall)
							{
								deltaRight = _stepHorizontalDistance;
							}
							else
							{
								deltaRight = _stepHorizontalDistance - (deltaRightBoundary - rightWall);
							}
							delta += new Vector3(deltaRight, 0f, 0f);
						}
					}

					transform.position = delta;
				}
}

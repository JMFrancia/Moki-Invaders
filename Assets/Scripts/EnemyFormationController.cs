using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Random = System.Random;

public class EnemyFormationController : MonoBehaviour
{
				[SerializeField] private float _shotSpeed = .01f;
				[SerializeField] private float _stepSpeed = .5f;
				[SerializeField] private float _stepHorizontalDistance = .5f;
				[SerializeField] private float _stepVerticalDistance = .5f;

				private float _timeSinceLastMove;
				private float _timeSinceLastShot;
				private EnemyColumn[] _columns;
				private EnemyColumn[] _orderedColumns;
				private bool _movingLeft;
				private int _activeColumnsMinIndex;
				private int _columnsDestroyed;
				private int _rightMostActiveColumn;
				private int _leftMostActiveColumn;

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
					_orderedColumns = new EnemyColumn[transform.childCount];
					for (int n = 0; n < transform.childCount; n++)
					{
						_orderedColumns[n] = transform.GetChild(n).GetComponent<EnemyColumn>();
						int columnIndex = n;
						_orderedColumns[n].ColumnDestroyed += () =>
						{
							OnColumnDestroyed(columnIndex);
						};
					}

					_columns = new EnemyColumn[transform.childCount];
					_orderedColumns.CopyTo(_columns, 0);
					
					ResetFormation(5);
				}

				private void Update()
				{
					if (_columnsDestroyed == _columns.Length)
						return;
					UpdateMove();
					UpdateShot();
				}
				
				/*
				I think we have 3 choices here
				1. This can be really efficient, but sort of difficult to understand
				   * 2 column arrays, one that stays ordered, and one that reorders itself to keep active columns on top
				   * For the second one, instead of using an array of columns, use an array of indicies to the ordered column array 
				2. We can give up on calculating boundaries and instead use colliders on the walls
				  * Use for player boundaries as well
				  * Place collider on individual enemies? Ignore multiple collisions
				       * Otherwise place collider on column, but re-shape if move enemies  
				3. We can give up on using an efficient swap-based methodology to get random active columns from a single array
				  * Just have a list containing only active columns, and add/remove to it as needed
				*/
				
				void OnColumnDestroyed(int index)
				{
					if (index != _activeColumnsMinIndex)
					{
						SwapColumnIndex(index, _activeColumnsMinIndex);
					}
					_activeColumnsMinIndex++;
					_columnsDestroyed++;
					
					if (_columnsDestroyed < _columns.Length)
					{
						RecalculateColumnBoundaries();
					}
				}

				void RecalculateColumnBoundaries()
				{
					//for(int n = 0; n < )
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
					for (int n = 0; n < _columns.Length; n++)
					{
						_columns[n].ResetColumn();
					}

					_columnsDestroyed = 0;
					_rightMostActiveColumn = _columns.Length;
					_leftMostActiveColumn = 0;
					_activeColumnsMinIndex = 0;
				}

				void SwapColumnIndex(int a, int b)
				{
					(_columns[a], _columns[b]) = (_columns[b], _columns[a]);
				}

				EnemyColumn GetRandomActiveColumn()
				{
					int randomActiveColumnIndex = UnityEngine.Random.Range(_activeColumnsMinIndex, _columns.Length);
					return _columns[randomActiveColumnIndex];
				}

				/*
				* Moves formation to next position
				*/
				public void Move()
				{
					
				}
}

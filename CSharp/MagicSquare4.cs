using System;
using System.Collections.Generic;

namespace CSharp
{
	class MagicSquare4
	{
		private static readonly bool[] A = new bool[17];
		private static int[][] _b;
		private static int _ans = 0;

		public MagicSquare4()
		{
			_b = new int[4][];
			for (var i = 0; i < 4; ++i)
			{
				_b[i] = new int[4];
			}
		}

		private static bool Judge(IReadOnlyList<int[]> b)
		{
			//列判断
			for (var i = 0; i < 4; ++i)
			{
				var max1 = 0;
				for (var j = 0; j < 4; ++j)
				{
					max1 += b[i][j];
				}

				if (max1 != 34)
				{
					return false;
				}
			}

			//行判断
			for (var i = 0; i < 4; ++i)
			{
				var max4 = 0;
				for (var j = 0; j < 4; ++j)
				{
					max4 += b[j][i];
				}

				if (max4 != 34)
				{
					return false;
				}
			}

			//对角线判断
			var sum2 = b[0][0] + b[1][1] + b[2][2] + b[3][3];
			if (sum2 != 34)
			{
				return false;
			}

			var sum3 = b[0][3] + b[1][2] + b[2][1] + b[3][0];
			if (sum3 != 34)
			{
				return false;
			}

			return true;
		}

		private static void Dfs(int i, int j, int current)
		{
			if (current > 34)
			{
				return;
			}

			if (i == 3 && j == 3)
			{
				if (Judge(_b))
				{
					++_ans;
					Output(_b);
				}
				return;
			}

			int iTemp = 0, jTemp = 0; //i_temp, j_temp为当前要前进的格子
			if (j == 3)
			{
				iTemp = i + 1;
				jTemp = 0;
			}
			else if (j < 3)
			{
				iTemp = i;
				jTemp = j + 1;
			} //向前走

			for (var k = 1; k <= 16; ++k)
			{
				if (jTemp == 3 && current + k != 34)
				{
					continue; //当前行不是34
				}

				if (A[k] == false)
				{
					//若前面未使用过的数字，则进行复制，递归
					A[k] = true;
					_b[iTemp][jTemp] = k;
					if (jTemp < 3)
					{
						Dfs(iTemp, jTemp, k + current);
					}
					else//jTemp == 3
					{
						Dfs(iTemp, jTemp, 0);
					}

					_b[iTemp][jTemp] = 0;
					A[k] = false;
				}
			}
		}

		public void Solve(int firstNum)
		{
			A[firstNum] = true;
			_b[0][0] = firstNum;
			Dfs(0, 0, firstNum);
			Console.WriteLine(_ans);
		}

		private static void Output(IReadOnlyList<int[]> a)
		{
			for (var i = 0; i < 4; ++i)
			{
				for (var j = 0; j < 4; ++j)
				{
					Console.Write($@"{a[i][j]}	");
				}
				Console.WriteLine();
			}

			Console.WriteLine();
		}
	}
}

#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syncfusion.Windows.Shared
{
  
    internal class Permuter
    {
        // Fields
        private int[] dir;
        private int NN = 0;
        private int[] p;
        private int[][] p1 = new int[1][];
        private int[][] p2 = new int[2][];
        private int[][] permutations;
        private int[] pi;
        private int ptr;

        // Methods
        public Permuter()
        {
            Console.WriteLine("Enter n: ");
            int nN = int.Parse(Console.ReadLine());
            Console.WriteLine("\n");
            Permuter permuter = new Permuter(nN);
            for (int i = 0; i < permuter.GetNumberOfPermutations(); i++)
            {
                for (int j = 0; j < nN; j++)
                {
                    Console.Write(permuter.GetPermutation(i)[j]);
                }
                Console.WriteLine();
            }
        }
        public Permuter(int NN)
        {
            int num = factorial(NN);
            permutations = new int[num][];
            if (NN == 1)
            {
                permutations[0] = new int[] { 1 };
            }
            else if (NN == 2)
            {
                permutations[0] = new int[] { 1, 2 };
                permutations[1] = new int[] { 2, 1 };
            }
            else if (NN == 3)
            {
                permutations[0] = new int[] { 1, 2, 3 };
                permutations[1] = new int[] { 1, 3, 2 };
                permutations[2] = new int[] { 3, 1, 2 };
                permutations[3] = new int[] { 3, 2, 1 };
                permutations[4] = new int[] { 2, 3, 1 };
                permutations[5] = new int[] { 2, 1, 3 };
            }
            else if (NN == 4)
            {
                permutations[0] = new int[] { 1, 2, 3, 4 };
                permutations[1] = new int[] { 1, 2, 4, 3 };
                permutations[2] = new int[] { 1, 4, 2, 3 };
                permutations[3] = new int[] { 4, 1, 2, 3 };
                permutations[4] = new int[] { 4, 1, 3, 2 };
                permutations[5] = new int[] { 1, 4, 3, 2 };
                permutations[6] = new int[] { 1, 3, 4, 2 };
                permutations[7] = new int[] { 1, 3, 2, 4 };
                permutations[8] = new int[] { 3, 1, 2, 4 };
                permutations[9] = new int[] { 3, 1, 4, 2 };
                permutations[10] = new int[] { 3, 4, 1, 2 };
                permutations[11] = new int[] { 4, 3, 1, 2 };
                permutations[12] = new int[] { 4, 3, 2, 1 };
                permutations[13] = new int[] { 3, 4, 2, 1 };
                permutations[14] = new int[] { 3, 2, 4, 1 };
                permutations[15] = new int[] { 3, 2, 1, 4 };
                permutations[0x10] = new int[] { 2, 3, 1, 4 };
                permutations[0x11] = new int[] { 2, 3, 4, 1 };
                permutations[0x12] = new int[] { 2, 4, 3, 1 };
                permutations[0x13] = new int[] { 4, 2, 3, 1 };
                permutations[20] = new int[] { 4, 2, 1, 3 };
                permutations[0x15] = new int[] { 2, 4, 1, 3 };
                permutations[0x16] = new int[] { 2, 1, 4, 3 };
                permutations[0x17] = new int[] { 2, 1, 3, 4 };
            }
            else if (NN == 5)
            {
                permutations[0] = new int[] { 1, 2, 3, 4, 5 };
                permutations[1] = new int[] { 1, 2, 3, 5, 4 };
                permutations[2] = new int[] { 1, 2, 5, 3, 4 };
                permutations[3] = new int[] { 1, 5, 2, 3, 4 };
                permutations[4] = new int[] { 5, 1, 2, 3, 4 };
                permutations[5] = new int[] { 5, 1, 2, 4, 3 };
                permutations[6] = new int[] { 1, 5, 2, 4, 3 };
                permutations[7] = new int[] { 1, 2, 5, 4, 3 };
                permutations[8] = new int[] { 1, 2, 4, 5, 3 };
                permutations[9] = new int[] { 1, 2, 4, 3, 5 };
                permutations[10] = new int[] { 1, 4, 2, 3, 5 };
                permutations[11] = new int[] { 1, 4, 2, 5, 3 };
                permutations[12] = new int[] { 1, 4, 5, 2, 3 };
                permutations[13] = new int[] { 1, 5, 4, 2, 3 };
                permutations[14] = new int[] { 5, 1, 4, 2, 3 };
                permutations[15] = new int[] { 5, 4, 1, 2, 3 };
                permutations[0x10] = new int[] { 4, 5, 1, 2, 3 };
                permutations[0x11] = new int[] { 4, 1, 5, 2, 3 };
                permutations[0x12] = new int[] { 4, 1, 2, 5, 3 };
                permutations[0x13] = new int[] { 4, 1, 2, 3, 5 };
                permutations[20] = new int[] { 4, 1, 3, 2, 5 };
                permutations[0x15] = new int[] { 4, 1, 3, 5, 2 };
                permutations[0x16] = new int[] { 4, 1, 5, 3, 2 };
                permutations[0x17] = new int[] { 4, 5, 1, 3, 2 };
                permutations[0x18] = new int[] { 5, 4, 1, 3, 2 };
                permutations[0x19] = new int[] { 5, 1, 4, 3, 2 };
                permutations[0x1a] = new int[] { 1, 5, 4, 3, 2 };
                permutations[0x1b] = new int[] { 1, 4, 5, 3, 2 };
                permutations[0x1c] = new int[] { 1, 4, 3, 5, 2 };
                permutations[0x1d] = new int[] { 1, 4, 3, 2, 5 };
                permutations[30] = new int[] { 1, 3, 4, 2, 5 };
                permutations[0x1f] = new int[] { 1, 3, 4, 5, 2 };
                permutations[0x20] = new int[] { 1, 3, 5, 4, 2 };
                permutations[0x21] = new int[] { 1, 5, 3, 4, 2 };
                permutations[0x22] = new int[] { 5, 1, 3, 4, 2 };
                permutations[0x23] = new int[] { 5, 1, 3, 2, 4 };
                permutations[0x24] = new int[] { 1, 5, 3, 2, 4 };
                permutations[0x25] = new int[] { 1, 3, 5, 2, 4 };
                permutations[0x26] = new int[] { 1, 3, 2, 5, 4 };
                permutations[0x27] = new int[] { 1, 3, 2, 4, 5 };
                permutations[40] = new int[] { 3, 1, 2, 4, 5 };
                permutations[0x29] = new int[] { 3, 1, 2, 5, 4 };
                permutations[0x2a] = new int[] { 3, 1, 5, 2, 4 };
                permutations[0x2b] = new int[] { 3, 5, 1, 2, 4 };
                permutations[0x2c] = new int[] { 5, 3, 1, 2, 4 };
                permutations[0x2d] = new int[] { 5, 3, 1, 4, 2 };
                permutations[0x2e] = new int[] { 3, 5, 1, 4, 2 };
                permutations[0x2f] = new int[] { 3, 1, 5, 4, 2 };
                permutations[0x30] = new int[] { 3, 1, 4, 5, 2 };
                permutations[0x31] = new int[] { 3, 1, 4, 2, 5 };
                permutations[50] = new int[] { 3, 4, 1, 2, 5 };
                permutations[0x33] = new int[] { 3, 4, 1, 5, 2 };
                permutations[0x34] = new int[] { 3, 4, 5, 1, 2 };
                permutations[0x35] = new int[] { 3, 5, 4, 1, 2 };
                permutations[0x36] = new int[] { 5, 3, 4, 1, 2 };
                permutations[0x37] = new int[] { 5, 4, 3, 1, 2 };
                permutations[0x38] = new int[] { 4, 5, 3, 1, 2 };
                permutations[0x39] = new int[] { 4, 3, 5, 1, 2 };
                permutations[0x3a] = new int[] { 4, 3, 1, 5, 2 };
                permutations[0x3b] = new int[] { 4, 3, 1, 2, 5 };
                permutations[60] = new int[] { 4, 3, 2, 1, 5 };
                permutations[0x3d] = new int[] { 4, 3, 2, 5, 1 };
                permutations[0x3e] = new int[] { 4, 3, 5, 2, 1 };
                permutations[0x3f] = new int[] { 4, 5, 3, 2, 1 };
                permutations[0x40] = new int[] { 5, 4, 3, 2, 1 };
                permutations[0x41] = new int[] { 5, 3, 4, 2, 1 };
                permutations[0x42] = new int[] { 3, 5, 4, 2, 1 };
                permutations[0x43] = new int[] { 3, 4, 5, 2, 1 };
                permutations[0x44] = new int[] { 3, 4, 2, 5, 1 };
                permutations[0x45] = new int[] { 3, 4, 2, 1, 5 };
                permutations[70] = new int[] { 3, 2, 4, 1, 5 };
                permutations[0x47] = new int[] { 3, 2, 4, 5, 1 };
                permutations[0x48] = new int[] { 3, 2, 5, 4, 1 };
                permutations[0x49] = new int[] { 3, 5, 2, 4, 1 };
                permutations[0x4a] = new int[] { 5, 3, 2, 4, 1 };
                permutations[0x4b] = new int[] { 5, 3, 2, 1, 4 };
                permutations[0x4c] = new int[] { 3, 5, 2, 1, 4 };
                permutations[0x4d] = new int[] { 3, 2, 5, 1, 4 };
                permutations[0x4e] = new int[] { 3, 2, 1, 5, 4 };
                permutations[0x4f] = new int[] { 3, 2, 1, 4, 5 };
                permutations[80] = new int[] { 2, 3, 1, 4, 5 };
                permutations[0x51] = new int[] { 2, 3, 1, 5, 4 };
                permutations[0x52] = new int[] { 2, 3, 5, 1, 4 };
                permutations[0x53] = new int[] { 2, 5, 3, 1, 4 };
                permutations[0x54] = new int[] { 5, 2, 3, 1, 4 };
                permutations[0x55] = new int[] { 5, 2, 3, 4, 1 };
                permutations[0x56] = new int[] { 2, 5, 3, 4, 1 };
                permutations[0x57] = new int[] { 2, 3, 5, 4, 1 };
                permutations[0x58] = new int[] { 2, 3, 4, 5, 1 };
                permutations[0x59] = new int[] { 2, 3, 4, 1, 5 };
                permutations[90] = new int[] { 2, 4, 3, 1, 5 };
                permutations[0x5b] = new int[] { 2, 4, 3, 5, 1 };
                permutations[0x5c] = new int[] { 2, 4, 5, 3, 1 };
                permutations[0x5d] = new int[] { 2, 5, 4, 3, 1 };
                permutations[0x5e] = new int[] { 5, 2, 4, 3, 1 };
                permutations[0x5f] = new int[] { 5, 4, 2, 3, 1 };
                permutations[0x60] = new int[] { 4, 5, 2, 3, 1 };
                permutations[0x61] = new int[] { 4, 2, 5, 3, 1 };
                permutations[0x62] = new int[] { 4, 2, 3, 5, 1 };
                permutations[0x63] = new int[] { 4, 2, 3, 1, 5 };
                permutations[100] = new int[] { 4, 2, 1, 3, 5 };
                permutations[0x65] = new int[] { 4, 2, 1, 5, 3 };
                permutations[0x66] = new int[] { 4, 2, 5, 1, 3 };
                permutations[0x67] = new int[] { 4, 5, 2, 1, 3 };
                permutations[0x68] = new int[] { 5, 4, 2, 1, 3 };
                permutations[0x69] = new int[] { 5, 2, 4, 1, 3 };
                permutations[0x6a] = new int[] { 2, 5, 4, 1, 3 };
                permutations[0x6b] = new int[] { 2, 4, 5, 1, 3 };
                permutations[0x6c] = new int[] { 2, 4, 1, 5, 3 };
                permutations[0x6d] = new int[] { 2, 4, 1, 3, 5 };
                permutations[110] = new int[] { 2, 1, 4, 3, 5 };
                permutations[0x6f] = new int[] { 2, 1, 4, 5, 3 };
                permutations[0x70] = new int[] { 2, 1, 5, 4, 3 };
                permutations[0x71] = new int[] { 2, 5, 1, 4, 3 };
                permutations[0x72] = new int[] { 5, 2, 1, 4, 3 };
                permutations[0x73] = new int[] { 5, 2, 1, 3, 4 };
                permutations[0x74] = new int[] { 2, 5, 1, 3, 4 };
                permutations[0x75] = new int[] { 2, 1, 5, 3, 4 };
                permutations[0x76] = new int[] { 2, 1, 3, 5, 4 };
                permutations[0x77] = new int[] { 2, 1, 3, 4, 5 };
            }
            else
            {
                ptr = 0;
                p = new int[NN + 1];
                pi = new int[NN + 1];
                dir = new int[NN + 1];
#pragma warning disable 1717
                NN = NN;
#pragma warning disable 1717
                for (int i = 1; i <= NN; i++)
                {
                    dir[i] = -1;
                    p[i] = i;
                    pi[i] = i;
                }
                Perm(1);
            }
        }

        private int factorial(int n)
        {
            int num = 1;
            for (int i = n; i > 1; i--)
            {
                num *= i;
            }
            return num;
        }

        public int GetNumberOfPermutations()
        {
            return permutations.Length;
        }

        public int[] GetPermutation(int p)
        {
            if (p < permutations.Length)
            {
                return permutations[p];
            }
            return permutations[permutations.Length - 1];
        }

        //public static void Main(string[] args)
        //{
           
        //}

        private void Move(int x, int d)
        {
            int index = p[pi[x] + d];
            p[pi[x]] = index;
            p[pi[x] + d] = x;
            pi[index] = pi[x];
            pi[x] += d;
        }

        private void Perm(int n)
        {
            if (n > NN)
            {
                RecordPerm();
            }
            else
            {
                Perm(n + 1);
                for (int i = 1; i <= (n - 1); i++)
                {
                    Move(n, dir[n]);
                    Perm(n + 1);
                }
                dir[n] = -dir[n];
            }
        }

        private void PrintPerm()
        {
            for (int i = 1; i <= NN; i++)
            {
                Console.Write(p[i]);
            }
            Console.WriteLine();
        }

        private void RecordPerm()
        {
            permutations[ptr] = new int[NN];
            for (int i = 0; i < NN; i++)
            {
                permutations[ptr][i] = p[i + 1];
            }
            ptr++;
        }
    }
}

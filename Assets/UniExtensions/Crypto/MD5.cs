using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace UniExtensions.Crypto
{
    public class MD5
    {
        int[] rotate_amounts;
        long[] constants, init_values;
        List<System.Func<long, long, long, long>> functions;
        List<System.Func<int, int>> index_functions;

        public MD5 ()
        {
            rotate_amounts = new int[] {
                7, 12, 17, 22, 7, 12, 17, 22, 7, 12, 17, 22, 7, 12, 17, 22,
                5,  9, 14, 20, 5,  9, 14, 20, 5,  9, 14, 20, 5,  9, 14, 20,
                4, 11, 16, 23, 4, 11, 16, 23, 4, 11, 16, 23, 4, 11, 16, 23,
                6, 10, 15, 21, 6, 10, 15, 21, 6, 10, 15, 21, 6, 10, 15, 21
            };
            constants = (from i in Enumerable.Range (0, 64) select (long)(Math.Abs (Math.Sin (i + 1)) * Math.Pow (2, 32)) & 0xFFFFFFFF).ToArray ();
            init_values = new long[] {
                0x67452301,
                0xefcdab89,
                0x98badcfe,
                0x10325476
            };
            functions = new List<Func<long, long, long, long>> (64);
            System.Func<long, long, long, long> FA = (b,c,d) => (b & c) | (~b & d);
            for (var count=0; count<16; count++) {
                functions.Add (FA);
            }
            System.Func<long, long, long, long> FB = (b,c,d) => (d & b) | (~d & c);
            for (var count=0; count<16; count++) {
                functions.Add (FB);
            }
            System.Func<long, long, long, long> FC = (b,c,d) => b ^ c ^ d;
            for (var count=0; count<16; count++) {
                functions.Add (FC);
            }
            System.Func<long, long, long, long> FD = (b,c,d) => c ^ (b | ~d);
            for (var count=0; count<16; count++) {
                functions.Add (FD);
            }

            index_functions = new List<Func<int, int>> (64);
            System.Func<int, int> IFA = (i) => i;
            for (var count=0; count<16; count++) {
                index_functions.Add (IFA);
            }
            System.Func<int, int> IFB = (i) => (5 * i + 1) % 16;
            for (var count=0; count<16; count++) {
                index_functions.Add (IFB);
            }
            System.Func<int, int> IFC = (i) => (3 * i + 5) % 16;
            for (var count=0; count<16; count++) {
                index_functions.Add (IFC);
            }
            System.Func<int, int> IFD = (i) => (7 * i) % 16;
            for (var count=0; count<16; count++) {
                index_functions.Add (IFD);
            }
     
        }

        long LeftRotate (long x, int amount)
        {
            x &= 0xFFFFFFFF;
            return ((x << amount) | (x >> (32 - amount))) & 0xFFFFFFFF;
        }

        public string HexDigest (byte[] msg)
        {
            var ba = Digest(msg);
            StringBuilder hex = new StringBuilder (ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat ("{0:x2}", b);
            return hex.ToString().PadLeft (32, '0');
        }

        public byte[] Digest (byte[] msg)
        {
            var messageList = msg.ToList ();
            var orig_len_in_bits = (long)(8 * messageList.Count);
            messageList.Add (0x80);
            while (messageList.Count%64 != 56)
                messageList.Add (0);

            messageList.AddRange (System.BitConverter.GetBytes (orig_len_in_bits));

            var hash_pieces = init_values.ToArray ();
            var message = messageList.ToArray ();
            for (var chunk_ofst = 0; chunk_ofst < message.Length; chunk_ofst+=64) {
                var a = hash_pieces [0];
                var b = hash_pieces [1];
                var c = hash_pieces [2];
                var d = hash_pieces [3];
                var chunk = new System.ArraySegment<byte> (message, chunk_ofst, 64).Array;
                for (var i=0; i<64; i++) {
                    var f = functions [i] (b, c, d);
                    var g = index_functions [i] (i);
                    var to_rotate = a + f + constants [i] + System.BitConverter.ToInt32 (chunk, 4 * g);
                    var new_b = (b + LeftRotate (to_rotate, rotate_amounts [i])) & 0xFFFFFFFF;
                    a = d;
                    d = c;
                    c = b;
                    b = new_b;
                }
                var abcd = new long[] { a, b, c, d };
                for (int i = 0; i < 4; i++) {
                    var val = abcd [i];
                    hash_pieces [i] += val;
                    hash_pieces [i] &= 0xFFFFFFFF;
                }
            }

            var A = new BigInteger(hash_pieces [0]) << (32 * 0);    
            var B = new BigInteger(hash_pieces [1]) << (32 * 1);
            var C = new BigInteger(hash_pieces [2]) << (32 * 2);
            var D = new BigInteger(hash_pieces [3]) << (32 * 3);
            return (A + B + C + D).getBytes();

        }

    }
}

        
 


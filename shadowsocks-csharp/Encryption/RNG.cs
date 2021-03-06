﻿using System;
using System.Security.Cryptography;
using System.Linq;

namespace Shadowsocks.Encryption
{
    public static class RNG
    {
        private static RNGCryptoServiceProvider _rng = null;

        public static void Init()
        {
            if (_rng == null)
                _rng = new RNGCryptoServiceProvider();
        }

        public static void Close()
        {
            if (_rng == null) return;
            _rng.Dispose();
            _rng = null;
        }

        public static void Reload()
        {
            Close();
            Init();
        }

        public static void GetBytes(byte[] buf)
        {
            _rng.GetBytes(buf);
        }

        public static void GetBytes(byte[] buf, int len)
        {
            try
            {
                _rng.GetBytes(buf.Take(len).ToArray());
            }
            catch (Exception)
            {
                // the backup way
                byte[] tmp = new byte[len];
                _rng.GetBytes(tmp);
                Buffer.BlockCopy(tmp, 0, buf, 0, len);
            }
        }
    }
}
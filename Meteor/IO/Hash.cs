using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

/*
 * File : Hash
 * Author : Filipe
 * Date : 10/02/2014 21:10:07
 * Description :
 *
 */

namespace Meteor.IO
{
    public class Hash
    {
        /// <summary>
        /// Hash the input
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static String HashMd5(String input)
        {
            MD5 _md5Hash = MD5.Create();
            Byte[] _data = _md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder _result = new StringBuilder();
            for (int i = 0; i < _data.Length; i++)
            {
                _result.Append(_data[i].ToString("x2"));
            }
            return _result.ToString();
        }
    }
}

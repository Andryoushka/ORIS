using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZProtocol;

public class ZProtocolEncryptor
{
    private static string Key { get; } = "2e985f930";

    public static byte[] Encrypt(byte[] data)
    {
        return RijndaelHandler.Encrypt(data, Key);
    }

    public static byte[] Decrypt(byte[] data)
    {
        return RijndaelHandler.Decrypt(data, Key);
    }
}

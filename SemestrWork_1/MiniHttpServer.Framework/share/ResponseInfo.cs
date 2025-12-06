using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniHttpServer.Framework.share;

public class ResponseInfo
{
    public int StatusCode { get; set; }
    public string Redirect { get; set; }
    public string ContentType { get; set; }
    public int ContentLength { get { return _body.Length; } }
    public string Message { get; set; }
    private byte[] _body;
    public byte[] Body
    {
        set
        {
            string content = value.ToString();
            _body = Encoding.UTF8.GetBytes(content); ;
        }

        get
        {
            return _body;
        }
    }

}

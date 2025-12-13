using System;
using XProtocol;
using XProtocol.Serializator;

namespace Test
{
    internal class TestPacket
    {
        [XField(0)]
        public int TestNumber;

        [XField(1)]
        public double TestDouble;

        [XField(2)]
        public bool TestBoolean;

        [XField(3)]
        public string Text = "sample text";
    }

    internal class Program
    {
        private static void Main()
        {
            Console.Title = "";
            Console.ForegroundColor = ConsoleColor.White;

            var packet = XPacket.Create(0, 0);
            //packet.SetValue(0, 12345);
            var test = new TestPacket()
            {
                TestBoolean = true,
                TestNumber = 52,
                TestDouble = 1.67d
            };
            
            packet = XPacketConverter.Serialize(XPacketType.PointPlased, test);
            var encr = packet.Encrypt().ToPacket();
            var decr = XPacket.Parse(encr);

            var result = XPacketConverter.Deserialize<TestPacket>(decr);
            Console.WriteLine(result.TestNumber);
            Console.WriteLine(result.TestBoolean);
            Console.WriteLine(result.TestDouble);
            Console.WriteLine(result.Text);
            //Console.WriteLine(decr.GetValue<int>(0));

            Console.ReadLine();
        }
    }
}

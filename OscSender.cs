using System.Net.Sockets;
using System.Text;

namespace SimpleLauncherWinForms;

public static class OscSender
{
    public static byte[] BuildPacket(string address, int value)
    {
        static byte[] PadTo4(byte[] src)
        {
            int padded = (src.Length + 3) & ~3;
            var buf = new byte[padded];
            src.CopyTo(buf, 0);
            return buf;
        }

        var addrBytes  = PadTo4(Encoding.ASCII.GetBytes(address + '\0'));
        var tagBytes   = PadTo4(Encoding.ASCII.GetBytes(",i\0\0"));
        var valueBytes = new byte[]
        {
            (byte)(value >> 24), (byte)(value >> 16),
            (byte)(value >>  8), (byte)value,
        };

        var packet = new byte[addrBytes.Length + tagBytes.Length + 4];
        addrBytes.CopyTo(packet, 0);
        tagBytes.CopyTo(packet, addrBytes.Length);
        valueBytes.CopyTo(packet, addrBytes.Length + tagBytes.Length);
        return packet;
    }

    public static async Task SendAsync(string host, int port, string address, int value)
    {
        var packet = BuildPacket(address, value);
        using var udp = new UdpClient();
        await udp.SendAsync(packet, packet.Length, host, port);
    }
}

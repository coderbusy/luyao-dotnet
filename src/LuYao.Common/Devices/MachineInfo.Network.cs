using System;
using System.Linq;
using System.Net.NetworkInformation;

namespace LuYao.Devices;

/// <summary>
/// MachineInfo 的网络监控部分
/// </summary>
public partial class MachineInfo
{
    private Int64 _lastTime;
    private Int64 _lastSent;
    private Int64 _lastReceived;

    /// <summary>
    /// 刷新网络速度信息
    /// </summary>
    public void RefreshSpeed()
    {
        try
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();
            
            Int64 sent = 0;
            Int64 received = 0;

            foreach (var ni in interfaces)
            {
                // 只统计活动的物理网络接口
                if (ni.OperationalStatus == OperationalStatus.Up &&
                    ni.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                    ni.NetworkInterfaceType != NetworkInterfaceType.Tunnel)
                {
                    var stats = ni.GetIPv4Statistics();
                    sent += stats.BytesSent;
                    received += stats.BytesReceived;
                }
            }

            var now = (Int64)(DateTimeOffset.UtcNow - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds;

            if (_lastTime > 0)
            {
                var elapsed = now - _lastTime;
                if (elapsed > 0)
                {
                    var sentDiff = sent - _lastSent;
                    var receivedDiff = received - _lastReceived;

                    if (sentDiff >= 0)
                        UplinkSpeed = (UInt64)(sentDiff / elapsed);
                    if (receivedDiff >= 0)
                        DownlinkSpeed = (UInt64)(receivedDiff / elapsed);
                }
            }

            _lastSent = sent;
            _lastReceived = received;
            _lastTime = now;
        }
        catch
        {
            // 忽略网络统计错误
        }
    }
}

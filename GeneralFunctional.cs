﻿using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace HustonRTEMS {
    [StructLayout(LayoutKind.Explicit)]
    public struct fl_un {
        [FieldOffset(0)]
        public byte byte1;
        [FieldOffset(1)]
        public byte byte2;
        [FieldOffset(2)]
        public byte byte3;
        [FieldOffset(3)]
        public byte byte4;

        [FieldOffset(0)]
        public float fl1;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct it_un {
        [FieldOffset(0)]
        public byte byte1;
        [FieldOffset(1)]
        public byte byte2;

        [FieldOffset(0)]
        public int it1;
    }
}

namespace HustonRTEMS {
    internal class GeneralFunctional {
        public void ChangingPositionAccelerometer() {
            int jon = 5;
            for(int i = 0; i < 5; i++) {
                Thread.Sleep(100);
                jon++;
            }
        }

        public async void SendMessageInSocket(Socket serverListener, fl_un fl, it_un it,
            DefaultTransmission DT, byte[] hardBufWrite, float value, TextBox logBox) {
            if(serverListener != null && serverListener.Connected) {
                it.it1 = DT.temperatureTransmission.TAddres.addres;
                hardBufWrite[20] = it.byte1;
                hardBufWrite[21] = it.byte2;
                fl.fl1 = value;
                hardBufWrite[26] = fl.byte1;
                hardBufWrite[27] = fl.byte2;
                hardBufWrite[28] = fl.byte3;
                hardBufWrite[29] = fl.byte4;
                _ = await serverListener.SendAsync(hardBufWrite, SocketFlags.None);
            } else {
                logBox.Text = "Socet don't open!";
            }
        }
    }
}

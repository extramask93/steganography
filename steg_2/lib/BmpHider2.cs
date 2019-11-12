using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace steg_2.lib
{
    class BmpHider2 : IBmpHider
    {
        public BmpHider2()
        {
        }
        unsafe public byte[] get_message(Bitmap bm)
        {
            uint message_length = 0;
            int k = 0;
            BitmapData data = bm.LockBits(
                new System.Drawing.Rectangle(0, 0, bm.Width, bm.Height),
                ImageLockMode.ReadWrite, bm.PixelFormat);
            byte* ptr = (byte*)data.Scan0;
            /*get message length*/
            for (int i = 0; i < 32; i+=2)
            {

                byte X1 = (byte)((*ptr ^ *(ptr + 2)) & 0x01);
                byte X2 = (byte)((*(ptr + 1) ^ *(ptr + 2)) & 0x01);
                message_length |= (uint)((X1 & 0x01) << i);
                message_length |= (uint)((X2 & 0x01) << (i+1));
                ptr+=3;
            }
            /*get message*/
            byte[] message = new byte[message_length];
            for (int i = 0; i < message_length;)
            {
                byte X1 = (byte)((*ptr ^ *(ptr + 2)) & 0x01);
                byte X2 = (byte)((*(ptr + 1) ^ *(ptr + 2)) & 0x01);
                message[i] |= (byte)((X1) << k++);
                message[i] |= (byte)((X2) << k++);
                ptr +=3;
                if (k >= 8)
                {
                    k = 0;
                    i++;
                }
            }
            bm.UnlockBits(data);
            return message;
        }
        unsafe public Bitmap embed_message(Bitmap bm, byte[] message)
        {
            uint message_length = (uint)message.Length;

            BitmapData data = bm.LockBits(
                new System.Drawing.Rectangle(0, 0, bm.Width, bm.Height),
                ImageLockMode.ReadWrite, bm.PixelFormat);
            byte* ptr = (byte*)data.Scan0;
            byte* end = ptr + (data.Stride * data.Height * 3);
            if ((message_length + 4) > (data.Stride * data.Height * 3))
            {
                return null;
            }
            /*set message length*/
            for (int i = 0; i < 32; i +=2)
            {
                byte X1 = (byte)((message_length >> i) & 0x01);
                byte X2 = (byte)((message_length >> i+1) & 0x01);
                byte XORAC = (byte)((*ptr ^ *(ptr+2)) & 0x01);
                byte XORBC = (byte)((*(ptr+1) ^ *(ptr+2)) & 0x01);
                if (XORAC == X1 && XORBC == X2)
                {
                    //change nothing
                }
                else if(XORAC != X1 && XORBC == X2)
                {
                    *ptr ^= 0x01;
                }
                else if(XORAC == X1 && XORBC != X2)
                {
                    *(ptr + 1) ^= 0x01;
                }
                else
                {
                    *(ptr + 2) ^= 0x1;
                };
                ptr += 3;
            }
            /*set message*/
            for (int i = 0; i < message_length; i++)
            {
                for (int j = 0; j < 8; j+=2)
                {

                    byte X1 = (byte)((message[i] >> j) & 0x01);
                    byte X2 = (byte)((message[i] >> j + 1) & 0x01);
                    byte XORAC = (byte)((*ptr ^ *(ptr + 2)) & 0x01);
                    byte XORBC = (byte)((*(ptr + 1) ^ *(ptr + 2)) & 0x01);
                    if (XORAC == X1 && XORBC == X2)
                    {
                        //change nothing
                    }
                    else if (XORAC != X1 && XORBC == X2)
                    {
                        *ptr ^= 0x01;
                    }
                    else if (XORAC == X1 && XORBC != X2)
                    {
                        *(ptr + 1) ^= 0x01;
                    }
                    else
                    {
                        *(ptr + 2) ^= 0x1;
                    };
                    ptr += 3;
                }
            }
            bm.UnlockBits(data);
            return bm;

        }

        public void setKey(string key)
        {
            throw new NotImplementedException();
        }
    }
}

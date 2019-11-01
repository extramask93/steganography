using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace steg_2.lib
{
    class BmpHider: IBmpHider
    {

        public BmpHider()
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
            for (int i = 0; i < 32; i++)
            {
                message_length |= (uint)((*ptr & 0x01) << i);
                ptr++;
            }
            /*get message*/
            byte[] message = new byte[message_length];
            for (int i = 0; i < message_length;)
            {
                message[i] |= (byte)((*ptr & 0x01) << k++);
                ptr++;
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
            for (int i = 0; i < 32; i++)
            {
                *ptr &= 0xFE;
                *ptr |= (byte)((message_length >> i) & 0x01);
                ptr++;
            }
            /*set message*/
            for (int i = 0; i < message_length; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    *ptr &= 0xFE;
                    *ptr |= (byte)((message[i] >> j) & 0x01);
                    ptr++;
                }
            }
            bm.UnlockBits(data);
            return bm;

        }

    }
}

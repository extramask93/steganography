using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace steg_2.lib
{
    class BmpHider3 : IBmpHider
    {
        string key = null;

        public string Key { get => key; set => key = value; }


        private byte[] apply_error_correction(byte[] message)
        {
            byte[] result = new byte[message.Length * 5];
            int j = 0;
            for(int i =0; i < message.Length; i++)
            {
                int offset = 0;
                for (int k = 0; k < 8; k++)
                {              
                    for(int p = 0; p < 5; p++)
                    {
                        result[j] |= (byte)(((message[i] >> k) & 0x1) << (offset++));
                        if (offset > 7)
                        {
                            j++;
                            offset = 0;
                        }                   
                    }
                }
            }
            return result;
        }
        private byte[] decode_with_error_correction(byte[] message)
        {
            byte[] result = new byte[message.Length / 5];
            int offset = 0;
            int p = 0;
            int b = 0;
            for(int k=0; k < message.Length; )
            {
                int ones = 0;
                for (int j = 0; j<5;j++)
                {
                    if((message[k] >> offset & 0x1) == 0x1) ones++;
                    offset++;
                    if (offset > 7)
                    {
                        k++;
                        offset = 0;
                    }
                }
                if(ones >= 3)
                {
                    result[p] |=(byte)(0x1 << b); 
                }
                b++;
                if (b > 7)
                {
                    b = 0;
                    p++;
                }
            }
            return result;
        }
        unsafe public byte[] get_message(Bitmap bm)
        {
            uint message_length = 0;
            int k = 0;
            BitmapData data = bm.LockBits(
                new System.Drawing.Rectangle(0, 0, bm.Width, bm.Height),
                ImageLockMode.ReadWrite, bm.PixelFormat);
            byte* ptr = (byte*)data.Scan0;
            var rand = new RemRandom(Key, data.Width*data.Height*3);
            int[] lookup = new int[3];
            /*get message length*/
            for (int i = 0; i < 32; i += 2)
            {
                lookup[0] = rand.Next(); lookup[1] = rand.Next(); lookup[2] = rand.Next();
                byte X1 = (byte)((*(ptr + lookup[0]) ^ *(ptr + lookup[2])) & 0x01);
                byte X2 = (byte)((*(ptr + lookup[1]) ^ *(ptr + lookup[2])) & 0x01);
                message_length |= (uint)((X1 & 0x01) << i);
                message_length |= (uint)((X2 & 0x01) << (i + 1));
                //ptr += 3;
            }
            /*get message*/
            byte[] message = new byte[message_length];
            for (int i = 0; i < message_length;)
            {
                lookup[0] = rand.Next(); lookup[1] = rand.Next(); lookup[2] = rand.Next();
                byte X1 = (byte)((*(ptr + lookup[0]) ^ *(ptr + lookup[2])) & 0x01);
                byte X2 = (byte)((*(ptr + lookup[1]) ^ *(ptr + lookup[2])) & 0x01);
                message[i] |= (byte)((X1) << k++);
                message[i] |= (byte)((X2) << k++);
                //ptr += 3;
                if (k >= 8)
                {
                    k = 0;
                    i++;
                }
            }
            bm.UnlockBits(data);
            var message_ = decode_with_error_correction(message);
            return message_;
        }
        unsafe public Bitmap embed_message(Bitmap bm, byte[] message_)
        {
            var message = apply_error_correction(message_);
            uint message_length = (uint)message.Length;
            BitmapData data = bm.LockBits(
                new System.Drawing.Rectangle(0, 0, bm.Width, bm.Height),
                ImageLockMode.ReadWrite, bm.PixelFormat);

            byte* ptr = (byte*)data.Scan0;
            var rand = new RemRandom(Key, data.Width * data.Height * 3);
            int[] lookup = new int[3];
            byte* end = ptr + (data.Stride * data.Height * 3);
            if ((message_length + 4) > (data.Stride * data.Height * 3))
            {
                return null;
            }
            /*set message length*/
            for (int i = 0; i < 32; i += 2)
            {
                lookup[0] = rand.Next(); lookup[1] = rand.Next(); lookup[2] = rand.Next();
                byte X1 = (byte)((message_length >> i) & 0x01);
                byte X2 = (byte)((message_length >> i + 1) & 0x01);
                byte XORAC = (byte)((*(ptr + lookup[0]) ^ *(ptr + lookup[2])) & 0x01);
                byte XORBC = (byte)((*(ptr + lookup[1]) ^ *(ptr + lookup[2])) & 0x01);
                if (XORAC == X1 && XORBC == X2)
                {
                    //change nothing
                }
                else if (XORAC != X1 && XORBC == X2)
                {
                    *(ptr+lookup[0]) ^= 0x01;
                }
                else if (XORAC == X1 && XORBC != X2)
                {
                    *(ptr + lookup[1]) ^= 0x01;
                }
                else
                {
                    *(ptr + lookup[2]) ^= 0x1;
                };
                //ptr += 3;
            }
            /*set message*/
            for (int i = 0; i < message_length; i++)
            {
                for (int j = 0; j < 8; j += 2)
                {
                    lookup[0] = rand.Next(); lookup[1] = rand.Next(); lookup[2] = rand.Next();
                    byte X1 = (byte)((message[i] >> j) & 0x01);
                    byte X2 = (byte)((message[i] >> j + 1) & 0x01);
                    byte XORAC = (byte)((*(ptr+lookup[0]) ^ *(ptr + lookup[2])) & 0x01);
                    byte XORBC = (byte)((*(ptr + lookup[1]) ^ *(ptr + lookup[2])) & 0x01);
                    if (XORAC == X1 && XORBC == X2)
                    {
                        //change nothing
                    }
                    else if (XORAC != X1 && XORBC == X2)
                    {
                        *(ptr+lookup[0]) ^= 0x01;
                    }
                    else if (XORAC == X1 && XORBC != X2)
                    {
                        *(ptr + lookup[1]) ^= 0x01;
                    }
                    else
                    {
                        *(ptr + lookup[2]) ^= 0x1;
                    };
                    //ptr += 3;
                }
            }
            bm.UnlockBits(data);
            return bm;

        }

        public void setKey(string key)
        {
            this.Key = key;
        }
    }
}

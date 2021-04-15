using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace HoneywellRfId.Utils
{
    public static class StrUtil
    {
        private const string TAG = "StrUtil";

        public static string ToHexString(byte[] bArray)
        {
            StringBuilder sb = new StringBuilder();
            int c;

            foreach (byte b in bArray)
            {
                c = b & 0xff;

                if (c < 0x10)
                {
                    sb.Append("0");
                }

                sb.Append(c.ToString("X"));
            }

            return sb.ToString();
        }

        public static String ToHexString(byte[] bArray, int start, int len)
        {
            return ToHexString(bArray, start, len, null);
        }

        public static String ToHexString(byte[] bArray, int start, int len,
                                         string separator)
        {
            if (bArray.Length < start + len)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder();
            int c;

            for (int i = start; i < len; i++)
            {
                c = bArray[i] & 0xff;

                if (c < 0x10)
                {
                    sb.Append("0");
                }

                sb.Append(c.ToString("X"));

                if (separator != null)
                {
                    sb.Append(separator);
                }
            }

            return sb.ToString();
        }

       /* public static String UrlDecode(String str)
        {
            return UrlDecode(str, "UTF-8");
        }

        public static String UrlDecode(String str, string encode)
        {
            String result = "";

            if (null == str)
            {
                return null;
            }

            try
            {
                result = java.net.URLDecoder.decode(str, encode);
            }
            catch (UnsupportedEncodingException e)
            {
                e.printStackTrace();
            }

            return result;
        }

        public static String urlEncode(String str)
        {
            return urlEncode(str, "UTF-8");
        }

        public static String urlEncode(String str, String encode)
        {
            String result = "";

            if (null == str)
            {
                return null;
            }

            try
            {
                result = java.net.URLEncoder.encode(str, encode);
            }
            catch (UnsupportedEncodingException e)
            {
                e.printStackTrace();
            }

            return result;
        }

        public static String getFileNameFromUrl(String urlStr)
        {
            if (urlStr == null)
            {
                return null;
            }

            String[] strs = urlStr.split("/");

            if (strs.length < 2)
            {
                return null;
            }

            String fileName = strs[strs.length - 1];
            return fileName;
        }

        public static String getTimeString(int type, Date date)
        {
            if (type == 0)
            {
                return getTimeString("HH:mm:ss.SSS", date);
            }

            return null;
        }

        public static String getTimeString(String formatStr, Date date)
        {
            Calendar calendar = Calendar.getInstance();
            SimpleDateFormat format = new SimpleDateFormat(formatStr);
            return format.format(calendar.getTime());
        }

        public static String getUuid()
        {
            return UUID.randomUUID().toString().replace("-", "");
        }

        public static boolean isEmpty(String s)
        {
            return s == null || s.isEmpty();
        }
       */
    }


}
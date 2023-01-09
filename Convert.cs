using System;
using System.Collections.Generic;
using System.Text;

namespace VSim
{
    static class Convert
    {
        public static bool isAcceptableDouble(string s)
        {
            for (int i = 0; i < s.Length; ++i)
            {
                if (!((s[i] <= '9' && s[i] >= '0') || (s[i] == '.') || (s[i] == '-' && i == 0)))
                {
                    return false;
                }
            }
            return true;
        }
        public static double convertToDouble(string s)
        {
            double dbsup = 0.0;
            double sgn = 1.0;

            int j = -1;
            for (int i = 0; i < s.Length; ++i)
            {
                if (i == 0 && s[i] == '-')
                {
                    sgn = -1.0;
                }
                else
                {
                    if (s[i] >= '0' && s[i] <= '9')
                    {
                        dbsup *= 10.0;
                        dbsup += (double)((int)(s[i] - '0'));
                    }
                    else if (s[i] == '.')
                    {
                        j = i;
                        break;
                    }
                }
            }

            double dbinf = 0.0;
            if (j != -1)
            {
                for (int i = s.Length - 1; i >= j + 1; --i)
                {
                    dbinf += (double)((int)(s[i] - '0'));
                    dbinf /= 10.0;
                }
            }

            double db = dbinf + dbsup;
            db *= sgn;

            return db;
        }
        public static string str(double d)
        {
            string s = d.ToString().Replace(',', '.');

            if (!s.Contains('.'))
            {
                s += ".0";
            }
            return s;
        }
        public static string str(int itg)
        {
            string s = itg.ToString();
            return s;
        }

        public static bool isAcceptableInteger(string s)
        {
            for (int i = 0; i < s.Length; ++i)
            {
                if (!(s[i] <= '9' && s[i] >= '0'))
                {
                    return false;
                }
            }
            return true;
        }
        public static int convertToInteger(string s)
        {
            int itg = 0;
            for (int i = 0; i < s.Length; ++i)
            {
                itg *= 10;
                itg += (int)(s[i] - '0');
            }

            return itg;
        }
    }
}

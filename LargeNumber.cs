using System;
using System.Collections.Generic;
using System.Linq;

namespace DiscordArithmeticBot.Modules
{
    public struct LargeNumber : IComparable, IComparable<LargeNumber>, IEquatable<LargeNumber>
    {
        #region Private Fields

        private const string EMPTY_STRING = "";
        private const string HIGH_PRECISION_FORMAT = "###############################################################################################################################################################################################################";

        #endregion Private Fields

        #region Internal Fields

        internal List<double> m_list_decimal;
        internal List<double> m_list_whole;

        private static int blockLength = 100;

        #endregion Internal Fields

        #region Public Constructors

        public LargeNumber(String x)
        {
            if (x == EMPTY_STRING)
            {
                m_list_whole = new List<double>();
                m_list_decimal = new List<double>();
                return;
            }
            if (!(x.Contains(".")))
            {
                List<double> wholeList = new List<double>();
                int i = 1;
                int length = x.Length;
                while (i * blockLength < length)
                {
                    String sub = x.Substring(x.Length - blockLength);
                    wholeList.Add(Double.Parse(sub));
                    x = x.Remove(x.Length - blockLength, blockLength);
                    i++;
                }
                wholeList.Add(Double.Parse(x));
                wholeList.Reverse();
                m_list_whole = wholeList;
                m_list_decimal = new List<double>();
            }
            else
            {
                String[] array = x.Split('.');

                String whole = array[0];
                String decimal_ = array[1];

                List<double> wholeList = new List<double>();
                int i = 1;
                int length = whole.Length;
                while (i * blockLength < length)
                {
                    String sub = whole.Substring(whole.Length - blockLength);
                    wholeList.Add(Double.Parse(sub));
                    whole = whole.Remove(whole.Length - blockLength, blockLength);
                    i++;
                }
                wholeList.Add(Double.Parse(whole));
                wholeList.Reverse();

                List<double> decimalList = new List<double>();
                int j = 1;
                length = decimal_.Length;
                while (j * blockLength < length)
                {
                    String sub = decimal_.Substring(decimal_.Length - blockLength);
                    decimalList.Add(Double.Parse(sub));
                    decimal_ = decimal_.Remove(decimal_.Length - blockLength, blockLength);
                    j++;
                }
                if (decimal_.Length < blockLength)
                {
                    for (int a = 0; a < blockLength - decimal_.Length; a++)
                    {
                        decimal_ = decimal_ + "0";
                    }
                }
                decimalList.Add(Double.Parse(decimal_));

                m_list_whole = wholeList;
                m_list_decimal = decimalList;
            }
        }

        #endregion Public Constructors

        #region Public Methods

        public static LargeNumber Add(LargeNumber x, LargeNumber y) => x + y;

        public static String Comma(String x)
        {
            if (x.Contains("."))
            {
                String[] parts = x.Split('.');
                x = parts[0];
                List<String> stringList = new List<String>();
                int i = 1;
                int length = x.Length;
                while (i * 3 < length)
                {
                    String sub = x.Substring(x.Length - 3);
                    stringList.Add(sub);
                    x = x.Remove(x.Length - 3, 3);
                    i++;
                }
                stringList.Add(x);
                stringList.Reverse();
                String result = String.Join(",", stringList);
                String realResult = result + "." + parts[1];
                StripZeros(realResult);
                return realResult;
            }
            else
            {
                List<String> stringList = new List<String>();
                int i = 1;
                int length = x.Length;
                while (i * 3 < length)
                {
                    String sub = x.Substring(x.Length - 3);
                    stringList.Add(sub);
                    x = x.Remove(x.Length - 3, 3);
                    i++;
                }
                stringList.Add(x);
                stringList.Reverse();
                String result = String.Join(",", stringList);
                StripZeros(result);
                return result;
            }
        }

        public static String Format(String x) => Comma(x);

        public static bool operator !=(LargeNumber left, LargeNumber right) => !left.Equals(right);

        public static LargeNumber operator +(LargeNumber x, LargeNumber y)
        {
            if (x.m_list_decimal.Count == 0 && y.m_list_decimal.Count == 0) // if we dont have any decimal places
            {
                if (x.m_list_whole.Count <= 0 && y.m_list_whole.Count <= 0) return new LargeNumber("0");
                else if (x.m_list_whole.Count == 0)
                {
                    return y;
                }
                else if (y.m_list_whole.Count == 0)
                {
                    return x;
                }
                else if (x.m_list_whole.Count == 1 && y.m_list_whole.Count == 1) //if each list has only one double
                {
                    LargeNumber wholeSum = new LargeNumber(EMPTY_STRING);
                    if (y.m_list_whole[0] + x.m_list_whole[0] >= Math.Pow(10, blockLength))
                    {
                        wholeSum.m_list_whole.Add(y.m_list_whole[0] + x.m_list_whole[0] - Math.Pow(10, blockLength));
                        wholeSum.m_list_whole.Add(1);
                        wholeSum.m_list_whole.Reverse();
                    }
                    else
                    {
                        wholeSum.m_list_whole.Add(y.m_list_whole[0] + x.m_list_whole[0]);
                    }
                    return wholeSum;
                }
                else // in all other cases
                {
                    List<double> wholeListX = x.m_list_whole;
                    wholeListX.Reverse();

                    List<double> wholeListY = y.m_list_whole;
                    wholeListY.Reverse();

                    //checking to make sure each list is the same length, if not we add zeros
                    if (wholeListX.Count > wholeListY.Count)
                    {
                        for (int i = 0; i < wholeListX.Count - wholeListY.Count; i++)
                        {
                            wholeListY.Add(0);
                        }
                    }
                    else if (wholeListY.Count > wholeListX.Count)
                    {
                        for (int i = 0; i < wholeListY.Count - wholeListX.Count; i++)
                        {
                            wholeListX.Add(0);
                        }
                    }

                    int carry = 0;
                    LargeNumber wholeSum = new LargeNumber(EMPTY_STRING);
                    for (int i = 0; i < wholeListY.Count - 1; i++)
                    {
                        if (wholeListY[i] + wholeListX[i] + carry >= Math.Pow(10, blockLength))
                        {
                            wholeSum.m_list_whole.Add(wholeListY[i] + wholeListX[i] + carry - Math.Pow(10, blockLength));
                            carry = 1;
                            if (wholeSum.m_list_whole[i].ToString().Length < blockLength)
                            {
                                String temp = wholeSum.m_list_whole[i].ToString();
                                for (int h = 0; h < blockLength - wholeSum.m_list_whole[i].ToString().Length; h++)
                                {
                                    temp = "0" + temp;
                                }
                                wholeSum.m_list_whole[i] = Double.Parse(temp);
                            }
                        }
                        else
                        {
                            carry = 0;
                            wholeSum.m_list_whole.Add(wholeListY[i] + wholeListX[i] + carry);
                            if (wholeSum.m_list_whole[i].ToString().Length < blockLength)
                            {
                                String temp = wholeSum.m_list_whole[i].ToString();
                                for (int h = 0; h < blockLength - wholeSum.m_list_whole[i].ToString().Length; h++)
                                {
                                    temp = "0" + temp;
                                }
                                wholeSum.m_list_whole[i] = Double.Parse(temp);
                            }
                        }
                    }
                    int a = wholeListY.Count - 1;
                    if (y.m_list_whole[a] + x.m_list_whole[a] >= Math.Pow(10, blockLength))
                    {
                        wholeSum.m_list_whole.Add(y.m_list_whole[a] + x.m_list_whole[a] - Math.Pow(10, blockLength));
                        wholeSum.m_list_whole.Add(1);
                        wholeSum.m_list_whole.Reverse();
                    }
                    else
                    {
                        wholeSum.m_list_whole.Add(y.m_list_whole[a] + x.m_list_whole[a]);
                        wholeSum.m_list_whole.Reverse();
                    }
                    return wholeSum;
                }
            }
            else
            {
                if ((x.m_list_whole.Count <= 0 && x.m_list_decimal.Count <= 0) && (y.m_list_decimal.Count <= 0 && y.m_list_whole.Count <= 0)) return new LargeNumber("0");
                List<double> decimalListX = x.m_list_decimal;
                decimalListX.Reverse();

                List<double> decimalListY = y.m_list_decimal;
                decimalListY.Reverse();

                //checking to make sure each list is the same length, if not we add zeros
                if (decimalListX.Count > decimalListY.Count)
                {
                    for (int i = 0; i < decimalListX.Count - decimalListY.Count; i++)
                    {
                        decimalListY.Add(0);
                    }
                }
                else if (decimalListY.Count > decimalListX.Count)
                {
                    for (int i = 0; i < decimalListY.Count - decimalListX.Count; i++)
                    {
                        decimalListX.Add(0);
                    }
                }

                int carry = 0;
                LargeNumber decimalSum = new LargeNumber(EMPTY_STRING);
                for (int i = 0; i < decimalListY.Count - 1; i++)
                {
                    if (decimalListY[i] + decimalListX[i] + carry >= Math.Pow(10, blockLength))
                    {
                        decimalSum.m_list_decimal.Add(decimalListY[i] + decimalListX[i] + carry - Math.Pow(10, blockLength));
                        carry = 1;
                        if (decimalSum.m_list_decimal[i].ToString().Length < blockLength)
                        {
                            String temp = decimalSum.m_list_decimal[i].ToString();
                            for (int h = 0; h < blockLength - decimalSum.m_list_decimal[i].ToString().Length; h++)
                            {
                                temp = "0" + temp;
                            }
                            decimalSum.m_list_decimal[i] = Double.Parse(temp);
                        }
                    }
                    else
                    {
                        carry = 0;
                        decimalSum.m_list_decimal.Add(decimalListY[i] + decimalListX[i] + carry);
                        if (decimalSum.m_list_decimal[i].ToString().Length < blockLength)
                        {
                            String temp = decimalSum.m_list_decimal[i].ToString();
                            for (int h = 0; h < blockLength - decimalSum.m_list_decimal[i].ToString().Length; h++)
                            {
                                temp = "0" + temp;
                            }
                            decimalSum.m_list_decimal[i] = Double.Parse(temp);
                        }
                    }
                }
                int a = decimalListY.Count - 1;
                if (y.m_list_decimal[a] + x.m_list_decimal[a] >= Math.Pow(10, blockLength))
                {
                    decimalSum.m_list_decimal[a] = y.m_list_decimal[a] + x.m_list_decimal[a] + carry - Math.Pow(10, blockLength);
                    carry = 1;
                    decimalSum.m_list_decimal.Reverse();
                }
                else
                {
                    decimalSum.m_list_decimal[a] = y.m_list_decimal[a] + x.m_list_decimal[a] + carry;
                    carry = 0;
                    decimalSum.m_list_whole.Reverse();
                }

                if (x.m_list_whole.Count == 1 && y.m_list_whole.Count == 1) //if each list has only one double
                {
                    LargeNumber wholeSum = new LargeNumber(EMPTY_STRING);
                    if (y.m_list_whole[0] + x.m_list_whole[0] >= Math.Pow(10, blockLength))
                    {
                        wholeSum.m_list_whole.Add(y.m_list_whole[0] + x.m_list_whole[0] + carry - Math.Pow(10, blockLength));
                        wholeSum.m_list_whole.Add(1);
                        wholeSum.m_list_whole.Reverse();
                    }
                    else
                    {
                        wholeSum.m_list_whole.Add(y.m_list_whole[0] + x.m_list_whole[0] + carry);
                    }
                    return wholeSum;
                }
                else // in all other cases
                {
                    List<double> wholeListX = x.m_list_whole;
                    wholeListX.Reverse();

                    List<double> wholeListY = y.m_list_whole;
                    wholeListY.Reverse();

                    //checking to make sure each list is the same length, if not we add zeros
                    if (wholeListX.Count > wholeListY.Count)
                    {
                        for (int i = 0; i < wholeListX.Count - wholeListY.Count; i++)
                        {
                            wholeListY.Add(0);
                        }
                    }
                    else if (wholeListY.Count > wholeListX.Count)
                    {
                        for (int i = 0; i < wholeListY.Count - wholeListX.Count; i++)
                        {
                            wholeListX.Add(0);
                        }
                    }

                    LargeNumber wholeSum = new LargeNumber(EMPTY_STRING);
                    for (int i = 0; i < wholeListY.Count - 1; i++)
                    {
                        if (wholeListY[i] + wholeListX[i] + carry >= Math.Pow(10, blockLength))
                        {
                            wholeSum.m_list_whole.Add(wholeListY[i] + wholeListX[i] + carry - Math.Pow(10, blockLength));
                            carry = 1;
                            if (wholeSum.m_list_whole[i].ToString().Length < blockLength)
                            {
                                String temp = wholeSum.m_list_whole[i].ToString();
                                for (int h = 0; h < blockLength - wholeSum.m_list_whole[i].ToString().Length; h++)
                                {
                                    temp = "0" + temp;
                                }
                                wholeSum.m_list_whole[i] = Double.Parse(temp);
                            }
                        }
                        else
                        {
                            carry = 0;
                            wholeSum.m_list_whole.Add(wholeListY[i] + wholeListX[i] + carry);
                            if (wholeSum.m_list_whole[i].ToString().Length < blockLength)
                            {
                                String temp = wholeSum.m_list_whole[i].ToString();
                                for (int h = 0; h < blockLength - wholeSum.m_list_whole[i].ToString().Length; h++)
                                {
                                    temp = "0" + temp;
                                }
                                wholeSum.m_list_whole[i] = Double.Parse(temp);
                            }
                        }
                    }
                    a = wholeListY.Count - 1;
                    if (y.m_list_whole[a] + x.m_list_whole[a] >= Math.Pow(10, blockLength))
                    {
                        wholeSum.m_list_whole.Add(y.m_list_whole[a] + x.m_list_whole[a] - Math.Pow(10, blockLength));
                        wholeSum.m_list_whole.Add(1);
                        wholeSum.m_list_whole.Reverse();
                    }
                    else
                    {
                        wholeSum.m_list_whole.Add(y.m_list_whole[a] + x.m_list_whole[a]);
                        wholeSum.m_list_whole.Reverse();
                    }

                    wholeSum.m_list_whole.AddRange(decimalSum.m_list_decimal);

                    return wholeSum;
                }
            }
        }

        public static bool operator <(LargeNumber left, LargeNumber right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(LargeNumber left, LargeNumber right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator ==(LargeNumber left, LargeNumber right)
        {
            return left.Equals(right);
        }

        public static bool operator >(LargeNumber left, LargeNumber right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(LargeNumber left, LargeNumber right)
        {
            return left.CompareTo(right) >= 0;
        }

        public static void StripZeros(String x)
        {
            while (x.First() == '0')
            {
                x.Remove(0, 1);
            }
            while (x.Last() == '0')
            {
                x.Remove(x.Length - 1, 1);
            }
            if (x.Last() == '.')
            {
                x.Remove(x.Length - 1, 1);
            }
        }

        public int CompareTo(LargeNumber other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        public bool Equals(LargeNumber other)
        {
            if (ToString() == other.ToString()) return true;
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (!(obj is LargeNumber))
                return false;
            return Equals((LargeNumber)obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public String LargeNumberString()
        {
            String result = EMPTY_STRING;
            if (m_list_decimal.Count != 0)
            {
                foreach (double item in m_list_whole)
                {
                    String itemString = item.ToString(HIGH_PRECISION_FORMAT);
                    if (itemString.Length < 200)
                    {
                        for (int i = 0; i < 200 - (itemString.Length); i++)
                        {
                            itemString = "0" + itemString;
                        }
                    }

                    result = result + itemString;
                }
                result = result + ".";
                foreach (double item in m_list_decimal)
                {
                    String itemString = item.ToString(HIGH_PRECISION_FORMAT);
                    if (itemString.Length < 200)
                    {
                        for (int i = 0; i < 200 - (itemString.Length); i++)
                        {
                            itemString = "0" + itemString;
                        }
                    }

                    result = result + itemString;
                }
            }
            else
            {
                foreach (double item in m_list_whole)
                {
                    String itemString = item.ToString(HIGH_PRECISION_FORMAT);
                    if (itemString.Length < 200)
                        for (int i = 0; i < 200 - (itemString.Length); i++)
                        {
                            itemString = "0" + itemString;
                        }
                    result = result + itemString;
                }
            }
            return Format(result);
        }

        public override string ToString()
        {
            return LargeNumberString();
        }

        #endregion Public Methods
    }
}
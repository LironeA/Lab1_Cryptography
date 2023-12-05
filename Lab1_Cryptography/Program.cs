using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1_Cryptography
{
    class Program
    {
        static void Main(string[] args)
        {
            
            //PhiTest();
            //MobiusTest();
            //LCMTest();
            //LegendreTest();
            //JacobiTest();
            //PollardTest();
            //BabyStepGiantStepTest();
            ChipollaTest();
            //MillerRabinTest();
        }
        
        static void PhiTest() {
            var x = new LongNumber(11);
            var res = Phi(x);
            Console.WriteLine(res.ToString());
        }
        
        static void MobiusTest() {
            var y = new LongNumber(11);
            var res2 = Mu(y);
            Console.WriteLine(res2.ToString());
        }
        
        static void LCMTest() {
            var z = new[] {new LongNumber(3), new LongNumber(4)};
            var res3 = LCM(z);
            Console.WriteLine(res3.ToString());
        }
        
        static void LegendreTest() {
            var a = new LongNumber(2);
            var b = new LongNumber(7);
            var res4 = Legendre(a, b);
            Console.WriteLine(res4.ToString());
        }
        
        static void JacobiTest() {
            var c = new LongNumber(2);
            var d = new LongNumber(7);
            var res5 = Jacobi(c, d);
            Console.WriteLine(res5.ToString());
        }
        
        static void PollardTest() {
            var e = new LongNumber("3498713");
            var res6 = FactorizePollard(e);
            Console.WriteLine(res6.ToString());
        }
        
        static void BabyStepGiantStepTest() {
            var f = new LongNumber(5);
            var g = new LongNumber(13);
            var h = new LongNumber(1);
            var res7 = LogBabyStepGiantStep(f, g, h);
            Console.WriteLine(res7.ToString());
        }

        static void ChipollaTest()
        {
            var i = new LongNumber(10);
            var j = new LongNumber(13);
            var res8 = SqrtCipolla(i, j);
            Console.WriteLine(res8.ToString());
        }

        static void MillerRabinTest() {
            var k = new LongNumber(10);
            var res8 = IsPrimeMillerRabin(k);
            Console.WriteLine(res8.ToString());
        }
        
        

        #region Функція Ейлера

        public static LongNumber Phi(LongNumber n) {
            if (n < 1) {
                return null;
            }
            
            var res = n;
            for (var i = new LongNumber(2); i * i <= n; ++i) {
                if (n % i == 0) {
                    while (n % i == 0) {
                        n /= i;
                    }
                    res -= res / i;
                }
            }

            return n > 1 ? res - res / n : res;
        }

        #endregion

        #region Функція Мобіуса

        public static int Mu(LongNumber n) {
            if (n == 1)
                return 1;

            var p = new LongNumber(0);
            for (var i = new LongNumber(2); i <= n; i++) {
                if (n % i == 0 && IsPrime(i)) {
                    if (n % (i * i) == 0)
                        return 0;
                    p++;
                }
            }

            return p % 2 == 0 ? 1 : -1;
        }

        #endregion

        #region Найменше спільне кратне набору чисел
        
        public static LongNumber LCM(LongNumber a, LongNumber b) {
            return a / LongNumber.Gcd(a, b) * b;
        }
        public static LongNumber LCM(params LongNumber[] numbers) {
            var result = numbers[0];
            for (var i = 1; i < numbers.Length; i++) {
                result = LCM(result, numbers[i]);
            }
            return result;
        }
        
        #endregion

        #region Символ Лежандра

        public static int? Legendre(LongNumber a, LongNumber p) {
            if (p < 3 || !IsPrime(p))
                return null;

            if (a % p == 0)
                return 0;

            return LongNumber.PowMod(a, (p - 1) / 2, p) == 1 ? 1 : -1;
        }

        #endregion
        
        #region Символ Якобі
        
        public static int? Jacobi(LongNumber a, LongNumber b) {
            if (b < 1 || b % 2 == 0)
                return null;

            if (LongNumber.Gcd(a, b) != 1)
                return 0;

            a %= b;
            var t = new LongNumber(1);
            while (a != 0) {
                while (a % 2 == 0) {
                    a /= 2;
                    var r = b % 8;
                    if (r == 3 || r == 5)
                        t = -t;
                }
                LongNumber.Swap(ref a, ref b);

                if (a % 4 == 3 && b % 4 == 3)
                    t = -t;
                a %= b;
            }
            return b == 1 ? t : new LongNumber(0);
        }
        
        #endregion


        #region ро-алгоритм Полларда

        private static LongNumber F(LongNumber x, LongNumber n) => (x * x + 1) % n;
        
        public static (LongNumber, LongNumber) FactorizePollard(LongNumber n) {
            var rnd = new Random();
            LongNumber x = n < int.MaxValue ? rnd.Next(0, n) : rnd.Next(0, int.MaxValue);
            LongNumber y = x;
            LongNumber d = new LongNumber(1);

            while (d == 1) {
                x = F(x, n);
                y = F(F(y, n), n);
                d = LongNumber.Gcd(n, LongNumber.Abs(x - y));
            }

            return d == n ? (null, null) : (d, n / d);
        }

        #endregion

        #region великий крок – малий крок

        public static LongNumber LogBabyStepGiantStep(LongNumber a, LongNumber b, LongNumber n) {
            a %= n;
            b %= n;
            var m = LongNumber.Sqrt(n) + 1;
            var g0 = LongNumber.PowMod(a, m, n);
            var g = g0;
            var t = new Dictionary<string, LongNumber>();

            for (var i = new LongNumber(1); i <= m; i++) {
                t.Add(g.ToString(), i);
                g = LongNumber.MulMod(g, g0, n);
            }

            for (var j = new LongNumber(0); j < m; j++) {
                var y = LongNumber.MulMod(b, LongNumber.PowMod(a, j, n), n);
                if (t.ContainsKey(y.ToString())) {
                    return m * t[y.ToString()] - j;
                }
            }

            return null;
        }


        #endregion

        #region Алгоритм Чіпполи

        public static (LongNumber, LongNumber) SqrtCipolla(LongNumber n, LongNumber p) {
            if (Legendre(n, p) != 1) {
                return (null, 0);
            }

            LongNumber a = 0;
            LongNumber w2;
            while (true) {
                w2 = (a * a + p - n) % p;
                if (Legendre(w2, p) != 1)
                    break;
                a++;
            }

            var finalW = w2;
            (LongNumber, LongNumber) MulExtended((LongNumber, LongNumber) aa, (LongNumber, LongNumber) bb) {
                return ((aa.Item1 * bb.Item1 + aa.Item2 * bb.Item2 * finalW) % p,
                    (aa.Item1 * bb.Item2 + bb.Item1 * aa.Item2) % p);
            }

            var r = (new LongNumber(1), new LongNumber(0));
            var s = (a, new LongNumber(1));
            var nn = (p + 1) / 2;
            while (nn > 0) {
                if (nn % 2 != 0) {
                    r = MulExtended(r, s);
                }
                s = MulExtended(s, s);
                nn /= 2;
            }

            if (r.Item2 != 0 || r.Item1 * r.Item1 % p != n) {
                return (0, null);
            }

            return (r.Item1, p - r.Item1);
        }

        #endregion
        
        #region алгоритм Міллера-Рабіна

        public static bool IsPrimeMillerRabin(LongNumber n, int fixedRand=0) {
            if (n == 2 || n == 3)
                return true;
            if (n < 2 || n % 2 == 0)
                return false;

            int k = 10;
            var d = n - 1;
            LongNumber r = 0;
            while (d % 2 == 0) {
                d /= 2;
                r++;
            }


            for (int i = 0; i < k; i++) {
                var a = fixedRand == 0 ? LongNumber.Rand(2, n - 1) : new LongNumber(fixedRand);
                var x = LongNumber.PowMod(a, d, n);

                if (x != 1 && x != n - 1 && !ContinueMillerRabin(r, x, n)) {
                    return false;
                }
            }
            return true;
        }
        
        private static bool ContinueMillerRabin(LongNumber r, LongNumber x, LongNumber n) {
            for (int j = 0; j < r - 1; j++) {
                x = LongNumber.PowMod(x, 2, n);

                if (x == n - 1) {
                    return true;
                }
            }
            return false;
        }

        #endregion
        #region Utils

        public static bool IsPrime(LongNumber n) {
            if (n == 2)
                return true;

            if (n % 2 == 0)
                return false;

            var t = LongNumber.Sqrt(n);
            for (LongNumber k = 3; k <= t; k += 2) {
                if (n % k == 0)
                    return false;
            }

            return true;
        }

        #endregion
        

    }
}
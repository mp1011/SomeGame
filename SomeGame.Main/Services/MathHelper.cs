using Microsoft.Xna.Framework;
using SomeGame.Main.Models;
using System;

namespace SomeGame.Main.Services
{
    static class MathFunctions
    {
        public static Angle RotateToward(this Angle a1, Angle a2, byte amount)
        {
            var distPos = a2 - a1;
            var distNeg = a1 - a2;

            if (distPos < distNeg)
            {
                var newAngle = a1 + amount;

                distPos = a2 - newAngle;
                distNeg = newAngle - a2;

                if (distPos >= distNeg)
                    return a2;
                else
                    return newAngle;
            }
            else
            {
                var newAngle = a1 - amount;

                distPos = a2 - newAngle;
                distNeg = newAngle - a2;

                if (distPos < distNeg)
                    return a2;
                else
                    return newAngle;
            }
        }

        public static Point ToPoint(this Angle angle, int distance)
        {
            var degrees = 360.0 * ((double)angle / 255.0);
            var ret = MathFunctions.RadToXY(degrees * (Math.PI / 180.0), distance);
            return new Point((int)ret.X, (int)ret.Y);
        }

        public static PixelPoint ToPixelPoint(this Angle angle, PixelValue distance)
        {
            var degrees = 360.0 * ((double)angle / 255.0);
            var ret = MathFunctions.RadToXY(degrees * (Math.PI / 180.0), distance);
            return new PixelPoint(ret);
        }


        public static double GetRadians(this Vector2 v)
        {
            double a, b;
            double angle;

            a = v.X;
            b = v.Y;

            if (a == 0 && b == 0)
                return 0;

            angle = Math.Atan(Math.Abs(b) / Math.Abs(a));

            if (a >= 0 && b >= 0)
            {
                angle *= -1;
            }
            else if (a < 0 && b >= 0)
            {
                angle += Math.PI;
            }
            else if (a >= 0 && b < 0)
            {

            }
            else if (a < 0 && b < 0)
            {
                angle = Math.PI - angle;
            }

            while (angle < 0)
                angle += Math.PI * 2;

            while (angle >= Math.PI * 2)
                angle -= Math.PI * 2;

            return angle;
        }


        /// <summary>
        /// Converts an angle in radians to a point
        /// </summary>
        /// <param name="angle">Angle in radians</param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static Vector2 RadToXY(double angle, double length)
        {
            double a, b;

            double t = Math.Tan(angle);

            double c2 = length * length;
            double t2 = t * t;

            a = Math.Sqrt(c2 / (t2 + 1));
            b = Math.Sqrt(c2 - a * a);

            if (angle < (Math.PI / 2))
            {
                b *= -1;
            }
            if (angle >= (Math.PI / 2) && angle < Math.PI)
            {
                a *= -1;
                b *= -1;
            }
            else if (angle >= Math.PI && angle < ((Math.PI / 2) + Math.PI))
            {
                a *= -1;
            }
            else if (angle >= ((Math.PI / 2) + Math.PI))
            {
                ;
            }

            var f = 1f;
            if (length < 0)
                f = -1f;


            return new Vector2((float)Math.Round(a, 4) * f, (float)Math.Round(b, 4) * f);
        }

    }
}

using System;
using System.Windows.Media;

namespace HSVtoRGB
{ 
    /// <summary>
    /// converts an HSV color to RGB
    /// code copied from: http://stackoverflow.com/questions/17080535/hsv-to-rgb-stops-at-yellow-c-sharp
    /// </summary>
    public static class HSV2RGB
    {
        public static Color Convert(float hue, float saturation, float value, float alpha)
        {
            // this procedure is copied from http://stackoverflow.com/questions/17080535/hsv-to-rgb-stops-at-yellow-c-sharp

            if (hue > 1 || saturation > 1 || value > 1) hue = 1;
            if (hue < 0 || saturation < 0 || value < 0) throw new Exception("values cannot be less than 0!");

            // range selection (my addition)
            hue = hue * 0.7f;
            hue = 0.7f - hue;

            Color output = new Color();
            if (Math.Abs(saturation) < 0.001)
            {
                output.R = (byte)(value * byte.MaxValue);
                output.G = (byte)(value * byte.MaxValue);
                output.B = (byte)(value * byte.MaxValue);
            }
            else
            {
                float h6 = hue * 6f;
                if (h6 == 6f) { h6 = 0f; }
                int ihue = (int)(h6);
                float p = value * (1f - saturation);
                float q = value * (1f - (saturation * (h6 - (float)ihue)));
                float t = value * (1f - (saturation * (1f - (h6 - (float)ihue))));

                switch (ihue)
                {
                    case (0):
                        output.R = (byte)(value * 255);
                        output.G = (byte)(t * 255);
                        output.B = (byte)(p * 255);
                        output.A = (byte)(alpha * 255);
                        //output = new Color(value * 255, t * 255, p * 255, alpha);
                        break;
                    case (1):
                        output.R = (byte)(q * 255);
                        output.G = (byte)(value * 255);
                        output.B = (byte)(p * 255);
                        output.A = (byte)(alpha * 255);
                        //output = new Color(q * 255, value * 255, p * 255, alpha);
                        break;
                    case (2):
                        output.R = (byte)(p * 255);
                        output.G = (byte)(value * 255);
                        output.B = (byte)(t * 255);
                        output.A = (byte)(alpha * 255);
                        //output = new Color(p * 255, value * 255, t * 255, alpha);
                        break;
                    case (3):
                        output.R = (byte)(p * 255);
                        output.G = (byte)(q * 255);
                        output.B = (byte)(value * 255);
                        output.A = (byte)(alpha * 255);
                        //output = new Color(p * 255, q * 255, value * 255, alpha);
                        break;
                    case (4):
                        output.R = (byte)(t * 255);
                        output.G = (byte)(p * 255);
                        output.B = (byte)(value * 255);
                        output.A = (byte)(alpha * 255);
                        //output = new Color(t * 255, p * 255, value * 255, alpha);
                        break;
                    case (5):
                        output.R = (byte)(value * 255);
                        output.G = (byte)(p * 255);
                        output.B = (byte)(q * 255);
                        output.A = (byte)(alpha * 255);
                        //output = new Color(value * 255, p * 255, q * 255, alpha);
                        break;
                    default:
                        throw new Exception("RGB color unknown!");
                }

            }
            return output;
        }
    }
}

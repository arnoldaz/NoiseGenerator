using NoiseGenerator.Source;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NoiseGenerator {
    public partial class MainForm : Form {
        public MainForm () {
            InitializeComponent ();

            var random = new Random ();
            var bitmap = new Bitmap (PictureBox.ClientSize.Width, PictureBox.ClientSize.Height);

            var green = Color.Green;
            var blue = Color.Blue;
            var yellow = Color.Yellow;

            for (var y = 0; y < PictureBox.ClientSize.Height; y++) {
                for (var x = 0; x < PictureBox.ClientSize.Width; x++) {
                    //var randomNumber = random.Next (0, 256);
                    //var color = Color.FromArgb (randomNumber, randomNumber, randomNumber);

                    var f = 10.0;
                    var a = 2.0;

                    var offsetX = 0;
                    var offsetY = 0;

                    var noise = 0.0;

                    for (int i = 0; i < 4; i++) {
                        double xx = (double)x / PictureBox.ClientSize.Width * f + offsetX;
                        double yy = (double)y / PictureBox.ClientSize.Height * f + offsetY;

                        var noiseValue = PerlinNoise.CalculateNoise (xx,yy);
                        noise += noiseValue * a;
                        a *= 0.5;
                        f *= 2;
                    }

                    if (noise < -1)
                        noise = -1;
                    if (noise > 1)
                        noise = 1;

                    var convertedValue = (int)(((noise + 1) / 2) * 255);
                    //Debug.WriteLine ($"{x} {y} {xx} {yy} {noiseValue} {convertedValue}");
                    var color = Color.FromArgb (0, convertedValue, 0);

                    //if (convertedValue < 30)
                    //    color = blue;
                    //else if (convertedValue < 50)
                    //    color = yellow;
                    //else
                    //    color = green;

                    bitmap.SetPixel (x, y, color);
                }
            }

            PictureBox.Image = bitmap;

            //var vvv = PerlinNoise.m_permutationTwice;
            //foreach(var v in vvv) {
            //    Debug.WriteLine (v);
            //}
        }

    }
}

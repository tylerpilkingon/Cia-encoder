using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Cia_project {
    class Encoding {
        private string _filetype;
        private List<string> _comments = new List<string>();
        private int _maxPixel;
        private int _length;
        private int _width;

        public string Filetype {
            get { return _filetype; }
        }

        public List<string> Comments {
            get { return _comments; }
        }

        public int MaxPixel {
            get { return _maxPixel; }
        }

        //This method generates a bitmap from the ppm file
        public Bitmap GenerateBitmap(string fileName) {
            //get all the info and comments at the top out of the way
            StreamReader inputFile = new StreamReader(fileName);
            _filetype = inputFile.ReadLine();
            bool commentsEnd = false;
            int count = 0;
            string LW = "";
            while (commentsEnd == false) {
                _comments.Add(inputFile.ReadLine());
                if (!_comments[count].Contains('#')) {
                    commentsEnd = true;
                    LW = _comments[count];
                    _comments.RemoveAt(count);
                }
                count++;
            }
            _maxPixel = int.Parse(inputFile.ReadLine());

            char[] lengthAndwidth = LW.ToCharArray();
            string num = "";

            
            for (int index = 0; index < lengthAndwidth.Length; index++) {
                if (lengthAndwidth[index] == ' ') {
                    _width = int.Parse(num);
                    num = "";
                } else if (int.Parse(lengthAndwidth[index].ToString()) >= 0 && int.Parse(lengthAndwidth[index].ToString()) <= 9) {
                    num += lengthAndwidth[index];
                }
                if (int.TryParse(num, out _) && _width != 0) _length = int.Parse(num);

            }
            #region p3
            if (_filetype == "P3") {
                //if it is a P3 it will just set the colors to the new image
                Bitmap newImg = new Bitmap(_width, _length);
                for (int y = 0; y < _length; y++) {
                    for (int x = 0; x < _width; x++) {
                        int r = int.Parse(inputFile.ReadLine()); 
                        int g = int.Parse(inputFile.ReadLine()); 
                        int b = int.Parse(inputFile.ReadLine());
                        Color colCurrent = Color.FromArgb(r, g, b);
                        newImg.SetPixel(x, y, colCurrent);
                    }
                }
                inputFile.Close();
                return newImg;
            #endregion
            #region p6
            } else {
                //if its a P6 it closes Streamreader and open filestream
                inputFile.Close();
                FileStream p6ByteFile = new FileStream(fileName, FileMode.Open);
                char curByte;

                //this gets rid of everything at the top
                for (int index = 0; index < 3 + _comments.Count; index++) {
                    curByte = (char)p6ByteFile.ReadByte();

                    while (curByte != '\n') {
                        curByte = (char)p6ByteFile.ReadByte();
                    }
                }

                Bitmap newImg = new Bitmap(_width, _length);

                for (int y = 0; y < _length; y++) {
                    for (int x = 0; x < _width; x++) {
                        int r = p6ByteFile.ReadByte(); 
                        int g = p6ByteFile.ReadByte(); 
                        int b = p6ByteFile.ReadByte();
                        Color colCurrent = Color.FromArgb(r, g, b);
                        newImg.SetPixel(x, y, colCurrent);
                    }
                }
                p6ByteFile.Close();
                return newImg;
            }
            #endregion



        }

        //This method preps the image to be encoded
        public Bitmap PrepImage(Bitmap newImg) {

            Color colCurrent = new Color();

            //it makes sure that there are no colors the correspond to ascii values
            for (int y = 0; y < newImg.Height; y++) {
                for (int x = 0; x < newImg.Width; x++) {
                    colCurrent = newImg.GetPixel(x, y);
                    if (colCurrent.G == 32) {
                        colCurrent = Color.FromArgb(colCurrent.R, 31, colCurrent.B);
                        newImg.SetPixel(x, y, colCurrent);
                    } else if (colCurrent.G >= 48 && colCurrent.G <= 58) {
                        colCurrent = Color.FromArgb(colCurrent.R, 59, colCurrent.B);
                        newImg.SetPixel(x, y, colCurrent);
                    } else if (colCurrent.G >= 97 && colCurrent.G <= 122) {
                        colCurrent = Color.FromArgb(colCurrent.R, 96, colCurrent.B);
                        newImg.SetPixel(x, y, colCurrent);
                    }
                }
            }

            return newImg;
        }

        //This method Builds The Dictionary
        public Dictionary<int, string> BuildDictionary() {
            Dictionary<int, string> codeDic = new Dictionary<int, string>();

            codeDic.Add(32, " ");
            codeDic.Add(48, "0");
            codeDic.Add(49, "1");
            codeDic.Add(50, "2");
            codeDic.Add(51, "3");
            codeDic.Add(52, "4");
            codeDic.Add(53, "5");
            codeDic.Add(54, "6");
            codeDic.Add(55, "7");
            codeDic.Add(56, "8");
            codeDic.Add(57, "9");
            codeDic.Add(97, "a");
            codeDic.Add(98, "b");
            codeDic.Add(99, "c");
            codeDic.Add(100, "d");
            codeDic.Add(101, "e");
            codeDic.Add(102, "f");
            codeDic.Add(103, "g");
            codeDic.Add(104, "h");
            codeDic.Add(105, "i");
            codeDic.Add(106, "j");
            codeDic.Add(107, "k");
            codeDic.Add(108, "l");
            codeDic.Add(109, "m");
            codeDic.Add(110, "n");
            codeDic.Add(111, "o");
            codeDic.Add(112, "p");
            codeDic.Add(113, "q");
            codeDic.Add(114, "r");
            codeDic.Add(115, "s");
            codeDic.Add(116, "t");
            codeDic.Add(117, "u");
            codeDic.Add(118, "v");
            codeDic.Add(119, "w");
            codeDic.Add(120, "x");
            codeDic.Add(121, "y");
            codeDic.Add(122, "z");

            return codeDic;
        }

        //This method encodes the image
        public Bitmap Encoder(Bitmap img, Dictionary<int, string> codeDic, string usermessage) {
            Color colCurrent = new Color();

            int x = 0;
            int y = 0;

            //it loops through the image and inputs the secret code by changing the colors of the pixels
            for (int index = 0; index < usermessage.Length; index++) {
                int green = usermessage[index];
                if (codeDic.ContainsKey(usermessage[index]))
                    colCurrent = img.GetPixel(x, y);
                    colCurrent = Color.FromArgb(colCurrent.R, green, colCurrent.B);
                    img.SetPixel(x, y, colCurrent);
                
                if (x < img.Width - 1){
                    x++;
                }else {
                    x = 0;
                    y++;
                }
            }
            return img;
        }

        //This method is for saving the encoded Image
        public void Saveimgae(string fileName, Bitmap newImage) {
            //First it prints out the top of the file
            StreamWriter outfile = new StreamWriter(fileName);
            Color colcurrent = new Color();
            outfile.WriteLine(_filetype);
            for (int index = 0; index < _comments.Count; index++) {
                outfile.WriteLine(_comments[index]);
            }
            outfile.WriteLine(_width + " " + _length);
            outfile.WriteLine(_maxPixel);

            //then if it is P3 it just prints out the numbers
            if (_filetype == "P3") {
                for (int y = 0; y < _length; y++) {
                    for (int x = 0; x < _width; x++) {
                        colcurrent = newImage.GetPixel(x, y);
                        outfile.WriteLine(colcurrent.R);
                        outfile.WriteLine(colcurrent.G);
                        if (x == newImage.Width - 1 && y == newImage.Height - 1) outfile.Write(colcurrent.B);
                        else outfile.WriteLine(colcurrent.B);
                    }
                }
                outfile.Close();
            //but if it is a P6 it will close the StreamWriter and open FileStream to insert the bytes 
            } else {
                outfile.Close();
                FileStream p6OutFile = new FileStream(fileName, FileMode.Append);
                for (int y = 0; y < _length; y++) {
                    for (int x = 0; x < _width; x++) {
                        colcurrent = newImage.GetPixel(x, y);
                        p6OutFile.WriteByte(colcurrent.R);
                        p6OutFile.WriteByte(colcurrent.G);
                        p6OutFile.WriteByte(colcurrent.B);
                    }
                }
                p6OutFile.Close();
            }
        }
    }
}

using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization;
using System.Text;

namespace VSim
{
    namespace GUI
    {
        class Button : RectangleShape
        {
            //RectangleShape rectangleShape;

            RenderTexture renderTexture;

            public Button(Vector2f position, Vector2f size, string imagePath) // Color color, string str = null)
            {
                this.Position = position;
                this.Size = size;
                //this.FillColor = color;

                renderTexture = new RenderTexture((uint) size.X, (uint)size.Y);
                renderTexture.Display();
                renderTexture.Clear();

                RectangleShape image = new RectangleShape();
                image.Position = new Vector2f(0.0f, 0.0f);
                image.Size = new Vector2f(size.X, size.Y);
                image.Texture = new Texture(System.IO.File.OpenRead(imagePath));

                renderTexture.Draw(image);
                this.Texture = renderTexture.Texture;
            }

            public bool isInRectangle(float x, float y)
            {
                Vector2f position = this.Position;
                Vector2f size = this.Size;

                return (x >= position.X && x <= position.X + size.X && y >= position.Y && y <= position.Y + size.Y);
            }
        }

        class Checkbox : RectangleShape
        {
            //RectangleShape rectangleShape;

            RenderTexture renderTexture;
            bool checkedValue;
            string textValue;

            public bool Checked
            {
                get
                {
                    return checkedValue;
                }
            }

            RectangleShape imageChecked;
            RectangleShape imageNotChecked; 

            public bool ChangeChecked()
            {
                checkedValue = !checkedValue;
                renderTexture.Clear();
                if (this.Checked)
                {
                    renderTexture.Draw(imageChecked);
                }
                else
                {
                    renderTexture.Draw(imageNotChecked);
                }
                Text text = new Text(textValue, new Font(@"arial.ttf"), 13);
                text.FillColor = Color.White;
                text.Position = new Vector2f(40.0f, 0.5f);

                renderTexture.Draw(text);
                this.Texture = renderTexture.Texture;

                return Checked;
            }
            public Checkbox(Vector2f position, Vector2f size, bool value = false, string textValue = "") // Color color, string str = null)
            {
                string imgNotChecked = "resources/checkboxNotChecked.png";
                string imgChecked = "resources/checkboxChecked2.png";

                this.Position = position;
                this.Size = size;
                this.checkedValue = value;
                this.textValue = textValue;
                //this.FillColor = color;

                renderTexture = new RenderTexture((uint)size.X, (uint)size.Y);
                renderTexture.Display();
                renderTexture.Clear();

                imageNotChecked = new RectangleShape();
                imageNotChecked.Position = new Vector2f(0.0f, 0.0f);
                imageNotChecked.Texture = new Texture(System.IO.File.OpenRead(imgNotChecked));
                imageNotChecked.Size = new Vector2f(imageNotChecked.Texture.Size.X, imageNotChecked.Texture.Size.Y);

                imageChecked = new RectangleShape();
                imageChecked.Position = new Vector2f(0.0f, 0.0f);
                //imageChecked.Size = new Vector2f(size.X, size.Y);
                imageChecked.Texture = new Texture(System.IO.File.OpenRead(imgChecked));
                imageChecked.Size = new Vector2f(imageChecked.Texture.Size.X, imageChecked.Texture.Size.Y);

                if (this.Checked)
                {
                    renderTexture.Draw(imageChecked);
                }
                else
                {
                    renderTexture.Draw(imageNotChecked);
                }

                Text text = new Text(textValue, new Font(@"arial.ttf"), 13);
                text.FillColor = Color.White;
                text.Position = new Vector2f(40.0f, 0.5f);

                renderTexture.Draw(text);
                this.Texture = renderTexture.Texture;
            }
            public bool isInRectangle(float x, float y)
            {
                Vector2f position = this.Position;
                Vector2f size = this.Size;

                return (x >= position.X && x <= position.X + size.X && y >= position.Y && y <= position.Y + size.Y);
            }
        }

        class Textbox : RectangleShape
        {
            //RectangleShape rectangleShape;

            RenderTexture renderTexture;
            Text text;
            string textValue;
            bool focus;
            bool enabled;
            int cursorPosition;

            public string TextValue
            {
                get
                {
                    return textValue;
                }
            }
            public bool Enabled
            {
                get
                {
                    return enabled;
                }
                set
                {
                    enabled = value;
                    if (!enabled)
                    {
                        setFocus(false);
                    }
                    if (enabled)
                    {
                        text.FillColor = Color.Black;
                    }
                    else
                    {
                        text.FillColor = new Color(128, 128, 128);
                    }
                    UpdateGraphics();
                }
            }


            public void setFocus(bool value)
            {
                if (Enabled)
                {
                    focus = value;
                    UpdateGraphics();
                }
            }
            public uint findNearestCharacter(float x)
            {
                string str = text.DisplayedString;

                uint imin = 0;
                float distance = Math.Abs(text.FindCharacterPos(imin).X - x);

                for (uint i = 0; i <= str.Length; ++i)
                {
                    float x0 = text.FindCharacterPos(i).X;
                    if (Math.Abs(x0 - x) < distance)
                    {
                        imin = i;
                        distance = Math.Abs(x0 - x);
                    }
                }

                return imin;
            }
            public void setCursorPositionByMouse(float x)
            {
                x -= text.Position.X;
                cursorPosition = (int) findNearestCharacter(x);
                UpdateGraphics();
            }

            public string Input(char c)
            {
                return Input(c.ToString());
            }
            public string Input(string str)
            {
                if (Enabled)
                {
                    textValue = textValue.Insert(cursorPosition, str);
                    Right();
                }
                //UpdateGraphics();
                return textValue;
            }
            public string Return()
            {
                if (cursorPosition > 0 && Enabled)
                {
                    textValue = textValue.Remove(cursorPosition - 1, 1);
                    Left();
                    UpdateGraphics();
                }
                return textValue;
            }
            public void Clear()
            {
                textValue = "";
                cursorPosition = 0;
                UpdateGraphics();
            }
            public bool hasFocus()
            {
                return focus;
            }
            public void Left()
            {
                if (Enabled)
                {
                    cursorPosition -= 1;
                    if (cursorPosition < 0)
                    {
                        cursorPosition = 0;
                    }

                    UpdateGraphics();
                }
            }
            public void Right()
            {
                if (Enabled)
                {
                    cursorPosition += 1;
                    if (cursorPosition > textValue.Length)
                    {
                        cursorPosition = textValue.Length;
                    }

                    UpdateGraphics();
                }
            }
            void UpdateGraphics()
            {
                if (Enabled)
                {
                    renderTexture.Clear(Color.White);
                }
                else
                {
                    renderTexture.Clear(new Color(211, 211, 211));
                }

                text.DisplayedString = this.TextValue;

                renderTexture.Draw(text);
                if (focus && text.DisplayedString.Length >= 0 && Enabled)
                {
                    RectangleShape bar = new RectangleShape();
                    bar.Position = new Vector2f(text.Position.X + text.FindCharacterPos((uint) cursorPosition).X, 4.0f);
                    bar.Size = new Vector2f(1.0f, this.Size.Y - 8.0f);

                    bar.FillColor = Color.Black;
                    
                    renderTexture.Draw(bar);
                }
                this.Texture = renderTexture.Texture;
            }

            public Textbox(Vector2f position, Vector2f size, string textValue = null) // Color color, string str = null)
            {
                this.Position = position;
                this.Size = size;
                this.textValue = textValue;

                enabled = true;
                //this.FillColor = color;

                text = new Text(textValue, new Font(@"arial.ttf"), 13);
                text.FillColor = Color.Black;
                text.Position = new Vector2f(10.0f, 5.0f);

                cursorPosition = TextValue.Length;
                focus = false;

                renderTexture = new RenderTexture((uint)size.X, (uint)size.Y);
                renderTexture.Display();
                UpdateGraphics();
            }
            public bool isInRectangle(float x, float y)
            {
                Vector2f position = this.Position;
                Vector2f size = this.Size;

                return (x >= position.X && x <= position.X + size.X && y >= position.Y && y <= position.Y + size.Y);
            }
        }

        class Label : Text
        {
            public Label(string text, Vector2f position, uint charactersize, Font font, Color color) : base(text, font, charactersize)
            {
                this.Position = position;
                this.FillColor = color;
            }
            public Label(string text, Vector2f position, uint charactersize = 15) : base(text, new Font(@"arial.ttf"), charactersize)
            {
                this.Position = position;
                this.FillColor = Color.White;
            }
        }

        class GraphBox : RectangleShape
        {
            RenderTexture renderTexture;
            Dictionary<string, Serie> series;

            double ymax;
            double xmax;
            Vector2f origin;
            void UpdateGraphics()
            {
                renderTexture.Clear(Color.White);

                Vertex[] lineHorizontal = new Vertex[2];
                lineHorizontal[0].Position = new Vector2f(0.0f, origin.Y);
                lineHorizontal[0].Color = Color.Black;

                lineHorizontal[1].Position = new Vector2f(renderTexture.Size.X, origin.Y);
                lineHorizontal[1].Color = Color.Black;

                Vertex[] lineVertical = new Vertex[2];
                lineVertical[0].Position = new Vector2f(origin.X, renderTexture.Size.Y);
                lineVertical[0].Color = Color.Black;

                lineVertical[1].Position = new Vector2f(origin.X, 0.0f);
                lineVertical[1].Color = Color.Black;

                renderTexture.Draw(lineHorizontal, PrimitiveType.Lines);
                renderTexture.Draw(lineVertical, PrimitiveType.Lines);

                float width = (float)renderTexture.Size.X;
                float height = (float)renderTexture.Size.Y;

                float intervalY = (float) origin.Y / (float) ymax;
                float intervalX = (width - origin.X) / (float) xmax;

                Vertex[] v = new Vertex[2];
                for (int j = 0; j < Math.Floor(ymax); ++j)
                {
                    v[0].Position = new Vector2f(origin.X, origin.Y - intervalY * (float) j);
                    v[0].Color = Color.Black;

                    v[1].Position = new Vector2f(origin.X - 5.0f, origin.Y - intervalY * (float) j);
                    v[1].Color = Color.Black;

                    renderTexture.Draw(v, PrimitiveType.Lines);
                }
                for (int i = 0; i < Math.Floor(xmax); ++i)
                {
                    v[0].Position = new Vector2f(origin.X + intervalX * (float)i, origin.Y);
                    v[0].Color = Color.Black;

                    v[1].Position = new Vector2f(origin.X + intervalX * (float)i, origin.Y + 5.0f);
                    v[1].Color = Color.Black;

                    renderTexture.Draw(v, PrimitiveType.Lines);
                }

                /*
                foreach (string key in series.Keys)
                {
                    List<double> values = series[key].Values;

                    Vertex[] vertices = new Vertex[values.Count];
                    for (int i = 0; i < values.Count; ++i)
                    {
                        float xvalue = origin.X + (((float)i) / ((float)values.Count + 1.0f)) * width;
                        float yvalue = origin.Y - (((float)values[i]) / ((float)ymax * 1.15f)) * height;

                        vertices[i].Position = new Vector2f(xvalue, yvalue);
                        vertices[i].Color = series[key].FillColor;
                    }
                    
                    renderTexture.Draw(vertices, PrimitiveType.LineStrip);
                }
                */
                Vertex[] vertices = new Vertex[2];
                foreach (string key in series.Keys)
                {
                    List<double> values = series[key].Values;

                    for (int i = 0; i < values.Count - 1; ++i)
                    {
                        float xvalue = origin.X + (((float)i) / ((float)values.Count + 1.0f)) * width;
                        float yvalue = origin.Y - (((float)values[i]) / ((float)ymax * 1.15f)) * height;

                        float xvalue2 = origin.X + (((float)(i + 1)) / ((float)values.Count + 1.0f)) * width;
                        float yvalue2 = origin.Y - (((float)values[i + 1]) / ((float)ymax * 1.15f)) * height;

                        vertices[0].Position = new Vector2f(xvalue, yvalue);
                        vertices[0].Color = series[key].FillColor;

                        vertices[1].Position = new Vector2f(xvalue2, yvalue2);
                        vertices[1].Color = series[key].FillColor;

                        renderTexture.Draw(vertices, PrimitiveType.LineStrip);
                    }
                }

                this.Texture = renderTexture.Texture;
            }
            public void Clear()
            {
                foreach (string key in series.Keys)
                {
                    series[key].Clear();
                }

                xmax = 1.0;
                ymax = 1.0;
                UpdateGraphics();
            }
            public void AddYValue(string key, double yvalue)
            {
                series[key].Values.Add(yvalue);
                if (yvalue > ymax)
                {
                    ymax = yvalue;
                }
                if (series[key].Values.Count > xmax)
                {
                    xmax = (double) series[key].Values.Count;
                }
                UpdateGraphics();
            }
            public void AddSerie(string key, Color color)
            {
                if (!series.ContainsKey(key))
                {
                    series.Add(key, new Serie(key, color));
                }
            }
            
            public GraphBox(Vector2f position, Vector2f size) // Color color, string str = null)
            {
                this.Position = position;
                this.Size = size;
                this.series = new Dictionary<string, Serie>();

                this.xmax = 1.0;
                this.ymax = 1.0;

                renderTexture = new RenderTexture((uint)size.X, (uint)size.Y);
                renderTexture.Display();
                
                this.origin = new Vector2f(0.025f * renderTexture.Size.X, 0.95f * renderTexture.Size.Y);

                UpdateGraphics();
            }
            public bool isInRectangle(float x, float y)
            {
                Vector2f position = this.Position;
                Vector2f size = this.Size;

                return (x >= position.X && x <= position.X + size.X && y >= position.Y && y <= position.Y + size.Y);
            }

            class Serie
            {
                string name;
                List<double> values;
                Color color;
                bool visible;

                public Serie(string name, Color fillColor)
                {
                    this.name = name;
                    values = new List<double>();
                    this.color = fillColor;
                    this.visible = true;
                }

                public void Clear()
                {
                    values.Clear();
                }

                public bool Visible
                {
                    get
                    {
                        return visible;
                    }
                    set
                    {
                        visible = value;
                    }
                }

                public string Name
                {
                    get
                    {
                        return name;
                    }
                }

                public List<double> Values
                {
                    get
                    {
                        return values;
                    }
                }
                public Color FillColor
                {
                    get
                    {
                        return color;
                    }
                }
            }
        }
    }
}

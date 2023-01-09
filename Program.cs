using System;
using SFML;
using SFML.System;
using SFML.Graphics;
using SFML.Window;
using VirusSimulator;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using VSim.GUI;
using VSim;
using System.Runtime.CompilerServices;

using System.Collections.Generic;

namespace VSim
{
    class Program
    {
        static RectangleShape epidemyBox;
        static Button buttonPlay;
        static Button buttonPause;
        static Button buttonNextStep;
        static Button buttonRestart;

        //static Checkbox checkboxRenderContactLinks;
        static Checkbox checkboxRenderContactDisk;
        static Checkbox checkboxRenderShowStats;

        static Textbox contaminationProbabilityTextbox;
        static Textbox cureProbabilityTextbox;
        static Textbox deathProbabilityTextbox;
        static Textbox immunityProbabilityTextbox;
        static Textbox contaminationRayonTextbox;
        static Textbox immunityLostRateTextbox;

        static Textbox populationSizeTextbox;
        static Textbox infectedPopulationSizeTextbox;

        static Label contaminationProbabilityLabel;
        static Label cureProbabilityLabel;
        static Label deathProbabilityLabel;
        static Label immunityProbabilityLabel;
        static Label contaminationRayonLabel;
        static Label immunityLostRateLabel;

        static Label populationSizeLabel;
        static Label infectedPopulationSizeLabel;

        static GraphBox graphbox;

        static RenderWindow window;
        static EpidemySimulator.Virus virus;
        static EpidemySimulator epidemySimulator;
        static MODE mode;
        static RenderTexture epidemyTexture;

        static double width;
        static double height;

        static double spWidth;
        static double spHeight;

        static int populationSize;
        static int numberOfInfected;

        static bool continueTick;
        static bool continueTickAfterEnding;

        static void OnClose(object sender, EventArgs e)
        {
            RenderWindow window = (RenderWindow)sender;
            window.Close();
        }

        static void SetEnableAll(bool enabled = true)
        {
            contaminationProbabilityTextbox.Enabled = enabled;
            cureProbabilityTextbox.Enabled = enabled;
            deathProbabilityTextbox.Enabled = enabled;
            immunityProbabilityTextbox.Enabled = enabled;
            contaminationRayonTextbox.Enabled = enabled;
            immunityLostRateTextbox.Enabled = enabled;
        }
        static void OnMouseButtonReleased(object sender, MouseButtonEventArgs e)
        {
            if (e.Button == Mouse.Button.Left)
            {
                if (buttonPlay.isInRectangle(e.X, e.Y))
                {
                    SetEnableAll(false);
                    if (epidemySimulator.getStats().infectedCount == 0)
                    {
                        continueTickAfterEnding = true;
                    }
                    continueTick = true;
                }
                if (buttonPause.isInRectangle(e.X, e.Y))
                {
                    continueTick = false;
                }
                if (buttonNextStep.isInRectangle(e.X, e.Y))
                {
                    nextStep();
                }
                if (buttonRestart.isInRectangle(e.X, e.Y))
                {
                    restart();
                }

                if (checkboxRenderShowStats.isInRectangle(e.X, e.Y))
                {
                    checkboxRenderShowStats.ChangeChecked();
                    if (checkboxRenderShowStats.Checked)
                    {
                        mode = mode | MODE.renderShowStats;
                    }
                    else
                    {
                        mode = mode & ~MODE.renderShowStats;
                    }
                    RenderImage(epidemyTexture, epidemySimulator, mode, width, height);
                }
                if (checkboxRenderContactDisk.isInRectangle(e.X, e.Y))
                {
                    checkboxRenderContactDisk.ChangeChecked();
                    if (checkboxRenderContactDisk.Checked)
                    {
                        mode = mode | MODE.renderContactDisk;
                    }
                    else
                    {
                        mode = mode & ~MODE.renderContactDisk;
                    }
                    RenderImage(epidemyTexture, epidemySimulator, mode, width, height);
                }

                if (contaminationProbabilityTextbox.isInRectangle(e.X, e.Y))
                {
                    contaminationProbabilityTextbox.setFocus(true);
                    contaminationProbabilityTextbox.setCursorPositionByMouse(e.X - contaminationProbabilityTextbox.Position.X);
                }
                else
                {
                    contaminationProbabilityTextbox.setFocus(false);
                }

                if (cureProbabilityTextbox.isInRectangle(e.X, e.Y))
                {
                    cureProbabilityTextbox.setFocus(true);
                    cureProbabilityTextbox.setCursorPositionByMouse(e.X - cureProbabilityTextbox.Position.X);
                }
                else
                {
                    cureProbabilityTextbox.setFocus(false);
                }

                if (deathProbabilityTextbox.isInRectangle(e.X, e.Y))
                {
                    deathProbabilityTextbox.setFocus(true);
                    deathProbabilityTextbox.setCursorPositionByMouse(e.X - deathProbabilityTextbox.Position.X);
                }
                else
                {
                    deathProbabilityTextbox.setFocus(false);
                }

                if (immunityProbabilityTextbox.isInRectangle(e.X, e.Y))
                {
                    immunityProbabilityTextbox.setFocus(true);
                    immunityProbabilityTextbox.setCursorPositionByMouse(e.X - immunityProbabilityTextbox.Position.X);
                }
                else
                {
                    immunityProbabilityTextbox.setFocus(false);
                }

                if (contaminationRayonTextbox.isInRectangle(e.X, e.Y))
                {
                    contaminationRayonTextbox.setFocus(true);
                    contaminationRayonTextbox.setCursorPositionByMouse(e.X - contaminationRayonTextbox.Position.X);
                }
                else
                {
                    contaminationRayonTextbox.setFocus(false);
                }

                if (immunityLostRateTextbox.isInRectangle(e.X, e.Y))
                {
                    immunityLostRateTextbox.setFocus(true);
                    immunityLostRateTextbox.setCursorPositionByMouse(e.X - immunityLostRateTextbox.Position.X);
                }
                else
                {
                    immunityLostRateTextbox.setFocus(false);
                }

                if (populationSizeTextbox.isInRectangle(e.X, e.Y))
                {
                    populationSizeTextbox.setFocus(true);
                    populationSizeTextbox.setCursorPositionByMouse(e.X - populationSizeTextbox.Position.X);
                }
                else
                {
                    populationSizeTextbox.setFocus(false);
                }

                if (infectedPopulationSizeTextbox.isInRectangle(e.X, e.Y))
                {
                    infectedPopulationSizeTextbox.setFocus(true);
                    infectedPopulationSizeTextbox.setCursorPositionByMouse(e.X - infectedPopulationSizeTextbox.Position.X);
                }
                else
                {
                    infectedPopulationSizeTextbox.setFocus(false);
                }

                UpdateGraphicsFlag();
            }
        }

        static int convert(Keyboard.Key code)
        {
            if (code == Keyboard.Key.Num0 || code == Keyboard.Key.Numpad0)
            {
                return 0;
            }
            else if (code == Keyboard.Key.Num1 || code == Keyboard.Key.Numpad1)
            {
                return 1;
            }
            else if (code == Keyboard.Key.Num2 || code == Keyboard.Key.Numpad2)
            {
                return 2;
            }
            else if (code == Keyboard.Key.Num3 || code == Keyboard.Key.Numpad3)
            {
                return 3;
            }
            else if (code == Keyboard.Key.Num4 || code == Keyboard.Key.Numpad4)
            {
                return 4;
            }
            else if (code == Keyboard.Key.Num5 || code == Keyboard.Key.Numpad5)
            {
                return 5;
            }
            else if (code == Keyboard.Key.Num6 || code == Keyboard.Key.Numpad6)
            {
                return 6;
            }
            else if (code == Keyboard.Key.Num7 || code == Keyboard.Key.Numpad7)
            {
                return 7;
            }
            else if (code == Keyboard.Key.Num8 || code == Keyboard.Key.Numpad8)
            {
                return 8;
            }
            else if (code == Keyboard.Key.Num9 || code == Keyboard.Key.Numpad9)
            {
                return 9;
            }
            return -1;
        }

        static RenderTexture screenshot;

        static Sprite spEpidemyBox; // = new Sprite();

        static Sprite spGraphbox; // = new Sprite();

        public static void Screenshot(string filename)
        {
            screenshot.Display();
            screenshot.Clear(Color.White);

            spEpidemyBox.Position = new Vector2f(0.0f, 0.0f);
            spEpidemyBox.Texture = epidemyBox.Texture;

            spGraphbox.Position = new Vector2f(epidemyBox.Size.X, 0.0f);
            spGraphbox.Texture = graphbox.Texture;

            screenshot.Draw(spEpidemyBox);
            screenshot.Draw(spGraphbox);
            screenshot.Texture.CopyToImage().SaveToFile(filename);
        }
        static int keyPressedTextbox(ref Textbox textbox, KeyEventArgs e, ref int value)
        {
            int val = convert(e.Code);

            if (val != -1)
            {
                string str = val.ToString();

                textbox.Input(str);
                if (Convert.isAcceptableInteger(textbox.TextValue))
                {
                    int intValue = Convert.convertToInteger(textbox.TextValue);

                    value = intValue;
                }

                return 1;
            }
            else if (e.Code == Keyboard.Key.Backspace)
            {
                textbox.Return();
                if (Convert.isAcceptableInteger(textbox.TextValue))
                {
                    int intValue = Convert.convertToInteger(textbox.TextValue);

                    value = intValue;
                }

                return 1;
            }
            else if (e.Code == Keyboard.Key.Left)
            {
                textbox.Left();

                return 1;
            }
            else if (e.Code == Keyboard.Key.Right)
            {
                textbox.Right();

                return 1;
            }

            return 0;
        }

        static int keyPressedTextbox(ref Textbox textbox, KeyEventArgs e, ref double value)
        {
            int val = convert(e.Code);

            if (val != -1)
            {
                string str = val.ToString();

                textbox.Input(str);
                if (Convert.isAcceptableDouble(textbox.TextValue))
                {
                    double dbValue = Convert.convertToDouble(textbox.TextValue);

                    value = dbValue;
                }

                return 1;
            }
            else if (e.Code == Keyboard.Key.Backspace)
            {
                textbox.Return();
                if (Convert.isAcceptableDouble(textbox.TextValue))
                {
                    double dbValue = Convert.convertToDouble(textbox.TextValue);

                    value = dbValue;
                }

                return 1;
            }
            else if (e.Code == Keyboard.Key.Period && e.Shift)
            {
                string str = ".";

                textbox.Input(str);
                if (Convert.isAcceptableDouble(textbox.TextValue))
                {
                    double dbValue = Convert.convertToDouble(textbox.TextValue);

                    value = dbValue;
                }

                return 1;
            }
            else if (e.Code == Keyboard.Key.Left)
            {
                textbox.Left();

                return 1;
            }
            else if (e.Code == Keyboard.Key.Right)
            {
                textbox.Right();

                return 1;
            }

            return 0;
        }

        static bool toggleScreenshotMode = false;
        static void OnKeyPressed(object sender, KeyEventArgs e)
        {
            if (contaminationProbabilityTextbox.hasFocus())
            {
                if (keyPressedTextbox(ref contaminationProbabilityTextbox, e, ref virus.contaminationProbability) == 1)
                {
                    epidemySimulator.setVirus(virus);
                    UpdateGraphicsFlag();
                }
            }
            else if (cureProbabilityTextbox.hasFocus())
            {
                if (keyPressedTextbox(ref cureProbabilityTextbox, e, ref virus.cureProbability) == 1)
                {
                    epidemySimulator.setVirus(virus);
                    UpdateGraphicsFlag();
                }
            }
            else if (deathProbabilityTextbox.hasFocus())
            {
                if (keyPressedTextbox(ref deathProbabilityTextbox, e, ref virus.deathProbability) == 1)
                {
                    epidemySimulator.setVirus(virus);
                    UpdateGraphicsFlag();
                }
            }
            else if (immunityProbabilityTextbox.hasFocus())
            {
                if (keyPressedTextbox(ref immunityProbabilityTextbox, e, ref virus.immuneProbability) == 1)
                {
                    epidemySimulator.setVirus(virus);
                    UpdateGraphicsFlag();
                }
            }
            else if (contaminationRayonTextbox.hasFocus())
            {
                if (keyPressedTextbox(ref contaminationRayonTextbox, e, ref virus.contaminationRayon) == 1)
                {
                    epidemySimulator.setVirus(virus);
                    UpdateGraphicsFlag();
                }
            }
            else if (immunityLostRateTextbox.hasFocus())
            {
                if (keyPressedTextbox(ref immunityLostRateTextbox, e, ref virus.immunityLostRate) == 1)
                {
                    epidemySimulator.setVirus(virus);
                    UpdateGraphicsFlag();
                }
            }
            else if (populationSizeTextbox.hasFocus())
            {
                if (keyPressedTextbox(ref populationSizeTextbox, e, ref populationSize) == 1)
                {
                    UpdateGraphicsFlag();
                }
            }
            else if (infectedPopulationSizeTextbox.hasFocus())
            {
                if (keyPressedTextbox(ref infectedPopulationSizeTextbox, e, ref numberOfInfected) == 1)
                {
                    UpdateGraphicsFlag();
                }
            }

            if (e.Control && e.Alt && e.Code == Keyboard.Key.D)
            {
                SetEnableAll(true);
            }
            if (e.Control && e.Alt && e.Code == Keyboard.Key.R)
            {
                if (!mode.HasFlag(MODE.recordScreenshot))
                {
                    mode = mode | MODE.recordScreenshot;

                    if (!toggleScreenshotMode)
                    {
                        sceneId += 1;
                        screenshotPathSave = screenshotPath + @"/scene" + sceneId.ToString() + @"/";
                        System.IO.Directory.CreateDirectory(screenshotPathSave);
                    }

                    toggleScreenshotMode = true;
                }
                else
                {
                    mode = mode & ~MODE.recordScreenshot;
                }
            }
        }

        static Font defaultFont = new Font(@"arial.ttf");
        static void RenderImage(RenderTexture renderTexture, EpidemySimulator epidemySimulator, MODE mode, double width, double height)
        {
            renderTexture.Display();

            double ratioX = width / epidemySimulator.SpaceWidth;
            double ratioY = height / epidemySimulator.SpaceHeight;

            double sizeAgent = 20.0;

            List<EpidemySimulator.Agent> agents = epidemySimulator.AgentsList;

            renderTexture.Clear(new Color(47, 79, 79));
            double rayon = epidemySimulator.VirusProp.contaminationRayon;

            for (int id = 0; id < agents.Count; ++id)
            {
                if (mode.HasFlag(MODE.renderContactDisk) && agents[id].state == EpidemySimulator.STATE.infected)
                {
                    double x0disk = (agents[id].x - rayon);
                    double y0disk = (agents[id].y - rayon);

                    CircleShape coloredDisk = new CircleShape((float)(rayon * ratioX));
                    coloredDisk.FillColor = new Color(255, 0, 0, 128);
                    coloredDisk.Position = new Vector2f((float)(x0disk * ratioX), (float)(y0disk * ratioY));

                    renderTexture.Draw(coloredDisk);
                }
                if (agents[id].state != EpidemySimulator.STATE.dead)
                {
                    double x0 = agents[id].x - sizeAgent / 2.0;
                    double y0 = agents[id].y - sizeAgent / 2.0;

                    Color color = Color.Magenta; // error color
                    if (agents[id].state == EpidemySimulator.STATE.infected)
                    {
                        color = new Color(0, 100, 0);
                    }
                    else if (agents[id].state == EpidemySimulator.STATE.healthy)
                    {
                        color = new Color(255, 255, 0);
                    }
                    else if (agents[id].state == EpidemySimulator.STATE.immune)
                    {
                        color = new Color(255, 192, 203);
                    }
                    else if (agents[id].state == EpidemySimulator.STATE.dead)
                    {
                        color = new Color(169, 169, 169);
                    }

                    CircleShape coloredDisk = new CircleShape((float) (sizeAgent / 2.0 * ratioX));
                    coloredDisk.FillColor = color;
                    coloredDisk.Position = new Vector2f((float)(x0 * ratioX), (float)(y0 * ratioY));

                    renderTexture.Draw(coloredDisk);
                }
            }

            if (mode.HasFlag(MODE.renderShowStats))
            {
                EpidemySimulator.Stats epidemyStats = epidemySimulator.getStats();

                RectangleShape rectangle = new RectangleShape();
                rectangle.Position = new Vector2f(0.0f, 0.0f);
                rectangle.Size = new Vector2f((float) width, (float) height / 10);
                rectangle.FillColor = new Color(68, 85, 90, 200);
                renderTexture.Draw(rectangle);

                Text epidemyStatsText = new Text(epidemyStats.getString(), defaultFont, 15);
                epidemyStatsText.FillColor = new Color(0, 255, 0);
                epidemyStatsText.Position = new Vector2f(15.0f, 10.0f);
                renderTexture.Draw(epidemyStatsText);
            }

            if (mode.HasFlag(MODE.renderVirusStats))
            {
                EpidemySimulator.Virus virusStats = epidemySimulator.VirusProp;
                RectangleShape rectangle = new RectangleShape();
                rectangle.Position = new Vector2f(0.0f, (float)height - (float)height / 6);
                rectangle.Size = new Vector2f((float)width, (float)height / 6);
                rectangle.FillColor = new Color(68, 85, 90, 200);
                renderTexture.Draw(rectangle);

                Text epidemyStatsText = new Text(virusStats.getString(), defaultFont, 15);
                epidemyStatsText.FillColor = new Color(0, 255, 0);
                epidemyStatsText.Position = new Vector2f(15.0f, (float) height - 120.0f);
                renderTexture.Draw(epidemyStatsText);
            }

            if (mode.HasFlag(MODE.recordScreenshot))
            {
                RectangleShape image = new RectangleShape();
                image.Position = new Vector2f(770.0f, 5.0f);
                image.Size = new Vector2f(25, 25);
                image.Texture = new Texture(System.IO.File.OpenRead("resources/img.png"));

                renderTexture.Draw(image);
            }
        }
        static void nextStep()
        {
            epidemySimulator.Next();
           
            RenderImage(epidemyTexture, epidemySimulator, mode, width, height);

            graphbox.AddYValue("Infected", epidemySimulator.getStats().infectedCount);
            graphbox.AddYValue("Immune", epidemySimulator.getStats().immuneCount);
            graphbox.AddYValue("Dead", epidemySimulator.getStats().deadCount);

            if (mode.HasFlag(MODE.recordScreenshot))
            {
                Screenshot(screenshotPathSave + @"/img" + imageCount.ToString() + @".png");
                ++imageCount;
            }

            UpdateGraphicsFlag();
        }

        static bool updateGraphicsFlag = false;
        static void UpdateGraphicsFlag()
        {
            updateGraphicsFlag = true;
        }
        static void UpdateGraphics()
        {
            window.Clear();

            window.Draw(epidemyBox);
            window.Draw(buttonPlay);
            window.Draw(buttonPause);
            window.Draw(buttonNextStep);
            window.Draw(buttonRestart);

            window.Draw(checkboxRenderContactDisk);
            window.Draw(checkboxRenderShowStats);
            window.Draw(contaminationProbabilityTextbox);
            window.Draw(cureProbabilityTextbox);
            window.Draw(deathProbabilityTextbox);
            window.Draw(immunityProbabilityTextbox);
            window.Draw(contaminationRayonTextbox);
            window.Draw(immunityLostRateTextbox);

            window.Draw(populationSizeTextbox);
            window.Draw(infectedPopulationSizeTextbox);

            window.Draw(contaminationProbabilityLabel);
            window.Draw(cureProbabilityLabel);
            window.Draw(deathProbabilityLabel);
            window.Draw(immunityProbabilityLabel);
            window.Draw(contaminationRayonLabel);
            window.Draw(immunityLostRateLabel);

            window.Draw(populationSizeLabel);
            window.Draw(infectedPopulationSizeLabel);

            window.Draw(graphbox);
            window.Display();
        }

        static void screenshotVideoThread(object obj)
        {
            Tuple<string, int, int> tuple = (Tuple<string,int,int>) obj;
            string sps = tuple.Item1;
            int scId = tuple.Item2;
            int imgCount = tuple.Item3;

            // save video
            Console.WriteLine("Start ffmpeg\n");

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = @"ffmpeg";

            startInfo.Arguments = @"-i " + sps + @"img%d.png -c:v libx264 -pix_fmt yuv420p " + sps + @"scene" + scId.ToString() + @".mp4";
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;

            Process processTemp = new Process();
            processTemp.StartInfo = startInfo;
            processTemp.EnableRaisingEvents = true;
            try
            {
                processTemp.Start();

                Console.WriteLine(processTemp.StandardOutput.ReadToEnd());
                Console.WriteLine(processTemp.StandardError.ReadToEnd());
            }
            catch (Exception e)
            {
                throw;
            }
        }
        static void restart()
        {
            if (numberOfInfected <= populationSize)
            {
                continueTick = false;
                continueTickAfterEnding = false;

                SetEnableAll(true);

                epidemySimulator = new EpidemySimulator((int)width, (int)height, spWidth, spHeight, populationSize, numberOfInfected, virus);

                RenderImage(epidemyTexture, epidemySimulator, mode, width, height);
                graphbox.Clear();
                UpdateGraphicsFlag();

                if (toggleScreenshotMode)
                {
                    Thread thread = new Thread(new ParameterizedThreadStart(screenshotVideoThread));
                    thread.Start(new Tuple<string, int, int>(screenshotPathSave, sceneId, imageCount));
                }
                imageCount = 0;
                if (mode.HasFlag(MODE.recordScreenshot))
                {
                    toggleScreenshotMode = true;

                    sceneId += 1;
                    screenshotPathSave = screenshotPath + @"/scene" + sceneId.ToString() + @"/";
                    System.IO.Directory.CreateDirectory(screenshotPathSave);
                }
                else
                {
                    toggleScreenshotMode = false;
                }
            }
        }

        static Cursor textCursor = new Cursor(Cursor.CursorType.Text);
        static Cursor arrowCursor = new Cursor(Cursor.CursorType.Arrow);
        static void OnMouseMoved(object sender, MouseMoveEventArgs e)
        {
            if (contaminationProbabilityTextbox.isInRectangle(e.X, e.Y))
            {
                window.SetMouseCursor(textCursor);
            }
            else if (cureProbabilityTextbox.isInRectangle(e.X, e.Y))
            {
                window.SetMouseCursor(textCursor);
            }
            else if (deathProbabilityTextbox.isInRectangle(e.X, e.Y))
            {
                window.SetMouseCursor(textCursor);
            }
            else if (immunityProbabilityTextbox.isInRectangle(e.X, e.Y))
            {
                window.SetMouseCursor(textCursor);
            }
            else if (contaminationRayonTextbox.isInRectangle(e.X, e.Y))
            {
                window.SetMouseCursor(textCursor);
            }
            else if (immunityLostRateTextbox.isInRectangle(e.X, e.Y))
            {
                window.SetMouseCursor(textCursor);
            }
            else if (populationSizeTextbox.isInRectangle(e.X, e.Y))
            {
                window.SetMouseCursor(textCursor);
            }
            else if (infectedPopulationSizeTextbox.isInRectangle(e.X, e.Y))
            {
                window.SetMouseCursor(textCursor);
            }
            else
            {
                window.SetMouseCursor(arrowCursor);
            }
        }

        static string screenshotPath;
        static string screenshotPathSave;
        static int sceneId;
        static int imageCount;

        static void Main(string[] args)
        {
            width = 800;
            height = 800;

            spWidth = 1000;
            spHeight = 1000;

            populationSize = 1000;
            numberOfInfected = 5;

            window = new RenderWindow(new VideoMode((uint) (width * 2.0), (uint) height), "VSim", Styles.Titlebar | Styles.Close);
            window.Closed += new EventHandler(OnClose);
            window.MouseButtonReleased += new EventHandler<MouseButtonEventArgs>(OnMouseButtonReleased);
            window.KeyPressed += new EventHandler<KeyEventArgs>(OnKeyPressed);
            window.MouseMoved += new EventHandler<MouseMoveEventArgs>(OnMouseMoved);

            virus.contaminationProbability = 0.1;
            virus.cureProbability = 0.07;
            virus.deathProbability = 0.0005;
            virus.immuneProbability = 0.95;
            virus.contaminationRayon = 55.0;
            virus.immunityLostRate = 0.0005;

            epidemySimulator = new EpidemySimulator((int) width, (int) height, spWidth, spHeight, populationSize, numberOfInfected, virus);
            mode = MODE.renderContactDisk | MODE.renderShowStats | MODE.renderVirusStats;
            epidemyTexture = new RenderTexture((uint)width, (uint)height);

            RenderImage(epidemyTexture, epidemySimulator, mode, 800, 800);

            window.Clear();

            epidemyBox = new RectangleShape();
            epidemyBox.Size = new Vector2f((float)width, (float)height);
            epidemyBox.Position = new Vector2f(0.0f, 0.0f);
            epidemyBox.Texture = epidemyTexture.Texture;

            buttonPlay = new Button(new Vector2f(820.0f, 20.0f), new Vector2f(50.0f, 50.0f), "resources/playButton.png");
            buttonPause = new Button(new Vector2f(820.0f, 90.0f), new Vector2f(50.0f, 50.0f), "resources/pauseButton.png");
            buttonNextStep = new Button(new Vector2f(820.0f, 160.0f), new Vector2f(50.0f, 50.0f), "resources/nextStepButton.png");
            buttonRestart = new Button(new Vector2f(820.0f, 230.0f), new Vector2f(50.0f, 50.0f), "resources/restartButton.png");

            checkboxRenderContactDisk = new Checkbox(new Vector2f(890.0f, 20.0f), new Vector2f(170.0f, 30.0f), mode.HasFlag(MODE.renderContactDisk), "Render contact disk");
            checkboxRenderShowStats = new Checkbox(new Vector2f(890.0f, 70.0f), new Vector2f(170.0f, 30.0f), mode.HasFlag(MODE.renderShowStats), "Render stats");

            contaminationProbabilityLabel = new Label("Contamination probability:", new Vector2f(1080.0f, 25.0f));
            cureProbabilityLabel = new Label("Healing probability:", new Vector2f(1080.0f, 75.0f));
            deathProbabilityLabel = new Label("Death probability:", new Vector2f(1080.0f, 125.0f));
            immunityProbabilityLabel = new Label("Immunity probability:", new Vector2f(1080.0f, 175.0f));
            contaminationRayonLabel = new Label("Contamination rayon:", new Vector2f(1080.0f, 225.0f));
            immunityLostRateLabel = new Label("Immunity lost rate:", new Vector2f(1080.0f, 275.0f));

            populationSizeLabel = new Label("Population size:", new Vector2f(1350.0f, 25.0f));
            infectedPopulationSizeLabel = new Label("Number of infected:", new Vector2f(1350.0f, 75.0f));

            contaminationProbabilityTextbox = new Textbox(new Vector2f(1270.0f, 20.0f), new Vector2f(60.0f, 30.0f), Convert.str(virus.contaminationProbability));
            cureProbabilityTextbox = new Textbox(new Vector2f(1270.0f, 70.0f), new Vector2f(60.0f, 30.0f), Convert.str(virus.cureProbability));
            deathProbabilityTextbox = new Textbox(new Vector2f(1270.0f, 120.0f), new Vector2f(60.0f, 30.0f), Convert.str(virus.deathProbability));
            immunityProbabilityTextbox = new Textbox(new Vector2f(1270.0f, 170.0f), new Vector2f(60.0f, 30.0f), Convert.str(virus.immuneProbability));
            contaminationRayonTextbox = new Textbox(new Vector2f(1270.0f, 220.0f), new Vector2f(60.0f, 30.0f), Convert.str(virus.contaminationRayon));
            immunityLostRateTextbox = new Textbox(new Vector2f(1270.0f, 270.0f), new Vector2f(60.0f, 30.0f), Convert.str(virus.immunityLostRate));

            populationSizeTextbox = new Textbox(new Vector2f(1500.0f, 20.0f), new Vector2f(60.0f, 30.0f), Convert.str(populationSize));
            infectedPopulationSizeTextbox = new Textbox(new Vector2f(1500.0f, 70.0f), new Vector2f(60.0f, 30.0f), Convert.str(numberOfInfected));

            graphbox = new GraphBox(new Vector2f(820.0f, 320.0f), new Vector2f(600.0f, 400.0f));
            graphbox.AddSerie("Infected", new Color(0, 100, 0));
            graphbox.AddSerie("Immune", new Color(255, 192, 203));
            graphbox.AddSerie("Dead", new Color(128, 128, 128));

            spEpidemyBox = new Sprite();
            spGraphbox = new Sprite();
            spEpidemyBox.Position = new Vector2f(0.0f, 0.0f);
            spGraphbox.Position = new Vector2f(epidemyBox.Size.X, 0.0f);

            screenshot = new RenderTexture((uint) (epidemyBox.Size.X + graphbox.Size.X), (uint) (epidemyBox.Size.Y));
            screenshotPath = @"screenshots/";
            DateTime dtime = DateTime.Now;
            string dtimeStr = dtime.Year.ToString() + "_" + dtime.Month.ToString() + "_" + dtime.Day.ToString();
            screenshotPath += dtimeStr + @"/";
            if (!System.IO.Directory.Exists(screenshotPath))
            {
                System.IO.Directory.CreateDirectory(screenshotPath);
            }


            int instance_k = 0;
            while (System.IO.Directory.Exists(screenshotPath + @"prg" + instance_k.ToString()))
            {
                ++instance_k;
            }
            screenshotPath += @"prg" + instance_k.ToString();
            System.IO.Directory.CreateDirectory(screenshotPath);
            sceneId = -1;
            imageCount = 0;
            screenshotPathSave = screenshotPath + @"scene" + sceneId.ToString() + @"/";

            toggleScreenshotMode = false;

            UpdateGraphics();
            continueTick = false;
            continueTickAfterEnding = false;
            while (window.IsOpen)
            {
                window.DispatchEvents();

                if (continueTick)
                {
                    nextStep();
                    if (epidemySimulator.getStats().infectedCount == 0 && !continueTickAfterEnding)
                    {
                        continueTick = false;
                    }
                    UpdateGraphics();
                }
                else if (updateGraphicsFlag)
                {
                    UpdateGraphics();
                }

                updateGraphicsFlag = false;
            }

            if (toggleScreenshotMode)
            {
                Thread thread = new Thread(new ParameterizedThreadStart(screenshotVideoThread));
                thread.Start(new Tuple<string, int, int>(screenshotPathSave, sceneId, imageCount));
            }
        }

        [Flags]
        public enum MODE
        {
            renderContactDisk = 1,
            renderAgentVector = 2,
            renderShowStats = 4,
            renderVirusStats = 8,
            recordScreenshot = 16
        }
    }
}

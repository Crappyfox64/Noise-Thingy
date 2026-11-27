using System;
using System.IO;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;
using CRPY.Graphics;
using CRPY.Numerics.Noise;
using System.Threading;
using System.Threading.Tasks;

namespace Noise
{
    public partial class MainPage : ContentPage
    {
        public string ScF = $"/storage/emulated/0/CRPY/Screenshots/{AppInfo.PackageName}/";

        private (Thread T, bool Running) Render;

        public (float x, float y) Offset = (0, 0);
        public CNoise2D N = new CNoise2D();
        public CNoise2D N1 = new CNoise2D();
        public CNoise2D N2 = new CNoise2D();

        public CABMP ABMP = new CABMP(240, 240);
        public bool AddFrame = false;

        public event Action<dynamic> Update;

        public bool Ticked = false;
        public float S = -0.05f;
        public float Scale = 240f;
        public int Zoom = 8;
        public bool Ready = true;
        public bool ExOut = false;
        
        public async void Beep(string Action)
        {
			int Freq = 0;
            int Dur = 0;
            int Sample = 0;
            
            switch ( Action.ToLower() )
            {
				case "movement":
                    Freq = 520;
                    Dur = 120;
                    Sample = 8000;
                break;
                
                case "render-finished":
                    Freq = 220;
                    Dur = 85;
                    Sample = 8000;
                break;
                
                case "render-start":
                    Freq = 320;
                    Dur = 75;
                    Sample = 8000;
                break;
                
                case "add-frame":
                    Freq = 520;
                    Dur = 120;
                    Sample = 44100;
                break;
                
                case "export":
                    Freq = 430;
                    Dur = 130;
                    Sample = 16000;
                break;
                
                case "export2":
                    Freq = 630;
                    Dur = 130;
                    Sample = 16000;
                break;
                
                case "zoom":
                    Freq = 420;
                    Dur = 140;
                    Sample = 16000;
                break;
                
                case "stop":
                    Freq = 420;
                    Dur = 75;
                    Sample = 16000;
                break;
                
                case "start":
                    Freq = 720;
                    Dur = 50;
                    Sample = 16000;
                break;
			}
            
            
			Console.Beep(Freq,Dur,Sample);
		}

        public MainPage()
        {
            InitializeComponent();

            Update += (v) =>
            {
                Ticked = true;
            };

            Add.Clicked += (s, e) =>
            {
                AddFrame = true;
                Add.Text = "Adding...";
                Beep("Add-Frame");
            };

            Step.Maximum = 1280;

            Step.DragCompleted += (s, e) =>
            {
                Scale = (float)Step.Value;
                Beep("Zoom");
                Update.Invoke("Zoom");
            };

            Stop.Clicked += (s, e) =>
            {
                if (Render.Running)
                {
                    Stop.Text = "Stopping";
                    Beep("Stop");
                    StopRender();
                }
                else
                {
                    StartRender();
                    Stop.Text = "Stop";
                    Beep("Start");
                }
            };

            float[] Zooms = new float[]
            {
                0.025f, 1, 5f, 15f, 25f, 40f, 60f,
                120f, 160f, 240f, 450f, 720f, 1280f
            };

            Export.Clicked += (s, e) =>
            {
                ExOut = true;
                Beep("Export");
            };

            Center.Clicked += (s, e) =>
            {
                Zoom++;
                Scale = Zooms[Zoom % Zooms.Length];
                Step.Value = (double)Scale;
                Update.Invoke("Zoom");
                Beep("Zoom");
            };

            Up.Clicked += (s, e) =>
            {
                Offset.y += S;
                Update.Invoke("Movement");
                Beep("Movement");
            };

            Down.Clicked += (s, e) =>
            {
                Offset.y -= S;
                Update.Invoke("Movement");
                Beep("Movement");
            };

            Left.Clicked += (s, e) =>
            {
                Offset.x += S;
                Update.Invoke("Movement");
                Beep("Movement");
            };

            Right.Clicked += (s, e) =>
            {
                Offset.x -= S;
                Update.Invoke("Movement");
                Beep("Movement");
            };

            StartRender();
        }

        public void StartRender()
        {
            Render.Running = true;

            Render.T = new Thread(() =>
            {
                CBMP Buffer = new CBMP(240, 240);
                Random Ran = new Random();

                while (Render.Running)
                {
                    Thread.Sleep(60);

                    if (!Ticked) continue;

                    if (!Ready)
                    {
                        Dispatcher.Dispatch(() =>
                        {
                            Stop.Text = "Waiting";
                        });
                        continue;
                    }

                    Ticked = false;
                    Ready = false;

                    Dispatcher.Dispatch(() =>
                    {
                        Stop.Text = "Rendering";
                    });

                    Console.WriteLine($"X: {Offset.x}, Y: {Offset.y}");

                    Parallel.For(0, Buffer.Width, i =>
                    {
                        for (int k = 0; k < Buffer.Height; k++)
                        {
                            float u = (float)i / (float)Buffer.Width;
                            float v = (float)k / (float)Buffer.Height;

                            float x = (Offset.x + (u - 0.5f)) * Scale;
                            float y = (Offset.y + (v - 0.5f)) * Scale;

                            // 1. ISLAND SHAPE - distance from center, 0 at center, 1 at edge
                            float dx = u - 0.5f;
                            float dy = v - 0.5f;
                            float dist = MathF.Sqrt(dx * dx + dy * dy) * 2f; // 0 to ~1.41

                            // 2. ALL 3 NOISES
                            float elevation = (float)N.From(x, y); // height
                            float variation = (float)N1.From(x, y); // grass / forest variation
                            float detail = (float)N2.From(x, y); // sand / rock detail

                            // 3. Make island - high in middle, falls off to ocean
                            float h = elevation - (dist * dist * 0.9f);

                            // 4. Color by height
                            byte r, g, b;

                            if (h < 0.05f)
                            {
                                // Deep water
                                r = 20;
                                g = 60;
                                b = 140;
                            }
                            else if (h < 0.12f)
                            {
                                // Shallow water - use detail for waves
                                r = (byte)(30 + detail * 30);
                                g = (byte)(100 + detail * 40);
                                b = (byte)(180 + detail * 40);
                            }
                            else if (h < 0.18f)
                            {
                                // Sand beach - use detail for speckles
                                r = (byte)(230 + detail * 25);
                                g = (byte)(210 + detail * 20);
                                b = (byte)(140 + detail * 20);
                            }
                            else if (h < 0.45f)
                            {
                                // Grass - use variation to make patches
                                r = (byte)(60 + variation * 50);
                                g = (byte)(140 + variation * 60);
                                b = (byte)(50 + variation * 30);
                            }
                            else if (h < 0.65f)
                            {
                                // Forest / Rock - mix variation + detail
                                r = (byte)(80 + variation * 40);
                                g = (byte)(100 + variation * 30);
                                b = (byte)(60 + detail * 20);
                            }
                            else
                            {
                                // Mountain / Snow
                                r = (byte)(180 + detail * 60);
                                g = (byte)(180 + detail * 60);
                                b = (byte)(180 + detail * 60);
                            }

                            Buffer.Set(i, k, new CColor(r, g, b));
                        }
                    });

                    // --- MAGIC: AddFrame ---
                    if (AddFrame)
                    {
                        AddFrame = false;
                        ABMP.Add(Buffer.Clone());
                        Dispatcher.Dispatch(() =>
                        {
                            Add.Text = $"AddFrame ({ABMP.FrameCount})";
                        });
                    }

                    // --- MAGIC: Export ---
                    if (ExOut)
                    {
                        ExOut = false;

                        Dispatcher.Dispatch(() =>
                        {
                            if (!Directory.Exists(ScF))
                            {
                                Directory.CreateDirectory(ScF);
                            }

                            string stamp = $"{DateTime.Now.Day}-{DateTime.Now.Month}-{DateTime.Now.Hour}-{DateTime.Now.Minute}-{DateTime.Now.Second}-{DateTime.Now.Ticks}";

                            if (ABMP.FrameCount > 0)
                            {
                                // Export GIF from collected frames
                                ABMP.Duration = ABMP.FrameCount * 0.12;
                                byte[] gif = ABMP.ToGIF();
                                File.WriteAllBytes($"{ScF}/NOISE [{stamp}].gif", gif);
                                ABMP.Clear();
                            }
                            else
                            {
                                // No frames = single BMP like before
                                File.WriteAllBytes($"{ScF}/NOISE [{stamp}].bmp", Buffer.ToBMP());
                            }

                            Export.Text = "Exported!";
                            Beep("export");
                            Thread.Sleep(25);
                            Beep("export2");
                        });

                        // Reset Export text after a bit
                        Task.Run(async () =>
                        {
                            await Task.Delay(1000);
                            Dispatcher.Dispatch(() => Export.Text = "Export");
                        });
                    }

                    Console.WriteLine("Rendering");

                    byte[] buff = Buffer.ToBMP();

                    Dispatcher.Dispatch(() =>
                    {
                        DisplayImage.Source = ImageSource.FromStream(() => new MemoryStream(buff));
                    });

                    Ready = true;

                    Dispatcher.Dispatch(() =>
                    {
                        Stop.Text = "Stop";
                    });
                }

                Dispatcher.Dispatch(() =>
                {
                    Stop.Text = "Start";
                });
            });

            Render.T.Start();
        }

        protected void StopRender()
        {
            Render.Running = false;
        }
    }
}
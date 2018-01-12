using System;
using Assimp;
using System.IO;
using System.Reflection;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.SceneGraph.Core;
using SharpGL;
using RacunarskaGrafikaP;
using System.Windows.Documents;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Threading;

namespace AssimpSample
{

    public class World : IDisposable
    {
      
        private AssimpScene m_scene;
        private AssimpScene m_strela;

        private float m_xRotation = 0.0f;
        private float m_yRotation = 0.0f;

        private float m_sceneDistance = 7000.0f;
        private float m_xtranslate = 0.0f;
        private float m_ytranslate = 0.0f;
        
        private int m_width;
        private int m_height;
        private int m_depth;

      

        private float targetValueTranslate = 1000.0f;

        private float wallVal = 1.0f;

        private float arrowVal = 2.0f;

        public AssimpScene Scene
        {
            get { return m_scene; }
            set { m_scene = value; }
        }
        public AssimpScene Scene1
        {
            get { return m_strela; }
            set { m_strela = value; }
        }

        public float RotationX
        {
            get { return m_xRotation; }
            set { m_xRotation = value; }
        }


        public float RotationY
        {
            get { return m_yRotation; }
            set { m_yRotation = value; }
        }

        /// <summary>
        /// Identifikatori tekstura
        /// </summary>
        private enum TextureObjects { grass , metal, mud };
        private readonly int m_textureCount = Enum.GetNames(typeof(TextureObjects)).Length;


        /// <summary>
        /// Indentifikatori OpenGL tekstura
        /// </summary>
        private uint[] m_textures = null;

        /// <summary>
        /// Putanje do fajlova za teksture
        /// </summary>
        private string[] m_textureFiles = { "..//..//images//grass.jpg", "..//..//images/metal.jpg", "..//..//images/mud.jpg" };


        public float SceneDistance
        {
            get { return m_sceneDistance; }
            set { m_sceneDistance = value; }
        }


        public int Width
        {
            get { return m_width; }
            set { m_width = value; }
        }

        bool m_culling = true;

        public bool AnimationRunning
        {
            get
            {
                return animationRunning;
            }
            set
            {
                animationRunning = value;
            }
        }
        bool m_depthTesting = true;

        //animacija
        private DispatcherTimer timer1;
        private DispatcherTimer timer2;

        private bool animationRunning=false;

        private float scaleArrow = 1.0f;
        private float arrowXTranslate = 0.0f;
        private float arrowYTranslate = 0.0f;
        private float arrowZTranslate = 0.0f;
        private float arrowXRotation = 0.0f;
        private int arrowCounter = 0;

        public int Height
        {
            get { return m_height; }
            set { m_height = value; }
        }
        public int Depth
        {
            get { return m_depth; }
            set { m_depth = value; }
        }


        /// <summary>
        /// Odredjuje rotiranje
        /// </summary>
        public float TargetValueTranslate
        {
            get
            {
                return targetValueTranslate;
            }
            set
            {
                targetValueTranslate = value;
            }
        }

      public float wallValue
        {
            get
            {
                return wallVal;
            }
            set
            {
                wallVal = value;
            }
        }

        public float arrowValue
        {
            get
            {
                return arrowVal;
            }
            set
            {
                arrowVal = value;
            }
        }
        public World(String scenePath, String sceneFileName, String scenePath1, String sceneFileName1, int width, int height, OpenGL gl)
        {
            this.m_scene = new AssimpScene(scenePath, sceneFileName, gl);
            this.m_strela = new AssimpScene(scenePath1, sceneFileName1, gl);

            this.m_width = width;
            this.m_height = height;
            m_textures = new uint[m_textureCount];
        }


        public void FireArrow(object sender, EventArgs e)
        {

            arrowYTranslate += 200;
            if (arrowYTranslate > 4000)
            {
                arrowYTranslate = -250.0f;
                arrowCounter += 1;
            }
            if (arrowCounter == 10)
            {
                timer2.Stop();
                Reset();
                animationRunning = false;
            }
        }

        public void Reset()
        {
            m_xRotation = 0.0f;
            m_yRotation = 0.0f;
            m_sceneDistance = 4000.0f;
            arrowCounter = 0;
            arrowXRotation = 0;
            arrowYTranslate = 0;
            arrowZTranslate = 0;
        }

        public void PomeranjeKameruizZamak(object sender, EventArgs e)
        {

            this.m_sceneDistance += 50;
            if (this.SceneDistance == +4000)
            {
                timer1.Stop();
                this.timer2 = new DispatcherTimer();
                timer2.Interval = TimeSpan.FromMilliseconds(20);
                timer2.Tick += new EventHandler(FireArrow);
                timer2.Start();
            }

        }
   
        public void startAnimation()
        {  
            this.timer1 = new DispatcherTimer();
            m_sceneDistance = 100.0f;
            m_ytranslate = 500;
            timer1.Interval = TimeSpan.FromMilliseconds(20);
            timer1.Tick += new EventHandler(PomeranjeKameruizZamak);
            timer1.Start();
            animationRunning = true;
        }

        ~World()
        {
            this.Dispose(false);
        }


        public void Initialize(OpenGL gl)
        {
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            gl.Color(0.5f, 0.5f, 0.5f);

            gl.ShadeModel(OpenGL.GL_FLAT);
            if (m_depthTesting)
            {
                gl.Enable(OpenGL.GL_DEPTH_TEST);
            }
            if (m_culling)
            {
                gl.Enable(OpenGL.GLU_CULLING);
            }

            gl.FrontFace(OpenGL.GL_CCW);
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            gl.ColorMaterial(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE);


           // setupLighting(gl);
            //setupTargetLight(gl);

            m_scene.LoadScene();
            m_scene.Initialize();
            m_strela.LoadScene();
            m_strela.Initialize();

            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);     // Linearno filtriranje
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);     // Linearno filtriranje
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_REPEAT);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_REPEAT);
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);

            gl.Enable(OpenGL.GL_TEXTURE_2D);

            gl.GenTextures(m_textureCount, m_textures);
            for (int i = 0; i < m_textureCount; ++i)
            {
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[i]);

                Bitmap image = new Bitmap(m_textureFiles[i]);
                image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
               
                BitmapData imageData = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
                                                      System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                gl.Build2DMipmaps(OpenGL.GL_TEXTURE_2D, (int)OpenGL.GL_RGBA8, image.Width, image.Height, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, imageData.Scan0);
                

                image.UnlockBits(imageData);
                image.Dispose();
            }

        }

        /// <summary>
        /// Podesava osvetljenje u scecni
        /// </summary>
        /// <param name="gl"></param>
        public void setupLighting(OpenGL gl)
        {
            float[] ambijentalnaKomponenta = { 1.0f, 1.0f, 1.0f, 1.0f };
            float[] difuznaKomponenta = { 1.0f, 1.0f, 1.0f, 1.0f };
            float[] lightPos0 = { -1000.0f, 1000.0f, 0.0f };
          
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, lightPos0);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, ambijentalnaKomponenta);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, difuznaKomponenta);

          
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPOT_CUTOFF, 180.0f);

            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);
            gl.Enable(OpenGL.GL_NORMALIZE);
        }

        public void setupTargetLight(OpenGL gl)
        {
            float[] ambijentalnaKomponenta = { 1.0f, 1.0f, 1.0f, 1.0f };
            float[] difuznaKomponenta = { 1.0f, 1.0f, 1.0f, 1.0f };
            float[] lightPos1 = { 0.0f, 1200.0f, 0.0f, 1.0f };
            float[] smer = { 0.0f, -1.0f, 0.0f };

            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, lightPos1);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_AMBIENT, ambijentalnaKomponenta);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_DIFFUSE, difuznaKomponenta);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_DIRECTION, smer);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_CUTOFF, 15.0f);

            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT1);
            gl.Enable(OpenGL.GL_NORMALIZE);

        }

        public void Resize(OpenGL gl, int width, int height)
        {
            m_width = width;
            m_height = height;
            gl.Viewport(0, 0, width, height);
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Perspective(60f, (double)width / height, 1f, 20000f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();   

        }

        public void Draw(OpenGL gl)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            gl.PushMatrix();
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();
            gl.LookAt(0.0f, 1000.0f, 300.0f,
                   0.0f, 950.0f, 0.0f,
                   0.0f, 1.0f, 0.0f);
            gl.Translate(m_xtranslate, m_ytranslate, -m_sceneDistance);
            gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f);
            gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);

            drawGround(gl);
            drawTrack(gl);


            gl.PushMatrix();
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);
            m_scene.Draw();
            gl.PopMatrix();

            drawArrow(gl);

            drawCube1(gl);
            drawCube2(gl);


            draw3DText(gl);
            setupLighting(gl);
            setupTargetLight(gl);
            gl.PopMatrix();
           
            gl.Flush();
        }

        public void draw3DText(OpenGL gl)
        {
              gl.Viewport(m_width / 2, 0, m_width / 2, m_height / 2);


              gl.PushMatrix();
              gl.Color(1.0f, 0.0f, 0.0f);
              gl.Translate(1000.0f, -1000.0f, 2000.0f);
              gl.Scale(100, 100, 100);
              gl.DrawText3D("Verdana Bold", 14.0f, 1.0f, 0.3f, "Predmet: Racunarska Grafika");
              gl.PopMatrix();

              gl.PushMatrix();
              gl.Color(1.0f, 0.0f, 0.0f);
              gl.Translate(1000.0f, -1100.0f, 2000.0f);
              gl.Scale(100, 100, 100);
              gl.DrawText3D("Verdana Bold", 14.0f, 1.0f, 0.3f, "Sk.god: 2017/18");
              gl.PopMatrix();

              gl.PushMatrix();
              gl.Color(1.0f, 0.0f, 0.0f);
              gl.Translate(1000.0f, -1200.0f, 2000.0f);
              gl.Scale(100, 100, 100);
              gl.DrawText3D("Verdana Bold", 14.0f, 1.0f, 0.3f, "Ime: Stefan");
              gl.PopMatrix();

              gl.PushMatrix();
              gl.Color(1.0f, 0.0f, 0.0f);
              gl.Translate(1000.0f, -1300.0f, 2000.0f);
              gl.Scale(100, 100, 100);
              gl.DrawText3D("Verdana Bold", 14.0f, 1.0f, 0.3f, "Prezime: Lukic");
              gl.PopMatrix();

              gl.PushMatrix();
              gl.Color(1.0f, 0.0f, 0.0f);
              gl.Translate(1000.0f, -1400.0f, 2000.0f); ;
              gl.Scale(100, 100, 100);
              gl.DrawText3D("Verdana Bold", 14.0f, 1.0f, 0.3f, "Sifra zad.: 20.2");
              gl.PopMatrix();


              gl.Viewport(0, 0, m_width, m_height);


          

        }

        public void drawArrow(OpenGL gl)
        {

            gl.PushMatrix();
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);
            gl.Translate(arrowXTranslate, arrowYTranslate, arrowZTranslate);
            gl.Scale(arrowVal, arrowVal, 2.0f);
            gl.Rotate(180.0f, 0.0f, 0.0f + arrowXRotation);
            gl.Color(1.0f, 0.0f, 1.0f);


            m_strela.Draw();

            gl.PopMatrix();
        }
        public void drawGround(OpenGL gl)
        {
            gl.PushMatrix();
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.grass]);
            gl.Color(0.0f, 0.0f, 0.0f);
            gl.Begin(OpenGL.GL_QUADS);
            gl.Color(0.5f, 0.5f, 0.5f);
            gl.Normal(0.0f, -1.0f, 0.0f);
            gl.TexCoord(0.0f, 0.0f);
            gl.Vertex(-1000.0f, 0.0f, 1000.0f);
            gl.TexCoord(0.0f, 1.0f);
            gl.Vertex(1000.0f, 0.0f, 1000.0f);
            gl.TexCoord(1.0f, 1.0f);
            gl.Vertex(1000.0f, 0.0f, -1000.0f);
            gl.TexCoord(1.0f, 0.0f);
            gl.Vertex(-1000.0f, 0.0f, -1000.0f);
            gl.End();
            gl.PopMatrix();
        }

        public void drawTrack(OpenGL gl)
        {
            gl.PushMatrix();
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.mud]);
            gl.Begin(OpenGL.GL_QUADS);
            gl.Normal(0.0f, -1.0f, 0.0f);

            gl.Vertex(-200.0f, 200.0f, 1000.0f); //top left
            gl.TexCoord(0.0f, 0.0f);
            gl.Vertex(200.0f, 200.0f, 1000.0f); //top right
            gl.TexCoord(0.0f, 1.0f);
            gl.Vertex(200.0f, -200.0f, -1000.0f); //bottom right
            gl.TexCoord(1.0f, 1.0f);
            gl.Vertex(-200.0f, -200.0f, -1000.0f); //bottom left
            gl.TexCoord(1.0f, 0.0f);
            gl.End();
            gl.PopMatrix();
        }

        public void drawCube1(OpenGL gl)
        {
            gl.PushMatrix();
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.metal]);
            gl.Translate(targetValueTranslate, 330.0f, 0.0f);
            gl.Scale(100, 300, 1500);
            gl.Color(1.0f, 1.0f, 1.0f);
         
            Cube cube = new Cube();
            cube.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

        }

        public void drawCube2(OpenGL gl)
        {
            gl.PushMatrix();
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.metal]);
            gl.Translate(-1000.0f, 330.0f, 0.0f);
            gl.Rotate(0.0f, wallVal, 0.0f);
            gl.Scale(100, 300, 1500);
            gl.Color(1.0f, 1.0f, 1.0f);
            
            Cube cube = new Cube();
            cube.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            
            gl.PopMatrix();

        }


        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_scene.Dispose();
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }


    }
}

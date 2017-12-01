
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

namespace AssimpSample
{

    public class World : IDisposable
    {
      
        private AssimpScene m_scene;
        private AssimpScene m_strela;

        private float m_xRotation = 0.0f;
        private float m_yRotation = 0.0f;

        private float m_sceneDistance = 7000.0f;

        private int m_width;
        private int m_height;
        private int m_depth;




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


        bool m_depthTesting = true;


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



        public World(String scenePath, String sceneFileName, String scenePath1, String sceneFileName1, int width, int height, OpenGL gl)
        {
            this.m_scene = new AssimpScene(scenePath, sceneFileName, gl);
            this.m_strela = new AssimpScene(scenePath1, sceneFileName1, gl);

            this.m_width = width;
            this.m_height = height;

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
            m_scene.LoadScene();
            m_scene.Initialize();
            m_strela.LoadScene();
            m_strela.Initialize();
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
            gl.Translate(0.0f, 0.0f, -m_sceneDistance);
            gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f);
            gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);

            drawGround(gl);
            drawTrack(gl);


            gl.PushMatrix();
            m_scene.Draw();
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(750.0f, 300.0f, 0.0f);
            gl.Scale(2.0f, 2.0f, 2.0f);
            m_strela.Draw();
            gl.PopMatrix();

          
            drawCube1(gl);
            drawCube2(gl);


            draw3DText(gl);

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
        public void drawGround(OpenGL gl)
        {
            gl.PushMatrix();
            gl.Begin(OpenGL.GL_QUADS);
            gl.Color(1.0f, 0.0f, 0.0f);
            gl.Vertex(-1000.0f, 0.0f, 1000.0f); //top left
            gl.Vertex(1000.0f, 0.0f, 1000.0f); //top right
            gl.Vertex(1000.0f, 0.0f, -1000.0f); //bottom right
            gl.Vertex(-1000.0f, 0.0f, -1000.0f); //bottom left
            gl.End();
            gl.PopMatrix();
        }

        public void drawTrack(OpenGL gl)
        {
            gl.PushMatrix();
            gl.Color(0.0f, 0.0f, 1.0f);
            gl.Begin(OpenGL.GL_QUADS);
            gl.Vertex(-100.0f, 100.0f, 1000.0f); //top left
            gl.Vertex(100.0f, 100.0f, 1000.0f); //top right
            gl.Vertex(100.0f, -100.0f, -1000.0f); //bottom right
            gl.Vertex(-100.0f, -100.0f, -1000.0f); //bottom left
            gl.End();
            gl.PopMatrix();
        }

        public void drawCube1(OpenGL gl)
        {
            gl.PushMatrix();
            gl.Translate(1000.0f, 330.0f, 0.0f);
            gl.Scale(100, 300, 1500);
            gl.Color(1.0f, 1.0f, 1.0f);
            Cube cube = new Cube();
            cube.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

        }

        public void drawCube2(OpenGL gl)
        {
            gl.PushMatrix();
            gl.Translate(-1000.0f, 330.0f, 0.0f);
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
